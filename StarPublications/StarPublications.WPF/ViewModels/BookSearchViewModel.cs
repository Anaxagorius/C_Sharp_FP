using System.Collections.ObjectModel;
using StarPublications.Commands;
using StarPublications.Models;
using StarPublications.Services;

namespace StarPublications.ViewModels;

public class BookSearchViewModel : BaseViewModel
{
    private readonly IBookService _bookService;

    private ObservableCollection<Title> _books = new();
    private ObservableCollection<Store> _storesWithBook = new();
    private Title? _selectedBook;
    private string _searchTerm = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _isLoading;

    public ObservableCollection<Title> Books
    {
        get => _books;
        set => SetProperty(ref _books, value);
    }

    public ObservableCollection<Store> StoresWithBook
    {
        get => _storesWithBook;
        set
        {
            SetProperty(ref _storesWithBook, value);
            OnPropertyChanged(nameof(HasNoStores));
        }
    }

    public bool HasNoStores => _selectedBook != null && _storesWithBook.Count == 0;

    public bool IsBookSelected => _selectedBook != null;

    public Title? SelectedBook
    {
        get => _selectedBook;
        set
        {
            SetProperty(ref _selectedBook, value);
            OnPropertyChanged(nameof(IsBookSelected));
            if (value != null) _ = LoadStoresWithBookAsync(value.TitleId);
            else
            {
                StoresWithBook.Clear();
                OnPropertyChanged(nameof(HasNoStores));
            }
        }
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

    public BookSearchViewModel(IBookService bookService)
    {
        _bookService = bookService;
        SearchCommand = new RelayCommand(async _ => await SearchBooksAsync());
        ClearCommand = new RelayCommand(_ => ClearSearch());
        _ = LoadAllBooksAsync();
    }

    private async Task LoadAllBooksAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading books...";
            var books = await _bookService.GetAllBooksAsync();
            Books = new ObservableCollection<Title>(books);
            StatusMessage = $"Loaded {Books.Count} books.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading books: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SearchBooksAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Searching...";
            var books = await _bookService.SearchBooksAsync(SearchTerm);
            Books = new ObservableCollection<Title>(books);
            StatusMessage = $"Found {Books.Count} books.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error searching books: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadStoresWithBookAsync(string titleId)
    {
        try
        {
            var stores = await _bookService.GetStoresWithBookAsync(titleId);
            StoresWithBook = new ObservableCollection<Store>(stores);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading store information: {ex.Message}";
        }
    }

    private void ClearSearch()
    {
        SearchTerm = string.Empty;
        SelectedBook = null;
        StoresWithBook.Clear();
        _ = LoadAllBooksAsync();
    }
}
