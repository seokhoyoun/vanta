using MongoDB.Driver;
using Vanta.Infrastructure.Mongo;
using Vanta.Models;
using Vanta.Repositories.ProjectEquipments;
using Vanta.Repositories.ProjectEquipmentModules;
using Vanta.Repositories.ProjectMembers;
using Vanta.Repositories.Projects;
using Vanta.ViewModels;

namespace Vanta.Services.Projects
{
    public class MongoProjectCatalogService : IProjectCatalogService
    {
#region Fields

        private const string SAMPLE_PROJECTS_SEED_STATE_ID = "sample-projects-v1";

        private readonly IProjectRepository mProjectRepository;

        private readonly IProjectEquipmentRepository mProjectEquipmentRepository;

        private readonly IProjectEquipmentModuleRepository mProjectEquipmentModuleRepository;

        private readonly IProjectMemberRepository mProjectMemberRepository;

        private readonly IMongoCollection<ProjectCatalogSeedState> mProjectCatalogSeedStates;

        private readonly SampleProjectCatalogService mSampleProjectCatalogService;

#endregion

#region Constructors

        public MongoProjectCatalogService(
            IProjectRepository projectRepository,
            IProjectEquipmentRepository projectEquipmentRepository,
            IProjectEquipmentModuleRepository projectEquipmentModuleRepository,
            IProjectMemberRepository projectMemberRepository,
            IMongoCollectionContext mongoCollectionContext,
            SampleProjectCatalogService sampleProjectCatalogService)
        {
            mProjectRepository = projectRepository;
            mProjectEquipmentRepository = projectEquipmentRepository;
            mProjectEquipmentModuleRepository = projectEquipmentModuleRepository;
            mProjectMemberRepository = projectMemberRepository;
            mProjectCatalogSeedStates = mongoCollectionContext.ProjectCatalogSeedStates;
            mSampleProjectCatalogService = sampleProjectCatalogService;
        }

#endregion

#region Public Methods

        public async Task<ProjectListPageViewModel> GetProjectListPage(CancellationToken cancellationToken = default)
        {
            await EnsureSampleProjectsSeeded(cancellationToken);

            List<Project> projects = await mProjectRepository.GetAll(cancellationToken);
            ProjectListPageViewModel page = new ProjectListPageViewModel();

            foreach (Project project in projects)
            {
                List<ProjectEquipment> equipments = await mProjectEquipmentRepository.GetByProjectId(project.Id, cancellationToken);
                int moduleCount = await CountModules(equipments, cancellationToken);

                ProjectListItemViewModel item = CreateProjectListItem(project, equipments.Count, moduleCount);
                page.Projects.Add(item);
            }

            return page;
        }

        public async Task<ProjectDashboardViewModel?> GetProjectDashboardOrNull(string code, CancellationToken cancellationToken = default)
        {
            await EnsureSampleProjectsSeeded(cancellationToken);

            Project? project = await GetProjectByCodeOrNull(code, cancellationToken);
            if (project == null)
            {
                return null;
            }

            ProjectDashboardViewModel dashboard = await CreateProjectDashboard(project, cancellationToken);
            return dashboard;
        }

        public async Task<bool> CreateProject(ProjectDashboardViewModel projectDashboard, CancellationToken cancellationToken = default)
        {
            await EnsureSampleProjectsSeeded(cancellationToken);

            Project? existingProject = await GetProjectByCodeOrNull(projectDashboard.Code, cancellationToken);
            if (existingProject != null)
            {
                return false;
            }

            await CreateProjectWithoutSeedCheck(projectDashboard, cancellationToken);
            return true;
        }

