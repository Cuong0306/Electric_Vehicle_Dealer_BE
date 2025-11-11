using ElectricVehicleDealer.DAL.Enum;
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
        Task<PagedResult<AgreementResponse>> GetPagedAgreementsAsync(
    int pageNumber = 1,
    int pageSize = 10,
    string? search = null,
    string? sortBy = null,
    bool sortDesc = false,
    AgreementEnum? statusFilter = null,
    int? storeIdFilter = null);
    }
}
