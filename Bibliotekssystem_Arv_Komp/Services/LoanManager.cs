namespace Bibliotekssystem_Arv_Komp
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

        public bool LoanItem(LibraryItem item, Member member, int loanDays = 14)
        {
            if (!item.IsAvailable)
            {
                Console.WriteLine($"{item.GetItemType()} '{item.Title}' är inte tillgänglig.");
                return false;
            }

            int activeLoans = member.Loans.Count(l => !l.IsReturned);
            if (activeLoans >= _maxLoansPerMember)
            {
                Console.WriteLine($"Medlem '{member.Name}' har nått maxgränsen för lån ({_maxLoansPerMember}).");
                return false;
            }

            var loan = new Loan(item, member, loanDays);
            _loans.Add(loan);
            member.AddLoan(loan);
            item.IsAvailable = false;

            Console.WriteLine($"{item.GetItemType()} '{item.Title}' har lånats ut till '{member.Name}'.");
            Console.WriteLine($"Förfallodatum: {loan.DueDate:yyyy-MM-dd}");
            return true;
        }

        public bool ReturnItem(string itemId, string memberId)
        {
            var loan = _loans.FirstOrDefault(l =>
                l.Item.ItemId == itemId &&
                l.Member.MemberId == memberId &&
                !l.IsReturned);

            if (loan == null)
            {
                Console.WriteLine("Inget aktivt lån hittades för denna kombination.");
                return false;
            }

            loan.ReturnItem();

            if (loan.IsOverdue)
            {
                int daysLate = (loan.ReturnDate!.Value - loan.DueDate).Days;
                Console.WriteLine($"{loan.Item.GetItemType()} '{loan.Item.Title}' har returnerats {daysLate} dag(ar) för sent!");
            }
            else
            {
                Console.WriteLine($"{loan.Item.GetItemType()} '{loan.Item.Title}' har returnerats.");
            }

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

        public void DisplayActiveLoans()
        {
            var activeLoans = GetActiveLoans();
            if (!activeLoans.Any())
            {
                Console.WriteLine("Inga aktiva lån.");
                return;
            }

            Console.WriteLine($"\n=== Aktiva lån ({activeLoans.Count}) ===");
            foreach (var loan in activeLoans)
            {
                Console.WriteLine($"\n{loan.GetLoanInfo()}");
                Console.WriteLine(new string('-', 40));
            }
        }

        public void DisplayOverdueLoans()
        {
            var overdueLoans = GetOverdueLoans();
            if (!overdueLoans.Any())
            {
                Console.WriteLine("Inga försenade lån.");
                return;
            }

            Console.WriteLine($"\n=== Försenade lån ({overdueLoans.Count}) ===");
            foreach (var loan in overdueLoans)
            {
                Console.WriteLine($"\n{loan.GetLoanInfo()}");
                int daysLate = (DateTime.Now - loan.DueDate).Days;
                Console.WriteLine($"Antal dagar försenad: {daysLate}");
                Console.WriteLine(new string('-', 40));
            }
        }

        // === DEL 4: STATISTIK - Visa lånestatistik ===
        public void DisplayStatistics(List<Member> allMembers)
        {
            var activeLoans = GetActiveLoans();
            var overdueLoans = GetOverdueLoans();
            var topMembers = GetMostActiveMembers(allMembers, 3);

            Console.WriteLine($"\n╔══════════════════════════════════════════════╗");
            Console.WriteLine($"║         LÅNESTATISTIK                        ║");
            Console.WriteLine($"╚══════════════════════════════════════════════╝");

            Console.WriteLine($"\n📋 LÅN:");
            Console.WriteLine($"   Totalt antal lån: {_loans.Count}");
            Console.WriteLine($"   Aktiva lån: {activeLoans.Count}");
            Console.WriteLine($"   Försenade lån: {overdueLoans.Count}");

            Console.WriteLine($"\n🏆 MEST AKTIVA LÅNTAGARE:");
            foreach (var stats in topMembers)
            {
                Console.WriteLine($"   {stats.Member.Name}:");
                Console.WriteLine($"      - Totalt antal lån: {stats.TotalLoans}");
                Console.WriteLine($"      - Aktiva lån: {stats.ActiveLoans}");
            }
        }
    }

    public class MemberLoanStats
    {
        public Member Member { get; init; } = null!;
        public int TotalLoans { get; init; }
        public int ActiveLoans { get; init; }
    }
}