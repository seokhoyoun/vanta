using MongoDB.Driver;
using Vanta.Models;

namespace Vanta.Infrastructure.Mongo
{
    public class MongoCollectionContext : IMongoCollectionContext
    {
#region Public Properties

        public IMongoCollection<User> Users { get; }

        public IMongoCollection<Project> Projects { get; }

        public IMongoCollection<ProjectMember> ProjectMembers { get; }

        public IMongoCollection<ProjectEquipment> ProjectEquipments { get; }

        public IMongoCollection<ProjectEquipmentModule> ProjectEquipmentModules { get; }

        public IMongoCollection<ProjectCatalogSeedState> ProjectCatalogSeedStates { get; }

#endregion

#region Fields

        private const string USERS_COLLECTION_NAME = "users";
        private const string PROJECTS_COLLECTION_NAME = "projects";
        private const string PROJECT_MEMBERS_COLLECTION_NAME = "project_members";
        private const string PROJECT_EQUIPMENTS_COLLECTION_NAME = "project_equipments";
        private const string PROJECT_EQUIPMENT_MODULES_COLLECTION_NAME = "project_equipment_modules";
        private const string PROJECT_CATALOG_SEED_STATES_COLLECTION_NAME = "project_catalog_seed_states";

#endregion

#region Constructors

        public MongoCollectionContext(IMongoDatabase mongoDatabase)
        {
            Users = mongoDatabase.GetCollection<User>(USERS_COLLECTION_NAME);
            Projects = mongoDatabase.GetCollection<Project>(PROJECTS_COLLECTION_NAME);
            ProjectMembers = mongoDatabase.GetCollection<ProjectMember>(PROJECT_MEMBERS_COLLECTION_NAME);
            ProjectEquipments = mongoDatabase.GetCollection<ProjectEquipment>(PROJECT_EQUIPMENTS_COLLECTION_NAME);
            ProjectEquipmentModules = mongoDatabase.GetCollection<ProjectEquipmentModule>(PROJECT_EQUIPMENT_MODULES_COLLECTION_NAME);
            ProjectCatalogSeedStates = mongoDatabase.GetCollection<ProjectCatalogSeedState>(PROJECT_CATALOG_SEED_STATES_COLLECTION_NAME);
        }

#endregion
    }
}
