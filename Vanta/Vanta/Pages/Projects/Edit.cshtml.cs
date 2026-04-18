using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vanta.Models;
using Vanta.Services.Projects;
using Vanta.ViewModels;

namespace Vanta.Pages.Projects
{
    public class EditModel : PageModel
    {
        #region Fields

        private readonly IProjectCatalogService mProjectCatalogService;

        #endregion

        #region Public Properties

        public ProjectDashboardViewModel Project { get; private set; } = new ProjectDashboardViewModel();

        public bool HasProject { get; private set; }

        [BindProperty]
        public string OriginalCode { get; set; } = string.Empty;

        [BindProperty]
        public string Code { get; set; } = string.Empty;

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public string CustomerName { get; set; } = string.Empty;

        [BindProperty]
        public string TeamName { get; set; } = string.Empty;

        [BindProperty]
        public string OwnerName { get; set; } = string.Empty;

        [BindProperty]
        public string StatusName { get; set; } = "Planning";

        [BindProperty]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [BindProperty]
        public DateTime? EndDate { get; set; }

        [BindProperty]
        public string Summary { get; set; } = string.Empty;

        [BindProperty]
        public string TargetProjectCode { get; set; } = string.Empty;

        [BindProperty]
        public string EquipmentOriginalCode { get; set; } = string.Empty;

        [BindProperty]
        public string EquipmentCode { get; set; } = string.Empty;

        [BindProperty]
        public string EquipmentName { get; set; } = string.Empty;

        [BindProperty]
        public string EquipmentStageName { get; set; } = string.Empty;

        [BindProperty]
        public string EquipmentPlatformName { get; set; } = string.Empty;

        [BindProperty]
        public string EquipmentDescription { get; set; } = string.Empty;

        [BindProperty]
        public string ModuleEquipmentCode { get; set; } = string.Empty;

        [BindProperty]
        public string ModuleOriginalCode { get; set; } = string.Empty;

        [BindProperty]
        public string ModuleCode { get; set; } = string.Empty;

        [BindProperty]
        public EProjectEquipmentModuleType ModuleType { get; set; } = EProjectEquipmentModuleType.Other;

        [BindProperty]
        public string ModuleName { get; set; } = string.Empty;

        [BindProperty]
        public string ModuleManufacturerName { get; set; } = string.Empty;

        [BindProperty]
        public string ModuleModelName { get; set; } = string.Empty;

        [BindProperty]
        public string ModuleSerialNumbersText { get; set; } = string.Empty;

        [BindProperty]
        public List<ModuleDriverInput> ModuleDrivers { get; set; } = new List<ModuleDriverInput>();

        [BindProperty]
        public string ModulePlatformSummary { get; set; } = string.Empty;

        [BindProperty]
        public string ModuleNotes { get; set; } = string.Empty;

        [BindProperty]
        public string ModulePcRoleName { get; set; } = string.Empty;

        [BindProperty]
        public string ModulePcCpuModel { get; set; } = string.Empty;

        [BindProperty]
        public string ModulePcMemorySpec { get; set; } = string.Empty;

        [BindProperty]
        public string ModulePcStorageSpec { get; set; } = string.Empty;

        [BindProperty]
        public string ModulePcGpuModel { get; set; } = string.Empty;

        [BindProperty]
        public string ModulePcOsName { get; set; } = string.Empty;

        [BindProperty]
        public string ModulePcOsVersion { get; set; } = string.Empty;

        [BindProperty]
        public string ModulePcMainApplicationName { get; set; } = string.Empty;

        [BindProperty]
        public string ModulePcNetworkNotes { get; set; } = string.Empty;

        #endregion

        #region Constructors

        public EditModel(IProjectCatalogService projectCatalogService)
        {
            mProjectCatalogService = projectCatalogService;
        }

        #endregion

        #region Public Methods

