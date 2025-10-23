namespace FaceLiveness.Application.Models
{
    public class FaceModel
    {
        public string Name { get; set; } = "Unknown";
        public double? Distance { get; set; }
        public BoundingBoxModel? Box { get; set; }
    }
}
