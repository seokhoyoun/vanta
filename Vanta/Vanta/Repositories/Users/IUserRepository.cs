using Vanta.Models;

namespace Vanta.Repositories.Users
{
    public interface IUserRepository
    {
        Task<bool> HasAny(CancellationToken cancellationToken = default);

        Task<List<User>> GetAll(CancellationToken cancellationToken = default);

        Task<User?> GetByIdOrNull(string id, CancellationToken cancellationToken = default);

        Task<User?> GetByLoginIdOrNull(string loginId, CancellationToken cancellationToken = default);

        Task Create(User user, CancellationToken cancellationToken = default);

        Task Replace(User user, CancellationToken cancellationToken = default);
    }
}
