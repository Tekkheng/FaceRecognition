namespace FaceLiveness.Application.Models
{
    public class RecognitionResultModel
    {
        public int FaceCount { get; set; }
        public LivenessModel Liveness { get; set; } = new();
        public List<FaceModel> Faces { get; set; } = new();
    }
}
