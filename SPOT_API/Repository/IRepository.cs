using SPOT_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Repository
{
    public interface IRepository<T> where T : BaseDapper
    {
        void Add(T item);
        void Remove(Guid id);
        void Update(T item);
        T FindByID(Guid id);
        IEnumerable<T> FindAll();
    }
}
