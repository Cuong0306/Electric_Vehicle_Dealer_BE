using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        IAgreementsRepository Agreements { get; }
        IStaffRepository Staff { get; }
        IDealerRepository Dealers { get; }
        IVehicleRepository Vehicles { get; }
        IStorageRepository Storages { get; }
        IOrderRepository Orders { get; }
        IPaymentRepository Payments { get; }
        ITestAppointmentRepository TestAppointments { get; }
        IQuoteRepository Quotes { get; }
        ICustomerRepository Customers { get; }
        IPromotionRepository Promotions { get; }
        Task<int> SaveAsync();
    }
}
