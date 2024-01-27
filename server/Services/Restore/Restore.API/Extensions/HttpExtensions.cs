using System.Text.Json;
using Restore.Core.Pagination;

namespace Restore.API.Extensions;

public static class HttpExtensions
{
    public static void AddPaginationHeader(this HttpResponse response, MetaData metaData)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        response.Headers.Append("Pagination", JsonSerializer.Serialize(metaData, options));
        response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
    }
}
