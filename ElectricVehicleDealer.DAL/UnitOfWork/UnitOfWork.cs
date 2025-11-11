using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories.Implementations;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private Hashtable _repositories;
        private bool _disposed;

        public IAgreementsRepository Agreements { get; }

        public IStaffRepository Staff { get; }

        public IDealerRepository Dealers { get; }

        public IVehicleRepository Vehicles { get; }

        public IStorageRepository Storages { get; }
        public IOrderRepository Orders { get; }

        public IPaymentRepository Payments { get; }

        public UnitOfWork(
            AppDbContext context,
            IAgreementsRepository agreementsRepository,
            IStaffRepository staffRepository,
            IDealerRepository dealers,
            IVehicleRepository vehicles,
            IStorageRepository storages,
            IOrderRepository orders,
            IPaymentRepository payments)
        {
            _context = context;
            Agreements = agreementsRepository;
            Staff = staffRepository;
            Dealers = dealers;
            Vehicles = vehicles;
            Storages = storages;
            Orders = orders;
            _repositories = new Hashtable();
            Payments = payments;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
               _context.Dispose();
            }
            _disposed = true;
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
                _repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<TEntity>)_repositories[type]!;
        }

        public async Task<int> SaveAsync()
        {
           
            return await _context.SaveChangesAsync();
        }
    }
}
