using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Intergations.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
        Task SendEmailWithAttachmentAsync(
    string to,
    string subject,
    string htmlBody,
    byte[] attachmentBytes,
    string attachmentName
);
    }

}
