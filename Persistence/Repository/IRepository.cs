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
        void Remove(int id);
        void Update(T item);
        T FindByID(int id);
        IEnumerable<T> FindAll();
        IEnumerable<T> FindAllBy(string deviceId, DateTime start, DateTime end);
        IEnumerable<string> FindUniqueDevice();
        IEnumerable<T> FindAllPendingByDeviceName(string deviceName);
    }
}
