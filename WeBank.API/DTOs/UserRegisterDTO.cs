using System.Collections.Generic;


namespace WeBank.API.DTOs
{    
    public class UserRegisterDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string password { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public List<ExtractDTO> Extracts { get; set; }

    }
}