using Microsoft.EntityFrameworkCore;
using StarPublications.Data;
using StarPublications.Models;

namespace StarPublications.Services;

public class PublisherService : IPublisherService
{
    private readonly PubsDbContext _context;

    public PublisherService(PubsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Publisher>> GetAllPublishersAsync()
    {
        return await _context.Publishers
            .Include(p => p.Titles)
            .OrderBy(p => p.PublisherName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Publisher>> SearchPublishersAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllPublishersAsync();

        return await _context.Publishers
            .Include(p => p.Titles)
            .Where(p => p.PublisherName!.Contains(searchTerm)
                     || p.City!.Contains(searchTerm)
                     || p.State!.Contains(searchTerm)
                     || p.Country!.Contains(searchTerm))
            .OrderBy(p => p.PublisherName)
            .ToListAsync();
    }
}
