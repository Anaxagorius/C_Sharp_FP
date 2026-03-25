using Microsoft.EntityFrameworkCore;
using StarPublications.Data;
using StarPublications.Models;

namespace StarPublications.Services;

public class SalesOrderService : ISalesOrderService
{
    private readonly PubsDbContext _context;

    public SalesOrderService(PubsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Sale>> GetAllSalesAsync()
    {
        return await _context.Sales
            .Include(s => s.Store)
            .Include(s => s.Title)
            .OrderByDescending(s => s.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> SearchSalesAsync(string storeId, string orderNumber, string titleId)
    {
        var query = _context.Sales
            .Include(s => s.Store)
            .Include(s => s.Title)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(storeId))
            query = query.Where(s => s.StoreId.Contains(storeId));

        if (!string.IsNullOrWhiteSpace(orderNumber))
            query = query.Where(s => s.OrderNumber.Contains(orderNumber));

        if (!string.IsNullOrWhiteSpace(titleId))
            query = query.Where(s => s.TitleId.Contains(titleId));

        return await query.OrderByDescending(s => s.OrderDate).ToListAsync();
    }

    public async Task<Sale?> GetSaleAsync(string storeId, string orderNumber, string titleId)
    {
        return await _context.Sales
            .Include(s => s.Store)
            .Include(s => s.Title)
            .FirstOrDefaultAsync(s => s.StoreId == storeId && s.OrderNumber == orderNumber && s.TitleId == titleId);
    }

    public async Task AddSaleAsync(Sale sale)
    {
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSaleAsync(Sale sale)
    {
        var existing = await _context.Sales.FindAsync(sale.StoreId, sale.OrderNumber, sale.TitleId);
        if (existing == null) throw new InvalidOperationException("Sale not found.");

        existing.OrderDate = sale.OrderDate;
        existing.Quantity = sale.Quantity;
        existing.PayTerms = sale.PayTerms;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSaleAsync(string storeId, string orderNumber, string titleId)
    {
        var sale = await _context.Sales.FindAsync(storeId, orderNumber, titleId);
        if (sale == null) throw new InvalidOperationException("Sale not found.");
        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync();
    }
}
