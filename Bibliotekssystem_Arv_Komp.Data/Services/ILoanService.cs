using Bibliotekssystem_Arv_Komp.Models;

namespace Bibliotekssystem_Arv_Komp.Data.Services;

/// <summary>
/// Hanterar affärslogik för utlåning (max 3 lån, tillgänglighetskontroll etc.)
/// </summary>
public interface ILoanService
{
    Task<Loan> CreateLoanAsync(int bookId, int memberId);
    Task<Loan> ReturnLoanAsync(int loanId);
    Task<IEnumerable<Loan>> GetActiveLoansAsync();
    Task<LibraryStats> GetStatsAsync();
}

public record LibraryStats(
    int BookCount,
    int MemberCount,
    int ActiveLoanCount,
    int OverdueLoanCount
);
