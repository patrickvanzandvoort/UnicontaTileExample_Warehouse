namespace Plugin.Uniconta.TileDemo.Models
{
    public enum WarehouseStatus { Red, Yellow, Green, Blue, Orange }

    public sealed class WarehouseSummaryItem
    {
        public WarehouseSummaryItem(string activity, int count, WarehouseStatus status)
        {
            Activity = activity;
            Count = count;
            Status = status;
        }

        public string Activity { get; }
        public int Count { get; }
        public WarehouseStatus Status { get; }
    }
}
