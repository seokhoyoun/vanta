using System;
using System.Collections.Generic;
using Vanta.Models;
using Vanta.ViewModels;

namespace Vanta.Services.Projects
{
    public class SampleProjectCatalogService
    {
        #region Fields

        private readonly List<ProjectDashboardViewModel> _projects;

        #endregion

        #region Constructors

        public SampleProjectCatalogService()
        {
            _projects = CreateProjects();
        }

        #endregion

        #region Public Methods

        public ProjectListPageViewModel GetProjectListPage()
        {
            ProjectListPageViewModel page = new ProjectListPageViewModel();

            foreach (ProjectDashboardViewModel project in _projects)
            {
                ProjectListItemViewModel item = new ProjectListItemViewModel();
                item.Code = project.Code;
                item.Name = project.Name;
                item.TeamName = project.TeamName;
                item.CustomerName = project.CustomerName;
                item.OwnerName = project.OwnerName;
                item.StatusName = project.StatusName;
                item.StartDate = project.StartDate;
                item.EndDate = project.EndDate;
                item.Summary = project.Summary;
                item.EquipmentCount = project.Equipments.Count;
                item.ModuleCount = CountModules(project);
                page.Projects.Add(item);
            }

            return page;
        }

        public ProjectDashboardViewModel? GetProjectDashboardOrNull(string code)
        {
            foreach (ProjectDashboardViewModel project in _projects)
            {
                if (string.Equals(project.Code, code, StringComparison.OrdinalIgnoreCase))
                {
                    return project;
                }
            }

            return null;
        }

        public bool CreateProject(ProjectDashboardViewModel projectDashboard)
        {
            if (GetProjectDashboardOrNull(projectDashboard.Code) != null)
            {
                return false;
            }

            _projects.Insert(0, projectDashboard);
            return true;
        }

