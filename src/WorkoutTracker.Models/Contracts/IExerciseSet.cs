using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Models.Contracts;

[JsonConverter(typeof(IExerciseSetPolymorphicJsonConverter))]
public interface IExerciseSet
{
    double Weight { get; set; }

    int Repetitions { get; set; }

    bool IsWarmup { get; set; }
}

public class IExerciseSetPolymorphicJsonConverter : JsonConverter<IExerciseSet>
{
    private const string DiscriminatorName = "$type";

    public override IExerciseSet Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (JsonDocument.TryParseValue(ref reader, out var doc))
        {
            if (doc.RootElement.TryGetProperty(DiscriminatorName, out var type))
            {
                return type.GetString() switch
                {
                    nameof(LegacySet) => JsonSerializer.Deserialize<LegacySet>(doc, options),
                    nameof(CompletedSet) => JsonSerializer.Deserialize<CompletedSet>(doc, options),
                    nameof(InProgressSet) => JsonSerializer.Deserialize<InProgressSet>(doc, options),
                    nameof(ProposedSet) => JsonSerializer.Deserialize<ProposedSet>(doc, options),
                    _ => throw new NotSupportedException()

                };
            }

            return JsonSerializer.Deserialize<LegacySet>(doc, options);
        }

        throw new JsonException("Failed to parse JsonDocument");
    }

    public override void Write(Utf8JsonWriter writer, IExerciseSet value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString(DiscriminatorName, value.GetType().Name);

        using (var document = JsonSerializer.SerializeToDocument(value, value.GetType(), options))
        {
            foreach (var property in document.RootElement.EnumerateObject())
            {
                property.WriteTo(writer);
            }
        }

        writer.WriteEndObject();
    }
}