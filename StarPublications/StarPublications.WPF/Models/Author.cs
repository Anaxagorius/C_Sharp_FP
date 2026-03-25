using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarPublications.Models;

[Table("authors")]
public class Author
{
    [Key]
    [Column("au_id")]
    [StringLength(11)]
    public string AuthorId { get; set; } = string.Empty;

    [Column("au_lname")]
    [StringLength(40)]
    public string LastName { get; set; } = string.Empty;

    [Column("au_fname")]
    [StringLength(20)]
    public string FirstName { get; set; } = string.Empty;

    [Column("phone")]
    [StringLength(12)]
    public string? Phone { get; set; }

    [Column("address")]
    [StringLength(40)]
    public string? Address { get; set; }

    [Column("city")]
    [StringLength(20)]
    public string? City { get; set; }

    [Column("state")]
    [StringLength(2)]
    public string? State { get; set; }

    [Column("zip")]
    [StringLength(5)]
    public string? Zip { get; set; }

    [Column("contract")]
    public bool Contract { get; set; }

    public ICollection<TitleAuthor> TitleAuthors { get; set; } = new List<TitleAuthor>();
    public string FullName => $"{FirstName} {LastName}";
}
