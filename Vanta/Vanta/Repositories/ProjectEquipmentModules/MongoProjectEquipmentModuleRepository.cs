using MongoDB.Driver;
using Vanta.Infrastructure.Mongo;
using Vanta.Models;

namespace Vanta.Repositories.ProjectEquipmentModules
{
    public class MongoProjectEquipmentModuleRepository : IProjectEquipmentModuleRepository
    {
#region Fields

        private readonly IMongoCollection<ProjectEquipmentModule> mProjectEquipmentModules;

#endregion

#region Constructors

        public MongoProjectEquipmentModuleRepository(IMongoCollectionContext mongoCollectionContext)
        {
            mProjectEquipmentModules = mongoCollectionContext.ProjectEquipmentModules;
        }

#endregion

#region Public Methods

        public Task<List<ProjectEquipmentModule>> GetByProjectEquipmentId(string projectEquipmentId, CancellationToken cancellationToken = default)
        {
            FilterDefinition<ProjectEquipmentModule> filter = Builders<ProjectEquipmentModule>.Filter.Eq(
                projectEquipmentModule => projectEquipmentModule.ProjectEquipmentId,
                projectEquipmentId);

            return mProjectEquipmentModules.Find(filter)
                .SortBy(projectEquipmentModule => projectEquipmentModule.ModuleType)
                .ThenBy(projectEquipmentModule => projectEquipmentModule.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProjectEquipmentModule?> GetByIdOrNull(string id, CancellationToken cancellationToken = default)
        {
            FilterDefinition<ProjectEquipmentModule> filter = Builders<ProjectEquipmentModule>.Filter.Eq(projectEquipmentModule => projectEquipmentModule.Id, id);
            ProjectEquipmentModule? projectEquipmentModule = await mProjectEquipmentModules.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return projectEquipmentModule;
        }

        public async Task<ProjectEquipmentModule?> GetByProjectEquipmentIdAndCodeOrNull(string projectEquipmentId, string code, CancellationToken cancellationToken = default)
        {
            FilterDefinition<ProjectEquipmentModule> filter = Builders<ProjectEquipmentModule>.Filter.And(
                Builders<ProjectEquipmentModule>.Filter.Eq(projectEquipmentModule => projectEquipmentModule.ProjectEquipmentId, projectEquipmentId),
                Builders<ProjectEquipmentModule>.Filter.Eq(projectEquipmentModule => projectEquipmentModule.Code, code));

            ProjectEquipmentModule? projectEquipmentModule = await mProjectEquipmentModules.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return projectEquipmentModule;
        }

        public Task Create(ProjectEquipmentModule projectEquipmentModule, CancellationToken cancellationToken = default)
        {
            return mProjectEquipmentModules.InsertOneAsync(projectEquipmentModule, cancellationToken: cancellationToken);
        }

        public Task Replace(ProjectEquipmentModule projectEquipmentModule, CancellationToken cancellationToken = default)
        {
            FilterDefinition<ProjectEquipmentModule> filter = Builders<ProjectEquipmentModule>.Filter.Eq(
                existingProjectEquipmentModule => existingProjectEquipmentModule.Id,
                projectEquipmentModule.Id);

            return mProjectEquipmentModules.ReplaceOneAsync(filter, projectEquipmentModule, cancellationToken: cancellationToken);
        }

        public Task Delete(string id, CancellationToken cancellationToken = default)
        {
            FilterDefinition<ProjectEquipmentModule> filter = Builders<ProjectEquipmentModule>.Filter.Eq(projectEquipmentModule => projectEquipmentModule.Id, id);
            return mProjectEquipmentModules.DeleteOneAsync(filter, cancellationToken);
        }

#endregion
    }
}
