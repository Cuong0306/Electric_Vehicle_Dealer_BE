using ElectricVehicleDealer.BLL.Intergations.Interfaces;
using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Enum;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace ElectricVehicleDealer.BLL.Services.Implementations
{
    public class QuoteService : IQuoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public QuoteService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // ============================
        // CRUD 
        // ============================
        public async Task<IEnumerable<QuoteResponse>> GetAllAsync()
        {
            var quotes = await _unitOfWork.Quotes.GetAllAsync();
            var result = new List<QuoteResponse>();
            foreach (var quote in quotes)
            {
                result.Add(await MapToResponseAsync(quote));
            }
            return result;
        }

        public async Task<QuoteResponse> GetByIdAsync(int id)
        {
            var quote = await _unitOfWork.Quotes.GetByIdAsync(id);
            if (quote == null) throw new Exception($"Quote with id {id} not found");
            return await MapToResponseAsync(quote);
        }

        public async Task<QuoteResponse> CreateAsync(CreateQuoteRequest dto)
        {
            // ===== 1️⃣ Kiểm tra request =====
            if (dto.CustomerId == 0) throw new Exception("CustomerId is required");
            if (dto.VehicleId == 0) throw new Exception("VehicleId is required");
            if (dto.DealerId == 0) throw new Exception("DealerId is required");
            if (!dto.QuoteDate.HasValue) dto.QuoteDate = DateTime.Now;

            // ===== 2️⃣ Load entities với Include =====
            var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId)
                ?? throw new Exception($"Customer {dto.CustomerId} not found");

            var vehicle = await _unitOfWork.Vehicles.GetByIdWithIncludesAsync(dto.VehicleId)
                ?? throw new Exception($"Vehicle {dto.VehicleId} not found");

            if (vehicle.Brand == null)
                throw new Exception($"Vehicle {vehicle.VehicleId} does not have a Brand assigned");

            var dealer = await _unitOfWork.Dealers.GetByIdAsync(dto.DealerId)
                ?? throw new Exception($"Dealer {dto.DealerId} not found");

            Promotion? promotion = null;
            if (dto.PromotionId.HasValue && dto.PromotionId.Value != 0)
            {
                promotion = await _unitOfWork.Promotions.GetByIdAsync(dto.PromotionId.Value)
                    ?? throw new Exception($"Promotion {dto.PromotionId.Value} not found");
            }

            // ===== 3️⃣ Tạo Quote =====
            var quote = new Quote
            {
                CustomerId = customer.CustomerId,
                VehicleId = vehicle.VehicleId,
                DealerId = dealer.DealerId,
                PromotionId = promotion?.PromotionId,
                TaxRate = dto.TaxRate ?? 0,
                QuoteDate = dto.QuoteDate.Value,
                Status = dto.Status ?? QuoteEnum.Draft
            };

            await _unitOfWork.Quotes.AddAsync(quote);
            await _unitOfWork.SaveAsync();

            // Gán navigation properties
            quote.Customer = customer;
            quote.Vehicle = vehicle;
            quote.Dealer = dealer;
            quote.Promotion = promotion;

            // Gửi email nếu Accepted
            if (quote.Status == QuoteEnum.Accepted)
            {
                await SendQuoteAcceptedEmailAsync(quote, customer, vehicle, dealer);
            }

            return await MapToResponseAsync(quote);
        }

        public async Task<QuoteResponse> UpdateAsync(int id, UpdateQuoteRequest dto)
        {
            var quote = await _unitOfWork.Quotes.GetByIdAsync(id);
            if (quote == null) throw new Exception($"Quote with id {id} not found");

            Customer? customer = null;
            Vehicle? vehicle = null;
            Dealer? dealer = null;
            Promotion? promotion = null;

            if (dto.CustomerId != 0 && dto.CustomerId != quote.CustomerId) quote.CustomerId = dto.CustomerId;
            customer = await _unitOfWork.Customers.GetByIdAsync(quote.CustomerId)
                             ?? throw new Exception($"Customer {quote.CustomerId} not found");

            if (dto.VehicleId != 0 && dto.VehicleId != quote.VehicleId) quote.VehicleId = dto.VehicleId;
            vehicle = await _unitOfWork.Vehicles.GetByIdWithIncludesAsync(quote.VehicleId)
                            ?? throw new Exception($"Vehicle {quote.VehicleId} not found");
            if (vehicle.Brand == null) throw new Exception($"Vehicle {vehicle.VehicleId} does not have a Brand assigned");

            if (dto.DealerId != 0 && dto.DealerId != quote.DealerId) quote.DealerId = dto.DealerId;
            dealer = await _unitOfWork.Dealers.GetByIdAsync(quote.DealerId)
                           ?? throw new Exception($"Dealer {quote.DealerId} not found");

            if (dto.PromotionId.HasValue && dto.PromotionId.Value != 0 && dto.PromotionId.Value != quote.PromotionId) quote.PromotionId = dto.PromotionId;
            if (quote.PromotionId.HasValue)
            {
                promotion = await _unitOfWork.Promotions.GetByIdAsync(quote.PromotionId.Value)
                                ?? throw new Exception($"Promotion {quote.PromotionId.Value} not found");
            }

            if (dto.TaxRate.HasValue) quote.TaxRate = dto.TaxRate.Value;
            if (dto.QuoteDate.HasValue) quote.QuoteDate = dto.QuoteDate.Value;
            if (!string.IsNullOrEmpty(dto.Status)) quote.Status = ParseQuoteStatus(dto.Status);

            _unitOfWork.Quotes.Update(quote);
            await _unitOfWork.SaveAsync();

            quote.Customer = customer;
            quote.Vehicle = vehicle;
            quote.Dealer = dealer;
            quote.Promotion = promotion;

            // Luôn gửi mail nếu trạng thái là Accepted
            if (quote.Status == QuoteEnum.Accepted)
            {
                await SendQuoteAcceptedEmailAsync(quote, customer, vehicle, dealer);
            }

            return await MapToResponseAsync(quote);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var quote = await _unitOfWork.Quotes.GetByIdAsync(id);
            if (quote == null) throw new Exception($"Quote with id {id} not found");

            _unitOfWork.Quotes.Remove(quote);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private static QuoteEnum ParseQuoteStatus(string status)
        {
            if (int.TryParse(status, out var numericStatus))
            {
                if (Enum.IsDefined(typeof(QuoteEnum), numericStatus))
                    return (QuoteEnum)numericStatus;
                throw new ArgumentException($"Invalid numeric quote status: {status}");
            }

            if (Enum.TryParse<QuoteEnum>(status, true, out var parsedEnum))
                return parsedEnum;

            throw new ArgumentException($"Invalid quote status: {status}");
        }

        private async Task<QuoteResponse> MapToResponseAsync(Quote quote)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(quote.CustomerId);
            var vehicle = await _unitOfWork.Vehicles.GetByIdWithIncludesAsync(quote.VehicleId);
            var dealer = await _unitOfWork.Dealers.GetByIdAsync(quote.DealerId);
            var promotion = quote.PromotionId.HasValue
                                ? await _unitOfWork.Promotions.GetByIdAsync(quote.PromotionId.Value)
                                : null;

            decimal basePrice = vehicle?.Price ?? 0;
            decimal priceWithTax = basePrice + (basePrice * quote.TaxRate / 100);

            decimal discountAmount = 0;
            if (promotion != null && promotion.DiscountPercent.HasValue && promotion.DiscountPercent.Value > 0)
            {
                discountAmount = basePrice * promotion.DiscountPercent.Value / 100;
            }
            decimal finalPrice = priceWithTax - discountAmount;


            return new QuoteResponse
            {
                QuoteId = quote.QuoteId,
                CustomerId = quote.CustomerId,
                VehicleId = quote.VehicleId,
                DealerId = quote.DealerId,
                PromotionId = quote.PromotionId,
                TaxRate = quote.TaxRate,
                QuoteDate = quote.QuoteDate,
                Status = quote.Status,
                VehiclePrice = basePrice,
                PriceWithTax = priceWithTax,
                DiscountAmount = discountAmount,
                FinalPrice = finalPrice
            };
        }


        private async Task SendQuoteAcceptedEmailAsync(Quote quote, Customer customer, Vehicle vehicle, Dealer dealer)
        {
            // Bỏ Try-Catch để debug dễ hơn
            if (customer == null || vehicle == null || dealer == null || string.IsNullOrEmpty(customer.Email))
            {
                return;
            }

            var pdfBytes = GenerateQuotePdf(quote, customer, vehicle, dealer);

            string subject = $"Your Quote #{quote.QuoteId} has been Accepted!";
            string body = $@"
                <p>Hello <b>{customer.FullName}</b>,</p>
                <p>Your quote <b>#{quote.QuoteId}</b> has been <b>accepted</b>.</p>
                <p>The full quote is attached as a PDF.</p>
                <p>— EV Dealer Team</p>";

            await _emailService.SendEmailWithAttachmentAsync(
                customer.Email,
                subject,
                body,
                pdfBytes,
                $"Quote_{quote.QuoteId}.pdf"
            );
        }

        private byte[] GenerateQuotePdf(Quote quote, Customer customer, Vehicle vehicle, Dealer dealer)
        {
            string brandName = vehicle.Brand?.BrandName ?? "Chưa xác định";
            string modelName = vehicle.ModelName ?? "Chưa xác định";
            string customerName = customer.FullName ?? "Khách hàng";
            string customerEmail = customer.Email ?? "Không có";
            string dealerName = dealer.FullName ?? "Đại lý";
            string dealerPhone = dealer.Phone ?? "Không có";

            DateTime quoteDate = quote.QuoteDate ?? DateTime.Now;

            decimal basePrice = vehicle.Price ?? 0;
            decimal taxRate = quote.TaxRate;
            decimal taxAmount = basePrice * taxRate / 100;
            decimal priceWithTax = basePrice + taxAmount;

            decimal discountAmount = 0;
            string promotionName = "Không có";

            if (quote.PromotionId.HasValue && quote.Promotion != null && quote.Promotion.DiscountPercent.HasValue)
            {
                discountAmount = basePrice * quote.Promotion.DiscountPercent.Value / 100;
                promotionName = quote.Promotion.Title ?? $"Giảm giá {quote.Promotion.DiscountPercent.Value}%";
            }
            decimal finalPrice = priceWithTax - discountAmount;

            var vietnamCulture = new CultureInfo("vi-VN");

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // ==== HEADER KHÔNG DÙNG ẢNH ====
                    page.Header().Column(headerCol =>
                    {
                        headerCol.Item().Column(col =>
                        {
                            col.Item().Text("CÔNG TY CỔ PHẦN EV DEALER").Bold().FontSize(20).FontColor(Colors.Blue.Medium).AlignCenter();
                            col.Item().Text("Địa chỉ: 123 Đường XYZ, Quận 1, TP. Hồ Chí Minh").FontSize(10).AlignCenter();
                            col.Item().Text("Điện thoại: (028) 1234 5678 | Email: info@evdealer.vn").FontSize(10).AlignCenter();
                        });

                        headerCol.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });
                    // ===============================

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Spacing(15);

                        col.Item().Text("PHIẾU BÁO GIÁ").Bold().FontSize(24).FontColor(Colors.Blue.Darken2).AlignCenter();
                        col.Item().PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        col.Item().Row(row =>
                        {
                            row.RelativeColumn().Column(colInfo =>
                            {
                                colInfo.Item().Text($"Mã báo giá: {quote.QuoteId}").SemiBold().FontSize(11);
                                colInfo.Item().Text($"Ngày báo giá: {quoteDate.ToString("dd/MM/yyyy", vietnamCulture)}").SemiBold().FontSize(11);
                                colInfo.Item().Text($"Người lập: {dealerName}").FontSize(11);
                                colInfo.Item().Text($"Điện thoại: {dealerPhone}").FontSize(11);
                            });

                            row.RelativeColumn().Column(colInfo =>
                            {
                                colInfo.Item().PaddingBottom(5).Text("THÔNG TIN KHÁCH HÀNG").Bold().FontSize(12);
                                colInfo.Item().Text($"Họ và tên: {customerName}").FontSize(11);
                                colInfo.Item().Text($"Email: {customerEmail}").FontSize(11);
                            });
                        });

                        col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        col.Item().Text("CHI TIẾT SẢN PHẨM").Bold().FontSize(14).FontColor(Colors.Blue.Darken2);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyleHeader).Text("STT").Bold();
                                header.Cell().Element(CellStyleHeader).Text("Tên Xe (Hãng)").Bold();
                                header.Cell().Element(CellStyleHeader).Text("Năm SX").Bold();
                                header.Cell().Element(CellStyleHeader).AlignRight().Text("Đơn Giá").Bold();
                                header.Cell().Element(CellStyleHeader).AlignRight().Text("Thuế (%)").Bold();
                                header.Cell().Element(CellStyleHeader).AlignRight().Text("Thành Tiền").Bold();
                            });

                            table.Cell().Element(CellStyleData).Text("1");
                            table.Cell().Element(CellStyleData).Text($"{brandName} {modelName} ({vehicle.Year})");
                            table.Cell().Element(CellStyleData).Text($"{vehicle.Year}");

                            table.Cell().Element(CellStyleData).AlignRight().Text(basePrice.ToString("N0", vietnamCulture) + " VNĐ");
                            table.Cell().Element(CellStyleData).AlignRight().Text(taxRate.ToString("F0", vietnamCulture) + "%");
                            table.Cell().Element(CellStyleData).AlignRight().Text(priceWithTax.ToString("N0", vietnamCulture) + " VNĐ");
                        });

                        col.Item().PaddingTop(20).AlignRight().Column(colTotal =>
                        {
                            colTotal.Item().Text($"Giá gốc: {basePrice.ToString("N0", vietnamCulture)} VNĐ").FontSize(11);
                            colTotal.Item().Text($"Thuế ({taxRate.ToString("F0", vietnamCulture)}%): {taxAmount.ToString("N0", vietnamCulture)} VNĐ").FontSize(11);

                            if (discountAmount > 0)
                            {
                                colTotal.Item().Text($"Chiết khấu ({promotionName}): -{discountAmount.ToString("N0", vietnamCulture)} VNĐ").FontSize(11).FontColor(Colors.Red.Medium);
                            }

                            colTotal.Item().PaddingTop(5).Text($"TỔNG THANH TOÁN: {finalPrice.ToString("N0", vietnamCulture)} VNĐ").Bold().FontSize(14).FontColor(Colors.Red.Darken4);
                        });

                        col.Item().PaddingTop(50).Row(row =>
                        {
                            row.RelativeColumn().Column(colSign =>
                            {
                                colSign.Item().Text("Khách hàng").Bold().FontSize(11).AlignCenter();
                                colSign.Item().PaddingTop(50).Text("(Ký và ghi rõ họ tên)").FontSize(10).AlignCenter();
                            });
                            row.RelativeColumn().Column(colSign =>
                            {
                                colSign.Item().Text("Đại diện EV Dealer").Bold().FontSize(11).AlignCenter();
                                colSign.Item().PaddingTop(10).Text(dealerName).FontSize(10).AlignCenter();
                                colSign.Item().PaddingTop(20).Text("(Ký và ghi rõ họ tên)").FontSize(10).AlignCenter();
                            });
                        });

                        col.Item().PaddingTop(30).Text("Ghi chú: Báo giá có hiệu lực trong 7 ngày làm việc.").FontSize(9).Italic();
                        col.Item().Text("Trân trọng cảm ơn quý khách hàng đã tin tưởng và lựa chọn EV Dealer!").FontSize(9).Italic();
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("EV Dealer © ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span(DateTime.Now.Year.ToString()).FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span(" | Trang ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span(" / ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            });

            static IContainer CellStyleHeader(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingVertical(5).Background(Colors.Grey.Lighten3);
            static IContainer CellStyleData(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);

            using var stream = new MemoryStream();
            doc.GeneratePdf(stream);
            return stream.ToArray();
        }
    }
}