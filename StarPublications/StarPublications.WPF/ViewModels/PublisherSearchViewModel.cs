using System.Collections.ObjectModel;
using StarPublications.Commands;
using StarPublications.Models;
using StarPublications.Services;

namespace StarPublications.ViewModels;

public class PublisherSearchViewModel : BaseViewModel
{
    private readonly IPublisherService _publisherService;

    private ObservableCollection<Publisher> _publishers = new();
    private Publisher? _selectedPublisher;
    private string _searchTerm = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _isLoading;

    public ObservableCollection<Publisher> Publishers
    {
        get => _publishers;
        set => SetProperty(ref _publishers, value);
    }

    public Publisher? SelectedPublisher
    {
        get => _selectedPublisher;
        set => SetProperty(ref _selectedPublisher, value);
    }

    public string SearchTerm
    {
        get => _searchTerm;
        set => SetProperty(ref _searchTerm, value);
    }

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

    public RelayCommand SearchCommand { get; }
    public RelayCommand ClearCommand { get; }

    public PublisherSearchViewModel(IPublisherService publisherService)
    {
        _publisherService = publisherService;
        SearchCommand = new RelayCommand(async _ => await SearchPublishersAsync());
        ClearCommand = new RelayCommand(_ => ClearSearch());
        _ = LoadAllPublishersAsync();
    }

    private async Task LoadAllPublishersAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading publishers...";
            var publishers = await _publisherService.GetAllPublishersAsync();
            Publishers = new ObservableCollection<Publisher>(publishers);
            StatusMessage = $"Loaded {Publishers.Count} publishers.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading publishers: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SearchPublishersAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Searching...";
            var publishers = await _publisherService.SearchPublishersAsync(SearchTerm);
            Publishers = new ObservableCollection<Publisher>(publishers);
            StatusMessage = $"Found {Publishers.Count} publishers.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error searching publishers: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ClearSearch()
    {
        SearchTerm = string.Empty;
        SelectedPublisher = null;
        _ = LoadAllPublishersAsync();
    }
}
