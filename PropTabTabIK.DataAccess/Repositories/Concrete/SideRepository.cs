using Microsoft.AspNetCore.Http;
using PropTabTabIK.Core.Entity.Concrete;
using PropTabTabIK.DataAccess.Context;
using PropTabTabIK.DataAccess.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;

namespace PropTabTabIK.DataAccess.Repositories.Concrete
{
    public class SideRepository<T> : ISideRepository<T> where T : SideEntity
    {
        private readonly ProjectContext _context;
        private IHttpContextAccessor _httpContext;

        public SideRepository(ProjectContext context, IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
            _context = context;

        }
        public bool Activate(Guid id)
        {
            T activated = GetByID(id);
            activated.Status = Core.Enum.Status.Active;
            return Update(activated);
        }

        public bool DeActivate(Guid id)
        {
            T activated = GetByID(id);
            activated.Status = Core.Enum.Status.Deactive;
            return Update(activated);
        }


        public bool Add(T item)
        {
            _context.Set<T>().Add(item);
            return Save() > 0;

        }

        public bool Add(List<T> items)
        {
            _context.Set<T>().AddRange(items);
            return Save() > 0;
        }

        public bool Any(Expression<Func<T, bool>> exp) => _context.Set<T>().Any(exp);


        public List<T> GetActive() => _context.Set<T>().Where(x => x.Status == Core.Enum.Status.Active).ToList();


        public List<T> GetAll() => _context.Set<T>().ToList();

        public T GetByDefault(Expression<Func<T, bool>> exp) => _context.Set<T>().Where(exp).FirstOrDefault();

        public T GetByID(Guid id)
        {
            return _context.Set<T>().Find(id);
        }

        public List<T> GetDefault(Expression<Func<T, bool>> exp) => _context.Set<T>().Where(exp).ToList();

        public bool Remove(T item)
        {
            item.Status = Core.Enum.Status.Deleted;
            return Update(item);
        }

        public bool Remove(Guid id)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    T deleted = GetByID(id);
                    deleted.Status = Core.Enum.Status.Deleted;
                    ts.Complete();
                    return Update(deleted);
                }
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool RemoveAll(Expression<Func<T, bool>> exp)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var collection = GetDefault(exp);
                    int count = 0;


                    foreach (var item in collection)
                    {
                        item.Status = Core.Enum.Status.Deleted;
                        bool opResult = Update(item);
                        if (opResult) count++;
                    }

                    if (collection.Count == count) ts.Complete();
                    else return false;
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public bool Update(T item)
        {
            try
            {
                _context.Set<T>().Update(item);
                return Save() > 0;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
