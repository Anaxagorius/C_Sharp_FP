using System.Collections.ObjectModel;
using System.Windows;
using StarPublications.Commands;
using StarPublications.Models;
using StarPublications.Services;

namespace StarPublications.ViewModels;

public delegate void SaleOperationCompletedEventHandler(object sender, SaleOperationEventArgs e);

public class SaleOperationEventArgs : EventArgs
{
    public string Operation { get; }
    public bool Success { get; }
    public string Message { get; }

    public SaleOperationEventArgs(string operation, bool success, string message)
    {
        Operation = operation;
        Success = success;
        Message = message;
    }
}

public class SalesOrderViewModel : BaseViewModel
{
    private readonly ISalesOrderService _salesOrderService;
    private readonly IBookService _bookService;

    private ObservableCollection<Sale> _sales = new();
    private ObservableCollection<Title> _availableTitles = new();
    private ObservableCollection<Store> _availableStores = new();
    private Sale? _selectedSale;
    private string _searchStoreId = string.Empty;
    private string _searchOrderNumber = string.Empty;
    private string _searchTitleId = string.Empty;
    private string _editStoreId = string.Empty;
    private string _editOrderNumber = string.Empty;
    private string _editTitleId = string.Empty;
    private DateTime _editOrderDate = DateTime.Today;
    private short _editQuantity = 1;
    private string _editPayTerms = string.Empty;
    private bool _isEditing;
    private bool _isAdding;
    private string _statusMessage = string.Empty;
    private bool _isLoading;

    public event SaleOperationCompletedEventHandler? SaleOperationCompleted;

    public ObservableCollection<Sale> Sales
    {
        get => _sales;
        set => SetProperty(ref _sales, value);
    }

    public ObservableCollection<Title> AvailableTitles
    {
        get => _availableTitles;
        set => SetProperty(ref _availableTitles, value);
    }

    public ObservableCollection<Store> AvailableStores
    {
        get => _availableStores;
        set => SetProperty(ref _availableStores, value);
    }

    public Sale? SelectedSale
    {
        get => _selectedSale;
        set
        {
            SetProperty(ref _selectedSale, value);
            if (value != null) PopulateEditFields(value);
        }
    }

    public string SearchStoreId
    {
        get => _searchStoreId;
        set => SetProperty(ref _searchStoreId, value);
    }

    public string SearchOrderNumber
    {
        get => _searchOrderNumber;
        set => SetProperty(ref _searchOrderNumber, value);
    }

    public string SearchTitleId
    {
        get => _searchTitleId;
        set => SetProperty(ref _searchTitleId, value);
    }

    public string EditStoreId
    {
        get => _editStoreId;
        set => SetProperty(ref _editStoreId, value);
    }

    public string EditOrderNumber
    {
        get => _editOrderNumber;
        set => SetProperty(ref _editOrderNumber, value);
    }

    public string EditTitleId
    {
        get => _editTitleId;
        set => SetProperty(ref _editTitleId, value);
    }

    public DateTime EditOrderDate
    {
        get => _editOrderDate;
        set => SetProperty(ref _editOrderDate, value);
    }

    public short EditQuantity
    {
        get => _editQuantity;
        set => SetProperty(ref _editQuantity, value);
    }

    public string EditPayTerms
    {
        get => _editPayTerms;
        set => SetProperty(ref _editPayTerms, value);
    }

    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    public bool IsAdding
    {
        get => _isAdding;
        set => SetProperty(ref _isAdding, value);
    }

    public bool IsEditOrAdd => IsEditing || IsAdding;

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public RelayCommand LoadSalesCommand { get; }
    public RelayCommand SearchCommand { get; }
    public RelayCommand AddNewCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand ClearSearchCommand { get; }

