using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bibliotekssystem_Arv_Komp.Data;
using Bibliotekssystem_Arv_Komp.Models;
using Bibliotekssystem_Arv_Komp.Web.Dtos;

namespace Bibliotekssystem_Arv_Komp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IDbContextFactory<LibraryContext> _dbFactory;

    public MembersController(IDbContextFactory<LibraryContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    // GET: api/members
    [HttpGet]
    public async Task<ActionResult<List<MemberDto>>> GetAll()
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        var members = await context.Members.ToListAsync();

        var loanCounts = await context.Loans
            .Where(l => l.ReturnDate == null)
            .GroupBy(l => EF.Property<int>(l, "MemberFkId"))
            .Select(g => new { MemberId = g.Key, Count = g.Count() })
            .ToListAsync();

        var dtos = members.Select(m => new MemberDto(
            m.Id,
            m.MemberId,
            m.Name,
            m.Email,
            m.MemberSince,
            loanCounts.FirstOrDefault(lc => lc.MemberId == m.Id)?.Count ?? 0
        )).ToList();

        return Ok(dtos);
    }

    // GET: api/members/5
    [HttpGet("{id}")]
    public async Task<ActionResult<MemberDetailDto>> GetById(int id)
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        var member = await context.Members.FindAsync(id);
        if (member == null) return NotFound();

        var loans = await context.Loans
            .Include(l => l.Item)
            .Where(l => EF.Property<int>(l, "MemberFkId") == id)
            .OrderByDescending(l => l.LoanDate)
            .Select(l => new LoanDto(
                l.Id,
                l.Item.Title,
                l.Item.Id,
                member.Name,
                member.Id,
                l.LoanDate,
                l.DueDate,
                l.ReturnDate
            ))
            .ToListAsync();

        var dto = new MemberDetailDto(
            member.Id, member.MemberId, member.Name,
            member.Email, member.MemberSince, loans
        );

        return Ok(dto);
    }

    // POST: api/members
    [HttpPost]
    public async Task<ActionResult<MemberDto>> Create([FromBody] CreateMemberRequest request)
    {
        using var context = await _dbFactory.CreateDbContextAsync();

        var existing = await context.Members.FirstOrDefaultAsync(m => m.MemberId == request.MemberId);
        if (existing != null)
            return Conflict($"En medlem med ID {request.MemberId} finns redan.");

        var member = new Member(request.MemberId, request.Name, request.Email);
        context.Members.Add(member);
        await context.SaveChangesAsync();

        var dto = new MemberDto(member.Id, member.MemberId, member.Name, member.Email, member.MemberSince, 0);
        return CreatedAtAction(nameof(GetById), new { id = member.Id }, dto);
    }
}
