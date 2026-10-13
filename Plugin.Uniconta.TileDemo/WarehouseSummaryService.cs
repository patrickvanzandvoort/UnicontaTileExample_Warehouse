
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Uniconta.TileDemo.Models;

namespace Plugin.Uniconta.TileDemo.Services
{
    public sealed class WarehouseSummary
    {
        internal WarehouseSummary(IEnumerable<WarehouseSummaryItem> items, string errorMessage = null)
        {
            Items = Array.AsReadOnly(new List<WarehouseSummaryItem>(items).ToArray());
            ErrorMessage = errorMessage;
        }

        public IReadOnlyList<WarehouseSummaryItem> Items { get; }
        public string ErrorMessage { get; }
        public bool IsAvailable => ErrorMessage == null;
    }

    /// <summary>
    /// Generates demo warehouse statistics.
    /// One shared snapshot is used by the tile and AI Assistant.
    /// </summary>
    public sealed class WarehouseSummaryService
    {
        public static WarehouseSummaryService Instance { get; }
            = new WarehouseSummaryService();

        readonly SemaphoreSlim loadLock = new SemaphoreSlim(1, 1);
        readonly Random random = new Random();

        WarehouseSummary cached;

        public async Task<WarehouseSummary> LoadAsync(
            bool reload = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await loadLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (!reload && cached != null)
                    return cached;

                cancellationToken.ThrowIfCancellationRequested();

                cached = Generate();

                return cached;
            }
            finally
            {
                loadLock.Release();
            }
        }

        WarehouseSummary Generate()
        {
            var items = new[]
            {
                new WarehouseSummaryItem(
                    "Orders waiting to be picked",
                    random.Next(10, 51),
                    WarehouseStatus.Yellow),

                new WarehouseSummaryItem(
                    "Orders currently being picked",
                    random.Next(3, 21),
                    WarehouseStatus.Blue),

                new WarehouseSummaryItem(
                    "Orders ready for shipment",
                    random.Next(5, 36),
                    WarehouseStatus.Green),

                new WarehouseSummaryItem(
                    "Incoming deliveries today",
                    random.Next(1, 16),
                    WarehouseStatus.Orange),

                new WarehouseSummaryItem(
                    "Items below minimum stock",
                    random.Next(0, 26),
                    WarehouseStatus.Red)
            };

            return new WarehouseSummary(items);
        }
    }
}
