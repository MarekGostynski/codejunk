using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class JsonEditor
{
    public void EditJsonFile(string filePath, Dictionary<string, string> replacements)
    {
        try
        {
            // Load the JSON file
            string json = File.ReadAllText(filePath);
            var jsonObj = JsonDocument.Parse(json).RootElement.Clone();

            // Edit the keys based on the provided dictionary
            var updatedJsonObj = EditJsonElement(jsonObj, replacements);

            // Write the changes back to the file
            var options = new JsonSerializerOptions { WriteIndented = true };
            string outputJson = JsonSerializer.Serialize(updatedJsonObj, options);
            File.WriteAllText(filePath, outputJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private JsonElement EditJsonElement(JsonElement element, Dictionary<string, string> replacements)
    {
        var jsonWriterOptions = new JsonWriterOptions { Indented = true };
        var stream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(stream, jsonWriterOptions))
        {
            WriteJsonElement(jsonWriter, element, replacements);
        }
        stream.Seek(0, SeekOrigin.Begin);
        return JsonDocument.Parse(stream).RootElement;
    }

    private void WriteJsonElement(Utf8JsonWriter writer, JsonElement element, Dictionary<string, string> replacements)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var property in element.EnumerateObject())
                {
                    string key = replacements.ContainsKey(property.Name) ? replacements[property.Name] : property.Name;
                    writer.WritePropertyName(key);
                    WriteJsonElement(writer, property.Value, replacements);
                }
                writer.WriteEndObject();
                break;
            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                {
                    WriteJsonElement(writer, item, replacements);
                }
                writer.WriteEndArray();
                break;
            default:
                element.WriteTo(writer);
                break;
        }
    }
}