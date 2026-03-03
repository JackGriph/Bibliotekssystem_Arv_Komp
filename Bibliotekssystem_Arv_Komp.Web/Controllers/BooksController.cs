using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bibliotekssystem_Arv_Komp.Data.Context;
using Bibliotekssystem_Arv_Komp.Models;
using Bibliotekssystem_Arv_Komp.Web.Dtos;

namespace Bibliotekssystem_Arv_Komp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IDbContextFactory<LibraryContext> _dbFactory;

    public BooksController(IDbContextFactory<LibraryContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    // GET: api/books
    [HttpGet]
    public async Task<ActionResult<List<BookDto>>> GetAll()
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        var books = await context.Books
            .Select(b => new BookDto(b.Id, b.ISBN, b.Title, b.Author, b.PublishedYear, b.Pages, b.IsAvailable))
            .ToListAsync();

        return Ok(books);
    }

    // GET: api/books/5
    [HttpGet("{id}")]
    public async Task<ActionResult<BookDetailDto>> GetById(int id)
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        var book = await context.Books.FindAsync(id);
        if (book == null) return NotFound();

        var loans = await context.Loans
            .Include(l => l.Member)
            .Where(l => EF.Property<int>(l, "LibraryItemId") == id)
            .OrderByDescending(l => l.LoanDate)
            .Select(l => new LoanDto(
                l.Id,
                book.Title,
                id,
                l.Member.Name,
                l.Member.Id,
                l.LoanDate,
                l.DueDate,
                l.ReturnDate
            ))
            .ToListAsync();

        var dto = new BookDetailDto(
            book.Id, book.ISBN, book.Title, book.Author,
            book.PublishedYear, book.Pages, book.IsAvailable, loans
        );

        return Ok(dto);
    }

    // GET: api/books/search?q=astrid
    [HttpGet("search")]
    public async Task<ActionResult<List<BookDto>>> Search([FromQuery] string q)
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        var books = await context.Books.ToListAsync();

        var filtered = string.IsNullOrWhiteSpace(q)
            ? books
            : books.Where(b => b.Matches(q)).ToList();

        var dtos = filtered.Select(b =>
            new BookDto(b.Id, b.ISBN, b.Title, b.Author, b.PublishedYear, b.Pages, b.IsAvailable)
        ).ToList();

        return Ok(dtos);
    }

    // POST: api/books
    [HttpPost]
    public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookRequest request)
    {
        using var context = await _dbFactory.CreateDbContextAsync();

        var existing = await context.Books.FirstOrDefaultAsync(b => b.ISBN == request.ISBN);
        if (existing != null)
            return Conflict($"En bok med ISBN {request.ISBN} finns redan.");

        var book = new Book(request.ISBN, request.Title, request.Author, request.PublishedYear, request.Pages);
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var dto = new BookDto(book.Id, book.ISBN, book.Title, book.Author, book.PublishedYear, book.Pages, book.IsAvailable);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, dto);
    }
}
