using StarPublications.Data;
using StarPublications.Services;

namespace StarPublications.ViewModels;

public class MainViewModel : BaseViewModel, IDisposable
{
    private readonly PubsDbContext _context;
    private BaseViewModel? _currentViewModel;
    private int _selectedTabIndex;
    private bool _disposed;

    public BaseViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => SetProperty(ref _selectedTabIndex, value);
    }

    public SalesOrderViewModel SalesOrderViewModel { get; }
    public BookSearchViewModel BookSearchViewModel { get; }
    public PublisherSearchViewModel PublisherSearchViewModel { get; }

    public MainViewModel()
    {
        _context = new PubsDbContext();
        var salesService = new SalesOrderService(_context);
        var bookService = new BookService(_context);
        var publisherService = new PublisherService(_context);

        SalesOrderViewModel = new SalesOrderViewModel(salesService, bookService);
        BookSearchViewModel = new BookSearchViewModel(bookService);
        PublisherSearchViewModel = new PublisherSearchViewModel(publisherService);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _context.Dispose();
            _disposed = true;
        }
    }
}
