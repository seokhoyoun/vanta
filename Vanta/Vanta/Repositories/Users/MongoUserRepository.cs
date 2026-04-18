using MongoDB.Driver;
using Vanta.Infrastructure.Mongo;
using Vanta.Models;

namespace Vanta.Repositories.Users
{
    public class MongoUserRepository : IUserRepository
    {
#region Fields

        private readonly IMongoCollection<User> mUsers;

#endregion

#region Constructors

        public MongoUserRepository(IMongoCollectionContext mongoCollectionContext)
        {
            mUsers = mongoCollectionContext.Users;
        }

#endregion

#region Public Methods

        public async Task<bool> HasAny(CancellationToken cancellationToken = default)
        {
            long count = await mUsers.CountDocumentsAsync(
                Builders<User>.Filter.Empty,
                cancellationToken: cancellationToken);
            return count > 0;
        }

        public Task<List<User>> GetAll(CancellationToken cancellationToken = default)
        {
            return mUsers.Find(Builders<User>.Filter.Empty)
                .SortBy(user => user.DisplayName)
                .ToListAsync(cancellationToken);
        }

        public async Task<User?> GetByIdOrNull(string id, CancellationToken cancellationToken = default)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(user => user.Id, id);
            User? user = await mUsers.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return user;
        }

        public async Task<User?> GetByLoginIdOrNull(string loginId, CancellationToken cancellationToken = default)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(user => user.LoginId, loginId);
            User? user = await mUsers.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return user;
        }

        public Task Create(User user, CancellationToken cancellationToken = default)
        {
            return mUsers.InsertOneAsync(user, cancellationToken: cancellationToken);
        }

        public Task Replace(User user, CancellationToken cancellationToken = default)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(existingUser => existingUser.Id, user.Id);
            return mUsers.ReplaceOneAsync(filter, user, cancellationToken: cancellationToken);
        }

#endregion
    }
}
