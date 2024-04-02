using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Strategy.SharedKernel;


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



public class CustomDoubleConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string stringValue = reader.GetString();
            return stringValue switch
            {
                "Infinity" => double.PositiveInfinity,
                "-Infinity" => double.NegativeInfinity,
                "NaN" => double.NaN,
                _ => double.Parse(stringValue, System.Globalization.CultureInfo.InvariantCulture)
            };
        }
        return reader.GetDouble();
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        if (double.IsPositiveInfinity(value))
        {
            writer.WriteStringValue("Infinity");
        }
        else if (double.IsNegativeInfinity(value))
        {
            writer.WriteStringValue("-Infinity");
        }
        else if (double.IsNaN(value))
        {
            writer.WriteStringValue("NaN");
        }
        else
        {
            writer.WriteNumberValue(value);
        }
    }
}