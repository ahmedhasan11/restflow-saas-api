using System.Text;

namespace RestflowAPI.Services.AI
{
    public class SchemaContextBuilder
    {
        public string Build()
        {
            StringBuilder sb = new();

            sb.AppendLine("Database Schema");
            sb.AppendLine();

            sb.AppendLine("Table Orders");
            sb.AppendLine("""
Id
OrderNumber
OrderType
OrderStatus
PaymentStatus
Subtotal
TotalAmount
CustomerId
CreatedAt
""");

            sb.AppendLine();

            sb.AppendLine("Table OrderItems");
            sb.AppendLine("""
Id
OrderId
ProductId
ProductNameSnapshot
Quantity
UnitPrice
LineTotal
""");

            sb.AppendLine();

            sb.AppendLine("Table Products");
            sb.AppendLine("""
Id
CategoryId
ProductName
Description
SellingPrice
IsAvailable
""");

            sb.AppendLine();

            sb.AppendLine("Table InventoryItems");
            sb.AppendLine("""
Id
CategoryId
ItemName
CurrentQuantity
MinimumQuantity
CostPerUnit
""");

            sb.AppendLine();

            sb.AppendLine("Table ProductIngredients");
            sb.AppendLine("""
Id
ProductId
InventoryItemId
QuantityRequired
""");

            sb.AppendLine();

            sb.AppendLine("Table StockMovements");
            sb.AppendLine("""
Id
InventoryItemId
TransactionType
Quantity
TransactionDate
UnitCost
TotalCost
""");

            return sb.ToString();
        }
    }
}
