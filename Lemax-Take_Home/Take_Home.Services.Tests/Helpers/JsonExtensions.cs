using NetTopologySuite.IO;
using Newtonsoft.Json;

namespace Take_Home.Services.Tests.Helpers
{
    public static partial class JsonExtensions
    {
        public static T LoadFromFileWithGeoJson<T>(string path, JsonSerializerSettings settings = null)
        {
            var serializer = GeoJsonSerializer.CreateDefault(settings);
            serializer.CheckAdditionalContent = true;
            using (var textReader = new StreamReader(path))
            using (var jsonReader = new JsonTextReader(textReader))
            {
                return serializer.Deserialize<T>(jsonReader);
            }
        }
    }
}