        public bool UpdateProject(string originalCode, ProjectDashboardViewModel projectDashboard)
        {
            for (int i = 0; i < _projects.Count; i++)
            {
                if (!string.Equals(_projects[i].Code, originalCode, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!string.Equals(originalCode, projectDashboard.Code, StringComparison.OrdinalIgnoreCase)
                    && GetProjectDashboardOrNull(projectDashboard.Code) != null)
                {
                    return false;
                }

                _projects[i] = projectDashboard;
                return true;
            }

            return false;
        }

        public bool DeleteProject(string code)
        {
            for (int i = 0; i < _projects.Count; i++)
            {
                if (string.Equals(_projects[i].Code, code, StringComparison.OrdinalIgnoreCase))
                {
                    _projects.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Private Methods

        private int CountModules(ProjectDashboardViewModel project)
        {
            int count = 0;

            foreach (EquipmentSectionViewModel equipment in project.Equipments)
            {
                count += equipment.Modules.Count;
            }

            return count;
        }

        private List<ProjectDashboardViewModel> CreateProjects()
        {
            List<ProjectDashboardViewModel> projects = new List<ProjectDashboardViewModel>();
            projects.Add(CreateAlphaPackagingProject());
            projects.Add(CreateBetaInspectionProject());
            projects.Add(CreateGammaTransferProject());
            projects.Add(CreateDeltaAssemblyProject());
            projects.Add(CreateEpsilonVisionProject());
            projects.Add(CreateZetaHandlerProject());
            projects.Add(CreateEtaRobotProject());
            projects.Add(CreateThetaProcessProject());
            projects.Add(CreateIotaCellProject());
            projects.Add(CreateKappaLineProject());
            return projects;
        }

        private ProjectDashboardViewModel CreateAlphaPackagingProject()
        {
            ProjectDashboardViewModel dashboard = CreateProject(
                "PKG-ALPHA",
                "Alpha Packaging Line Rev.B",
                "Automation Platform Team",
                "Samsung Electronics",
                "김민수",
                "Active",
                new DateTime(2025, 9, 1),
                null,
                "Packaging line sample with mixed LoadPort, WTR, FTR, PC, PLC, and RFID modules.");

            EquipmentSectionViewModel eqp01 = CreateEquipment(
                "EQP-01",
                "Wafer Loader",
                "LoadPort 2, WTR 1, FTR 1, PC 2, PLC 1",
                "Commissioning");
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.LoadPort, "LP Front A", "Brooks", "LP3000", 2, "Brooks Unified Driver", "5.2.1", "SECS/GEM over Ethernet", "Dual port loader"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Wtr, "Vacuum WTR", "Rorze", "RR730", 1, "Rorze Motion Pack", "3.8.4", "RS-232 / Motion Controller", "Home sequence tuned for vacuum arm"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Ftr, "Front Transfer Robot", "Hinec", "FTR-X2", 1, "Hinec Transfer Driver", "1.9.0", "EtherCAT", "Guide sensor offset is tracked separately"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Pc, "Main Control PC", "Advantech", "IPC-610", 1, "Control Runtime Pack", "2025.11", "Intel i7 / 32GB / Win10 IoT", "Runs line control and device SDKs"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Pc, "Vision PC", "Axiomtek", "IPC974", 1, "VisionPro Runtime", "10.3.2", "Intel i5 / 16GB / Win10", "USB capture card version fixed"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Plc, "Main PLC", "Mitsubishi", "Q06UDV", 1, "GX Works Runtime", "1.104", "CC-Link IE", "Main sequence and interlock"));

            EquipmentSectionViewModel eqp02 = CreateEquipment(
                "EQP-02",
                "Inspection Cell",
                "LoadPort, FTR, RFID, inspection PC",
                "Mass Production");
            eqp02.Modules.Add(CreateModule(EProjectEquipmentModuleType.LoadPort, "LP Rear", "Brooks", "LP2500", 1, "Brooks Unified Driver", "5.1.9", "SECS/GEM over Ethernet", "Single port setup"));
            eqp02.Modules.Add(CreateModule(EProjectEquipmentModuleType.Ftr, "Rear Transfer Robot", "Yaskawa", "FTR-220", 1, "YRC Transfer Runtime", "2.2.7", "EtherNet/IP", "Alarm mapping managed in operation guide"));
            eqp02.Modules.Add(CreateModule(EProjectEquipmentModuleType.Rfid, "Carrier RFID Reader", "Omron", "V750", 2, "Omron RFID Pack", "3.1.0", "Ethernet / TCP", "Carrier ID check before inspection"));
            eqp02.Modules.Add(CreateModule(EProjectEquipmentModuleType.Pc, "Inspection PC", "Dell", "Precision 3680", 1, "Halcon Runtime", "22.11", "Intel i9 / 64GB / RTX A2000 / Win11", "GPU driver family fixed"));

            dashboard.Equipments.Add(eqp01);
            dashboard.Equipments.Add(eqp02);
            return dashboard;
        }

        private ProjectDashboardViewModel CreateBetaInspectionProject()
        {
            ProjectDashboardViewModel dashboard = CreateProject(
                "INSP-BETA",
                "Beta Inline Inspection",
                "Inline Inspection Team",
                "SK hynix",
                "박지훈",
                "Validation",
                new DateTime(2026, 1, 15),
                new DateTime(2026, 7, 31),
                "Inspection sample focused on I/O, PC, PLC, and RFID composition.");

            EquipmentSectionViewModel eqp01 = CreateEquipment(
                "EQP-A",
                "Top Inspection Cell",
                "I/O trigger unit, PC, PLC, RFID",
                "Pilot");
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Io, "Camera Trigger I/O", "Moxa", "ioLogik E1214", 2, "Moxa ioSearch", "2.5", "Ethernet I/O", "Camera trigger and strobe lines"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Pc, "Vision Workstation", "Lenovo", "P3 Tower", 1, "Halcon Runtime", "23.05", "i7 / 32GB / RTX 4060 / Win11", "Recipe and model files stored locally"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Plc, "I/O PLC", "Mitsubishi", "Q03UDV", 1, "GX Works Runtime", "1.104", "CC-Link IE", "Inspection handshake signals"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Rfid, "Lot RFID Gate", "Turck", "TNLR", 1, "Turck RFID Service", "4.2", "Profinet", "Lot tracking tied to inspection history"));

            dashboard.Equipments.Add(eqp01);
            return dashboard;
        }

