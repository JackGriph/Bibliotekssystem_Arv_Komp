namespace Bibliotekssystem_Arv_Komp
{
    public class Member : ISearchable
    {
        public string MemberId { get; init; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime MemberSince { get; init; }

        private List<Loan> _loans;
        public IReadOnlyList<Loan> Loans => _loans.AsReadOnly();

        public Member(string memberId, string name, string email)
        {
            MemberId = memberId;
            Name = name;
            Email = email;
            MemberSince = DateTime.Now;
            _loans = new List<Loan>();
        }

        public void AddLoan(Loan loan)
        {
            _loans.Add(loan);
        }

        public string GetMemberInfo()
        {
            int activeLoans = _loans.Count(l => !l.IsReturned);
            return $"Medlems-ID: {MemberId}\n" +
                   $"Namn: {Name}\n" +
                   $"E-post: {Email}\n" +
                   $"Medlem sedan: {MemberSince:yyyy-MM-dd}\n" +
                   $"Aktiva lån: {activeLoans}";
        }

        public bool Matches(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return false;

            return MemberId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                   Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                   Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        }
    }
}