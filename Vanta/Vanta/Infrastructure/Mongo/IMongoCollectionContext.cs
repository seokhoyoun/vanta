using MongoDB.Driver;
using Vanta.Models;

namespace Vanta.Infrastructure.Mongo
{
    public interface IMongoCollectionContext
    {
        IMongoCollection<User> Users { get; }

        IMongoCollection<Project> Projects { get; }

        IMongoCollection<ProjectMember> ProjectMembers { get; }

        IMongoCollection<ProjectEquipment> ProjectEquipments { get; }

        IMongoCollection<ProjectEquipmentModule> ProjectEquipmentModules { get; }

        IMongoCollection<ProjectCatalogSeedState> ProjectCatalogSeedStates { get; }
    }
}
