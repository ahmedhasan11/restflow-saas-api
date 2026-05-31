using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.StockTransaction
{
    public class CreateStockMovementDto
    {
        public TransactionType TransactionType { get; set; }

        public decimal Quantity { get; set; }

        public string? Note { get; set; }
    }
}
