using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WeBank.Domain.Models
{
    public class Role : IdentityRole<int>
    {
        public List<UserRole> UserRoles { get; set; }
    }
}