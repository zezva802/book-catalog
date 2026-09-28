using BookCatalog.Api.Entities;
using BookCatalog.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookStorage _bookStorage;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookStorage bookStorage, ILogger<BooksController> logger)
    {
        _bookStorage = bookStorage;
        _logger = logger;
    }


    /// <summary>
    /// Returns all books.
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<Book>> GetAll()
    {
        return Ok(_bookStorage.GetAll());
    }

    /// <summary>
    /// Gets a book by its id.
    /// </summary>
    /// <param name="id">Id of the book.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<Book> Get(int id)
    {
        var book = _bookStorage.GetById(id);
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
    public ActionResult<Book> Create([FromBody] Book request)
    {
        var book = _bookStorage.Create(request.Title, request.Author, request.Year);
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
    public ActionResult<Book> Update(int id, [FromBody] Book request)
    {
        var book = _bookStorage.Update(id, request);
        if (book == null)
        {
            _logger.LogInformation("Update - Not found book {BookId}", id);
            return NotFound();
        }
        _logger.LogInformation("Updated book {BookId}", id);
        return Ok(book);
    }

    /// <summary>
    /// Deletes a book.
    /// </summary>
    /// <param name="id">Id of the book to delete.</param>
    /// <response code="204">The book was deleted, or no book with this id existed</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete(int id)
    {
        bool isDeleted = _bookStorage.Delete(id);
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