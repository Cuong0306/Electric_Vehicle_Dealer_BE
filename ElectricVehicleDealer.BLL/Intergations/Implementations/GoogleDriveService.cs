using ElectricVehicleDealer.BLL.Intergations.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Intergations.Implementations
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly IConfiguration _configuration;
        public GoogleDriveService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            try
            {
                // 1. Lấy thông tin từ appsettings.json
                var clientId = _configuration["GoogleDrive:ClientId"];
                var clientSecret = _configuration["GoogleDrive:ClientSecret"];
                var refreshToken = _configuration["GoogleDrive:RefreshToken"];
                var folderId = _configuration["GoogleDrive:FolderId"];

                // Kiểm tra xem config có đủ không
                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(refreshToken))
                {
                    throw new Exception("Thiếu cấu hình GoogleDrive trong appsettings.json (ClientId, ClientSecret, RefreshToken).");
                }

                // 2. Tạo Credential từ Refresh Token (Không cần đăng nhập lại)
                var tokenResponse = new TokenResponse { RefreshToken = refreshToken };

                var credential = new UserCredential(new GoogleAuthorizationCodeFlow(
                    new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = new ClientSecrets
                        {
                            ClientId = clientId,
                            ClientSecret = clientSecret
                        }
                    }),
                    "user",
                    tokenResponse);

                // 3. Khởi tạo Drive Service
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "EV Dealer App",
                });

                // 4. Cấu hình file upload
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = fileName,
                    Parents = new List<string> { folderId } // Upload vào folder
                };

                FilesResource.CreateMediaUpload request;
                request = service.Files.Create(fileMetadata, fileStream, contentType);
                request.Fields = "id, webViewLink, webContentLink";

                // 5. Thực hiện Upload
                var progress = await request.UploadAsync();

                if (progress.Status != Google.Apis.Upload.UploadStatus.Completed)
                {
                    throw new Exception("Upload failed: " + progress.Exception?.Message);
                }

                var file = request.ResponseBody;

                // Trả về link xem file
                return file.WebViewLink;
            }
            catch (Exception ex)
            {
                throw new Exception($"Google Drive OAuth Error: {ex.Message}");
            }
        }
    }
}