using System.Collections.Generic;

namespace WeBank.API.DTOs
{
    public class UserBalanceDTO
    {
        public decimal Balance { get; set; }
        public decimal SavedBalance { get; set; }
        public List<ExtractDTO> Extracts { get; set; }
        
    }
}