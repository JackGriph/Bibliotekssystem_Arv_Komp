using Microsoft.EntityFrameworkCore;
using Bibliotekssystem_Arv_Komp.Models;
using Bibliotekssystem_Arv_Komp.Data.Context;

namespace Bibliotekssystem_Arv_Komp.Data.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly LibraryContext _context;

    public MemberRepository(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Member>> GetAllAsync()
    {
        return await _context.Members.ToListAsync();
    }

    public async Task<Member?> GetByIdAsync(int id)
    {
        return await _context.Members.FindAsync(id);
    }

    public async Task<Member?> GetByMemberIdAsync(string memberId)
    {
        return await _context.Members.FirstOrDefaultAsync(m => m.MemberId == memberId);
    }

    public async Task AddAsync(Member member)
    {
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
    }
}
