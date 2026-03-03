using Microsoft.EntityFrameworkCore;
using Bibliotekssystem_Arv_Komp.Models;
using Bibliotekssystem_Arv_Komp.Data.Context;

namespace Bibliotekssystem_Arv_Komp.Data.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly LibraryContext _context;

    public LoanRepository(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
    {
        return await _context.Loans
            .Include(l => l.Item)
            .Include(l => l.Member)
            .Where(l => l.ReturnDate == null)
            .OrderBy(l => l.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetLoansByBookIdAsync(int bookId)
    {
        return await _context.Loans
            .Include(l => l.Member)
            .Where(l => EF.Property<int>(l, "LibraryItemId") == bookId)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetLoansByMemberIdAsync(int memberId)
    {
        return await _context.Loans
            .Include(l => l.Item)
            .Where(l => EF.Property<int>(l, "MemberFkId") == memberId)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();
    }

    public async Task<Loan?> GetByIdAsync(int id)
    {
        return await _context.Loans
            .Include(l => l.Item)
            .Include(l => l.Member)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<int> GetActiveLoanCountForMemberAsync(int memberId)
    {
        return await _context.Loans
            .Where(l => EF.Property<int>(l, "MemberFkId") == memberId && l.ReturnDate == null)
            .CountAsync();
    }

    public async Task AddAsync(Loan loan)
    {
        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
