using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WeBank.Domain.Models;

namespace WeBank.Repository
{
    public class WeBankRepository : IWeBankRepository
    {
        private readonly WeBankContext _context;
        public WeBankRepository(WeBankContext context)
        {
            this._context = context;
            this._context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public void Add<T>(T entity) where T : class
        {
            this._context.Add(entity);   
        }

        public void Update<T>(T entity) where T : class
        {
            this._context.Update(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            this._context.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<User[]> GetAllUserAsync()
        {
            IQueryable<User> query = _context.User.Include(n => n.Extracts);

            query = query.AsNoTracking().OrderBy(c => c.Id);

            return await query.ToArrayAsync();
        }


        //Busca User por ID
        public async Task<User> GetUserAsyncById(int userId)
        {
            IQueryable<User> query = this._context.User.Include(n => n.Extracts);

            query = query.AsNoTracking().Where(u => u.Id == userId);

            return await query.FirstOrDefaultAsync();
        }

        //Busca User por NumAccount
        public async Task<User> GetUserAsyncByNumAccount(string numAccount)
        {
            IQueryable<User> query = this._context.User.Include(n => n.Extracts);

            query = query.AsNoTracking().Where(n => n.NumAccount.Contains(numAccount));

            return await query.FirstOrDefaultAsync();
        }

        //Cria o numero da conta e garante que ele seja unico.
        public async Task<string> VerifyNumAccount()
        {
            var numbers = "0123456789";
            var random = new Random();
            var numP1 = new string(Enumerable.Repeat(numbers, 6).Select(n => n[random.Next(n.Length)]).ToArray());
            var numP2 = new string(Enumerable.Repeat(numbers, 2).Select(n => n[random.Next(n.Length)]).ToArray());
            var numAccount = numP1 + "-" + numP2;

            var query = this._context.User.AsNoTracking().Where(n => n.NumAccount.Contains(numAccount)).FirstOrDefaultAsync();

            if (query.Result == null)
            {
                return numAccount;
            }
            
            return await this.VerifyNumAccount();
        
        }
    }
}