using StarPublications.Models;

namespace StarPublications.Services;

public interface ISalesOrderService
{
    Task<IEnumerable<Sale>> GetAllSalesAsync();
    Task<IEnumerable<Sale>> SearchSalesAsync(string storeId, string orderNumber, string titleId);
    Task<Sale?> GetSaleAsync(string storeId, string orderNumber, string titleId);
    Task AddSaleAsync(Sale sale);
    Task UpdateSaleAsync(Sale sale);
    Task DeleteSaleAsync(string storeId, string orderNumber, string titleId);
}
