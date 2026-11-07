using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SafeDeskInventory.Core.Models;
using SafeDeskInventory.Core.Services;
using SafeDeskInventory.Windows;
using Microsoft.Win32;

namespace SafeDeskInventory.App;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<InventoryRecord> _records = new();
    private readonly ICollectionView _view;
    private readonly WindowsInventoryScanner _scanner = new();
    private bool _demoMode = true;

    public MainWindow()
    {
        InitializeComponent();
        InventoryGrid.ItemsSource = _records;
        _view = CollectionViewSource.GetDefaultView(_records);
        _view.Filter = MatchesSearch;
        ShowDemo();
    }

    private bool MatchesSearch(object obj)
    {
        if (obj is not InventoryRecord record) return false;
        string term = SearchBox?.Text?.Trim() ?? "";
        if (term.Length == 0) return true;
        return new[] { record.Name, record.Kind, record.Source, record.Browser, record.Profile }
            .Any(value => value.Contains(term, StringComparison.OrdinalIgnoreCase));
    }

    private void Demo_Click(object sender, RoutedEventArgs e) => ShowDemo();

    private void ShowDemo()
    {
        _demoMode = true;
        SetRecords(DemoData.Create());
        ModeText.Text = "DEMO — SYNTHETIC DATA";
        StatusText.Text = "Illustrative records only — nothing was scanned. Click Scan this PC for a real local inventory.";
    }

    private async void Scan_Click(object sender, RoutedEventArgs e)
    {
        var confirm = MessageBox.Show(this,
            "Scan public application uninstall records and known browser extension package folders on this Windows account?\n\n" +
            "The scan stays on this PC. No secrets, wallet balances or extension storage are read.",
            "Consent to local inventory", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        ScanButton.IsEnabled = false;
        DemoButton.IsEnabled = false;
        StatusText.Text = "Scanning locally. No network requests are made.";
        try
        {
            ScanReport result = await Task.Run(() => _scanner.Scan());
            _demoMode = false;
            SetRecords(result.Records);
            ModeText.Text = "LIVE — THIS PC";
            StatusText.Text = $"Scan finished: {result.Records.Count} public installation observations. " +
                (result.Warnings.Count > 0 ? string.Join(" ", result.Warnings) : "No access warnings.") +
                " A package folder does not prove that an extension is enabled.";
        }
        catch (Exception)
        {
            StatusText.Text = "Could not complete local inventory. No data was transmitted.";
            MessageBox.Show(this, "The local scan could not finish. Please review Windows permissions.",
                "Inventory error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            ScanButton.IsEnabled = true;
            DemoButton.IsEnabled = true;
        }
    }

    private void SetRecords(IEnumerable<InventoryRecord> items)
    {
        _records.Clear();
        foreach (var item in items) _records.Add(item);
        _view.Refresh();
        TotalText.Text = _records.Count.ToString();
        DesktopText.Text = _records.Count(x => x.Kind == "Desktop application").ToString();
        ExtensionText.Text = _records.Count(x => x.Kind == "Browser extension").ToString();
        ExportButton.IsEnabled = _records.Count > 0;
    }

    private void Search_Changed(object sender, TextChangedEventArgs e) => _view?.Refresh();

    private void Export_Click(object sender, RoutedEventArgs e)
    {
        if (_records.Count == 0) return;
        var dlg = new SaveFileDialog
        {
            Title = _demoMode ? "Save demo CSV (synthetic data)" : "Save local inventory CSV",
            FileName = _demoMode ? "safedesk-demo.inventory.csv" : "safedesk-local.inventory.csv",
            Filter = "CSV document (*.csv)|*.csv",
            DefaultExt = ".csv",
            AddExtension = true
        };
        if (dlg.ShowDialog(this) != true) return;
        try
        {
            // CSV export is opt-in; UTF-8 BOM improves compatibility with Excel.
            File.WriteAllText(dlg.FileName, CsvExporter.Export(_view.Cast<InventoryRecord>()),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            StatusText.Text = _demoMode
                ? "Synthetic demo CSV saved locally; do not present it as a real device scan."
                : "Local inventory CSV saved to your chosen file. No upload occurred.";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(this, "Could not write CSV to the selected location.", "Export failed",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
