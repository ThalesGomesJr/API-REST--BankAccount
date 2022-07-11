using System.Threading.Tasks;
using BankAccount.Domain.Models;

namespace BankAccount.Repository
{
    public interface IWeBankRepository
    {
        Task<User[]> GetAllUserAsync();
        Task<Extract[]> GetExtractAsyncById(int id);
        Task<User> GetUserAsyncByNumAccount(string numAccount);
        Task<string> VerifyNumAccount();
        Task<Extract> CreateMovement(string typeMovement, decimal value, string receiver);
        
    }
}