using ElectricVehicleDealer.BLL.Intergations.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;

namespace ElectricVehicleDealer.BLL.Intergations.Implementations
{
    public class PdfService : IPdfService
    {
        public byte[] GenerateQuotePdf(Quote quote)
        {
            // Check null toàn bộ
            var customerName = quote.Customer?.FullName ?? "Unknown Customer";
            var dealerName = quote.Dealer?.FullName ?? "Unknown Dealer";

            var vehicle = quote.Vehicle ?? new Vehicle
            {
                Brand = new Brand { BrandName = "Unknown Brand" },
                ModelName = "Unknown Model",
                Version = "N/A",
                Color = "Không rõ",
                Year = 0,
                Price = 0
            };

            var brandName = vehicle.Brand?.BrandName ?? "Unknown Brand";
            var modelName = vehicle.ModelName ?? "Unknown Model";
            var version = vehicle.Version ?? "N/A";
            var color = vehicle.Color ?? "Không rõ";
            var year = vehicle.Year ?? 0;
            var price = vehicle.Price ?? 0;

            // Giảm giá
            decimal discountPercent = quote.Promotion?.DiscountPercent ?? 0;
            decimal discountAmount = price * (discountPercent / 100m);

            // Thuế
            decimal taxAmount = (price - discountAmount) * (quote.TaxRate / 100m);

            // Tổng cộng
            decimal finalPrice = price - discountAmount + taxAmount;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Text("BÁO GIÁ XE ĐIỆN")
                            .FontSize(22).Bold().FontColor(Colors.Blue.Medium);

                        row.ConstantItem(120).Text($"#{quote.QuoteId}")
                            .FontSize(16).Bold();
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Text($"Ngày báo giá: {quote.QuoteDate:dd/MM/yyyy}")
                            .FontSize(12);

                        col.Item().Text($"Khách hàng: {customerName}")
                            .FontSize(14);

                        col.Item().Text($"Tư vấn viên: {dealerName}")
                            .FontSize(14);

                        col.Item().LineHorizontal(1);

                        // Thông tin xe
                        col.Item().Text("Thông tin xe")
                            .FontSize(16).Bold().FontColor(Colors.Blue.Darken2);

                        col.Item().Text($"• Hãng xe: {brandName}");
                        col.Item().Text($"• Model: {modelName}");
                        col.Item().Text($"• Phiên bản: {version}");
                        col.Item().Text($"• Năm sản xuất: {year}");
                        col.Item().Text($"• Màu sắc: {color}");

                        col.Item().LineHorizontal(1);

                        // Bảng giá
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Hạng mục").Bold();
                                header.Cell().Text("Giá trị").Bold();
                            });

                            table.Cell().Text("Giá xe");
                            table.Cell().Text(price.ToString("C"));

                            table.Cell().Text($"Giảm giá ({discountPercent}%)");
                            table.Cell().Text($"- {discountAmount:C}");

                            table.Cell().Text($"Thuế ({quote.TaxRate}%)");
                            table.Cell().Text(taxAmount.ToString("C"));

                            table.Cell().Text("Tổng thanh toán").Bold();
                            table.Cell().Text(finalPrice.ToString("C")).Bold();
                        });

                        col.Item().LineHorizontal(1);

                        col.Item().Text("Xin cảm ơn quý khách đã quan tâm đến EV Dealer!")
                            .FontSize(12);
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("EV Dealer © ").FontSize(10);
                        text.Span(DateTime.Now.Year.ToString()).FontSize(10);
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
