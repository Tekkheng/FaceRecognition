using FaceLiveness.Application.Models;

namespace FaceLiveness.Application.Interfaces
{
    public interface IFaceClient
    {
        Task<RecognitionResultModel> RecognizeAsync(string imageBase64, bool multi = true);
    }
}