        public async Task<bool> UpdateProject(string originalCode, ProjectDashboardViewModel projectDashboard, CancellationToken cancellationToken = default)
        {
            await EnsureSampleProjectsSeeded(cancellationToken);

            Project? project = await GetProjectByCodeOrNull(originalCode, cancellationToken);
            if (project == null)
            {
                return false;
            }

            Project? duplicateProject = await GetProjectByCodeOrNull(projectDashboard.Code, cancellationToken);
            if (duplicateProject != null && !string.Equals(duplicateProject.Id, project.Id, StringComparison.Ordinal))
            {
                return false;
            }

            DateTime updatedUtc = DateTime.UtcNow;
            ApplyProjectDashboard(project, projectDashboard, updatedUtc);
            await mProjectRepository.Replace(project, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteProject(string code, CancellationToken cancellationToken = default)
        {
            await EnsureSampleProjectsSeeded(cancellationToken);

            Project? project = await GetProjectByCodeOrNull(code, cancellationToken);
            if (project == null)
            {
                return false;
            }

            List<ProjectEquipment> equipments = await mProjectEquipmentRepository.GetByProjectId(project.Id, cancellationToken);
            foreach (ProjectEquipment equipment in equipments)
            {
                List<ProjectEquipmentModule> modules = await mProjectEquipmentModuleRepository.GetByProjectEquipmentId(equipment.Id, cancellationToken);
                foreach (ProjectEquipmentModule module in modules)
                {
                    await mProjectEquipmentModuleRepository.Delete(module.Id, cancellationToken);
                }

                await mProjectEquipmentRepository.Delete(equipment.Id, cancellationToken);
            }

            await mProjectRepository.Delete(project.Id, cancellationToken);
            return true;
        }

        public async Task<bool> CreateEquipment(string projectCode, EquipmentSectionViewModel equipment, CancellationToken cancellationToken = default)
        {
            await EnsureSampleProjectsSeeded(cancellationToken);

            Project? project = await GetProjectByCodeOrNull(projectCode, cancellationToken);
            if (project == null)
            {
                return false;
            }

            ProjectEquipment? existingEquipment = await mProjectEquipmentRepository.GetByProjectIdAndCodeOrNull(
                project.Id,
                equipment.Code,
                cancellationToken);
            if (existingEquipment != null)
            {
                return false;
            }

            DateTime createdUtc = DateTime.UtcNow;
            ProjectEquipment projectEquipment = CreateProjectEquipment(project.Id, equipment, createdUtc);
            await mProjectEquipmentRepository.Create(projectEquipment, cancellationToken);
            return true;
        }

        public async Task<bool> UpdateEquipment(
            string projectCode,
            string originalEquipmentCode,
            EquipmentSectionViewModel equipment,
            CancellationToken cancellationToken = default)
        {
            await EnsureSampleProjectsSeeded(cancellationToken);

            ProjectEquipment? projectEquipment = await GetProjectEquipmentByCodeOrNull(projectCode, originalEquipmentCode, cancellationToken);
            if (projectEquipment == null)
            {
                return false;
            }

            Project? project = await mProjectRepository.GetByIdOrNull(projectEquipment.ProjectId, cancellationToken);
            if (project == null)
            {
                return false;
            }

            ProjectEquipment? duplicateEquipment = await mProjectEquipmentRepository.GetByProjectIdAndCodeOrNull(
                project.Id,
                equipment.Code,
                cancellationToken);
            if (duplicateEquipment != null && !string.Equals(duplicateEquipment.Id, projectEquipment.Id, StringComparison.Ordinal))
            {
                return false;
            }

            ApplyProjectEquipment(projectEquipment, equipment, DateTime.UtcNow);
            await mProjectEquipmentRepository.Replace(projectEquipment, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteEquipment(string projectCode, string equipmentCode, CancellationToken cancellationToken = default)
        {
            await EnsureSampleProjectsSeeded(cancellationToken);

            ProjectEquipment? equipment = await GetProjectEquipmentByCodeOrNull(projectCode, equipmentCode, cancellationToken);
            if (equipment == null)
            {
                return false;
            }

            List<ProjectEquipmentModule> modules = await mProjectEquipmentModuleRepository.GetByProjectEquipmentId(equipment.Id, cancellationToken);
            foreach (ProjectEquipmentModule module in modules)
            {
                await mProjectEquipmentModuleRepository.Delete(module.Id, cancellationToken);
            }

            await mProjectEquipmentRepository.Delete(equipment.Id, cancellationToken);
            return true;
        }

        public async Task<bool> CreateModule(
            string projectCode,
            string equipmentCode,
            ModuleItemViewModel module,
            CancellationToken cancellationToken = default)
        {
            await EnsureSampleProjectsSeeded(cancellationToken);

            ProjectEquipment? equipment = await GetProjectEquipmentByCodeOrNull(projectCode, equipmentCode, cancellationToken);
            if (equipment == null)
            {
                return false;
            }

            string moduleCode = module.Code;
            ProjectEquipmentModule? existingModule = await mProjectEquipmentModuleRepository.GetByProjectEquipmentIdAndCodeOrNull(
                equipment.Id,
                moduleCode,
                cancellationToken);
            if (existingModule != null)
            {
                return false;
            }

            DateTime createdUtc = DateTime.UtcNow;
            ProjectEquipmentModule projectEquipmentModule = CreateProjectEquipmentModule(equipment.ProjectId, equipment.Id, module, createdUtc);
            await mProjectEquipmentModuleRepository.Create(projectEquipmentModule, cancellationToken);
            return true;
        }

        public async Task<bool> UpdateModule(
            string projectCode,
            string equipmentCode,
            string originalModuleCode,
            ModuleItemViewModel module,
            CancellationToken cancellationToken = default)
        {
            await EnsureSampleProjectsSeeded(cancellationToken);

            ProjectEquipment? equipment = await GetProjectEquipmentByCodeOrNull(projectCode, equipmentCode, cancellationToken);
            if (equipment == null)
            {
                return false;
            }

            ProjectEquipmentModule? projectEquipmentModule = await mProjectEquipmentModuleRepository.GetByProjectEquipmentIdAndCodeOrNull(
                equipment.Id,
                originalModuleCode,
                cancellationToken);
            if (projectEquipmentModule == null)
            {
                return false;
            }

            string moduleCode = module.Code;
            ProjectEquipmentModule? duplicateModule = await mProjectEquipmentModuleRepository.GetByProjectEquipmentIdAndCodeOrNull(
                equipment.Id,
                moduleCode,
                cancellationToken);
            if (duplicateModule != null && !string.Equals(duplicateModule.Id, projectEquipmentModule.Id, StringComparison.Ordinal))
            {
                return false;
            }

            ApplyProjectEquipmentModule(projectEquipmentModule, module, DateTime.UtcNow);
            await mProjectEquipmentModuleRepository.Replace(projectEquipmentModule, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteModule(
            string projectCode,
            string equipmentCode,
            string moduleCode,
            CancellationToken cancellationToken = default)
        {
            await EnsureSampleProjectsSeeded(cancellationToken);

            ProjectEquipment? equipment = await GetProjectEquipmentByCodeOrNull(projectCode, equipmentCode, cancellationToken);
            if (equipment == null)
            {
                return false;
            }

            ProjectEquipmentModule? module = await mProjectEquipmentModuleRepository.GetByProjectEquipmentIdAndCodeOrNull(
                equipment.Id,
                moduleCode,
                cancellationToken);
            if (module == null)
            {
                return false;
            }

            await mProjectEquipmentModuleRepository.Delete(module.Id, cancellationToken);
            return true;
        }

        public async Task<bool> CanEditProject(
            string code,
            string userId,
            bool isAdmin,
            CancellationToken cancellationToken = default)
        {
            if (isAdmin)
            {
                return true;
            }

            Project? project = await GetProjectByCodeOrNull(code, cancellationToken);
            if (project == null)
            {
                return false;
            }

            if (userId.Length == 0)
            {
                return false;
            }

            if (string.Equals(project.OwnerUserId, userId, StringComparison.Ordinal))
            {
                return true;
            }

            List<ProjectMember> projectMembers = await mProjectMemberRepository.GetByProjectId(project.Id, cancellationToken);
            foreach (ProjectMember projectMember in projectMembers)
            {
                if (!string.Equals(projectMember.UserId, userId, StringComparison.Ordinal))
                {
                    continue;
                }

                if (CanProjectMemberEdit(projectMember.Role))
                {
                    return true;
                }
            }

            return false;
        }

#endregion

#region Private Methods

        private async Task EnsureSampleProjectsSeeded(CancellationToken cancellationToken)
        {
            FilterDefinition<ProjectCatalogSeedState> filter = Builders<ProjectCatalogSeedState>.Filter.Eq(seedState => seedState.Id, SAMPLE_PROJECTS_SEED_STATE_ID);
            ProjectCatalogSeedState? seedState = await mProjectCatalogSeedStates.Find(filter).FirstOrDefaultAsync(cancellationToken);
            if (seedState != null)
            {
                return;
            }

            List<Project> projects = await mProjectRepository.GetAll(cancellationToken);
            if (projects.Count == 0)
            {
                ProjectListPageViewModel samplePage = mSampleProjectCatalogService.GetProjectListPage();
                foreach (ProjectListItemViewModel sampleProject in samplePage.Projects)
                {
                    ProjectDashboardViewModel? dashboard = mSampleProjectCatalogService.GetProjectDashboardOrNull(sampleProject.Code);
                    if (dashboard != null)
                    {
                        await CreateProjectWithoutSeedCheck(dashboard, cancellationToken);
                    }
                }
            }

            ProjectCatalogSeedState newSeedState = new ProjectCatalogSeedState();
            newSeedState.Id = SAMPLE_PROJECTS_SEED_STATE_ID;
            newSeedState.CreatedUtc = DateTime.UtcNow;

            ReplaceOptions replaceOptions = new ReplaceOptions();
            replaceOptions.IsUpsert = true;
            await mProjectCatalogSeedStates.ReplaceOneAsync(filter, newSeedState, replaceOptions, cancellationToken);
        }

        private async Task<Project?> GetProjectByCodeOrNull(string code, CancellationToken cancellationToken)
        {
            return await mProjectRepository.GetByCodeOrNull(code, cancellationToken);
        }

        private async Task<ProjectEquipment?> GetProjectEquipmentByCodeOrNull(
            string projectCode,
            string equipmentCode,
            CancellationToken cancellationToken)
        {
            Project? project = await GetProjectByCodeOrNull(projectCode, cancellationToken);
            if (project == null)
            {
                return null;
            }

            ProjectEquipment? equipment = await mProjectEquipmentRepository.GetByProjectIdAndCodeOrNull(
                project.Id,
                equipmentCode,
                cancellationToken);
            return equipment;
        }

        private async Task CreateProjectWithoutSeedCheck(ProjectDashboardViewModel projectDashboard, CancellationToken cancellationToken)
        {
            DateTime createdUtc = DateTime.UtcNow;

            Project project = CreateProject(projectDashboard, createdUtc);
            await mProjectRepository.Create(project, cancellationToken);

            foreach (EquipmentSectionViewModel equipmentViewModel in projectDashboard.Equipments)
            {
                ProjectEquipment equipment = CreateProjectEquipment(project.Id, equipmentViewModel, createdUtc);
                await mProjectEquipmentRepository.Create(equipment, cancellationToken);

                foreach (ModuleItemViewModel moduleViewModel in equipmentViewModel.Modules)
                {
                    ProjectEquipmentModule module = CreateProjectEquipmentModule(project.Id, equipment.Id, moduleViewModel, createdUtc);
                    await mProjectEquipmentModuleRepository.Create(module, cancellationToken);
                }
            }
        }

        private Project CreateProject(ProjectDashboardViewModel projectDashboard, DateTime createdUtc)
        {
            Project project = new Project();
            project.CreatedUtc = createdUtc;
            ApplyProjectDashboard(project, projectDashboard, createdUtc);
            return project;
        }

        private void ApplyProjectDashboard(Project project, ProjectDashboardViewModel projectDashboard, DateTime updatedUtc)
        {
            project.Code = projectDashboard.Code;
            project.Name = projectDashboard.Name;
            project.TeamName = projectDashboard.TeamName;
            project.CustomerName = projectDashboard.CustomerName;
            project.OwnerName = projectDashboard.OwnerName;
            project.StatusName = projectDashboard.StatusName;
            project.StartDate = projectDashboard.StartDate;
            project.EndDate = projectDashboard.EndDate;
            project.Description = projectDashboard.Summary;
            project.UpdatedUtc = updatedUtc;
        }

        private ProjectEquipment CreateProjectEquipment(string projectId, EquipmentSectionViewModel equipmentViewModel, DateTime createdUtc)
        {
            ProjectEquipment equipment = new ProjectEquipment();
            equipment.ProjectId = projectId;
            equipment.IsActive = true;
            equipment.CreatedUtc = createdUtc;
            ApplyProjectEquipment(equipment, equipmentViewModel, createdUtc);
            return equipment;
        }

        private void ApplyProjectEquipment(ProjectEquipment equipment, EquipmentSectionViewModel equipmentViewModel, DateTime updatedUtc)
        {
            equipment.Code = equipmentViewModel.Code;
            equipment.Name = equipmentViewModel.Name;
            equipment.Description = equipmentViewModel.Description;
            equipment.StageName = equipmentViewModel.StageName;
            equipment.PlatformName = equipmentViewModel.PlatformName;
            equipment.UpdatedUtc = updatedUtc;
        }

        private ProjectEquipmentModule CreateProjectEquipmentModule(
            string projectId,
            string projectEquipmentId,
            ModuleItemViewModel moduleViewModel,
            DateTime createdUtc)
        {
            ProjectEquipmentModule module = new ProjectEquipmentModule();
            module.ProjectId = projectId;
            module.ProjectEquipmentId = projectEquipmentId;
            module.IsActive = true;
            module.CreatedUtc = createdUtc;
            ApplyProjectEquipmentModule(module, moduleViewModel, createdUtc);
            return module;
        }

        private void ApplyProjectEquipmentModule(ProjectEquipmentModule module, ModuleItemViewModel moduleViewModel, DateTime updatedUtc)
        {
            module.Code = moduleViewModel.Code;
            module.Name = moduleViewModel.Name;
            module.ModuleType = moduleViewModel.ModuleType;
            module.ManufacturerName = moduleViewModel.ManufacturerName;
            module.ModelName = moduleViewModel.ModelName;
            module.SerialNumbers = new List<string>(moduleViewModel.SerialNumbers);
            module.Drivers = CreateDrivers(moduleViewModel);
            module.PlatformSummary = moduleViewModel.PlatformSummary;
            module.Notes = moduleViewModel.Notes;
            module.PcSpec = CreatePcSpecOrNull(moduleViewModel);
            module.UpdatedUtc = updatedUtc;
        }

        private async Task<ProjectDashboardViewModel> CreateProjectDashboard(Project project, CancellationToken cancellationToken)
        {
            ProjectDashboardViewModel dashboard = new ProjectDashboardViewModel();
            dashboard.Code = project.Code;
            dashboard.Name = project.Name;
            dashboard.TeamName = project.TeamName;
            dashboard.CustomerName = project.CustomerName;
            dashboard.OwnerName = project.OwnerName;
            dashboard.StatusName = project.StatusName;
            dashboard.StartDate = project.StartDate;
            dashboard.EndDate = project.EndDate;
            dashboard.Summary = project.Description;

            List<ProjectEquipment> equipments = await mProjectEquipmentRepository.GetByProjectId(project.Id, cancellationToken);
            foreach (ProjectEquipment equipment in equipments)
            {
                EquipmentSectionViewModel equipmentViewModel = await CreateEquipmentSection(equipment, cancellationToken);
                dashboard.Equipments.Add(equipmentViewModel);
            }

            return dashboard;
        }

        private async Task<EquipmentSectionViewModel> CreateEquipmentSection(ProjectEquipment equipment, CancellationToken cancellationToken)
        {
            EquipmentSectionViewModel equipmentViewModel = new EquipmentSectionViewModel();
            equipmentViewModel.Code = equipment.Code;
            equipmentViewModel.Name = equipment.Name;
            equipmentViewModel.Description = equipment.Description;
            equipmentViewModel.StageName = equipment.StageName;
            equipmentViewModel.PlatformName = equipment.PlatformName;

            List<ProjectEquipmentModule> modules = await mProjectEquipmentModuleRepository.GetByProjectEquipmentId(equipment.Id, cancellationToken);
            foreach (ProjectEquipmentModule module in modules)
            {
                ModuleItemViewModel moduleViewModel = CreateModuleItem(module);
                equipmentViewModel.Modules.Add(moduleViewModel);
            }

            return equipmentViewModel;
        }

        private ModuleItemViewModel CreateModuleItem(ProjectEquipmentModule module)
        {
            ModuleItemViewModel moduleViewModel = new ModuleItemViewModel();
            moduleViewModel.Code = module.Code;
            moduleViewModel.ModuleType = module.ModuleType;
            moduleViewModel.Name = module.Name;
            moduleViewModel.ManufacturerName = module.ManufacturerName;
            moduleViewModel.ModelName = module.ModelName;
            moduleViewModel.SerialNumbers = new List<string>(module.SerialNumbers);
            moduleViewModel.Drivers = CreateDriverViewModels(module);
            moduleViewModel.PlatformSummary = module.PlatformSummary;
            moduleViewModel.Notes = module.Notes;
            ApplyPcSpec(moduleViewModel, module.PcSpec);
            return moduleViewModel;
        }

        private List<ProjectEquipmentModuleDriver> CreateDrivers(ModuleItemViewModel moduleViewModel)
        {
            List<ProjectEquipmentModuleDriver> drivers = new List<ProjectEquipmentModuleDriver>();
            foreach (ModuleDriverViewModel driverViewModel in moduleViewModel.Drivers)
            {
                ProjectEquipmentModuleDriver driver = new ProjectEquipmentModuleDriver();
                driver.Name = driverViewModel.Name;
                driver.Version = driverViewModel.Version;
                driver.Notes = driverViewModel.Notes;
                drivers.Add(driver);
            }

            return drivers;
        }

        private List<ModuleDriverViewModel> CreateDriverViewModels(ProjectEquipmentModule module)
        {
            List<ModuleDriverViewModel> drivers = new List<ModuleDriverViewModel>();
            foreach (ProjectEquipmentModuleDriver driver in module.Drivers)
            {
                ModuleDriverViewModel driverViewModel = new ModuleDriverViewModel();
                driverViewModel.Name = driver.Name;
                driverViewModel.Version = driver.Version;
                driverViewModel.Notes = driver.Notes;
                drivers.Add(driverViewModel);
            }

            return drivers;
        }

        private ProjectEquipmentModulePcSpec? CreatePcSpecOrNull(ModuleItemViewModel moduleViewModel)
        {
            if (moduleViewModel.ModuleType != EProjectEquipmentModuleType.Pc)
            {
                return null;
            }

            ProjectEquipmentModulePcSpec pcSpec = new ProjectEquipmentModulePcSpec();
            pcSpec.RoleName = moduleViewModel.PcRoleName;
            pcSpec.CpuModel = moduleViewModel.PcCpuModel;
            pcSpec.MemorySpec = moduleViewModel.PcMemorySpec;
            pcSpec.StorageSpec = moduleViewModel.PcStorageSpec;
            pcSpec.GpuModel = moduleViewModel.PcGpuModel;
            pcSpec.OsName = moduleViewModel.PcOsName;
            pcSpec.OsVersion = moduleViewModel.PcOsVersion;
            pcSpec.MainApplicationName = moduleViewModel.PcMainApplicationName;
            pcSpec.NetworkNotes = moduleViewModel.PcNetworkNotes;
            return pcSpec;
        }

        private void ApplyPcSpec(ModuleItemViewModel moduleViewModel, ProjectEquipmentModulePcSpec? pcSpec)
        {
            if (pcSpec == null)
            {
                return;
            }

            moduleViewModel.PcRoleName = pcSpec.RoleName;
            moduleViewModel.PcCpuModel = pcSpec.CpuModel;
            moduleViewModel.PcMemorySpec = pcSpec.MemorySpec;
            moduleViewModel.PcStorageSpec = pcSpec.StorageSpec;
            moduleViewModel.PcGpuModel = pcSpec.GpuModel;
            moduleViewModel.PcOsName = pcSpec.OsName;
            moduleViewModel.PcOsVersion = pcSpec.OsVersion;
            moduleViewModel.PcMainApplicationName = pcSpec.MainApplicationName;
            moduleViewModel.PcNetworkNotes = pcSpec.NetworkNotes;
        }

        private ProjectListItemViewModel CreateProjectListItem(Project project, int equipmentCount, int moduleCount)
        {
            ProjectListItemViewModel item = new ProjectListItemViewModel();
            item.Code = project.Code;
            item.Name = project.Name;
            item.TeamName = project.TeamName;
            item.CustomerName = project.CustomerName;
            item.OwnerName = project.OwnerName;
            item.StatusName = project.StatusName;
            item.StartDate = project.StartDate;
            item.EndDate = project.EndDate;
            item.Summary = project.Description;
            item.EquipmentCount = equipmentCount;
            item.ModuleCount = moduleCount;
            return item;
        }

        private async Task<int> CountModules(List<ProjectEquipment> equipments, CancellationToken cancellationToken)
        {
            int count = 0;
            foreach (ProjectEquipment equipment in equipments)
            {
                List<ProjectEquipmentModule> modules = await mProjectEquipmentModuleRepository.GetByProjectEquipmentId(equipment.Id, cancellationToken);
                count += modules.Count;
            }

            return count;
        }

        private bool CanProjectMemberEdit(EProjectMemberRole role)
        {
            switch (role)
            {
                case EProjectMemberRole.Owner:
                case EProjectMemberRole.Maintainer:
                case EProjectMemberRole.Developer:
                    return true;
                case EProjectMemberRole.Member:
                case EProjectMemberRole.Viewer:
                    return false;
                default:
                    return false;
            }
        }

#endregion
    }
}
