using BookCatalog.Api.Entities;
using BookCatalog.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookStorage _bookStorage;

    public BooksController(IBookStorage bookStorage)
    {
        _bookStorage = bookStorage;
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
        if(book == null)
        {
            return NotFound();
        } else
        {
            return Ok(book);
        }
    }

    [HttpPost]
    public IActionResult Create([FromBody] Book request)
    {
        var book = _bookStorage.Create(request.Title, request.Author, request.Year);
        return CreatedAtAction(nameof(Get), new {id = book.Id}, book);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Book request)
    {
        var book = _bookStorage.Update(id, request);
        if(book == null)
        {
            return NotFound();
        }
        return Ok(book);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _bookStorage.Delete(id);
        return NoContent();
    }

}