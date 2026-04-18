using Vanta.Models;

namespace Vanta.Repositories.Projects
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetAll(CancellationToken cancellationToken = default);

        Task<Project?> GetByIdOrNull(string id, CancellationToken cancellationToken = default);

        Task<Project?> GetByCodeOrNull(string code, CancellationToken cancellationToken = default);

        Task Create(Project project, CancellationToken cancellationToken = default);

        Task Replace(Project project, CancellationToken cancellationToken = default);

        Task Delete(string id, CancellationToken cancellationToken = default);
    }
}
