namespace Bibliotekssystem_Arv_Komp
{
    public class Loan
    {
        public LibraryItem Item { get; init; }
        public Member Member { get; init; }
        public DateTime LoanDate { get; init; }
        public DateTime DueDate { get; init; }
        public DateTime? ReturnDate { get; set; }
        public bool IsOverdue
        {
            get
            {
                if (ReturnDate.HasValue)
                {
                    return ReturnDate.Value > DueDate;
                }
                return DateTime.Now > DueDate;
            }
        }

        public bool IsReturned => ReturnDate.HasValue;

        public Loan(LibraryItem item, Member member, int loanDays = 14)
        {
            Item = item;
            Member = member;
            LoanDate = DateTime.Now;
            DueDate = LoanDate.AddDays(loanDays);
            ReturnDate = null;
        }

        public void ReturnItem()
        {
            ReturnDate = DateTime.Now;
            Item.IsAvailable = true;
        }

        public string GetLoanInfo()
        {
            string status = IsReturned ?
                $"Returnerad: {ReturnDate:yyyy-MM-dd}" :
                $"Förfaller: {DueDate:yyyy-MM-dd}" + (IsOverdue ? " (FÖRSENAD!)" : "");

            return $"{Item.GetItemType()}: {Item.Title}\n" +
                   $"Medlem: {Member.Name}\n" +
                   $"Lånedatum: {LoanDate:yyyy-MM-dd}\n" +
                   $"{status}";
        }
    }
}