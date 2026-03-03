using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bibliotekssystem_Arv_Komp.Data;
using Bibliotekssystem_Arv_Komp.Web.Dtos;

namespace Bibliotekssystem_Arv_Komp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly IDbContextFactory<LibraryContext> _dbFactory;

    public StatsController(IDbContextFactory<LibraryContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    // GET: api/stats
    [HttpGet]
    public async Task<ActionResult<StatsDto>> Get()
    {
        using var context = await _dbFactory.CreateDbContextAsync();

        var stats = new StatsDto(
            BookCount: await context.Books.CountAsync(),
            MemberCount: await context.Members.CountAsync(),
            ActiveLoanCount: await context.Loans.CountAsync(l => l.ReturnDate == null),
            OverdueLoanCount: await context.Loans.CountAsync(l => l.ReturnDate == null && l.DueDate < DateTime.Now)
        );

        return Ok(stats);
    }
}
