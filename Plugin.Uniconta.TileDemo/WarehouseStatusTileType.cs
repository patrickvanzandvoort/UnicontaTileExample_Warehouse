using DevExpress.XtraRichEdit.Import.Html;
using Plugin.Uniconta.TileDemo.Services;
using Plugin.Uniconta.TileDemo.Tiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Uniconta.API.System;
using Uniconta.ClientTools;

namespace Plugin.Uniconta.TileDemo
{
    [DisplayName("Warehouse status")]
    public sealed class WarehouseStatusTileType : ITileType
    {
        readonly WarehouseSummaryService service = WarehouseSummaryService.Instance;
        private WarehouseStatusTile content;
        bool fetched;

        public void Initialize(CrudAPI api)
        {
        }

        public UserControl GetContent()
        {
            if (content == null)
            {
                content = new WarehouseStatusTile();
                content.RefreshAsync = FetchAsync;
            }

            return content;
        }

        public async Task FetchAsync()
        {
            var summary = await service.LoadAsync(fetched);
            fetched = true;

            var tile = (WarehouseStatusTile)GetContent();

            if (tile.Dispatcher.CheckAccess())
                tile.SetSummary(summary);
            else
                await tile.Dispatcher.InvokeAsync(() => tile.SetSummary(summary));
        }
    }
}
