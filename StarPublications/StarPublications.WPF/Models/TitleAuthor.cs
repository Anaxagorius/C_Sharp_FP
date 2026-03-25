using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarPublications.Models;

[Table("titleauthor")]
public class TitleAuthor
{
    [Column("au_id")]
    [StringLength(11)]
    public string AuthorId { get; set; } = string.Empty;

    [Column("title_id")]
    [StringLength(6)]
    public string TitleId { get; set; } = string.Empty;

    [Column("au_ord")]
    public byte? AuthorOrder { get; set; }

    [Column("royaltyper")]
    public int? RoyaltyPer { get; set; }

    public Author? Author { get; set; }
    public Title? Title { get; set; }
}
