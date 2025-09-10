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
        //private readonly DbContext _context;
        private Hashtable _repositories;
        private bool _disposed;
        public UnitOfWork(/*DbContext context*/)
        {
            //_context = context;
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
               // _context.Dispose();
            }
            _disposed = true;
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                //var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
               // _repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<TEntity>)_repositories[type]!;
        }

        public Task<int> SaveAsync()
        {
            throw new NotImplementedException();
            //return await _context.SaveChangesAsync();
        }
    }
}
