using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bibliotekssystem_Arv_Komp.Data;
using Bibliotekssystem_Arv_Komp.Models;
using Bibliotekssystem_Arv_Komp.Web.Dtos;

namespace Bibliotekssystem_Arv_Komp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly IDbContextFactory<LibraryContext> _dbFactory;

    public LoansController(IDbContextFactory<LibraryContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    // GET: api/loans/active
    [HttpGet("active")]
    public async Task<ActionResult<List<LoanDto>>> GetActive()
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        var loans = await context.Loans
            .Include(l => l.Item)
            .Include(l => l.Member)
            .Where(l => l.ReturnDate == null)
            .OrderBy(l => l.DueDate)
            .Select(l => new LoanDto(
                l.Id,
                l.Item.Title,
                l.Item.Id,
                l.Member.Name,
                l.Member.Id,
                l.LoanDate,
                l.DueDate,
                l.ReturnDate
            ))
            .ToListAsync();

        return Ok(loans);
    }

    // POST: api/loans
    [HttpPost]
    public async Task<ActionResult<LoanDto>> Create([FromBody] CreateLoanRequest request)
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        var book = await context.Books.FindAsync(request.BookId);
        var member = await context.Members.FindAsync(request.MemberId);

        if (book == null || member == null)
            return BadRequest("Bok eller medlem hittades inte.");

        if (!book.IsAvailable)
            return Conflict("Boken är redan utlånad.");

        var memberActiveLoans = await context.Loans
            .Where(l => EF.Property<int>(l, "MemberFkId") == request.MemberId && l.ReturnDate == null)
            .CountAsync();

        if (memberActiveLoans >= 3)
            return Conflict($"{member.Name} har redan 3 aktiva lån (max tillåtet).");

        var loan = new Loan(book, member, 14);
        context.Loans.Add(loan);
        book.IsAvailable = false;
        await context.SaveChangesAsync();

        var dto = new LoanDto(loan.Id, book.Title, book.Id, member.Name, member.Id, loan.LoanDate, loan.DueDate, loan.ReturnDate);
        return Created("", dto);
    }

    // PUT: api/loans/5/return
    [HttpPut("{id}/return")]
    public async Task<ActionResult> Return(int id)
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        var loan = await context.Loans
            .Include(l => l.Item)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null) return NotFound();

        loan.ReturnItem();
        await context.SaveChangesAsync();

        return Ok(new { message = $"{loan.Item.Title} har returnerats." });
    }
}
