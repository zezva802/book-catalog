using BookCatalog.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using BookCatalog.Application.Abstractions;

namespace BookCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookRepository bookRepository, ILogger<BooksController> logger)
    {
        _bookRepository = bookRepository;
        _logger = logger;
    }


    /// <summary>
    /// Returns all books.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Book>>> GetAll(CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetAllAsync(cancellationToken);
        return Ok(books);
    }

    /// <summary>
    /// Gets a book by its id.
    /// </summary>
    /// <param name="id">Id of the book.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Book>> Get(int id, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(id, cancellationToken);
        if (book == null)
        {
            _logger.LogInformation("Get - Not found book {BookId}", id);
            return NotFound();
        }
        return Ok(book);
    }

    /// <summary>
    /// Creates a book.
    /// </summary>
    /// <param name="request">Body of the book to be created.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Book>> Create([FromBody] Book request, CancellationToken cancellationToken)
    {   
        var book = await _bookRepository.CreateAsync(request, cancellationToken);
        _logger.LogInformation("Created book {BookId}", book.Id);
        return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
    }

    /// <summary>
    /// Updates a book.
    /// </summary>
    /// <param name="id">Id of the book to be updated.</param>
    /// <param name="request">Data to fully replace existing.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Book>> Update(int id, [FromBody] Book request, CancellationToken cancellationToken)
    {
        request.Id = id;
        var isUpdated = await _bookRepository.UpdateAsync(request, cancellationToken);
        if (!isUpdated)
        {
            _logger.LogInformation("Update - Not found book {BookId}", id);
            return NotFound();
        }
        _logger.LogInformation("Updated book {BookId}", id);
        return Ok(request);
    }

    /// <summary>
    /// Deletes a book.
    /// </summary>
    /// <param name="id">Id of the book to delete.</param>
    /// <response code="204">The book was deleted, or no book with this id existed</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        bool isDeleted = await _bookRepository.DeleteAsync(id, cancellationToken);
        if (isDeleted)
        {
            _logger.LogInformation("Deleted book {BookId}", id);
        }
        else
        {
            _logger.LogInformation("Delete - Not found book {BookId}", id);
        }

        return NoContent();
    }

}