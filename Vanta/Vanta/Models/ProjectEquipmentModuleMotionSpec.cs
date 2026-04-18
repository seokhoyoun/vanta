namespace Vanta.Models
{
    public class ProjectEquipmentModuleMotionSpec
    {
        public string ControllerManufacturerName { get; set; } = string.Empty;

        public string ControllerModelName { get; set; } = string.Empty;

        public int AxisCount { get; set; }

        public string DriveSpec { get; set; } = string.Empty;

        public string SafetySpec { get; set; } = string.Empty;

        public string MechanicalNotes { get; set; } = string.Empty;
    }
}
