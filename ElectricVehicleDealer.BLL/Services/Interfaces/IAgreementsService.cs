using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces
{
    public interface IAgreementsService
    {
        Task<List<AgreementResponse>> GetAllAgreementsAsync();
        Task<AgreementResponse> GetAgreementByIdAsync(int id);
        Task<AgreementResponse> GetAgreementByNameAsync(string name);
        Task<AgreementResponse> AddAgreementAsync(CreateAgreementRequest dto);
        Task<bool> DeleteAgreementAsync(int id);
        Task<bool> UpdateAgreementAsync(UpdateAgreementRequest dto, int id);
    }
}
