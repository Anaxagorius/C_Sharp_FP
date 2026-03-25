using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarPublications.Models;

[Table("publishers")]
public class Publisher
{
    [Key]
    [Column("pub_id")]
    [StringLength(4)]
    public string PubId { get; set; } = string.Empty;

    [Column("pub_name")]
    [StringLength(40)]
    public string? PublisherName { get; set; }

    [Column("city")]
    [StringLength(20)]
    public string? City { get; set; }

    [Column("state")]
    [StringLength(2)]
    public string? State { get; set; }

    [Column("country")]
    [StringLength(30)]
    public string? Country { get; set; }

    public ICollection<Title> Titles { get; set; } = new List<Title>();
}
