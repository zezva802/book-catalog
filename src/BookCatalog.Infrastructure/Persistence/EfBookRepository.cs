using BookCatalog.Application.Abstractions;
using BookCatalog.Application.Common;
using BookCatalog.Application.Queries;
using BookCatalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace BookCatalog.Infrastructure.Persistence;

internal sealed class EfBookRepository(BookCatalogDbContext db): IBookRepository
{
    public async Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    } 

    public async Task<PagedResult<Book>> ListAsync(BookListQuery query, CancellationToken cancellationToken = default){
        IQueryable<Book> books = ApplyFilters(db.Books.AsNoTracking(), query);

        var totalCount = await books.CountAsync(cancellationToken);

        var items = await books
            .OrderBy(b => b.Title).ThenBy(b => b.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<Book>(items, totalCount, query.Page, query.PageSize);
    }

    public async Task<Book> CreateAsync(Book book, CancellationToken cancellationToken = default)
    {
        db.Books.Add(book);
        await db.SaveChangesAsync(cancellationToken);
        return book;
    }

    public async Task<bool> UpdateAsync(Book book, CancellationToken cancellationToken = default)
    {
        var rows = await db.Books.Where(b => b.Id == book.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.Title, book.Title)
                .SetProperty(b => b.Author, book.Author)
                .SetProperty(b => b.Year, book.Year),
                cancellationToken);
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken= default)
    {
        return await db.Books.Where(b => b.Id == id).ExecuteDeleteAsync(cancellationToken) > 0;
    }

    private static IQueryable<Book> ApplyFilters(IQueryable<Book> books, BookListQuery query)
    {
        if (query.Title is not null)
        {
            books = books.Where(b => b.Title.Contains(query.Title));
        }

        if (query.Author is not null)
        {
            books = books.Where(b => b.Author != null && b.Author.Contains(query.Author));
        }

        if (query.YearFrom is not null)
        {
            books = books.Where(b => b.Year >= query.YearFrom);
        }

        if (query.YearTo is not null)
        {
            books = books.Where(b => b.Year <= query.YearTo);
        }

        return books;
    }
}