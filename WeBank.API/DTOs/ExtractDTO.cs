using System;

namespace WeBank.API.DTOs
{
    public class ExtractDTO
    {
        public int Id { get; set; }
        public string TypeMovement { get; set; }
        public decimal Value { get; set; }
        public string Receiver { get; set; }
        public string DateMovement { get; set; }

    }
}