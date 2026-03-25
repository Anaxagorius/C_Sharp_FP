using StarPublications.Data;
using StarPublications.Services;

namespace StarPublications.ViewModels;

public class MainViewModel : BaseViewModel
{
    private BaseViewModel? _currentViewModel;
    private int _selectedTabIndex;

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
        var context = new PubsDbContext();
        var salesService = new SalesOrderService(context);
        var bookService = new BookService(context);
        var publisherService = new PublisherService(context);

        SalesOrderViewModel = new SalesOrderViewModel(salesService, bookService);
        BookSearchViewModel = new BookSearchViewModel(bookService);
        PublisherSearchViewModel = new PublisherSearchViewModel(publisherService);
    }
}
