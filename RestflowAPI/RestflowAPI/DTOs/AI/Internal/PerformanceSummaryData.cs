namespace RestflowAPI.DTOs.AI.Internal
{
    public class PerformanceSummaryData
    {
        public decimal TodayRevenue { get; set; }

        public decimal YesterdayRevenue { get; set; }

        public int TotalOrdersToday { get; set; }

        public int DeliveryOrders { get; set; }

        public int DineInOrders { get; set; }

        public int TakeAwayOrders { get; set; }
    }
}