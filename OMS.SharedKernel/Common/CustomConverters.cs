using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OMS.SharedKernel.Common;



public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateTimeOffset = DateTimeOffset.Parse(reader.GetString());
        return dateTimeOffset.DateTime; // Or `.UtcDateTime` if you want the UTC time
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}


