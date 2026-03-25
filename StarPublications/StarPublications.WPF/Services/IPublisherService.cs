using StarPublications.Models;

namespace StarPublications.Services;

public interface IPublisherService
{
    Task<IEnumerable<Publisher>> GetAllPublishersAsync();
    Task<IEnumerable<Publisher>> SearchPublishersAsync(string searchTerm);
}
