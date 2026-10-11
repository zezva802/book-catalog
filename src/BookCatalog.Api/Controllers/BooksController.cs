using Microsoft.AspNetCore.Mvc;
using BookCatalog.Application.Abstractions;
using BookCatalog.Api.Dtos;

namespace BookCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }


    /// <summary>
    /// Returns all books.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var books = await _bookService.GetAllAsync(cancellationToken);
        return Ok(books.Select(b => b.ToResponse()));
    }

    /// <summary>
    /// Gets a book by its id.
    /// </summary>
    /// <param name="id">Id of the book.</param>
    /// <param name="cancellationToken">Cancelled if the client disconnects.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BookResponse>> Get(int id, CancellationToken cancellationToken)
    {
        var book = await _bookService.GetByIdAsync(id, cancellationToken);
        if(book == null)
        {
            return NotFound();
        }
        return Ok(book.ToResponse());
    }

    /// <summary>
    /// Creates a book.
    /// </summary>
    /// <param name="request">Body of the book to be created.</param>
    /// <param name="cancellationToken">Cancelled if the client disconnects.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookResponse>> Create([FromBody] BookRequest request, CancellationToken cancellationToken)
    {   
        var book = await _bookService.CreateAsync(request.ToEntity(), cancellationToken);
        return CreatedAtAction(nameof(Get), new {id = book.Id}, book.ToResponse());
    }

    /// <summary>
    /// Updates a book.
    /// </summary>
    /// <param name="id">Id of the book to be updated.</param>
    /// <param name="request">Data to fully replace existing.</param>
    /// <param name="cancellationToken">Cancelled if the client disconnects.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BookResponse>> Update(int id, [FromBody] BookRequest request, CancellationToken cancellationToken)
    {
        var book = request.ToEntity(id);
        var isUpdated = await _bookService.UpdateAsync(book, cancellationToken);
        if (isUpdated)
        {
            return Ok(book.ToResponse());
        }
        return NotFound();
    }

    /// <summary>
    /// Deletes a book.
    /// </summary>
    /// <param name="id">Id of the book to delete.</param>
    /// <response code="204">The book was deleted, or no book with this id existed</response>
    /// <param name="cancellationToken">Cancelled if the client disconnects.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _bookService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

}