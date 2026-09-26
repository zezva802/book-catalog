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

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_bookStorage.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var book = _bookStorage.GetById(id);
        if (book == null)
        {
            _logger.LogInformation("Get - Not found book {BookId}", id);
            return NotFound();
        }
        return Ok(book);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Book request)
    {
        var book = _bookStorage.Create(request.Title, request.Author, request.Year);
        _logger.LogInformation("Created book {BookId}", book.Id);
        return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Book request)
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

    [HttpDelete("{id}")]
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