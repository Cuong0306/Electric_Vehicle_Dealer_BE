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
            // 1. Validate & Lấy dữ liệu
            var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId);
            if (customer == null) throw new Exception("Customer not found");

            Store? store = null;
            if (dto.StoreId != 0)
            {
                store = await _unitOfWork.Repository<Store>().GetByIdAsync(dto.StoreId);
            }

            // 2. Tạo Entity
            var newAgreement = new Agreement
            {
                CustomerId = dto.CustomerId,
                TermsAndConditions = dto.TermsAndConditions,
                Status = dto.Status ?? AgreementEnum.Pending,
                FileUrl = "",
                StoreId = dto.StoreId == 0 ? null : dto.StoreId,
                AgreementDate = dto.AgreementDate ?? DateTime.Now
            };

            await _unitOfWork.Agreements.CreateAsync(newAgreement);
            await _unitOfWork.SaveAsync();

            // 3. Tạo PDF & Upload Google Drive

            var pdfBytes = GenerateAgreementPdf(newAgreement, customer, store);

            using (var stream = new MemoryStream(pdfBytes))
            {
                string fileName = $"HopDong_MuaBan_{newAgreement.AgreementId}_{DateTime.Now:yyyyMMdd}.pdf";

                // Upload lên Drive
                string driveLink = await _driveService.UploadFileAsync(stream, fileName, "application/pdf");

                if (string.IsNullOrEmpty(driveLink))
                {
                    throw new Exception("Upload Google Drive thất bại: Không nhận được Link trả về.");
                }

                // 4. Cập nhật Link vào DB
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
                    //Hotline = agreement.Store.Hotline,
                    Email = agreement.Store.Email
                };
            }

            return response;
        }

        private byte[] GenerateAgreementPdf(Agreement agreement, Customer customer, Store? store)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    // HEADER
                    page.Header().Column(col =>
                    {
                        col.Item().AlignCenter().Text("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM").Bold();
                        col.Item().AlignCenter().Text("Độc lập - Tự do - Hạnh phúc").Bold().Underline();
                        col.Item().PaddingTop(20).AlignCenter().Text("HỢP ĐỒNG MUA BÁN XE").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
                        col.Item().PaddingTop(5).AlignCenter().Text($"Số: {agreement.AgreementId}/HĐMB").Italic();
                    });

                    // NỘI DUNG
                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Item().PaddingBottom(10).Text($"Hôm nay, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}, chúng tôi gồm có:");

                        // BÊN A (Bán)
                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(c =>
                        {
                            c.Item().Text("BÊN A (BÊN BÁN):").Bold().FontSize(12);
                            c.Item().Text($"Tên đơn vị: {store?.StoreName ?? "CÔNG TY CỔ PHẦN EV DEALER"}").Bold();
                            c.Item().Text($"Địa chỉ: {store?.Address ?? "TP. Hồ Chí Minh"}");
                            c.Item().Text("Điện thoại: 1900 1234");
                        });

                        col.Item().PaddingVertical(10);

                        // BÊN B (Mua)
                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(c =>
                        {
                            c.Item().Text("BÊN B (BÊN MUA):").Bold().FontSize(12);
                            string customerNameUpper = customer.FullName?.ToUpper() ?? "KHÁCH HÀNG";
                            c.Item().Text($"Ông/Bà: {customerNameUpper}").Bold();
                            c.Item().Text($"Email: {customer.Email}");
                            c.Item().Text($"Địa chỉ: {customer.Address ?? "...................................."}");
                        });

                        col.Item().PaddingVertical(15);

                        // ĐIỀU KHOẢN
                        col.Item().Text("Hai bên thống nhất ký kết hợp đồng với các điều khoản sau:").Bold();

                        col.Item().PaddingTop(10).Text("Điều 1: Đối tượng hợp đồng").Bold();
                        col.Item().Text("Bên A đồng ý bán và Bên B đồng ý mua xe ô tô điện theo báo giá đã thống nhất.");

                        col.Item().PaddingTop(10).Text("Điều 2: Giá cả và Thanh toán").Bold();
                        col.Item().Text("Thanh toán bằng chuyển khoản hoặc tiền mặt. Bên B có trách nhiệm thanh toán đầy đủ trước khi nhận xe.");

                        col.Item().PaddingTop(10).Text("Điều 3: Thỏa thuận khác").Bold();
                        col.Item().Text(agreement.TermsAndConditions ?? "Theo quy định hiện hành của đại lý.");

                        // CHỮ KÝ
                        col.Item().PaddingTop(50).Row(row =>
                        {
                            row.RelativeColumn().Column(c =>
                            {
                                c.Item().AlignCenter().Text("ĐẠI DIỆN BÊN A").Bold();
                                c.Item().AlignCenter().Text("(Ký, đóng dấu)");
                            });

                            row.RelativeColumn().Column(c =>
                            {
                                c.Item().AlignCenter().Text("ĐẠI DIỆN BÊN B").Bold();
                                c.Item().AlignCenter().Text("(Ký, ghi rõ họ tên)");
                                c.Item().PaddingTop(50).AlignCenter().Text(customer.FullName).Bold();
                            });
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Trang ");
                        x.CurrentPageNumber();
                    });
                });
            });

            using var stream = new MemoryStream();
            doc.GeneratePdf(stream);
            return stream.ToArray();
        }
    }
}