using ElectricVehicleDealer.BLL.Extensions;
using ElectricVehicleDealer.BLL.Intergations.Interfaces;
using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Enum;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Implementations
{
    public class AgreementsService : IAgreementsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGoogleDriveService _driveService;

        public AgreementsService(IUnitOfWork unitOfWork, IGoogleDriveService driveService)
        {
            _unitOfWork = unitOfWork;
            _driveService = driveService;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<AgreementResponse> AddAgreementAsync(CreateAgreementRequest dto)
        {
           
            var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId);
            if (customer == null) throw new Exception("Customer not found");

            // Lấy thông tin Order chi tiết
            var order = await _unitOfWork.Agreements.GetOrderDetailsForAgreementAsync(dto.OrderId);
            if (order == null) throw new Exception("Order not found or missing details.");

           
            int? finalStoreId = null;

            // Nếu JSON có gửi storeId (khác 0) thì lấy nó
            if (dto.StoreId != 0 && dto.StoreId != null)
            {
                finalStoreId = dto.StoreId;
            }
            else
            {
                finalStoreId = order.StoreId;
            }

            // Lấy object Store từ DB để dùng tạo PDF và trả về API
            Store? store = null;
            if (finalStoreId.HasValue && finalStoreId.Value > 0)
            {
                store = await _unitOfWork.Repository<Store>().GetByIdAsync(finalStoreId.Value);
            }
            // --------------------------------------------------

            // 2. Tạo Entity
            var newAgreement = new Agreement
            {
                CustomerId = dto.CustomerId,
                TermsAndConditions = dto.TermsAndConditions,
                Status = dto.Status ?? AgreementEnum.Pending,
                FileUrl = "",
                StoreId = finalStoreId,
                AgreementDate = dto.AgreementDate ?? DateTime.Now
            };

            await _unitOfWork.Agreements.CreateAsync(newAgreement);
            await _unitOfWork.SaveAsync();

            // 3. Tạo PDF & Upload Google Drive
            var pdfBytes = GenerateAgreementPdf(newAgreement, customer, store, order);

            using (var stream = new MemoryStream(pdfBytes))
            {
                string fileName = $"HopDong_MuaBan_{newAgreement.AgreementId}_{DateTime.Now:yyyyMMdd}.pdf";
                string driveLink = await _driveService.UploadFileAsync(stream, fileName, "application/pdf");

                if (string.IsNullOrEmpty(driveLink))
                {
                    throw new Exception("Upload Google Drive thất bại.");
                }

                newAgreement.FileUrl = driveLink;
                await _unitOfWork.Agreements.UpdateAsync(newAgreement);
                await _unitOfWork.SaveAsync();
            }

            
            newAgreement.Customer = customer;
            newAgreement.Store = store;
            

            return MapToResponse(newAgreement);
        }

        public async Task<bool> DeleteAgreementAsync(int id)
        {
            await _unitOfWork.Agreements.DeleteAsync(id);
            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<AgreementResponse> GetAgreementByIdAsync(int id)
        {
            var agreement = await _unitOfWork.Agreements.GetByIdAsync(id);
            if (agreement == null) throw new Exception("Agreement not found");
            return MapToResponse(agreement);
        }

        public async Task<List<AgreementResponse>> GetAllAgreementsAsync()
        {
            var agreements = await _unitOfWork.Agreements.GetAll();
            if (agreements == null || !agreements.Any()) return new List<AgreementResponse>();

            return agreements.Select(a => MapToResponse(a)).ToList();
        }

        public async Task<bool> UpdateAgreementAsync(UpdateAgreementRequest dto, int id)
        {
            var agreement = await _unitOfWork.Agreements.GetByIdAsync(id);
            if (agreement == null) throw new Exception("Agreement not found");

            if (dto.Status.HasValue)
                agreement.Status = dto.Status.Value;

            if (!string.IsNullOrEmpty(dto.TermsAndConditions))
                agreement.TermsAndConditions = dto.TermsAndConditions;

            await _unitOfWork.Agreements.UpdateAsync(agreement);
            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<AgreementResponse> GetAgreementByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<AgreementResponse>> GetPagedAgreementsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            string? sortBy = null,
            bool sortDesc = false,
            AgreementEnum? statusFilter = null,
            int? storeIdFilter = null)
        {
            var query = _unitOfWork.Agreements.GetAllQuery()
                .Include(a => a.Customer)
                .Include(a => a.Store)
                .AsQueryable();

            if (statusFilter.HasValue)
                query = query.Where(a => a.Status == statusFilter.Value);

            if (storeIdFilter.HasValue && storeIdFilter.Value != 0)
                query = query.Where(a => a.StoreId == storeIdFilter.Value);

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(a =>
                    a.Customer.FullName.ToLower().Contains(search) ||
                    (a.TermsAndConditions != null && a.TermsAndConditions.ToLower().Contains(search)));
            }

            query = sortBy?.ToLower() switch
            {
                "status" => sortDesc ? query.OrderByDescending(a => a.Status) : query.OrderBy(a => a.Status),
                "agreementdate" => sortDesc ? query.OrderByDescending(a => a.AgreementDate) : query.OrderBy(a => a.AgreementDate),
                _ => query.OrderByDescending(a => a.AgreementDate)
            };

            var paged = await query.ToPagedResultAsync(pageNumber, pageSize);
            var mapped = paged.Items.Select(a => MapToResponse(a)).ToList();

            return new PagedResult<AgreementResponse>
            {
                Items = mapped,
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }

        private AgreementResponse MapToResponse(Agreement agreement)
        {
            var response = new AgreementResponse
            {
                AgreementId = agreement.AgreementId,
                CustomerId = agreement.CustomerId,
                CustomerName = agreement.Customer?.FullName ?? "Unknown",
                TermsAndConditions = agreement.TermsAndConditions,
                StoreId = agreement.StoreId,
                Status = agreement.Status,
                AgreementDate = agreement.AgreementDate,
                FileUrl = agreement.FileUrl
            };

            if (agreement.Customer != null)
            {
                response.Customer = new CustomerResponse
                {
                    CustomerId = agreement.Customer.CustomerId,
                    FullName = agreement.Customer.FullName,
                    Email = agreement.Customer.Email,
                    Phone = agreement.Customer.Phone,
                    Address = agreement.Customer.Address
                };
            }

            if (agreement.Store != null)
            {
                response.Store = new StoreResponse
                {
                    StoreId = agreement.Store.StoreId,
                    StoreName = agreement.Store.StoreName,
                    Address = agreement.Store.Address,
                    Email = agreement.Store.Email
                };
            }

            return response;
        }

        private byte[] GenerateAgreementPdf(Agreement agreement, Customer customer, Store? store, Order order)
        {
            // --- 1. KHAI BÁO BIẾN ---
            var vehicle = order.Vehicle;
            var quote = order.Quote;
            var dealer = order.Dealer;
            var brand = order.Vehicle.Brand;
            var promotion = quote.Promotion;

            // Helper định dạng tiền tệ
            Func<decimal?, string> formatMoney = (amount) =>
            {
                return (amount.HasValue ? amount.Value.ToString("#,##0", CultureInfo.GetCultureInfo("vi-VN")) : "0") + " VNĐ";
            };

            // Helper định dạng thông số xe
            Func<object?, string, string> formatSpec = (value, unit) =>
            {
                if (value == null) return "---";
                if (value is string str && string.IsNullOrWhiteSpace(str)) return "---";
                return $"{value} {unit}".Trim();
            };

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // --- NỘI DUNG (Content) CÓ KHUNG VIỀN ---
                    page.Content()
                        .Border(1)
                        .BorderColor(Colors.Black)
                        .Padding(20)
                        .Column(col =>
                        {
                            // 1. HEADER
                            col.Item().AlignCenter().Text("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM").Bold();
                            col.Item().AlignCenter().Text("Độc lập - Tự do - Hạnh phúc").Bold().Underline();
                            col.Item().PaddingTop(20).AlignCenter().Text("HỢP ĐỒNG MUA BÁN XE").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
                            col.Item().PaddingTop(5).AlignCenter().Text($"Số: {agreement.AgreementId}/HĐMB | Ngày đặt: {order.OrderDate?.ToString("dd/MM/yyyy")}");

                            col.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            col.Item().PaddingBottom(10).Text($"Hôm nay, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}, tại văn phòng đại lý, chúng tôi gồm có:");

                            // 2. THÔNG TIN CÁC BÊN
                            col.Item().Row(row =>
                            {
                                // BÊN A
                                row.RelativeColumn().Border(1).BorderColor(Colors.Grey.Lighten3).Padding(10).Column(c =>
                                {
                                    c.Item().Text("BÊN A (BÊN BÁN)").Bold().FontSize(11).FontColor(Colors.Blue.Medium);
                                    string storeName = !string.IsNullOrEmpty(store?.StoreName) ? store.StoreName : "EV DEALER SYSTEM";
                                    c.Item().Text(storeName.ToUpper()).Bold();
                                    string storeAddress = !string.IsNullOrEmpty(store?.Address) ? store.Address : "--- (Chưa cập nhật địa chỉ) ---";
                                    c.Item().Text($"Đ/c: {storeAddress}");
                                    c.Item().Text($"Đại diện: {dealer.FullName}");
                                });

                                row.Spacing(15);

                                // BÊN B
                                row.RelativeColumn().Border(1).BorderColor(Colors.Grey.Lighten3).Padding(10).Column(c =>
                                {
                                    c.Item().Text("BÊN B (BÊN MUA)").Bold().FontSize(11).FontColor(Colors.Blue.Medium);
                                    c.Item().Text(customer.FullName.ToUpper()).Bold();
                                    c.Item().Text($"Đ/c: {customer.Address}");
                                    c.Item().Text($"SĐT: {customer.Phone}");
                                    c.Item().Text($"CCCD: {customer.LicenseUp ?? "---"}");
                                });
                            });

                            col.Item().PaddingVertical(15).Text("Hai bên thống nhất ký kết hợp đồng mua bán xe ô tô với các điều khoản cụ thể như sau:").Italic();

                            // 3. ĐIỀU 1: CHI TIẾT XE (FULL FIELDS)
                            int quantity = order.Quantity ?? 1;

                            col.Item().PaddingTop(5).Text("ĐIỀU 1: ĐỐI TƯỢNG HỢP ĐỒNG").Bold().FontSize(11);
                            col.Item().PaddingBottom(5).Text($"Bên A bán và Bên B mua {quantity:00} xe ô tô điện với thông số kỹ thuật chi tiết:");

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns => { columns.RelativeColumn(4); columns.RelativeColumn(6); });

                                static IContainer CellHeader(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(5);
                                static IContainer CellLabel(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5);
                                static IContainer CellValue(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5);

                                // --- I. THÔNG TIN CƠ BẢN ---
                                table.Cell().ColumnSpan(2).Element(CellHeader).Text("I. THÔNG TIN CƠ BẢN").Bold();
                                table.Cell().Element(CellLabel).Text("Hãng & Model:");
                                table.Cell().Element(CellValue).Text($"{brand.BrandName} {vehicle.ModelName}").Bold();
                                table.Cell().Element(CellLabel).Text("Phiên bản / Năm:");
                                table.Cell().Element(CellValue).Text($"{vehicle.Version ?? "Tiêu chuẩn"} / {vehicle.Year}");
                                table.Cell().Element(CellLabel).Text("Màu sắc:");
                                table.Cell().Element(CellValue).Text(vehicle.Color ?? "---");
                                table.Cell().Element(CellLabel).Text("Loại xe / Số cửa:");
                                table.Cell().Element(CellValue).Text($"{vehicle.VehicleType} / {formatSpec(vehicle.DoorCount, "cửa")}");
                                table.Cell().Element(CellLabel).Text("Số chỗ ngồi:");
                                table.Cell().Element(CellValue).Text($"{vehicle.SeatingCapacity} chỗ");

                                // --- II. KÍCH THƯỚC & VẬN HÀNH ---
                                table.Cell().ColumnSpan(2).Element(CellHeader).Text("II. KÍCH THƯỚC & VẬN HÀNH").Bold();
                                table.Cell().Element(CellLabel).Text("Kích thước (DxRxC):");
                                table.Cell().Element(CellValue).Text($"{vehicle.LengthMm} x {vehicle.WidthMm} x {vehicle.HeightMm} mm");
                                table.Cell().Element(CellLabel).Text("Động cơ / Mã lực:");
                                table.Cell().Element(CellValue).Text(formatSpec(vehicle.Horsepower, "HP"));
                                table.Cell().Element(CellLabel).Text("Pin / Quãng đường:");
                                table.Cell().Element(CellValue).Text($"{vehicle.BatteryCapacity} / {vehicle.RangePerCharge}");
                                table.Cell().Element(CellLabel).Text("Hộp số / Khung gầm:");
                                table.Cell().Element(CellValue).Text($"{vehicle.Transmission ?? "Tự động"} / {vehicle.FrameChassis}");
                                table.Cell().Element(CellLabel).Text("Giới hạn lái khuyến nghị:");
                                table.Cell().Element(CellValue).Text(formatSpec(vehicle.DailyDrivingLimit, "km/ngày"));

                                // --- III. NGOẠI THẤT ---
                                table.Cell().ColumnSpan(2).Element(CellHeader).Text("III. NGOẠI THẤT").Bold();
                                table.Cell().Element(CellLabel).Text("Đèn trước / sau:");
                                table.Cell().Element(CellValue).Text($"{vehicle.Headlights} / {vehicle.Taillights}");
                                table.Cell().Element(CellLabel).Text("Mâm & Bánh xe:");
                                table.Cell().Element(CellValue).Text(vehicle.Wheels);
                                table.Cell().Element(CellLabel).Text("Gương chiếu hậu:");
                                table.Cell().Element(CellValue).Text(vehicle.Mirrors);
                                table.Cell().Element(CellLabel).Text("Kính xe:");
                                table.Cell().Element(CellValue).Text(vehicle.GlassWindows);

                                // --- IV. NỘI THẤT & TIỆN NGHI ---
                                table.Cell().ColumnSpan(2).Element(CellHeader).Text("IV. NỘI THẤT & TIỆN NGHI").Bold();
                                table.Cell().Element(CellLabel).Text("Chất liệu Ghế / Nội thất:");
                                table.Cell().Element(CellValue).Text($"{vehicle.SeatMaterial} / {vehicle.InteriorMaterial}");
                                table.Cell().Element(CellLabel).Text("Màn hình / Loa:");
                                table.Cell().Element(CellValue).Text($"{vehicle.Screen} / {vehicle.SpeakerSystem}");
                                table.Cell().Element(CellLabel).Text("Điều hòa:");
                                table.Cell().Element(CellValue).Text(vehicle.AirConditioning);
                                table.Cell().Element(CellLabel).Text("Tiện ích khác:");
                                table.Cell().Element(CellValue).Text($"Tủ: {vehicle.InVehicleCabinet} | Cốp: {formatSpec(vehicle.TrunkCapacity, "L")}");

                                // --- V. AN TOÀN & BẢO HÀNH ---
                                table.Cell().ColumnSpan(2).Element(CellHeader).Text("V. AN TOÀN & BẢO HÀNH").Bold();
                                table.Cell().Element(CellLabel).Text("Túi khí:");
                                table.Cell().Element(CellValue).Text(formatSpec(vehicle.Airbags, "túi khí"));
                                table.Cell().Element(CellLabel).Text("Camera / Cảm biến:");
                                table.Cell().Element(CellValue).Text(vehicle.Cameras);
                                table.Cell().Element(CellLabel).Text("Bảo hành:");
                                table.Cell().Element(CellValue).Text(vehicle.WarrantyPeriod);
                            });

                            // 4. ĐIỀU 2: GIÁ CẢ & THANH TOÁN
                            col.Item().PaddingTop(10).Text("ĐIỀU 2: GIÁ CẢ VÀ THANH TOÁN").Bold().FontSize(11);
                            col.Item().PaddingBottom(5).Text("2.1. Giá trị hợp đồng được tính toán chi tiết như sau:");

                            // --- LOGIC TÍNH GIÁ ---
                            decimal rawTax = quote.TaxRate;
                            decimal taxRateCalc = (rawTax > 1) ? rawTax / 100m : rawTax;
                            decimal taxPercentDisplay = taxRateCalc * 100m;

                            decimal finalTotal = order.TotalPrice ?? 0m;
                            decimal netPriceAfterDiscountTotal = finalTotal / (1m + taxRateCalc);
                            decimal taxAmountTotal = finalTotal - netPriceAfterDiscountTotal;

                            decimal unitListPrice = vehicle.Price ?? 0m;
                            decimal unitDiscountAmount = 0m;

                            if (promotion != null && promotion.DiscountPercent > 0)
                            {
                                decimal percentValue = promotion.DiscountPercent ?? 0m;
                                decimal discountRate = (percentValue > 1) ? percentValue / 100m : percentValue;
                                unitDiscountAmount = unitListPrice * discountRate;
                            }

                            decimal totalListPrice = unitListPrice * quantity;
                            decimal totalDiscountAmount = unitDiscountAmount * quantity;
                            decimal totalNetPriceBeforeTax = totalListPrice - totalDiscountAmount;

                            // --- BẢNG GIÁ ---
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns => { columns.RelativeColumn(7); columns.RelativeColumn(3); });

                                table.Header(header =>
                                {
                                    header.Cell().BorderBottom(1).Padding(5).Text("Nội dung").Bold();
                                    header.Cell().BorderBottom(1).Padding(5).AlignRight().Text("Thành tiền (VNĐ)").Bold();
                                });

                                // A. GIÁ NIÊM YẾT
                                table.Cell().Container().Padding(5).Column(c => {
                                    c.Item().Text($"Giá niêm yết xe {vehicle.ModelName} (Chưa VAT)");
                                    c.Item().Text($"Đơn giá: {formatMoney(unitListPrice)} x SL: {quantity} xe").FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
                                });
                                table.Cell().AlignRight().Padding(5).Text(formatMoney(totalListPrice)).Bold();

                                // B. KHUYẾN MÃI
                                if (promotion != null && totalDiscountAmount > 0)
                                {
                                    decimal percentValue = promotion.DiscountPercent ?? 0m;
                                    decimal discountDisplay = (percentValue > 1) ? percentValue : percentValue * 100m;
                                    table.Cell().Container().Padding(5).Column(c => {
                                        c.Item().Text($"Giảm giá khuyến mãi: {promotion.Title} (-{discountDisplay:0.##}%)").FontColor(Colors.Green.Darken1);
                                        c.Item().Text($"Đơn giá giảm: {formatMoney(unitDiscountAmount)} x SL: {quantity} xe").FontSize(9).Italic().FontColor(Colors.Green.Darken1);
                                    });
                                    table.Cell().AlignRight().Padding(5).Text($"- {formatMoney(totalDiscountAmount)}").FontColor(Colors.Green.Darken1).Bold();
                                }

                                // C. GIÁ TRỊ XE SAU KM
                                table.Cell().ColumnSpan(2).BorderTop(1).PaddingVertical(3).Row(row =>
                                {
                                    row.RelativeItem().Text("A. TỔNG GIÁ TRỊ XE SAU KHUYẾN MÃI (Chưa VAT):").Bold().FontSize(10);
                                    row.RelativeItem().AlignRight().Text(formatMoney(totalNetPriceBeforeTax)).Bold().FontSize(10);
                                });

                                // D. THUẾ
                                table.Cell().Container().Padding(5).Text($"B. Thuế GTGT ({taxPercentDisplay:0.##}%) áp dụng cho Giá A:");
                                table.Cell().AlignRight().Padding(5).Text(formatMoney(taxAmountTotal));

                                // E. TỔNG CỘNG
                                table.Cell().ColumnSpan(2).BorderTop(1).BorderBottom(1).PaddingVertical(5).Row(row =>
                                {
                                    row.RelativeItem().Text("TỔNG GIÁ TRỊ HỢP ĐỒNG (A + B):").Bold().FontSize(12);
                                    row.RelativeItem().AlignRight().Text(formatMoney(finalTotal)).Bold().FontSize(12).FontColor(Colors.Red.Darken2);
                                });
                            });

                            col.Item().PaddingTop(5).Text("2.2. Phương thức thanh toán: Bên B thanh toán cho Bên A bằng hình thức chuyển khoản hoặc tiền mặt trước khi nhận bàn giao xe.");

                            // 5. CHỮ KÝ
                            col.Item().PaddingTop(20).Row(row =>
                            {
                                row.RelativeColumn().Column(c =>
                                {
                                    c.Item().AlignCenter().Text("ĐẠI DIỆN BÊN A").Bold();
                                    c.Item().AlignCenter().Text("(Ký, ghi rõ họ tên)").FontSize(9).Italic();
                                    c.Item().PaddingTop(60).AlignCenter().Text(dealer.FullName).Bold();
                                });
                                row.RelativeColumn().Column(c =>
                                {
                                    c.Item().AlignCenter().Text("ĐẠI DIỆN BÊN B").Bold();
                                    c.Item().AlignCenter().Text("(Ký, ghi rõ họ tên)").FontSize(9).Italic();
                                    c.Item().PaddingTop(60).AlignCenter().Text(customer.FullName).Bold();
                                });
                            });
                        });

                    // --- FOOTER ---
                    page.Footer().PaddingTop(5).AlignCenter().Text(x =>
                    {
                        x.Span("Trang ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            using var stream = new MemoryStream();
            doc.GeneratePdf(stream);
            return stream.ToArray();
        }
    }
}