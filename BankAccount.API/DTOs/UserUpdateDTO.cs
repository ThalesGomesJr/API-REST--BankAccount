using System.ComponentModel.DataAnnotations;

namespace BankAccount.API.DTOs
{
    public class UserUpdateDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string UserName { get; set; }
       
        [Required(ErrorMessage = "Campo Obrigatório")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(11, ErrorMessage = "CPF Inválido")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string Address { get; set; }    
    }
}