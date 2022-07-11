using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankAccount.Domain.Models;

namespace BankAccount.Repository
{
    public class WeBankRepository : IWeBankRepository
    {
        private readonly WeBankContext _context;
        public WeBankRepository(WeBankContext context)
        {
            this._context = context;
            this._context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        //Busca todos os Usuários cadastrados
        public async Task<User[]> GetAllUserAsync()
        {
            IQueryable<User> query = _context.User.Include(u => u.Extract);

            query = query.AsNoTracking().OrderBy(u => u.Id);            
            
            return await query.ToArrayAsync();
        }

        //Busca o Extrato do User
        public async Task<Extract[]> GetExtractAsyncById(int id)
        {
            var query = this._context.Extract.AsNoTracking().OrderByDescending(e => e.Date).Where(u => u.UserId == id);

            return await query.ToArrayAsync();
        }

        //Busca User por NumAccount
        public async Task<User> GetUserAsyncByNumAccount(string numAccount)
        {
            var query = this._context.User.AsNoTracking().Where(n => n.NumAccount.Contains(numAccount));

            return await query.FirstOrDefaultAsync();
        }

        //Cria o numero da conta e garante que ele seja unico.
        public async Task<string> VerifyNumAccount()
        {
            var numbers = "0123456789";
            var random = new Random();
            var numP1 = new string(Enumerable.Repeat(numbers, 6).Select(n => n[random.Next(n.Length)]).ToArray());
            var numP2 = new string(Enumerable.Repeat(numbers, 2).Select(n => n[random.Next(n.Length)]).ToArray());
            var numAccount = numP1 + numP2;

            var query = await this.GetUserAsyncByNumAccount(numAccount);

            if (query == null)
            {
                return numAccount;
            }
            
            return await this.VerifyNumAccount();
        
        }
        public async Task<Extract> CreateMovement(string typeMovement, decimal value, string receiver)
        {
            var movement = new Extract();                
            movement.TypeMovement = typeMovement;
            movement.Value = value;
            movement.Receiver = receiver;
            movement.Date = DateTime.Now;

            return movement;    
        }


    
    }
}