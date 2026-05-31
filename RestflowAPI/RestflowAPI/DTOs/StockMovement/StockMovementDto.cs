using RestflowAPI.Entities;
using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.StockTransaction
{
    public class StockMovementDto
    {
        public Guid Id { get; set; }

        public TransactionType TransactionType { get; set; } 

        public decimal Quantity { get; set; }

        public string? Note { get; set; }

        public DateTime TransactionDate { get; set; }

        public string? CreatedBy { get; set; }
    }
}
