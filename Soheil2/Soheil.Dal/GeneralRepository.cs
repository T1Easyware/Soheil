using System;
using System.Data.Common;
using System.Data.Objects;
using System.Linq;
using Soheil.Dal.Interfaces;

namespace Soheil.Dal
{
    public class Repository
    {
        private readonly SoheilEdmContext _context;

        public Repository(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            _context = unitOfWork as SoheilEdmContext;
        }

        public IQueryable<DbDataRecord> All(string query, params ObjectParameter[] parameters)
        {
            return _context.CreateQuery<DbDataRecord>(query, parameters);
        }

        public DbDataRecord Single(string query, params ObjectParameter[] parameters)
        {
            return _context.CreateQuery<DbDataRecord>(query, parameters).FirstOrDefault();
        }
    }
}