using System.Text.Json;
using System.Text;

namespace Core.Helpers
{
    public static class HttpContentGenerator
    {
        public static StringContent ConvertToJson<TObject>(TObject item)
            where TObject : class
        {
            var json = JsonSerializer.Serialize(item);
            return new StringContent(json, UnicodeEncoding.UTF8, "application/json");
        }
    }
}
