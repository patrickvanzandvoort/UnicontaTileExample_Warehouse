using Plugin.Uniconta.TileDemo.Models;
using Plugin.Uniconta.TileDemo.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Uniconta.ClientTools.Controls;

namespace Plugin.Uniconta.TileDemo.Tiles
{
    public partial class WarehouseStatusTile : UserControl, INotifyPropertyChanged
    {
        public WarehouseStatusTile()
        {
            InitializeComponent();
            DataContext = this;
        }

        public IReadOnlyList<WarehouseSummaryItem> Activities { get; private set; }
            = new WarehouseSummaryItem[0];

        public bool IsLoading { get; private set; }
        public bool CanRefresh => !IsLoading;

        public string ErrorMessage { get; private set; }
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
        public bool IsEmpty => !IsLoading && !HasError && Activities.Count == 0;

        public Func<Task> RefreshAsync { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetSummary(WarehouseSummary summary)
        {
            Activities = summary.Items;
            ErrorMessage = summary.ErrorMessage;
            Notify();
        }

        public void SetLoading(bool loading)
        {
            IsLoading = loading;

            if (loading)
                ErrorMessage = null;

            Notify();
        }

        public void SetError(string message)
        {
            Activities = new WarehouseSummaryItem[0];
            ErrorMessage = message;
            Notify();
        }

        async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (RefreshAsync != null && CanRefresh)
                await RefreshAsync();
        }

        void Notify()
        {
            foreach (var property in new[]
            {
                nameof(Activities),
                nameof(IsLoading),
                nameof(CanRefresh),
                nameof(ErrorMessage),
                nameof(HasError),
                nameof(IsEmpty)
            })
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}