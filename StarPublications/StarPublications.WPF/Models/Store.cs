using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarPublications.Models;

[Table("stores")]
public class Store
{
    [Key]
    [Column("stor_id")]
    [StringLength(4)]
    public string StoreId { get; set; } = string.Empty;

    [Column("stor_name")]
    [StringLength(40)]
    public string? StoreName { get; set; }

    [Column("stor_address")]
    [StringLength(40)]
    public string? StoreAddress { get; set; }

    [Column("city")]
    [StringLength(20)]
    public string? City { get; set; }

    [Column("state")]
    [StringLength(2)]
    public string? State { get; set; }

    [Column("zip")]
    [StringLength(5)]
    public string? Zip { get; set; }

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public string FullAddress => $"{StoreAddress}, {City}, {State} {Zip}";
}
