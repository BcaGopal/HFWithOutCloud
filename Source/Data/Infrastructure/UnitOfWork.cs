﻿#region

using Model;
using System;
using System.Collections;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;


#endregion

namespace Data.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Private Fields

        private readonly IDataContext _context;
        private readonly Guid _instanceId;
        private bool _disposed;
        private Hashtable _repositories;

        #endregion Private Fields

        #region Constuctor/Dispose

        public UnitOfWork(IDataContext context)
        {
            _context = context;
            _instanceId = Guid.NewGuid();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }

        #endregion Constuctor/Dispose

        public Guid InstanceId
        {
            get { return _instanceId; }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        public Task<int> SaveAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : EntityBase
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();
            }

            var type = typeof (TEntity).Name;

            if (_repositories.ContainsKey(type))
            {
                return (IRepository<TEntity>) _repositories[type];
            }

            var repositoryType = typeof (Repository<>);
            _repositories.Add(type, Activator.CreateInstance(repositoryType.MakeGenericType(typeof (TEntity)), _context));

            return (IRepository<TEntity>) _repositories[type];
        }

        public DbRawSqlQuery<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return ((DataContext)_context).Database.SqlQuery<TElement>(sql, parameters);
        }
    }
}
