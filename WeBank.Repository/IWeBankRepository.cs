using System.Threading.Tasks;
using WeBank.Domain.Models;

namespace WeBank.Repository
{
    public interface IWeBankRepository
    {
        Task<string> VerifyNumAccount();
        Task<User[]> GetAllUserAsync();
        Task<User> GetUserAsyncByNumAccount(string numAccount);
        
    }
}