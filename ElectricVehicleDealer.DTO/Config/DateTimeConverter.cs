using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElectricVehicleDealer.DTO.Config
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        // Danh sách các định dạng chấp nhận
        private readonly string[] _formats = new[]
        {
            "dd/MM/yyyy HH:mm:ss",    // Ưu tiên: Ngày giờ VN đầy đủ
            "dd/MM/yyyy HH:mm",       // Ngày giờ VN (không giây) - cái bạn đang cần
            "dd/MM/yyyy",             // Chỉ ngày VN
            "yyyy-MM-ddTHH:mm:ss",    // ISO chuẩn (Swagger hay gửi)
            "yyyy-MM-ddTHH:mm:ss.fffZ", // ISO có mili giây
            "yyyy-MM-dd"              // Chỉ ngày ISO
        };

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            if (string.IsNullOrWhiteSpace(str)) return default;

            // 1. Thử parse theo danh sách định dạng
            foreach (var format in _formats)
            {
                if (DateTime.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    return date;
                }
            }

            // 2. Fallback: Thử parse tự động theo Culture của máy
            if (DateTime.TryParse(str, out var fallbackDate))
            {
                return fallbackDate;
            }

            // Nếu không được thì báo lỗi rõ ràng
            throw new JsonException($"Không thể đọc ngày giờ: '{str}'. Vui lòng dùng định dạng: dd/MM/yyyy HH:mm");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Khi trả dữ liệu về FE: Luôn trả về định dạng có giờ phút để hiển thị
            // Nếu giờ phút = 00:00:00 thì trả về ngày thôi cho gọn (tuỳ chọn)
            if (value.TimeOfDay == TimeSpan.Zero)
            {
                writer.WriteStringValue(value.ToString("dd/MM/yyyy"));
            }
            else
            {
                writer.WriteStringValue(value.ToString("dd/MM/yyyy HH:mm"));
            }
        }
    }
}