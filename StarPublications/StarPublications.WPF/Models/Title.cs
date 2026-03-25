using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarPublications.Models;

[Table("titles")]
public class Title
{
    [Key]
    [Column("title_id")]
    [StringLength(6)]
    public string TitleId { get; set; } = string.Empty;

    [Column("title")]
    [StringLength(80)]
    public string TitleName { get; set; } = string.Empty;

    [Column("type")]
    [StringLength(12)]
    public string? Type { get; set; }

    [Column("pub_id")]
    [StringLength(4)]
    public string? PubId { get; set; }

    [Column("price", TypeName = "money")]
    public decimal? Price { get; set; }

    [Column("advance", TypeName = "money")]
    public decimal? Advance { get; set; }

    [Column("royalty")]
    public int? Royalty { get; set; }

    [Column("ytd_sales")]
    public int? YtdSales { get; set; }

    [Column("notes")]
    [StringLength(200)]
    public string? Notes { get; set; }

    [Column("pubdate")]
    public DateTime PubDate { get; set; }

    public Publisher? Publisher { get; set; }
    public ICollection<TitleAuthor> TitleAuthors { get; set; } = new List<TitleAuthor>();
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public string AuthorNames => string.Join(", ", TitleAuthors?.Select(ta => ta.Author != null ? $"{ta.Author.FirstName} {ta.Author.LastName}" : "") ?? Enumerable.Empty<string>());
    public string DisplayPrice => Price.HasValue ? Price.Value.ToString("C") : "N/A";
}
