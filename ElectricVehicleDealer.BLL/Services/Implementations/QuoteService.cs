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
        public async Task<IEnumerable<CustomerResponse>> GetCustomersWithQuotesAsync()
        {
            var quotes = await _unitOfWork.Quotes.GetAllAsync();

            var customerIds = quotes.Select(q => q.CustomerId).Distinct().ToList();

            var customers = new List<CustomerResponse>();

            foreach (var id in customerIds)
            {
                var c = await _unitOfWork.Customers.GetByIdAsync(id);
                if (c != null)
                {
                    customers.Add(new CustomerResponse
                    {
                        CustomerId = c.CustomerId,
                        FullName = c.FullName,
                        Email = c.Email,
                        Phone = c.Phone,
                        Address = c.Address,
                       
                    });
                }
            }
            return customers;
        }
        public async Task<IEnumerable<QuoteResponse>> GetQuotesByCustomerIdAsync(int customerId)
        {
            var allQuotes = await _unitOfWork.Quotes.GetAllAsync();
            var customerQuotes = allQuotes.Where(q => q.CustomerId == customerId);

            var result = new List<QuoteResponse>();
            foreach (var quote in customerQuotes)
            {
                result.Add(await MapToResponseAsync(quote));
            }
            return result;
        }
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
            if (dto.CustomerId == 0) throw new Exception("CustomerId is required");
            if (dto.VehicleId == 0) throw new Exception("VehicleId is required");
            if (dto.DealerId == 0) throw new Exception("DealerId is required");
            
            if (dto.Quantity <= 0) throw new Exception("Quantity must be greater than zero");
            if (!dto.QuoteDate.HasValue) dto.QuoteDate = DateTime.Now;
            
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

            var quote = new Quote
            {
                CustomerId = customer.CustomerId,
                VehicleId = vehicle.VehicleId,
                DealerId = dealer.DealerId,
                PromotionId = promotion?.PromotionId,
                TaxRate = dto.TaxRate ?? 0,
                QuoteDate = dto.QuoteDate.Value,
                Status = dto.Status ?? QuoteEnum.Draft,
                Quantity = dto.Quantity
            };

            await _unitOfWork.Quotes.AddAsync(quote);
            await _unitOfWork.SaveAsync();

            quote.Customer = customer;
            quote.Vehicle = vehicle;
            quote.Dealer = dealer;
            quote.Promotion = promotion;

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

            if (dto.Quantity.HasValue && dto.Quantity.Value > 0)
            {
                quote.Quantity = dto.Quantity.Value;
            }
            else if (dto.Quantity.HasValue && dto.Quantity.Value <= 0)
            {
                throw new Exception("Quantity must be greater than zero for update");
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
            int quantity = quote.Quantity;
            decimal totalPriceBeforeDiscount = basePrice * quantity;

            decimal discountAmount = 0;
            if (promotion != null && promotion.DiscountPercent.HasValue && promotion.DiscountPercent.Value > 0)
            {
                discountAmount = totalPriceBeforeDiscount * promotion.DiscountPercent.Value / 100;
            }

            decimal priceAfterDiscount = totalPriceBeforeDiscount - discountAmount;
            decimal taxAmount = priceAfterDiscount * quote.TaxRate / 100;
            decimal finalPrice = priceAfterDiscount + taxAmount;

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
                Quantity = quantity,
                VehiclePrice = basePrice,
                PriceWithTax = finalPrice,
                DiscountAmount = discountAmount,
                FinalPrice = finalPrice
            };
        }

        private async Task SendQuoteAcceptedEmailAsync(Quote quote, Customer customer, Vehicle vehicle, Dealer dealer)
        {
            if (customer == null || vehicle == null || dealer == null || string.IsNullOrEmpty(customer.Email))
                return;

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

            // THÊM: Lấy số lượng
            int quantity = quote.Quantity;
            decimal totalPriceBeforeDiscount = basePrice * quantity;
            // Tính toán
            decimal discountAmount = 0;
            string promotionName = "";
            if (quote.PromotionId.HasValue && quote.Promotion != null && quote.Promotion.DiscountPercent.HasValue)
            {
                discountAmount = totalPriceBeforeDiscount * quote.Promotion.DiscountPercent.Value / 100;
                promotionName = quote.Promotion.Title ?? $"{quote.Promotion.DiscountPercent.Value}%";
            }

            decimal priceAfterDiscount = totalPriceBeforeDiscount - discountAmount;
            decimal taxAmount = priceAfterDiscount * taxRate / 100;
            decimal finalPrice = priceAfterDiscount + taxAmount;

            var vietnamCulture = new CultureInfo("vi-VN");


            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // 1. HEADER
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeColumn().Column(c =>
                            {
                                c.Item().Text("CÔNG TY CỔ PHẦN EV DEALER VIỆT NAM").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
                                c.Item().Text("Địa chỉ: Tòa nhà ABC, 123 Đường XYZ, Quận 1, TP. Hồ Chí Minh").FontSize(9).FontColor(Colors.Grey.Darken2);
                                c.Item().Text("Hotline: 1900 1234  |  Email: support@evdealer.vn  |  Website: www.evdealer.vn").FontSize(9).FontColor(Colors.Grey.Darken2);
                            });
                        });
                        col.Item().PaddingVertical(10).LineHorizontal(2).LineColor(Colors.Blue.Medium);
                    });

                    // 2. CONTENT
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Text("BẢNG BÁO GIÁ").FontSize(22).Bold().FontColor(Colors.Blue.Darken3).AlignCenter();
                        col.Item().PaddingBottom(20).Text($"(Số: BG-{quote.QuoteId:000000})").FontSize(10).Italic().AlignCenter();

                        // Thông tin 2 cột
                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(15).Row(row =>
                        {
                            // Cột trái: Khách hàng
                            row.RelativeColumn().Column(c =>
                            {
                                c.Item().PaddingBottom(5).Text("THÔNG TIN KHÁCH HÀNG").Bold().FontSize(11).FontColor(Colors.Blue.Darken2);
                                c.Item().Text(t => { t.Span("Họ và tên: ").SemiBold(); t.Span(customerName); });
                                c.Item().Text(t => { t.Span("Email: ").SemiBold(); t.Span(customerEmail); });
                                c.Item().Text(t => { t.Span("Ngày báo giá: ").SemiBold(); t.Span(quoteDate.ToString("dd/MM/yyyy", vietnamCulture)); });
                            });

                            // Cột phải: Người phụ trách
                            row.RelativeColumn().PaddingLeft(20).Column(c =>
                            {
                                c.Item().PaddingBottom(5).Text("NHÂN VIÊN PHỤ TRÁCH").Bold().FontSize(11).FontColor(Colors.Blue.Darken2);
                                c.Item().Text(t => { t.Span("Nhân viên: ").SemiBold(); t.Span(dealerName); });
                                c.Item().Text(t => { t.Span("Điện thoại: ").SemiBold(); t.Span(dealerPhone); });
                                c.Item().Text(t => { t.Span("Hiệu lực: ").SemiBold(); t.Span("07 ngày kể từ ngày báo giá"); });
                            });
                        });

                        col.Item().PaddingBottom(20);

                        // Bảng chi tiết sản phẩm
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            // Header Bảng
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyleHeader).Text("STT");
                                header.Cell().Element(CellStyleHeader).Text("Mô tả / Tên xe");
                                header.Cell().Element(CellStyleHeader).AlignCenter().Text("Năm");
                                header.Cell().Element(CellStyleHeader).AlignCenter().Text("SL");
                                header.Cell().Element(CellStyleHeader).AlignRight().Text("Đơn giá");
                                header.Cell().Element(CellStyleHeader).AlignRight().Text("Thành tiền");
                            });

                            // Nội dung Bảng
                            table.Cell().Element(CellStyleData).AlignCenter().Text("1");
                            table.Cell().Element(CellStyleData).Column(c => {
                                c.Item().Text($"{brandName} {modelName}").Bold();
                            });
                            table.Cell().Element(CellStyleData).AlignCenter().Text($"{vehicle.Year}");
                            table.Cell().Element(CellStyleData).AlignCenter().Text(quantity.ToString());
                            table.Cell().Element(CellStyleData).AlignRight().Text(basePrice.ToString("N0", vietnamCulture));
                            table.Cell().Element(CellStyleData).AlignRight().Text(totalPriceBeforeDiscount.ToString("N0", vietnamCulture));
                        });

                        // PHẦN TỔNG KẾT
                        col.Item().PaddingTop(10).Row(row =>
                        {
                            // Bên trái: Số tiền bằng chữ
                            row.RelativeColumn(6).PaddingRight(20).Column(c =>
                            {
                                c.Item().Text("Số tiền bằng chữ:").Bold().FontSize(10);
                                c.Item().Text("...............................................................................................................").Italic().FontColor(Colors.Grey.Darken1);
                            });

                            // Bên phải: Bảng tính tổng
                            row.RelativeColumn(4).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(summary =>
                            {
                                summary.Item().Table(t =>
                                {
                                    t.ColumnsDefinition(cd =>
                                    {
                                        cd.RelativeColumn(2); // Label
                                        cd.RelativeColumn(3); // Value
                                    });

                                    // Cộng tiền hàng
                                    t.Cell().PaddingBottom(4).Text("Cộng tiền hàng:");
                                    t.Cell().PaddingBottom(4).AlignRight().Text(totalPriceBeforeDiscount.ToString("N0", vietnamCulture));

                                    // Chiết khấu
                                    if (discountAmount > 0)
                                    {
                                        t.Cell().PaddingBottom(4).Text($"Chiết khấu ({promotionName}):").FontColor(Colors.Red.Medium).FontSize(9);
                                        t.Cell().PaddingBottom(4).AlignRight().Text("-" + discountAmount.ToString("N0", vietnamCulture)).FontColor(Colors.Red.Medium);
                                    }

                                    // Thuế (đã sửa text)
                                    t.Cell().PaddingBottom(4).Text($"Thuế ({taxRate.ToString("F0", vietnamCulture)}%):");
                                    t.Cell().PaddingBottom(4).AlignRight().Text(taxAmount.ToString("N0", vietnamCulture));

                                    // Đường kẻ ngăn cách
                                    t.Cell().ColumnSpan(2).PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                                    // Tổng cộng
                                    t.Cell().Text("TỔNG CỘNG:").Bold();
                                    t.Cell().AlignRight().Text(finalPrice.ToString("N0", vietnamCulture) + " VNĐ").Bold().FontColor(Colors.Blue.Darken2).FontSize(12);
                                });
                            });
                        });

                        // CHỮ KÝ
                        col.Item().PaddingTop(40).Row(row =>
                        {
                            row.RelativeColumn().Column(c =>
                            {
                                c.Item().AlignCenter().Text("ĐẠI DIỆN KHÁCH HÀNG").Bold();
                                c.Item().AlignCenter().Text("(Ký và ghi rõ họ tên)").FontSize(9).Italic();
                            });

                            row.RelativeColumn().Column(c =>
                            {
                                c.Item().AlignCenter().Text("ĐẠI DIỆN CÔNG TY").Bold();
                                c.Item().AlignCenter().Text("(Ký, đóng dấu và ghi rõ họ tên)").FontSize(9).Italic();
                                c.Item().PaddingTop(60).AlignCenter().Text(dealerName).Bold();
                            });
                        });
                    });

                    // 3. FOOTER
                    page.Footer().Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        col.Item().PaddingTop(5).AlignCenter().Text("Cảm ơn Quý khách đã tin tưởng và lựa chọn dịch vụ của chúng tôi!").FontSize(9).Italic().FontColor(Colors.Grey.Darken2);
                        col.Item().AlignCenter().Text(x =>
                        {
                            x.Span("Trang ");
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                    });
                });
            });

            // Style Helper
            static IContainer CellStyleHeader(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Darken1).PaddingVertical(5).Background(Colors.Grey.Lighten3).DefaultTextStyle(x => x.Bold().FontSize(10));
            static IContainer CellStyleData(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(8).DefaultTextStyle(x => x.FontSize(10));

            using var stream = new MemoryStream();
            doc.GeneratePdf(stream);
            return stream.ToArray();
        }
    }
}