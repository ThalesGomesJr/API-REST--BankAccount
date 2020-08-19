using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WeBank.Domain.Models
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }
        public string NumAccount { get; set; }
        public decimal Balance { get; set; }
        public decimal SavedBalance { get; set; }
        public string ImageURL { get; set; }
        public string Cpf { get; set; }
        public string Address { get; set; }
        public List<Extract> Extracts { get; set; }
        public List<UserRole> UserRoles { get; set; }
    }
}