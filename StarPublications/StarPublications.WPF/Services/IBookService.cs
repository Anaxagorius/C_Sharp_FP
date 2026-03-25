using StarPublications.Models;

namespace StarPublications.Services;

public interface IBookService
{
    Task<IEnumerable<Title>> GetAllBooksAsync();
    Task<IEnumerable<Title>> SearchBooksAsync(string searchTerm);
    Task<IEnumerable<Store>> GetStoresWithBookAsync(string titleId);
    Task<IEnumerable<Store>> GetAllStoresAsync();
}
