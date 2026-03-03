using Microsoft.AspNetCore.Mvc;
using Bibliotekssystem_Arv_Komp.Data.Services;
using Bibliotekssystem_Arv_Komp.Web.Dtos;

namespace Bibliotekssystem_Arv_Komp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    // GET: api/loans/active
    [HttpGet("active")]
    public async Task<ActionResult<List<LoanDto>>> GetActive()
    {
        var loans = await _loanService.GetActiveLoansAsync();

        var dtos = loans.Select(l => new LoanDto(
            l.Id, l.Item.Title, l.Item.Id,
            l.Member.Name, l.Member.Id,
            l.LoanDate, l.DueDate, l.ReturnDate
        )).ToList();

        return Ok(dtos);
    }

    // POST: api/loans
    [HttpPost]
    public async Task<ActionResult<LoanDto>> Create([FromBody] CreateLoanRequest request)
    {
        try
        {
            var loan = await _loanService.CreateLoanAsync(request.BookId, request.MemberId);

            var dto = new LoanDto(
                loan.Id, loan.Item.Title, loan.Item.Id,
                loan.Member.Name, loan.Member.Id,
                loan.LoanDate, loan.DueDate, loan.ReturnDate
            );
            return Created("", dto);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // PUT: api/loans/5/return
    [HttpPut("{id}/return")]
    public async Task<ActionResult> Return(int id)
    {
        try
        {
            var loan = await _loanService.ReturnLoanAsync(id);
            return Ok(new { message = $"{loan.Item.Title} har returnerats." });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
