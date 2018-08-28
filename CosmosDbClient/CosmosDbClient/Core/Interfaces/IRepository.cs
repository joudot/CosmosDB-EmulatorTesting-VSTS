using ReviewWebsite.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewWebsite.Core.Interfaces
{
    public interface IRepository<T> where T : BaseDocument
    {
        Task<T> GetByIdAsync(string id);
        Task<List<T>> ListAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(string id);
    }
}