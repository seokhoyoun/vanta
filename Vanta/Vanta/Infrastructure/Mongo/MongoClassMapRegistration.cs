using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Vanta.Models;

namespace Vanta.Infrastructure.Mongo
{
    public static class MongoClassMapRegistration
    {
        private static bool mIsRegistered;

        public static void Register()
        {
            if (mIsRegistered)
            {
                return;
            }

            ConventionPack conventionPack = new ConventionPack();
            conventionPack.Add(new IgnoreExtraElementsConvention(true));
            ConventionRegistry.Register(
                "VantaMongoConventions",
                conventionPack,
                type => type.Namespace != null && type.Namespace.StartsWith("Vanta"));

            RegisterUser();
            RegisterProject();
            RegisterProjectMember();
            RegisterProjectEquipment();
            RegisterProjectEquipmentModule();
            RegisterProjectEquipmentModuleDriver();
            RegisterProjectCatalogSeedState();
            RegisterProjectEquipmentModulePcSpec();
            RegisterProjectEquipmentModuleMotionSpec();
            RegisterProjectEquipmentModuleIoSpec();
            RegisterProjectEquipmentModulePlcSpec();
            RegisterProjectEquipmentModuleRfidSpec();
            RegisterProjectEquipmentModuleSoftwarePlatformSpec();

            mIsRegistered = true;
        }

        private static void RegisterUser()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(User)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<User>(classMap =>
            {
                classMap.AutoMap();
                classMap.MapIdMember(model => model.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });
        }

        private static void RegisterProject()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Project)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<Project>(classMap =>
            {
                classMap.AutoMap();
                classMap.MapIdMember(model => model.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });
        }

        private static void RegisterProjectMember()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(ProjectMember)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<ProjectMember>(classMap =>
            {
                classMap.AutoMap();
                classMap.MapIdMember(model => model.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });
        }

        private static void RegisterProjectEquipment()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(ProjectEquipment)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<ProjectEquipment>(classMap =>
            {
                classMap.AutoMap();
                classMap.MapIdMember(model => model.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });
        }

        private static void RegisterProjectEquipmentModule()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(ProjectEquipmentModule)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<ProjectEquipmentModule>(classMap =>
            {
                classMap.AutoMap();
                classMap.MapIdMember(model => model.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });
        }

        private static void RegisterProjectEquipmentModuleDriver()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(ProjectEquipmentModuleDriver)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<ProjectEquipmentModuleDriver>(classMap =>
            {
                classMap.AutoMap();
            });
        }

        private static void RegisterProjectCatalogSeedState()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(ProjectCatalogSeedState)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<ProjectCatalogSeedState>(classMap =>
            {
                classMap.AutoMap();
            });
        }

        private static void RegisterProjectEquipmentModulePcSpec()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(ProjectEquipmentModulePcSpec)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<ProjectEquipmentModulePcSpec>(classMap =>
            {
                classMap.AutoMap();
            });
        }

        private static void RegisterProjectEquipmentModuleMotionSpec()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(ProjectEquipmentModuleMotionSpec)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<ProjectEquipmentModuleMotionSpec>(classMap =>
            {
                classMap.AutoMap();
            });
        }

        private static void RegisterProjectEquipmentModuleIoSpec()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(ProjectEquipmentModuleIoSpec)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<ProjectEquipmentModuleIoSpec>(classMap =>
            {
                classMap.AutoMap();
            });
        }

        private static void RegisterProjectEquipmentModulePlcSpec()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(ProjectEquipmentModulePlcSpec)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<ProjectEquipmentModulePlcSpec>(classMap =>
            {
                classMap.AutoMap();
            });
        }

        private static void RegisterProjectEquipmentModuleRfidSpec()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(ProjectEquipmentModuleRfidSpec)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<ProjectEquipmentModuleRfidSpec>(classMap =>
            {
                classMap.AutoMap();
            });
        }

        private static void RegisterProjectEquipmentModuleSoftwarePlatformSpec()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(ProjectEquipmentModuleSoftwarePlatformSpec)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<ProjectEquipmentModuleSoftwarePlatformSpec>(classMap =>
            {
                classMap.AutoMap();
            });
        }
    }
}