        private ProjectDashboardViewModel CreateGammaTransferProject()
        {
            ProjectDashboardViewModel dashboard = CreateProject(
                "TRF-GAMMA",
                "Gamma Transfer Platform",
                "Robot Integration Team",
                "Micron",
                "이선우",
                "Completed",
                new DateTime(2024, 3, 4),
                new DateTime(2025, 12, 19),
                "Transfer platform sample using FTR, LoadPort, RFID, and PC modules.");

            EquipmentSectionViewModel eqp01 = CreateEquipment(
                "EQP-R1",
                "Transfer Station A",
                "FTR, FTR, PC",
                "Production");
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Ftr, "Transfer Robot", "Yaskawa", "GP88", 1, "MotoCom SDK", "4.7", "EtherNet/IP", "Tool changer included"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Ftr, "Feeder Transfer Robot", "Hinec", "FTR-S", 1, "Hinec Transfer Driver", "2.0.4", "EtherCAT", "Infeed timing tuned"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Pc, "Control IPC", "Advantech", "UNO-348", 1, "TwinCAT Runtime", "4026", "i5 / 16GB / Win10 IoT", "Backup image stored on NAS"));

            EquipmentSectionViewModel eqp02 = CreateEquipment(
                "EQP-R2",
                "Transfer Station B",
                "FTR, LoadPort, RFID, PC",
                "Production");
            eqp02.Modules.Add(CreateModule(EProjectEquipmentModuleType.Ftr, "Rear Transfer Robot", "Staubli", "TX2-90", 1, "Staubli Robotics Suite", "2024.2", "TCP/IP", "Deployment procedure differs by customer"));
            eqp02.Modules.Add(CreateModule(EProjectEquipmentModuleType.LoadPort, "Rear LoadPort", "Brooks", "LP2800", 2, "Brooks Unified Driver", "5.0.8", "SECS/GEM", "Barcode reader linked by port"));
            eqp02.Modules.Add(CreateModule(EProjectEquipmentModuleType.Rfid, "Barcode/RFID Gateway", "SICK", "RFU61x", 1, "SICK Connect", "6.0", "Ethernet", "Carrier identification shared with MES"));
            eqp02.Modules.Add(CreateModule(EProjectEquipmentModuleType.Pc, "Station IPC", "Neousys", "Nuvo-9100", 1, "Device Pack", "3.4", "i9 / 64GB / Win11", "PCIe expansion card installed"));

