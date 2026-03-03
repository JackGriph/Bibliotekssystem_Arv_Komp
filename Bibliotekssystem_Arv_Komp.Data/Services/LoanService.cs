using Microsoft.EntityFrameworkCore;
using Bibliotekssystem_Arv_Komp.Models;
using Bibliotekssystem_Arv_Komp.Data.Context;

namespace Bibliotekssystem_Arv_Komp.Data.Services;

/// <summary>
/// Implementerar affärsregler för utlåning.
/// Alla regler (max 3 lån, tillgänglighetskontroll) finns här – inte i Controllers.
/// </summary>
public class LoanService : ILoanService
{
    private readonly IDbContextFactory<LibraryContext> _dbFactory;
    private const int MaxLoansPerMember = 3;

    public LoanService(IDbContextFactory<LibraryContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<Loan> CreateLoanAsync(int bookId, int memberId)
    {
        using var context = await _dbFactory.CreateDbContextAsync();

        var book = await context.Books.FindAsync(bookId)
            ?? throw new InvalidOperationException("Boken hittades inte.");

        var member = await context.Members.FindAsync(memberId)
            ?? throw new InvalidOperationException("Medlemmen hittades inte.");

        if (!book.IsAvailable)
            throw new InvalidOperationException("Boken är redan utlånad.");

        var activeLoanCount = await context.Loans
            .Where(l => EF.Property<int>(l, "MemberFkId") == memberId && l.ReturnDate == null)
            .CountAsync();

        if (activeLoanCount >= MaxLoansPerMember)
            throw new InvalidOperationException($"{member.Name} har redan {MaxLoansPerMember} aktiva lån (max tillåtet).");

        var loan = new Loan(book, member, 14);
        context.Loans.Add(loan);
        book.IsAvailable = false;
        await context.SaveChangesAsync();

        return loan;
    }

    public async Task<Loan> ReturnLoanAsync(int loanId)
    {
        using var context = await _dbFactory.CreateDbContextAsync();

        var loan = await context.Loans
            .Include(l => l.Item)
            .FirstOrDefaultAsync(l => l.Id == loanId)
            ?? throw new InvalidOperationException("Lånet hittades inte.");

        loan.ReturnItem();
        await context.SaveChangesAsync();

        return loan;
    }

    public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        return await context.Loans
            .Include(l => l.Item)
            .Include(l => l.Member)
            .Where(l => l.ReturnDate == null)
            .OrderBy(l => l.DueDate)
            .ToListAsync();
    }

    public async Task<LibraryStats> GetStatsAsync()
    {
        using var context = await _dbFactory.CreateDbContextAsync();

        return new LibraryStats(
            BookCount: await context.Books.CountAsync(),
            MemberCount: await context.Members.CountAsync(),
            ActiveLoanCount: await context.Loans.CountAsync(l => l.ReturnDate == null),
            OverdueLoanCount: await context.Loans.CountAsync(l => l.ReturnDate == null && l.DueDate < DateTime.Now)
        );
    }
}
