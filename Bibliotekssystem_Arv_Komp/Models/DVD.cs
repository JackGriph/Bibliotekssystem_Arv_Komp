namespace Bibliotekssystem_Arv_Komp.Models
{
    public class DVD : LibraryItem
    {
        public string Director { get; set; }
        public int Duration { get; set; }
        public string Genre { get; set; }
        public List<string> Actors { get; set; }

        private DVD() : base() { Actors = new List<string>(); }

        public DVD(string itemId, string title, string director, int publishedYear, int duration, string genre)
            : base(itemId, title, publishedYear)
        {
            Director = director;
            Duration = duration;
            Genre = genre;
            Actors = new List<string>();
        }

        public override string GetInfo()
        {
            string availability = IsAvailable ? "Tillgänglig" : "Utlånad";
            string actors = Actors.Any() ? string.Join(", ", Actors) : "Inga registrerade";

            return $"[DVD]\n" +
                   $"ID: {ItemId}\n" +
                   $"Titel: {Title}\n" +
                   $"Regissör: {Director}\n" +
                   $"Genre: {Genre}\n" +
                   $"Längd: {Duration} minuter\n" +
                   $"År: {PublishedYear}\n" +
                   $"Skådespelare: {actors}\n" +
                   $"Status: {availability}";
        }

        public override string GetItemType()
        {
            return "DVD";
        }

        public void AddActor(string actor)
        {
            Actors.Add(actor);
        }

        public override bool Matches(string searchTerm)
        {
            if (base.Matches(searchTerm))
                return true;

            return Director.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                   Genre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                   Actors.Any(a => a.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }
    }
}