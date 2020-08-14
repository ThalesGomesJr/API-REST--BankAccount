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

        //Busca User por ID
        public async Task<User> GetUserAsyncById(int userId)
        {
            IQueryable<User> query = this. _context.User.Include(n => n.Extracts);

            query = query.AsNoTracking().Where(u => u.Id == userId);

            return await query.FirstOrDefaultAsync();
        }

        //Busca User por NumAccount
        public async Task<User> GetUserAsyncByNumAccount(string numAccount)
        {
            IQueryable<User> query = this. _context.User.Include(n => n.Extracts);

            query = query.AsNoTracking().Where(n => n.NumAccount.Contains(numAccount));

            return await query.FirstOrDefaultAsync();
        }
    }
}