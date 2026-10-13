using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Uniconta.TileDemo.Services;
using Uniconta.ClientTools.Controls.AI.Assistant.DataProviders;

namespace Plugin.Uniconta.TileDemo.DataProviders
{
    public sealed class WarehouseStatusRequest : AIDataRequestBase
    {
        public bool Refresh { get; set; }
    }

    public sealed class WarehouseStatusDataProvider : AIDataProviderBase<WarehouseStatusRequest>
    {
        readonly WarehouseSummaryService service;
        public WarehouseStatusDataProvider() : this(WarehouseSummaryService.Instance) { }
        public WarehouseStatusDataProvider(WarehouseSummaryService service) { this.service = service ?? throw new ArgumentNullException(nameof(service)); }

        public override string Name => "WarehouseStatus";
        public override string UserFriendlyText => "Reading warehouse demonstration statistics";
        public override string Description => "Provides local CSV demonstration warehouse operational statistics: orders waiting to be picked, currently being picked, ready for shipment, incoming deliveries today and items below minimum stock. Returns all activities, counts and status colors. These are demonstration values, not live Uniconta data. Do not infer deadlines, actual stock availability or performance metrics. Set Refresh=true to reload the CSV; otherwise uses the shared snapshot. Works without an open warehouse tile.";

        protected override async Task<AIDataProviderInvokeResult> Invoke(WarehouseStatusRequest request)
        {
            var summary = await service.LoadAsync(request != null && request.Refresh).ConfigureAwait(false);
            var data = new
            {
                IsDemoData = true,
                Source = "warehouse_summary.csv",
                Available = summary.IsAvailable,
                Error = summary.ErrorMessage,
                Activities = summary.Items.Select(item => new { item.Activity, item.Count, Status = item.Status.ToString() }).ToArray()
            };
            return Result(summary.Items.Count, JsonConvert.SerializeObject(data));
        }
    }
}
