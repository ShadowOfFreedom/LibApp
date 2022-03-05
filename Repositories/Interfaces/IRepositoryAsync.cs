using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibApp.Repositories.Interfaces {
    public interface IRepositoryAsync<T> {
        Task<List<T>> GetAll();
        Task<T> GetById(int id);
        Task<int> Add(T entity);
        Task<int> Update(T entity);
        Task<int> Delete(int id);
    }
}