    public SalesOrderViewModel(ISalesOrderService salesOrderService, IBookService bookService)
    {
        _salesOrderService = salesOrderService;
        _bookService = bookService;

        LoadSalesCommand = new RelayCommand(async _ => await LoadSalesAsync());
        SearchCommand = new RelayCommand(async _ => await SearchSalesAsync());
        AddNewCommand = new RelayCommand(_ => StartAdd());
        EditCommand = new RelayCommand(_ => StartEdit(), _ => SelectedSale != null);
        SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => IsEditOrAdd);
        DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedSale != null && !IsEditOrAdd);
        CancelCommand = new RelayCommand(_ => CancelEdit(), _ => IsEditOrAdd);
        ClearSearchCommand = new RelayCommand(_ => ClearSearch());

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadSalesAsync();
        await LoadReferenceDataAsync();
    }

    private async Task LoadSalesAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading sales...";
            var sales = await _salesOrderService.GetAllSalesAsync();
            Sales = new ObservableCollection<Sale>(sales);
            StatusMessage = $"Loaded {Sales.Count} sales orders.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading sales: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SearchSalesAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Searching...";
            var sales = await _salesOrderService.SearchSalesAsync(SearchStoreId, SearchOrderNumber, SearchTitleId);
            Sales = new ObservableCollection<Sale>(sales);
            StatusMessage = $"Found {Sales.Count} sales orders.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error searching sales: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadReferenceDataAsync()
    {
        try
        {
            var titles = await _bookService.GetAllBooksAsync();
            AvailableTitles = new ObservableCollection<Title>(titles);
            var stores = await _bookService.GetAllStoresAsync();
            AvailableStores = new ObservableCollection<Store>(stores);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading reference data: {ex.Message}";
        }
    }

    private void StartAdd()
    {
        IsAdding = true;
        IsEditing = false;
        SelectedSale = null;
        ClearEditFields();
        OnPropertyChanged(nameof(IsEditOrAdd));
    }

    private void StartEdit()
    {
        if (SelectedSale == null) return;
        IsEditing = true;
        IsAdding = false;
        PopulateEditFields(SelectedSale);
        OnPropertyChanged(nameof(IsEditOrAdd));
    }

    private void PopulateEditFields(Sale sale)
    {
        EditStoreId = sale.StoreId;
        EditOrderNumber = sale.OrderNumber;
        EditTitleId = sale.TitleId;
        EditOrderDate = sale.OrderDate;
        EditQuantity = sale.Quantity;
        EditPayTerms = sale.PayTerms ?? string.Empty;
    }

    private void ClearEditFields()
    {
        EditStoreId = string.Empty;
        EditOrderNumber = string.Empty;
        EditTitleId = string.Empty;
        EditOrderDate = DateTime.Today;
        EditQuantity = 1;
        EditPayTerms = string.Empty;
    }

    private async Task SaveAsync()
    {
        try
        {
            if (!ValidateInput()) return;

            IsLoading = true;
            var sale = new Sale
            {
                StoreId = EditStoreId.Trim(),
                OrderNumber = EditOrderNumber.Trim(),
                TitleId = EditTitleId.Trim(),
                OrderDate = EditOrderDate,
                Quantity = EditQuantity,
                PayTerms = EditPayTerms.Trim()
            };

            if (IsAdding)
            {
                await _salesOrderService.AddSaleAsync(sale);
                SaleOperationCompleted?.Invoke(this, new SaleOperationEventArgs("Add", true, "Sale order added successfully."));
                StatusMessage = "Sale order added successfully.";
            }
            else
            {
                await _salesOrderService.UpdateSaleAsync(sale);
                SaleOperationCompleted?.Invoke(this, new SaleOperationEventArgs("Update", true, "Sale order updated successfully."));
                StatusMessage = "Sale order updated successfully.";
            }

            IsEditing = false;
            IsAdding = false;
            OnPropertyChanged(nameof(IsEditOrAdd));
            await LoadSalesAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving sale: {ex.Message}";
            SaleOperationCompleted?.Invoke(this, new SaleOperationEventArgs("Save", false, ex.Message));
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteAsync()
    {
        if (SelectedSale == null) return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete order '{SelectedSale.OrderNumber}' for store '{SelectedSale.StoreId}'?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            IsLoading = true;
            await _salesOrderService.DeleteSaleAsync(SelectedSale.StoreId, SelectedSale.OrderNumber, SelectedSale.TitleId);
            SaleOperationCompleted?.Invoke(this, new SaleOperationEventArgs("Delete", true, "Sale order deleted successfully."));
            StatusMessage = "Sale order deleted successfully.";
            await LoadSalesAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deleting sale: {ex.Message}";
            SaleOperationCompleted?.Invoke(this, new SaleOperationEventArgs("Delete", false, ex.Message));
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void CancelEdit()
    {
        IsEditing = false;
        IsAdding = false;
        ClearEditFields();
        OnPropertyChanged(nameof(IsEditOrAdd));
        StatusMessage = "Operation cancelled.";
    }

    private void ClearSearch()
    {
        SearchStoreId = string.Empty;
        SearchOrderNumber = string.Empty;
        SearchTitleId = string.Empty;
        _ = LoadSalesAsync();
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(EditStoreId))
        {
            StatusMessage = "Store ID is required.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(EditOrderNumber))
        {
            StatusMessage = "Order Number is required.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(EditTitleId))
        {
            StatusMessage = "Title ID is required.";
            return false;
        }
        if (EditQuantity <= 0)
        {
            StatusMessage = "Quantity must be greater than zero.";
            return false;
        }
        return true;
    }
}
