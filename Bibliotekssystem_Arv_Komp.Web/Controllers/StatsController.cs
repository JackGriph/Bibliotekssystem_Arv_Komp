using Microsoft.AspNetCore.Mvc;
using Bibliotekssystem_Arv_Komp.Data.Services;
using Bibliotekssystem_Arv_Komp.Web.Dtos;

namespace Bibliotekssystem_Arv_Komp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly ILoanService _loanService;

    public StatsController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    // GET: api/stats
    [HttpGet]
    public async Task<ActionResult<StatsDto>> Get()
    {
        var stats = await _loanService.GetStatsAsync();

        return Ok(new StatsDto(
            stats.BookCount,
            stats.MemberCount,
            stats.ActiveLoanCount,
            stats.OverdueLoanCount
        ));
    }
}
