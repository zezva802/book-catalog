namespace BookCatalog.Api.Entities;

public class Book{
    public int Id {get; set;}
    public required string Title {get; set;}
    public string? Author {get; set;}
    public int? Year {get; set;}
    
}