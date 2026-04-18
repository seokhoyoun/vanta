using Vanta.Models;

namespace Vanta.Repositories.ProjectMembers
{
    public interface IProjectMemberRepository
    {
        Task<List<ProjectMember>> GetByProjectId(string projectId, CancellationToken cancellationToken = default);

        Task<List<ProjectMember>> GetByUserId(string userId, CancellationToken cancellationToken = default);

        Task Create(ProjectMember projectMember, CancellationToken cancellationToken = default);

        Task Replace(ProjectMember projectMember, CancellationToken cancellationToken = default);

        Task Delete(string id, CancellationToken cancellationToken = default);
    }
}
