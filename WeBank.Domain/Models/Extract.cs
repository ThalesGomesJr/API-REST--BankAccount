using System;

namespace BankAccount.Domain.Models
{
    public class Extract
    {
        public int Id  { get; set; }
        public string TypeMovement { get; set; }
        public decimal Value { get; set; }
        public string Receiver { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public User User { get; }
    }
}