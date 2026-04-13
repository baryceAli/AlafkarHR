using System.Text.Json;
using System.Text.Json.Nodes;

namespace AlAfkarERP.Shared.Utilities;

public static class DeserializeAPIResponse
{
    public static T Deserialize<T>(string content, string? node)
    {
        if(node == null)
        {
            return Deserialize<T>(content);
            
        }
        var json = JsonNode.Parse(content);
        if (json == null || json[node] == null)
            throw new Exception($"Node '{node}' not found in response");

        var nodeJson = json[node]!.ToJsonString();


        var result = JsonSerializer.Deserialize<T>(nodeJson!, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result == null)
            throw new Exception("Deserialization returned null");

        return result;
    }

    private static T Deserialize<T>(string content)
    {
        //var json = JsonNode.Parse(content);
        //if (json == null || json[node] == null)
        //    throw new Exception($"Node '{node}' not found in response");

        //var nodeJson = json[node]!.ToJsonString();


        var result = JsonSerializer.Deserialize<T>(content!, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result == null)
            throw new Exception("Deserialization returned null");

        return result;
    }
    
}
