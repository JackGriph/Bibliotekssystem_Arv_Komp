namespace Bibliotekssystem_Arv_Komp.Models
{
    public class Magazine : LibraryItem 
    {
        public string ISSN { get; init; }
        public int IssueNumber { get; set; }
        public string Publisher { get; set; }
        public int Month { get; set; }

        public Magazine(string issn, string title, int issueNumber, string publisher, int publishedYear, int month)
            : base(issn, title, publishedYear)
        {
            ISSN = issn;
            IssueNumber = issueNumber;
            Publisher = publisher;
            Month = month;
        }

        public override string GetInfo()
        {
            string availability = IsAvailable ? "Tillgänglig" : "Utlånad";
            return $"[TIDSKRIFT]\n" +
                   $"ISSN: {ISSN}\n" +
                   $"Titel: {Title}\n" +
                   $"Nummer: {IssueNumber}\n" +
                   $"Utgivare: {Publisher}\n" +
                   $"Utgåva: {Month}/{PublishedYear}\n" +
                   $"Status: {availability}";
        }

        public override string GetItemType()
        {
            return "Tidskrift";
        }

        public override bool Matches(string searchTerm)
        {
            if (base.Matches(searchTerm))
                return true;

            return Publisher.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                   IssueNumber.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        }
    }
}