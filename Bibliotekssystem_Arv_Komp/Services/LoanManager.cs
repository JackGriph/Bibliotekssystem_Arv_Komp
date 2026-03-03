using Bibliotekssystem_Arv_Komp.Models;

namespace Bibliotekssystem_Arv_Komp.Services
{
    public class LoanManager
    {
        private List<Loan> _loans;
        private readonly int _maxLoansPerMember;

        public LoanManager(int maxLoansPerMember = 3)
        {
            _loans = new List<Loan>();
            _maxLoansPerMember = maxLoansPerMember;
        }

        /// <summary>
        /// Lånar ut ett objekt till en medlem. Returnerar true om lyckat, false om ej tillåtet.
        /// </summary>
        public bool LoanItem(LibraryItem item, Member member, int loanDays = 14)
        {
            if (!item.IsAvailable)
                return false;

            int activeLoans = member.Loans.Count(l => !l.IsReturned);
            if (activeLoans >= _maxLoansPerMember)
                return false;

            var loan = new Loan(item, member, loanDays);
            _loans.Add(loan);
            member.AddLoan(loan);
            item.IsAvailable = false;

            return true;
        }

        /// <summary>
        /// Returnerar ett objekt. Returnerar true om lyckat, false om inget aktivt lån hittades.
        /// </summary>
        public bool ReturnItem(string itemId, string memberId)
        {
            var loan = _loans.FirstOrDefault(l =>
                l.Item.ItemId == itemId &&
                l.Member.MemberId == memberId &&
                !l.IsReturned);

            if (loan == null)
                return false;

            loan.ReturnItem();
            return true;
        }

        public List<Loan> GetActiveLoans()
        {
            return _loans.Where(l => !l.IsReturned).ToList();
        }

        public List<Loan> GetOverdueLoans()
        {
            return _loans.Where(l => !l.IsReturned && l.IsOverdue).ToList();
        }

        public List<MemberLoanStats> GetMostActiveMembers(List<Member> allMembers, int topCount = 3)
        {
            return allMembers
                .Select(member => new MemberLoanStats
                {
                    Member = member,
                    TotalLoans = member.Loans.Count,
                    ActiveLoans = member.Loans.Count(l => !l.IsReturned)
                })
                .OrderByDescending(stats => stats.TotalLoans)
                .ThenByDescending(stats => stats.ActiveLoans)
                .Take(topCount)
                .ToList();
        }
    }

    public class MemberLoanStats
    {
        public Member Member { get; init; } = null!;
        public int TotalLoans { get; init; }
        public int ActiveLoans { get; init; }
    }
}
