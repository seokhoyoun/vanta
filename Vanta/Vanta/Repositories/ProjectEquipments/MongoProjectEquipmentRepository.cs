using MongoDB.Driver;
using Vanta.Infrastructure.Mongo;
using Vanta.Models;

namespace Vanta.Repositories.ProjectEquipments
{
    public class MongoProjectEquipmentRepository : IProjectEquipmentRepository
    {
#region Fields

        private readonly IMongoCollection<ProjectEquipment> mProjectEquipments;

#endregion

#region Constructors

        public MongoProjectEquipmentRepository(IMongoCollectionContext mongoCollectionContext)
        {
            mProjectEquipments = mongoCollectionContext.ProjectEquipments;
        }

#endregion

#region Public Methods

        public Task<List<ProjectEquipment>> GetByProjectId(string projectId, CancellationToken cancellationToken = default)
        {
            FilterDefinition<ProjectEquipment> filter = Builders<ProjectEquipment>.Filter.Eq(projectEquipment => projectEquipment.ProjectId, projectId);
            return mProjectEquipments.Find(filter)
                .SortBy(projectEquipment => projectEquipment.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProjectEquipment?> GetByIdOrNull(string id, CancellationToken cancellationToken = default)
        {
            FilterDefinition<ProjectEquipment> filter = Builders<ProjectEquipment>.Filter.Eq(projectEquipment => projectEquipment.Id, id);
            ProjectEquipment? projectEquipment = await mProjectEquipments.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return projectEquipment;
        }

        public async Task<ProjectEquipment?> GetByProjectIdAndCodeOrNull(string projectId, string code, CancellationToken cancellationToken = default)
        {
            FilterDefinition<ProjectEquipment> filter = Builders<ProjectEquipment>.Filter.And(
                Builders<ProjectEquipment>.Filter.Eq(projectEquipment => projectEquipment.ProjectId, projectId),
                Builders<ProjectEquipment>.Filter.Eq(projectEquipment => projectEquipment.Code, code));

            ProjectEquipment? projectEquipment = await mProjectEquipments.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return projectEquipment;
        }

        public Task Create(ProjectEquipment projectEquipment, CancellationToken cancellationToken = default)
        {
            return mProjectEquipments.InsertOneAsync(projectEquipment, cancellationToken: cancellationToken);
        }

        public Task Replace(ProjectEquipment projectEquipment, CancellationToken cancellationToken = default)
        {
            FilterDefinition<ProjectEquipment> filter = Builders<ProjectEquipment>.Filter.Eq(existingProjectEquipment => existingProjectEquipment.Id, projectEquipment.Id);
            return mProjectEquipments.ReplaceOneAsync(filter, projectEquipment, cancellationToken: cancellationToken);
        }

        public Task Delete(string id, CancellationToken cancellationToken = default)
        {
            FilterDefinition<ProjectEquipment> filter = Builders<ProjectEquipment>.Filter.Eq(projectEquipment => projectEquipment.Id, id);
            return mProjectEquipments.DeleteOneAsync(filter, cancellationToken);
        }

#endregion
    }
}
