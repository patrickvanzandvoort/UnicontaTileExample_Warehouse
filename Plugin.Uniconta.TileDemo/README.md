# Warehouse Operations tile demo

This demo is compiled into the existing **UnicontaPages** assembly. Its XAML,
datasource, provider, model, service, and CSV are isolated under
`Plugin.Uniconta.TileDemo` for later extraction into a partner assembly.

```text
Plugin.Uniconta.TileDemo/
  Tiles/WarehouseStatusTile.xaml
  Tiles/WarehouseStatusTile.xaml.cs
  Tiles/WarehouseStatusTileDataSource.cs
  DataProviders/WarehouseStatusDataProvider.cs
  Services/WarehouseSummaryService.cs
  Models/WarehouseSummaryItem.cs
  Data/warehouse_summary.csv
```

## Open the tile

Open the dashboard (`TilePage`) and click **Add warehouse demo**. The existing
dashboard CRUD controller saves a tile on the selected dashboard with a 2×2
size and the title **Warehouse Operations**. Existing drag, resize, delete,
dashboard persistence, and refresh actions apply. The tile's Refresh button
also explicitly reloads the CSV. No polling or file watchers are used.

`TilePage` registers `WarehouseStatusTileDataSource` through the existing
`Plugin.TileDataSourceCache.RegisterSourceType` method. The persisted
`TileDefinition.SourceName` identifies that registered type; the existing tile
factory resolves it before the table-source path. The datasource derives from
`TileDataSourcePluginBase` and supplies its XAML `WarehouseStatusTile` through
`ITileDataSourceWithUserControl.TileContent()`. Plugin loading/discovery code
has not been changed. Built-in registration is needed because this demo does
not yet live in a separate plugin DLL. Validate external assembly discovery
separately when extracting it later.

## CSV and shared service

The project copies `Data/warehouse_summary.csv` to
`Plugin.Uniconta.TileDemo/Data/warehouse_summary.csv` under the output directory
using `CopyToOutputDirectory=PreserveNewest`. `WarehouseSummaryService` resolves
that location relative to its assembly, never the process working directory.

The process-wide service caches an immutable snapshot. Both the control and
provider use it. Reload replaces the snapshot, including explicit error results,
so malformed/missing data is not silently replaced by stale successful values.
The CSV header must be `Activity,Count,Status`; counts are non-negative integers
and statuses are Red, Yellow, Green, Blue, or Orange. Quoted fields, commas and
escaped quotes are supported; multiline quoted fields are not supported.

For a manual refresh check, change a count in the **output** CSV and click
Refresh. Remove or corrupt the file to check the error state; restore it and
refresh to recover. A valid header with no records produces the empty state.

## Ask the assistant

Open the assistant on the dashboard and ask **How is our warehouse doing today?**
or **How many orders are waiting to be picked?** The warehouse provider is
available even if no warehouse tile has been added or instantiated.

The actual interface in this solution is **IAIAssistantDataProvider**, not
`IAIDataProvider`. `WarehouseStatusDataProvider` implements it through the
existing `AIDataProviderBase<WarehouseStatusRequest>`. The dashboard's existing
`GetAIAssistantDataProviders` extension point supplies it. Its response uses the
existing `AIDataProviderInvokeResult` / `AIAssistantDataProviderResult` contracts
and includes the underlying activities, counts, status names, availability,
and an explicit `IsDemoData` flag. It never reads a WPF control or live company
records. AI conversation/authentication requirements remain the application's
existing requirements.

To exercise the provider without an AI call or a tile:

```csharp
var provider = new WarehouseStatusDataProvider();
var response = await provider.RequestDataAsync(new WarehouseStatusRequest());
// Inspect response.ReturnedData as JSON.
var refreshed = await provider.RequestDataAsync(new WarehouseStatusRequest { Refresh = true });
```

The base provider contract has no cancellation-token parameter. The shared
service supports a cancellation token for direct callers and propagates
cancellation rather than presenting it as malformed data.

## Verification

Tests are added to the existing `ClientTools.BusinessRules.Tests` xUnit project:
`WarehouseSummaryServiceTests` covers CSV parsing, caching, reload, concurrency,
missing/malformed files, empty data, quoting, and cancellation.
`WarehouseStatusDataProviderTests` covers tile-independent provider responses,
refresh, existing source registration, and persistence of the source and size.

In this Linux setup, 13 service cases passed using the real service/model/test
sources and existing xUnit assertions in an isolated .NET 8 runner. The direct
provider request path also passed checks for the five exact shipped records,
shared caching, explicit reload, and missing-file JSON, without creating a tile.
That check used the real provider and extracted, unchanged existing provider
base/interface source; unused GPT-context/upload dependencies were stubbed.
The complete `net481` test project was not executed.

XAML XML, class/namespace matching, project source/content entries, all five
status triggers, and theme resource availability were checked. The application
build was attempted and stopped with `MSB3644` (missing .NET Framework 4.8
reference assemblies). XAML compilation and actual UI rendering remain unverified.

Full application/XAML compilation and UI tests require the complete Windows/WPF
source tree, .NET Framework targeting packs, and the existing DevExpress
dependencies. Isolated data checks do not validate WPF rendering. Check Light
and Dark themes, the initial 2×2 layout, smaller/larger resizes, row colors,
loading/error/empty states, and both refresh buttons on that environment.

The required `Plugin.*` namespace shadows imported references to the existing
`Uniconta.ClientTools.Plugin` class. Affected existing call sites are qualified
with `global::Uniconta.ClientTools.Plugin`; their behavior is unchanged.
