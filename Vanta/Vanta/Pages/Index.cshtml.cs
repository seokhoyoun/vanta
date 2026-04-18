using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vanta.Services.Projects;
using Vanta.ViewModels;

namespace Vanta.Pages
{
    public class IndexModel : PageModel
    {
#region Fields

        private readonly IProjectCatalogService mProjectCatalogService;

#endregion

#region Public Properties

        public ProjectListPageViewModel PageData { get; private set; } = new ProjectListPageViewModel();

        public ProjectDashboardViewModel SelectedProject { get; private set; } = new ProjectDashboardViewModel();

        public bool HasSelectedProject { get; private set; }

        public bool HasSelectedProjectEditPermission { get; private set; }

#endregion

#region Constructors

        public IndexModel(IProjectCatalogService projectCatalogService)
        {
            mProjectCatalogService = projectCatalogService;
        }

#endregion

#region Public Methods

        public async Task OnGet(string code = "")
        {
            await LoadPageData(code, HttpContext.RequestAborted);
        }

#endregion

#region Private Methods

        private async Task LoadPageData(string code = "", CancellationToken cancellationToken = default)
        {
            PageData = await mProjectCatalogService.GetProjectListPage(cancellationToken);
            HasSelectedProject = false;
            HasSelectedProjectEditPermission = false;
            SelectedProject = new ProjectDashboardViewModel();

            if (string.IsNullOrWhiteSpace(code))
            {
                return;
            }

            ProjectDashboardViewModel? project = await mProjectCatalogService.GetProjectDashboardOrNull(code.Trim(), cancellationToken);
            if (project == null)
            {
                return;
            }

            SelectedProject = project;
            HasSelectedProject = true;
            HasSelectedProjectEditPermission = await CanCurrentUserEditProject(project.Code, cancellationToken);
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

#endregion
    }
}