            dashboard.Equipments.Add(eqp01);
            dashboard.Equipments.Add(eqp02);
            return dashboard;
        }

        private ProjectDashboardViewModel CreateDeltaAssemblyProject()
        {
            ProjectDashboardViewModel dashboard = CreateProject(
                "ASM-DELTA",
                "Delta Assembly Cell",
                "Assembly Automation Team",
                "LG Energy Solution",
                "최유나",
                "Active",
                new DateTime(2025, 11, 10),
                null,
                "Assembly cell sample where motion is represented as a Motor module with PC and PLC.");

            EquipmentSectionViewModel eqp01 = CreateEquipment(
                "EQP-D1",
                "Assembly Station",
                "Motor, PC, PLC",
                "Setup");
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Motor, "Assembly Motion Pack", "Fanuc", "M-10iD", 1, "Fanuc PC SDK", "3.2", "EtherNet/IP", "Motion tool parameters backed up weekly"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Pc, "Assembly IPC", "Advantech", "MIC-770", 1, "Device Utility Pack", "2.5", "i5 / 16GB / Win10 IoT", "MES client installed"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Plc, "Assembly PLC", "Keyence", "KV-8000", 1, "KV Studio Runtime", "12.1", "EtherNet/IP", "Assembly interlock and alarm reset sequence"));

            dashboard.Equipments.Add(eqp01);
            return dashboard;
        }

        private ProjectDashboardViewModel CreateEpsilonVisionProject()
        {
            ProjectDashboardViewModel dashboard = CreateProject(
                "VIS-EPSILON",
                "Epsilon Review Station",
                "Machine Vision Team",
                "Sony",
                "한서진",
                "Completed",
                new DateTime(2024, 7, 1),
                new DateTime(2025, 2, 28),
                "Review station sample with I/O, analysis PC, and software platform modules.");

            EquipmentSectionViewModel eqp01 = CreateEquipment(
                "EQP-E1",
                "Review Station",
                "I/O gateway, analysis PC, software platform",
                "Production");
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Io, "Vision I/O Gateway", "Advantech", "ADAM-6050", 1, "AdamApax .NET Utility", "3.3", "Modbus TCP", "Camera trigger and tower lamp outputs"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Pc, "Analysis PC", "HP", "Z2", 1, "OpenCV Runtime", "4.10", "i7 / 32GB / RTX 3060 / Win11", "GPU-enabled review analysis"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.SoftwarePlatform, "Review Runtime Platform", "OpenCV", "Runtime Pack", 1, ".NET Hosting Bundle", "8.0", ".NET 8 / C# / OpenCV", "Deployment runtime for review models"));

            dashboard.Equipments.Add(eqp01);
            return dashboard;
        }

        private ProjectDashboardViewModel CreateZetaHandlerProject()
        {
            ProjectDashboardViewModel dashboard = CreateProject(
                "HDL-ZETA",
                "Zeta Wafer Handler",
                "Wafer Handling Team",
                "TSMC",
                "정우빈",
                "Validation",
                new DateTime(2026, 2, 3),
                new DateTime(2026, 10, 30),
                "Wafer handler sample centered on LoadPort, WTR, and FIMS.");

            EquipmentSectionViewModel eqp01 = CreateEquipment(
                "EQP-Z1",
                "Handler Main",
                "LoadPort 2, WTR 1, FIMS 1",
                "Validation");
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.LoadPort, "LP Main", "Brooks", "LP3200", 2, "Brooks Unified Driver", "5.3.0", "SECS/GEM", "Port sensor sensitivity tuned"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Wtr, "Core WTR", "Rorze", "RR840", 1, "Rorze Motion Pack", "4.0.1", "RS-232", "Periodic alignment check required"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Fims, "FIMS Interface", "Brooks", "FIMS-IF", 1, "Brooks FIMS Driver", "2.6.0", "SEMI E84 / Serial", "FIMS handshake tested with stocker simulator"));

            dashboard.Equipments.Add(eqp01);
            return dashboard;
        }

        private ProjectDashboardViewModel CreateEtaRobotProject()
        {
            ProjectDashboardViewModel dashboard = CreateProject(
                "RBT-ETA",
                "Eta Robot Integration",
                "Robot Integration Team",
                "Bosch",
                "서도연",
                "Active",
                new DateTime(2025, 12, 8),
                null,
                "Integration sample using FTR, control PC, and safety PLC.");

            EquipmentSectionViewModel eqp01 = CreateEquipment(
                "EQP-H1",
                "Robot Cell",
                "FTR, PC, PLC",
                "Integration");
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Ftr, "Main Transfer Robot", "ABB", "IRB1200", 1, "RobotStudio Runtime", "2025.2", "TCP/IP", "Safety PLC interlock applied"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Pc, "Robot Control PC", "Beckhoff", "C6030", 1, "TwinCAT Runtime", "4028", "i7 / 16GB / Win10 IoT", "Real-time task enabled"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Plc, "Safety PLC", "Pilz", "PNOZmulti 2", 1, "PNOZmulti Configurator", "11.5", "Ethernet/IP", "Safety door and robot zone interlock"));

            dashboard.Equipments.Add(eqp01);
            return dashboard;
        }

        private ProjectDashboardViewModel CreateThetaProcessProject()
        {
            ProjectDashboardViewModel dashboard = CreateProject(
                "PCS-THETA",
                "Theta Process Tool",
                "Process Equipment Team",
                "Intel",
                "문재호",
                "Planning",
                new DateTime(2026, 5, 1),
                new DateTime(2027, 1, 31),
                "Process tool sample with PLC, engineering PC, DeviceNet I/O, and software platform.");

            EquipmentSectionViewModel eqp01 = CreateEquipment(
                "EQP-T1",
                "Process Chamber Front",
                "PLC, PC, I/O, software platform",
                "Planning");
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Plc, "Main PLC", "Siemens", "S7-1500", 1, "TIA Runtime", "19", "Profinet", "Communication structure under review"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Pc, "Engineering Station", "Dell", "OptiPlex 7010", 1, "Engineering Tool Pack", "1.0", "i5 / 16GB / Win11", "Engineering documents and backups"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Io, "DeviceNet Gateway", "HMS", "Anybus X-gateway", 1, "Anybus Configuration Manager", "3.1", "DeviceNet / Profinet", "Legacy DeviceNet nodes aggregated here"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.SoftwarePlatform, "Process Control Platform", "Siemens", "WinCC OA", 1, "WinCC Runtime", "3.19", "SCADA / Historian", "Initial process UI and historian stack"));

            dashboard.Equipments.Add(eqp01);
            return dashboard;
        }

        private ProjectDashboardViewModel CreateIotaCellProject()
        {
            ProjectDashboardViewModel dashboard = CreateProject(
                "CLL-IOTA",
                "Iota Inspection Cell",
                "Inspection Automation Team",
                "Texas Instruments",
                "류해인",
                "Completed",
                new DateTime(2023, 10, 16),
                new DateTime(2024, 8, 23),
                "Closed inspection cell sample with I/O, inspection PC, and RFID tracking.");

            EquipmentSectionViewModel eqp01 = CreateEquipment(
                "EQP-I1",
                "Inspection Main",
                "I/O, inspection PC, RFID",
                "Closed");
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Io, "Inspection I/O Hub", "Wago", "750-8202", 1, "Wago I/O Check", "2.7", "Ethernet I/O", "Lighting trigger and reject outputs"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Pc, "Inspection IPC", "Avalue", "EMS-TGL", 1, "Vision Runtime Pack", "7.3", "i7 / 32GB / Win10", "Spare disk image stored"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Rfid, "Tray RFID Reader", "Pepperl+Fuchs", "IQT1", 1, "RFID Control", "5.4", "Ethernet/IP", "Tray traceability for outgoing inspection"));

            dashboard.Equipments.Add(eqp01);
            return dashboard;
        }

        private ProjectDashboardViewModel CreateKappaLineProject()
        {
            ProjectDashboardViewModel dashboard = CreateProject(
                "LNE-KAPPA",
                "Kappa Packaging Line",
                "Packaging Systems Team",
                "Amkor",
                "백지수",
                "Active",
                new DateTime(2025, 6, 2),
                new DateTime(2026, 12, 18),
                "Multi-EQP packaging line sample with LoadPort, FIMS, FTR, and RFID modules.");

            EquipmentSectionViewModel eqp01 = CreateEquipment(
                "EQP-K1",
                "Loader Station",
                "LoadPort, FIMS, PC",
                "Ramp Up");
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.LoadPort, "Loader Port", "Brooks", "LP2900", 2, "Brooks Unified Driver", "5.2.4", "SECS/GEM", "Recipe mapping per port"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Fims, "Line FIMS Interface", "Brooks", "FIMS-L", 1, "Brooks FIMS Driver", "2.5.1", "SEMI E84", "Customer-specific stocker parameter set"));
            eqp01.Modules.Add(CreateModule(EProjectEquipmentModuleType.Pc, "Loader IPC", "Advantech", "ARK-2250", 1, "Control Runtime", "2.8", "i5 / 16GB / Win10", "Linked with OP panel"));

            EquipmentSectionViewModel eqp02 = CreateEquipment(
                "EQP-K2",
                "Transfer Station",
                "FTR, FTR, RFID",
                "Ramp Up");
            eqp02.Modules.Add(CreateModule(EProjectEquipmentModuleType.Ftr, "Transfer Robot", "Staubli", "TS2-80", 1, "Staubli Robotics Suite", "2025.1", "TCP/IP", "Gripper type is interchangeable"));
            eqp02.Modules.Add(CreateModule(EProjectEquipmentModuleType.Ftr, "Inline Transfer Robot", "Hinec", "FTR-L", 1, "Hinec Transfer Driver", "2.1.0", "EtherCAT", "Cycle time optimization in progress"));
            eqp02.Modules.Add(CreateModule(EProjectEquipmentModuleType.Rfid, "Package RFID Scanner", "SICK", "RFH630", 1, "SICK RFU Runtime", "2.8", "Ethernet", "Linked to final packout traceability"));

            dashboard.Equipments.Add(eqp01);
            dashboard.Equipments.Add(eqp02);
            return dashboard;
        }

        private ProjectDashboardViewModel CreateProject(
            string code,
            string name,
            string teamName,
            string customerName,
            string ownerName,
            string statusName,
            DateTime startDate,
            DateTime? endDate,
            string summary)
        {
            ProjectDashboardViewModel dashboard = new ProjectDashboardViewModel();
            dashboard.Code = code;
            dashboard.Name = name;
            dashboard.TeamName = teamName;
            dashboard.CustomerName = customerName;
            dashboard.OwnerName = ownerName;
            dashboard.StatusName = statusName;
            dashboard.StartDate = startDate;
            dashboard.EndDate = endDate;
            dashboard.Summary = summary;
            return dashboard;
        }

        private EquipmentSectionViewModel CreateEquipment(string code, string name, string description, string stageName)
        {
            EquipmentSectionViewModel equipment = new EquipmentSectionViewModel();
            equipment.Code = code;
            equipment.Name = name;
            equipment.Description = description;
            equipment.StageName = stageName;
            equipment.PlatformName = GetEquipmentPlatformName(code);
            return equipment;
        }

        private string GetEquipmentPlatformName(string equipmentCode)
        {
            string[] platformNames = new string[]
            {
                "CommDaemon",
                "YCL",
                "EZ Cluster"
            };

            int platformIndex = Math.Abs(equipmentCode.GetHashCode()) % platformNames.Length;
            return platformNames[platformIndex];
        }

        private ModuleItemViewModel CreateModule(
            EProjectEquipmentModuleType moduleType,
            string name,
            string manufacturerName,
            string modelName,
            int serialNumberCount,
            string driverName,
            string driverVersion,
            string platformSummary,
            string notes)
        {
            ModuleItemViewModel moduleItem = new ModuleItemViewModel();
            moduleItem.Code = CreateSampleModuleCode(moduleType, name);
            moduleItem.ModuleType = moduleType;
            moduleItem.Name = name;
            moduleItem.ManufacturerName = manufacturerName;
            moduleItem.ModelName = modelName;
            moduleItem.SerialNumbers = CreateSerialNumbers(modelName, serialNumberCount);
            ModuleDriverViewModel driver = new ModuleDriverViewModel();
            driver.Name = driverName;
            driver.Version = driverVersion;
            moduleItem.Drivers.Add(driver);
            moduleItem.PlatformSummary = platformSummary;
            moduleItem.Notes = notes;
            return moduleItem;
        }

        private string CreateSampleModuleCode(EProjectEquipmentModuleType moduleType, string name)
        {
            string code = name.Replace(" ", string.Empty).Replace("-", string.Empty).ToUpperInvariant();
            return moduleType.ToString().ToUpperInvariant() + "-" + code;
        }

        private List<string> CreateSerialNumbers(string modelName, int serialNumberCount)
        {
            List<string> serialNumbers = new List<string>();
            string prefix = modelName.Replace(" ", string.Empty).Replace("-", string.Empty).ToUpperInvariant();

            for (int i = 1; i <= serialNumberCount; i++)
            {
                serialNumbers.Add(prefix + "-" + i.ToString("D3"));
            }

            return serialNumbers;
        }

        #endregion
    }
}
