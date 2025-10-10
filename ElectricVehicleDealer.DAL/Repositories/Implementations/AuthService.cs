using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ElectricVehicleDealer.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string?> LoginAsync(LoginRequest model)
        {
            if (model is null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return null;

            // Chuẩn hóa email
            var email = model.Email.Trim().ToLowerInvariant();

            // ---- 1) Thử Staff trước
            var staff = await _context.Staff
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email != null && s.Email.ToLower() == email);

            if (staff is not null)
            {
                // Nếu staff.Password đã hash BCrypt:
                var ok = BCrypt.Net.BCrypt.Verify(model.Password, staff.Password);

                // Nếu hiện tại staff còn đang lưu plain-text, tạm fallback:
                if (!ok && staff.Password == model.Password) ok = true;

                if (!ok) return null;

                return GenerateJwtToken(
                    subjectId: staff.StaffId.ToString(),
                    name: staff.FullName,
                    email: staff.Email,
                    role: "Staff",
                    storeId: staff.StoreId?.ToString()
                );
            }

            // ---- 2) Thử Dealer
            var dealer = await _context.Dealers
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Email != null && d.Email.ToLower() == email);

            if (dealer is not null)
            {
                // Dealer bạn đã hash BCrypt khi tạo
                var ok = BCrypt.Net.BCrypt.Verify(model.Password, dealer.Password);

                // (tuỳ chọn) fallback nếu dữ liệu cũ còn plain-text
                if (!ok && dealer.Password == model.Password) ok = true;

                if (!ok) return null;

                return GenerateJwtToken(
                    subjectId: dealer.DealerId.ToString(),
                    name: dealer.FullName,
                    email: dealer.Email,
                    role: "Dealer",
                    storeId: dealer.StoreId?.ToString()
                );
            }

            // ---- 3) Không tìm thấy
            return null;
        }

        private string GenerateJwtToken(string subjectId, string name, string? email, string role, string? storeId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, subjectId),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email ?? string.Empty),
                new Claim(ClaimTypes.Role, role),             // <-- phân quyền
                new Claim("UserType", role),                  // <-- thêm 1 claim phụ nếu cần
                new Claim("StoreId", storeId ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
