using Bibliotekssystem_Arv_Komp.Models;

namespace Bibliotekssystem_Arv_Komp.Data.Repositories;

public interface ILoanRepository
{
    Task<IEnumerable<Loan>> GetActiveLoansAsync();
    Task<IEnumerable<Loan>> GetLoansByBookIdAsync(int bookId);
    Task<IEnumerable<Loan>> GetLoansByMemberIdAsync(int memberId);
    Task<Loan?> GetByIdAsync(int id);
    Task<int> GetActiveLoanCountForMemberAsync(int memberId);
    Task AddAsync(Loan loan);
    Task SaveChangesAsync();
}
