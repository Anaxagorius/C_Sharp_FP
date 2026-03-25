using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarPublications.Models;

[Table("sales")]
public class Sale
{
    [Column("stor_id")]
    [StringLength(4)]
    public string StoreId { get; set; } = string.Empty;

    [Column("ord_num")]
    [StringLength(20)]
    public string OrderNumber { get; set; } = string.Empty;

    [Column("ord_date")]
    public DateTime OrderDate { get; set; }

    [Column("qty")]
    public short Quantity { get; set; }

    [Column("payterms")]
    [StringLength(12)]
    public string? PayTerms { get; set; }

    [Column("title_id")]
    [StringLength(6)]
    public string TitleId { get; set; } = string.Empty;

    public Store? Store { get; set; }
    public Title? Title { get; set; }
}
