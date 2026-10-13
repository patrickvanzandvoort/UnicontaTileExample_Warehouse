using Plugin.Uniconta.TileDemo.Models;
using Plugin.Uniconta.TileDemo.Services;
using System;
using System.Threading.Tasks;
using Uniconta.API.System;
using Uniconta.ClientTools;
using UnicontaClient.Pages;

namespace Plugin.Uniconta.TileDemo.Tiles
{
    public sealed class WarehouseStatusTileDataSource : TileDataSourcePluginBase
    {
        readonly WarehouseSummaryService service = WarehouseSummaryService.Instance;
        bool fetched;

        public WarehouseStatusTileDataSource(CrudAPI api, TileDefinition definition) : base(api, definition) { }

        public override Type DataType => typeof(WarehouseSummaryItem);

        public override string DisplayName => "Warehouse Operations data (demo)";

        public override async Task<TileDataSourceResult> FetchAsync()
        {
            var summary = await service.LoadAsync(fetched);
            fetched = true;

            if (!summary.IsAvailable)
                throw new InvalidOperationException(
                    summary.ErrorMessage ?? "Warehouse demo data is unavailable.");

            return new TileDataSourceResult(
                summary.Items.Count,
                null);
        }
    }
}