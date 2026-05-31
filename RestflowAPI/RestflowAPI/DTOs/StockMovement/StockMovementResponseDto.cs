using RestflowAPI.Enums;

namespace RestflowAPI.DTOs.StockTransaction
{
    public class StockMovementResponseDto
    {
        public Guid Id { get; set; }

        public Guid InventoryItemId { get; set; }

        public string InventoryItemName { get; set; }

        public TransactionType TransactionType { get; set; }

        public decimal Quantity { get; set; }

        public string? Note { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