        public async Task<IActionResult> OnGet(string code)
        {
            bool loaded = await LoadProject(code, true, HttpContext.RequestAborted);
            if (!loaded)
            {
                return NotFound();
            }

            bool canEdit = await CanCurrentUserEditProject(code, HttpContext.RequestAborted);
            if (!canEdit)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSaveProject()
        {
            bool canEdit = await CanCurrentUserEditProject(OriginalCode, HttpContext.RequestAborted);
            if (!canEdit)
            {
                return Forbid();
            }

            ValidateProjectInput();
            if (!ModelState.IsValid)
            {
                await LoadProject(OriginalCode, false, HttpContext.RequestAborted);
                return Page();
            }

            ProjectDashboardViewModel dashboard = BuildProjectInput();
            bool saved = await mProjectCatalogService.UpdateProject(
                OriginalCode,
                dashboard,
                HttpContext.RequestAborted);

            if (!saved)
            {
                ModelState.AddModelError(nameof(Code), "Project code already exists or the project was not found.");
                await LoadProject(OriginalCode, false, HttpContext.RequestAborted);
                return Page();
            }

            return RedirectToPage("/Projects/Edit", new { code = dashboard.Code });
        }

        public async Task<IActionResult> OnPostCreateEquipment()
        {
            bool canEdit = await CanCurrentUserEditProject(TargetProjectCode, HttpContext.RequestAborted);
            if (!canEdit)
            {
                return Forbid();
            }

            EquipmentSectionViewModel equipment = BuildEquipmentInput();
            if (HasRequiredEquipmentInput(equipment))
            {
                await mProjectCatalogService.CreateEquipment(TargetProjectCode, equipment, HttpContext.RequestAborted);
            }

            return RedirectToEditPage();
        }

        public async Task<IActionResult> OnPostUpdateEquipment()
        {
            bool canEdit = await CanCurrentUserEditProject(TargetProjectCode, HttpContext.RequestAborted);
            if (!canEdit)
            {
                return Forbid();
            }

            EquipmentSectionViewModel equipment = BuildEquipmentInput();
            if (!string.IsNullOrWhiteSpace(EquipmentOriginalCode) && HasRequiredEquipmentInput(equipment))
            {
                await mProjectCatalogService.UpdateEquipment(
                    TargetProjectCode,
                    EquipmentOriginalCode,
                    equipment,
                    HttpContext.RequestAborted);
            }

            return RedirectToEditPage();
        }

        public async Task<IActionResult> OnPostDeleteEquipment()
        {
            bool canEdit = await CanCurrentUserEditProject(TargetProjectCode, HttpContext.RequestAborted);
            if (!canEdit)
            {
                return Forbid();
            }

            if (!string.IsNullOrWhiteSpace(EquipmentOriginalCode))
            {
                await mProjectCatalogService.DeleteEquipment(
                    TargetProjectCode,
                    EquipmentOriginalCode,
                    HttpContext.RequestAborted);
            }

            return RedirectToEditPage();
        }

        public async Task<IActionResult> OnPostCreateModule()
        {
            bool canEdit = await CanCurrentUserEditProject(TargetProjectCode, HttpContext.RequestAborted);
            if (!canEdit)
            {
                return Forbid();
            }

            ModuleItemViewModel module = BuildModuleInput();
            if (!string.IsNullOrWhiteSpace(ModuleEquipmentCode) && HasRequiredModuleInput(module))
            {
                await mProjectCatalogService.CreateModule(
                    TargetProjectCode,
                    ModuleEquipmentCode,
                    module,
                    HttpContext.RequestAborted);
            }

            return RedirectToEditPage();
        }

        public async Task<IActionResult> OnPostUpdateModule()
        {
            bool canEdit = await CanCurrentUserEditProject(TargetProjectCode, HttpContext.RequestAborted);
            if (!canEdit)
            {
                return Forbid();
            }

            ModuleItemViewModel module = BuildModuleInput();
            if (!string.IsNullOrWhiteSpace(ModuleEquipmentCode)
                && !string.IsNullOrWhiteSpace(ModuleOriginalCode)
                && HasRequiredModuleInput(module))
            {
                await mProjectCatalogService.UpdateModule(
                    TargetProjectCode,
                    ModuleEquipmentCode,
                    ModuleOriginalCode,
                    module,
                    HttpContext.RequestAborted);
            }

            return RedirectToEditPage();
        }

        public async Task<IActionResult> OnPostDeleteModule()
        {
            bool canEdit = await CanCurrentUserEditProject(TargetProjectCode, HttpContext.RequestAborted);
            if (!canEdit)
            {
                return Forbid();
            }

            if (!string.IsNullOrWhiteSpace(ModuleEquipmentCode) && !string.IsNullOrWhiteSpace(ModuleOriginalCode))
            {
                await mProjectCatalogService.DeleteModule(
                    TargetProjectCode,
                    ModuleEquipmentCode,
                    ModuleOriginalCode,
                    HttpContext.RequestAborted);
            }

            return RedirectToEditPage();
        }

        #endregion

        #region Private Methods

        private async Task<bool> LoadProject(string code, bool populateForm, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            ProjectDashboardViewModel? project = await mProjectCatalogService.GetProjectDashboardOrNull(code, cancellationToken);
            if (project == null)
            {
                return false;
            }

            Project = project;
            HasProject = true;

            if (populateForm)
            {
                PopulateProjectForm(project);
            }

            return true;
        }

        private async Task<bool> CanCurrentUserEditProject(string code, CancellationToken cancellationToken)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            bool isAdmin = User.IsInRole("Admin");

            return await mProjectCatalogService.CanEditProject(
                code,
                userId,
                isAdmin,
                cancellationToken);
        }

        private void ValidateProjectInput()
        {
            AddRequiredError(Code, nameof(Code), "Project code is required.");
            AddRequiredError(Name, nameof(Name), "Project name is required.");
            AddRequiredError(CustomerName, nameof(CustomerName), "Customer is required.");
            AddRequiredError(TeamName, nameof(TeamName), "Team is required.");
            AddRequiredError(OwnerName, nameof(OwnerName), "Owner is required.");
            AddRequiredError(StatusName, nameof(StatusName), "Status is required.");
        }

