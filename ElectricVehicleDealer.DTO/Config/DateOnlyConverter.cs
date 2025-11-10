using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElectricVehicleDealer.DTO.Config
{
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        private readonly string _format = "dd/MM/yyyy";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            return DateOnly.ParseExact(str!, _format, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format, CultureInfo.InvariantCulture));
        }
    }
}
