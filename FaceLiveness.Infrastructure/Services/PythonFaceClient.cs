using FaceLiveness.Application.Models;
using FaceLiveness.Application.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace FaceLiveness.Infrastructure.Services
{
    public class PythonFaceClient : IFaceClient
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public PythonFaceClient(HttpClient http, string baseUrl)
        {
            _http = http;
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public async Task<RecognitionResultModel> RecognizeAsync(string imageBase64, bool multi = true)
        {
            var payload = new { image = imageBase64, multi = multi };
            var resp = await _http.PostAsJsonAsync($"{_baseUrl}/recognize", payload);
            resp.EnsureSuccessStatusCode();
            var doc = await resp.Content.ReadFromJsonAsync<JsonElement>();
            var result = new RecognitionResultModel();
            if (doc.TryGetProperty("face_count", out var fc))
                result.FaceCount = fc.GetInt32();
            if (doc.TryGetProperty("liveness", out var l))
            {
                result.Liveness = new LivenessModel
                {
                    Score = l.GetProperty("score").GetDouble(),
                    Live = l.GetProperty("live").GetBoolean(),
                    Raw = l.GetProperty("raw").GetDouble()
                };
            }
            if (doc.TryGetProperty("faces", out var faces))
            {
                foreach (var f in faces.EnumerateArray())
                {
                    var face = new FaceModel();
                    if (f.TryGetProperty("name", out var name)) face.Name = name.GetString() ?? "Unknown";
                    if (f.TryGetProperty("distance", out var d) && d.ValueKind != JsonValueKind.Null) face.Distance = d.GetDouble();
                    if (f.TryGetProperty("box", out var box))
                    {
                        face.Box = new BoundingBoxModel
                        {
                            Top = box.GetProperty("top").GetInt32(),
                            Right = box.GetProperty("right").GetInt32(),
                            Bottom = box.GetProperty("bottom").GetInt32(),
                            Left = box.GetProperty("left").GetInt32()
                        };
                    }
                    result.Faces.Add(face);
                }
            }
            return result;
        }
    }
}
