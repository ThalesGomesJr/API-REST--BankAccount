using System.Threading.Tasks;
using WeBank.Domain.Models;

namespace WeBank.Repository
{
    public interface IWeBankRepository
    {
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        
        Task<User> GetUserAsyncById(int userId);
        Task<User> GetUserAsyncByNumAccount(string numAccount);
        
    }
}