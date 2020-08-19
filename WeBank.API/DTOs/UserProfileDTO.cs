namespace WeBank.API.DTOs
{
    public class UserProfileDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string NumAccount { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }       
    }
}