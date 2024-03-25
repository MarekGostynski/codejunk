private void WriteJsonElement(Utf8JsonWriter writer, JsonElement element, Dictionary<string, string> replacements)
{
    switch (element.ValueKind)
    {
        case JsonValueKind.Object:
            writer.WriteStartObject();
            foreach (var property in element.EnumerateObject())
            {
                writer.WritePropertyName(property.Name);
                if (replacements.ContainsKey(property.Name) && property.Value.ValueKind == JsonValueKind.String)
                {
                    writer.WriteStringValue(replacements[property.Name]);
                }
                else
                {
                    if (property.Value.ValueKind == JsonValueKind.Object || property.Value.ValueKind == JsonValueKind.Array)
                    {
                        WriteJsonElement(writer, property.Value, replacements);
                    }
                    else
                    {
                        property.Value.WriteTo(writer);
                    }
                }
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