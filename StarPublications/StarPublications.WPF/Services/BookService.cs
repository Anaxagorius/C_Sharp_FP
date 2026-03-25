using Microsoft.EntityFrameworkCore;
using StarPublications.Data;
using StarPublications.Models;

namespace StarPublications.Services;

public class BookService : IBookService
{
    private readonly PubsDbContext _context;

    public BookService(PubsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Title>> GetAllBooksAsync()
    {
        return await _context.Titles
            .Include(t => t.Publisher)
            .Include(t => t.TitleAuthors)
                .ThenInclude(ta => ta.Author)
            .OrderBy(t => t.TitleName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Title>> SearchBooksAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllBooksAsync();

        return await _context.Titles
            .Include(t => t.Publisher)
            .Include(t => t.TitleAuthors)
                .ThenInclude(ta => ta.Author)
            .Where(t => t.TitleName.Contains(searchTerm)
                     || t.Type!.Contains(searchTerm)
                     || (t.Publisher != null && t.Publisher.PublisherName!.Contains(searchTerm))
                     || t.TitleAuthors.Any(ta => ta.Author != null &&
                        (ta.Author.FirstName.Contains(searchTerm) || ta.Author.LastName.Contains(searchTerm))))
            .OrderBy(t => t.TitleName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Store>> GetStoresWithBookAsync(string titleId)
    {
        return await _context.Sales
            .Where(s => s.TitleId == titleId)
            .Select(s => s.Store!)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<Store>> GetAllStoresAsync()
    {
        return await _context.Stores.OrderBy(s => s.StoreName).ToListAsync();
    }
}
