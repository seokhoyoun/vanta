using MongoDB.Driver;
using Vanta.Infrastructure.Mongo;
using Vanta.Models;

namespace Vanta.Repositories.Projects
{
    public class MongoProjectRepository : IProjectRepository
    {
#region Fields

        private readonly IMongoCollection<Project> mProjects;

#endregion

#region Constructors

        public MongoProjectRepository(IMongoCollectionContext mongoCollectionContext)
        {
            mProjects = mongoCollectionContext.Projects;
        }

#endregion

#region Public Methods

        public Task<List<Project>> GetAll(CancellationToken cancellationToken = default)
        {
            return mProjects.Find(Builders<Project>.Filter.Empty)
                .SortBy(project => project.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Project?> GetByIdOrNull(string id, CancellationToken cancellationToken = default)
        {
            FilterDefinition<Project> filter = Builders<Project>.Filter.Eq(project => project.Id, id);
            Project? project = await mProjects.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return project;
        }

        public async Task<Project?> GetByCodeOrNull(string code, CancellationToken cancellationToken = default)
        {
            FilterDefinition<Project> filter = Builders<Project>.Filter.Eq(project => project.Code, code);
            Project? project = await mProjects.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return project;
        }

        public Task Create(Project project, CancellationToken cancellationToken = default)
        {
            return mProjects.InsertOneAsync(project, cancellationToken: cancellationToken);
        }

        public Task Replace(Project project, CancellationToken cancellationToken = default)
        {
            FilterDefinition<Project> filter = Builders<Project>.Filter.Eq(existingProject => existingProject.Id, project.Id);
            return mProjects.ReplaceOneAsync(filter, project, cancellationToken: cancellationToken);
        }

        public Task Delete(string id, CancellationToken cancellationToken = default)
        {
            FilterDefinition<Project> filter = Builders<Project>.Filter.Eq(project => project.Id, id);
            return mProjects.DeleteOneAsync(filter, cancellationToken);
        }

#endregion
    }
}
