using System.Collections.Generic;

namespace WeBank.API.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }        
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string NumAccount { get; set; }
        public decimal Balance { get; set; }
        public decimal SavedBalance { get; set; }
        public string ImageURL { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public List<ExtractDTO> Extract { get; set; }
    }
}