        private void AddRequiredError(string? value, string key, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                ModelState.AddModelError(key, message);
            }
        }

        private ProjectDashboardViewModel BuildProjectInput()
        {
            ProjectDashboardViewModel dashboard = new ProjectDashboardViewModel();
            dashboard.Code = Code;
            dashboard.Name = Name;
            dashboard.CustomerName = CustomerName;
            dashboard.TeamName = TeamName;
            dashboard.OwnerName = OwnerName;
            dashboard.StatusName = StatusName;
            dashboard.StartDate = StartDate;
            dashboard.EndDate = EndDate;
            dashboard.Summary = Summary;
            return dashboard;
        }

        private void PopulateProjectForm(ProjectDashboardViewModel project)
        {
            OriginalCode = project.Code;
            Code = project.Code;
            Name = project.Name;
            CustomerName = project.CustomerName;
            TeamName = project.TeamName;
            OwnerName = project.OwnerName;
            StatusName = project.StatusName;
            StartDate = project.StartDate;
            EndDate = project.EndDate;
            Summary = project.Summary;
        }

        private IActionResult RedirectToEditPage()
        {
            if (string.IsNullOrWhiteSpace(TargetProjectCode))
            {
                return RedirectToPage("/Index");
            }

            return RedirectToPage("/Projects/Edit", new { code = TargetProjectCode });
        }

        private EquipmentSectionViewModel BuildEquipmentInput()
        {
            EquipmentSectionViewModel equipment = new EquipmentSectionViewModel();
            equipment.Code = EquipmentCode;
            equipment.Name = EquipmentName;
            equipment.StageName = EquipmentStageName;
            equipment.PlatformName = EquipmentPlatformName;
            equipment.Description = EquipmentDescription;
            return equipment;
        }

        private ModuleItemViewModel BuildModuleInput()
        {
            ModuleItemViewModel module = new ModuleItemViewModel();
            module.Code = ModuleCode;
            module.ModuleType = ModuleType;
            module.Name = ModuleName;
            module.ManufacturerName = ModuleManufacturerName;
            module.ModelName = ModuleModelName;
            module.SerialNumbers = ParseSerialNumbers(ModuleSerialNumbersText);
            module.Drivers = BuildDrivers();
            module.PlatformSummary = ModulePlatformSummary;
            module.Notes = ModuleNotes;

            if (ModuleType == EProjectEquipmentModuleType.Pc)
            {
                module.PcRoleName = ModulePcRoleName;
                module.PcCpuModel = ModulePcCpuModel;
                module.PcMemorySpec = ModulePcMemorySpec;
                module.PcStorageSpec = ModulePcStorageSpec;
                module.PcGpuModel = ModulePcGpuModel;
                module.PcOsName = ModulePcOsName;
                module.PcOsVersion = ModulePcOsVersion;
                module.PcMainApplicationName = ModulePcMainApplicationName;
                module.PcNetworkNotes = ModulePcNetworkNotes;
            }

            return module;
        }

        private bool HasRequiredEquipmentInput(EquipmentSectionViewModel equipment)
        {
            return !string.IsNullOrWhiteSpace(equipment.Code)
                && !string.IsNullOrWhiteSpace(equipment.Name);
        }

        private bool HasRequiredModuleInput(ModuleItemViewModel module)
        {
            return !string.IsNullOrWhiteSpace(module.Code)
                && !string.IsNullOrWhiteSpace(module.Name)
                && !string.IsNullOrWhiteSpace(module.ManufacturerName)
                && !string.IsNullOrWhiteSpace(module.ModelName);
        }

        private List<string> ParseSerialNumbers(string? serialNumbersText)
        {
            List<string> serialNumbers = new List<string>();
            if (string.IsNullOrWhiteSpace(serialNumbersText))
            {
                return serialNumbers;
            }

            string[] items = serialNumbersText.Split(
                new char[] { '\r', '\n', ',', ';' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string item in items)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    serialNumbers.Add(item);
                }
            }

            return serialNumbers;
        }

        private List<ModuleDriverViewModel> BuildDrivers()
        {
            List<ModuleDriverViewModel> drivers = new List<ModuleDriverViewModel>();
            if (ModuleDrivers == null)
            {
                return drivers;
            }

            foreach (ModuleDriverInput input in ModuleDrivers)
            {
                if (input == null || string.IsNullOrWhiteSpace(input.Name))
                {
                    continue;
                }

                ModuleDriverViewModel driver = new ModuleDriverViewModel();
                driver.Name = input.Name;
                driver.Version = input.Version;
                driver.Notes = input.Notes;
                drivers.Add(driver);
            }

            return drivers;
        }

        #endregion

        #region Input Models

        public class ModuleDriverInput
        {
            public string Name { get; set; } = string.Empty;

            public string Version { get; set; } = string.Empty;

            public string Notes { get; set; } = string.Empty;
        }

        #endregion
    }
}
