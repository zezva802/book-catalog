
namespace BookCatalog.Domain.Entities;

public class Book
{

    public int Id { get; set; }

    /// <summary>
    /// Title of the work.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// (Optional) Author of the work.
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// (Optional) Year the work was written. Negative numbers are BCE.
    /// </summary>
    public int? Year { get; set; }
}