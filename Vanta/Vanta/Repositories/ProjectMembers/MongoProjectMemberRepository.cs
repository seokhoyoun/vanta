using MongoDB.Driver;
using Vanta.Infrastructure.Mongo;
using Vanta.Models;

namespace Vanta.Repositories.ProjectMembers
{
    public class MongoProjectMemberRepository : IProjectMemberRepository
    {
#region Fields

        private readonly IMongoCollection<ProjectMember> mProjectMembers;

#endregion

#region Constructors

        public MongoProjectMemberRepository(IMongoCollectionContext mongoCollectionContext)
        {
            mProjectMembers = mongoCollectionContext.ProjectMembers;
        }

#endregion

#region Public Methods

        public Task<List<ProjectMember>> GetByProjectId(string projectId, CancellationToken cancellationToken = default)
        {
            FilterDefinition<ProjectMember> filter = Builders<ProjectMember>.Filter.Eq(projectMember => projectMember.ProjectId, projectId);
            return mProjectMembers.Find(filter)
                .SortBy(projectMember => projectMember.JoinedUtc)
                .ToListAsync(cancellationToken);
        }

        public Task<List<ProjectMember>> GetByUserId(string userId, CancellationToken cancellationToken = default)
        {
            FilterDefinition<ProjectMember> filter = Builders<ProjectMember>.Filter.Eq(projectMember => projectMember.UserId, userId);
            return mProjectMembers.Find(filter)
                .SortBy(projectMember => projectMember.JoinedUtc)
                .ToListAsync(cancellationToken);
        }

        public Task Create(ProjectMember projectMember, CancellationToken cancellationToken = default)
        {
            return mProjectMembers.InsertOneAsync(projectMember, cancellationToken: cancellationToken);
        }

        public Task Replace(ProjectMember projectMember, CancellationToken cancellationToken = default)
        {
            FilterDefinition<ProjectMember> filter = Builders<ProjectMember>.Filter.Eq(existingProjectMember => existingProjectMember.Id, projectMember.Id);
            return mProjectMembers.ReplaceOneAsync(filter, projectMember, cancellationToken: cancellationToken);
        }

        public Task Delete(string id, CancellationToken cancellationToken = default)
        {
            FilterDefinition<ProjectMember> filter = Builders<ProjectMember>.Filter.Eq(projectMember => projectMember.Id, id);
            return mProjectMembers.DeleteOneAsync(filter, cancellationToken);
        }

#endregion
    }
}
