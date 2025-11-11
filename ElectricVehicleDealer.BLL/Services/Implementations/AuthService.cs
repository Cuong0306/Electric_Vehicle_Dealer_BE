    using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Enum;
using ElectricVehicleDealer.DAL.Services.Interfaces;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Config;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ElectricVehicleDealer.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork, IOptions<JwtSettings> jwtOptions)
        {
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<StaffResponse?> CreateStaffAsync(CreateStaffRequest dto)
        {
            string role = dto.Role.ToString();
            var normalizedEmail = dto.Email?.Trim().ToLower();
            var staff = await _unitOfWork.Staff.GetByEmailAsync(normalizedEmail);

            if (await _unitOfWork.Staff.IsEmailExistsAsync(dto.Email, 0))
                throw new Exception("Email đã được sử dụng.");

            var newStaff = new Staff
            {
                BrandId = dto.BrandId,
                FullName = dto.FullName,
                Email = normalizedEmail,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                Role = Enum.Parse<RoleStaffEnum>(role),
                Status = "Active"
            };

            // Thêm người dùng mới vào cơ sở dữ liệu
            await _unitOfWork.Staff.CreateAsync(newStaff);
            await _unitOfWork.SaveAsync();

            // Trả về thông tin người dùng mới
            return new StaffResponse
            {
                StaffId = newStaff.StaffId,
                BrandId = newStaff.BrandId,
                FullName = newStaff.FullName,
                Email = newStaff.Email,
                Phone = newStaff.Phone,
                Role = newStaff.Role,
                Position = newStaff.Position,
            };
        }
        public async Task<DealerResponse?> CreateDealerAsync(CreateDealerRequest dto)
        {
            string role = dto.Role.ToString();
            var normalizedEmail = dto.Email?.Trim().ToLower();
            var dealer = await _unitOfWork.Dealers.GetByEmailAsync(normalizedEmail);

            if (await _unitOfWork.Dealers.IsEmailExistsAsync(dto.Email, 0))
                throw new Exception("Email đã được sử dụng.");

            var newDealer = new Dealer
            {
                StoreId = dto.StoreId,
                FullName = dto.FullName,
                Email = normalizedEmail,
                Address = dto.Address,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                Role = Enum.Parse<RoleDealerEnum>(role),
                Status = "Active"
            };

            // Thêm người dùng mới vào cơ sở dữ liệu
            await _unitOfWork.Dealers.CreateAsync(newDealer);
            await _unitOfWork.SaveAsync();

            // Trả về thông tin người dùng mới
            return new DealerResponse
            {
                DealerId = newDealer.DealerId,
                StoreId = newDealer.StoreId,
                Address = newDealer.Address,
                FullName = newDealer.FullName,
                Email = newDealer.Email,
                Phone = newDealer.Phone,
                Role = newDealer.Role,
                Status = newDealer.Status

            };
        }
        public async Task<LoginResponse?> LoginAsync(LoginRequest dto)
        {
            var email = dto.Email?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(email)) throw new Exception("Email is required");

            // Thử tìm ở cả hai bảng
            var staff = await _unitOfWork.Staff.GetByEmailAsync(email);
            var dealer = await _unitOfWork.Dealers.GetByEmailAsync(email);

            // Ưu tiên đối tượng khớp mật khẩu
            var now = DateTime.UtcNow;
            var lifetime = TimeSpan.FromMinutes(_jwtSettings.ExpiryMinutes);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            // Nếu có staff và password đúng => đăng nhập staff
            if (staff != null && BCrypt.Net.BCrypt.Verify(dto.Password, staff.Password))
            {
                var claims = new[]
                {
            new Claim(ClaimTypes.NameIdentifier, staff.StaffId.ToString()),
            new Claim(ClaimTypes.Email, staff.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, staff.FullName ?? string.Empty),
            new Claim(ClaimTypes.Role, staff.Role.ToString()),
            new Claim("user_type", "staff")
        };

                var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    NotBefore = now,
                    IssuedAt = now,
                    Expires = now.Add(lifetime),
                    Issuer = _jwtSettings.Issuer,
                    Audience = _jwtSettings.Audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                });

                return new LoginResponse { Token = tokenHandler.WriteToken(token), ExpiresIn = (int)lifetime.TotalSeconds };
            }

            // Nếu có dealer và password đúng => đăng nhập dealer
            if (dealer != null && BCrypt.Net.BCrypt.Verify(dto.Password, dealer.Password))
            {
                var claims = new[]
                {
            new Claim(ClaimTypes.NameIdentifier, dealer.DealerId.ToString()),
            new Claim(ClaimTypes.Email, dealer.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, dealer.FullName ?? string.Empty),
            new Claim(ClaimTypes.Role, dealer.Role.ToString()),
            new Claim("user_type", "dealer")
        };

                var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    NotBefore = now,
                    IssuedAt = now,
                    Expires = now.Add(lifetime),
                    Issuer = _jwtSettings.Issuer,
                    Audience = _jwtSettings.Audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                });

                return new LoginResponse { Token = tokenHandler.WriteToken(token), ExpiresIn = (int)lifetime.TotalSeconds };
            }

            // Không khớp tài khoản nào
            throw new Exception("Invalid email or password");
        }


    }
}
