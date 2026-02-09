using System.Text;

namespace Bibliotekssystem_Arv_Komp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Fixa svenska tecken i konsolen
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Console.WriteLine("=================================================");
            Console.WriteLine("    BIBLIOTEKSSYSTEM - FULLSTÄNDIG DEMO");
            Console.WriteLine("=================================================\n");

            // Skapa managers
            var catalog = new ItemCatalog();
            var loanManager = new LoanManager();
            var members = new List<Member>();

            // Lägg till olika typer av objekt (polymorfism)
            var book1 = new Book("978-0-7475-3269-9", "Harry Potter och De vises sten", "J.K. Rowling", 1997, 223);
            var book2 = new Book("978-91-0-012825-0", "Pippi Langstrump", "Astrid Lindgren", 1945, 158);
            var book3 = new Book("978-0-261-10238-4", "Sagan om ringen", "J.R.R. Tolkien", 1954, 423);
            var book4 = new Book("978-0-7475-4215-5", "Harry Potter och Hemligheternas kammare", "J.K. Rowling", 1998, 251);
                
            var magazine1 = new Magazine("1234-5678", "National Geographic", 156, "National Geographic Society", 2024, 3);
            var magazine2 = new Magazine("8765-4321", "Science Illustrated", 89, "Bonnier", 2024, 2);

            var dvd1 = new DVD("DVD001", "Inception", "Christopher Nolan", 2010, 148, "Sci-Fi");
            dvd1.AddActor("Leonardo DiCaprio");
            dvd1.AddActor("Ellen Page");

            var dvd2 = new DVD("DVD002", "The Matrix", "The Wachowskis", 1999, 136, "Sci-Fi");
            dvd2.AddActor("Keanu Reeves");
            dvd2.AddActor("Laurence Fishburne");

            // Lägg till i katalogen
            catalog.AddItem(book1);
            catalog.AddItem(book2);
            catalog.AddItem(book3);
            catalog.AddItem(book4);
            catalog.AddItem(magazine1);
            catalog.AddItem(magazine2);
            catalog.AddItem(dvd1);
            catalog.AddItem(dvd2);

            // Lägg till medlemmar
            var member1 = new Member("M001", "Anna Andersson", "anna@example.com");
            var member2 = new Member("M002", "Bengt Bengtsson", "bengt@example.com");
            var member3 = new Member("M003", "Cecilia Carlsson", "cecilia@example.com");
            members.Add(member1);
            members.Add(member2);
            members.Add(member3);

            Console.WriteLine($"Registrerade {members.Count} medlemmar\n");

            // === DEL 4: DEMONSTRATION AV ALGORITMER ===

            Console.WriteLine(new string('=', 50));
            Console.WriteLine("    SÖKFUNKTIONER");
            Console.WriteLine(new string('=', 50));

            // 1. Sök på författare
            Console.WriteLine("\n[1] Sök böcker av Rowling:");
            var rowlingBooks = catalog.SearchByAuthor("Rowling");
            foreach (var book in rowlingBooks)
            {
                Console.WriteLine($"   - {book.Title} ({book.PublishedYear})");
            }

            // 2. Sök på titel
            Console.WriteLine("\n[2] Sök på titel 'Harry':");
            var harryBooks = catalog.SearchByTitle("Harry");
            foreach (var item in harryBooks)
            {
                Console.WriteLine($"   - {item.Title}");
            }

            // 3. Sök på ISBN
            Console.WriteLine("\n[3] Sök på ISBN '978-91-0-012825-0':");
            var pippi = catalog.SearchByISBN("978-91-0-012825-0");
            if (pippi != null)
                Console.WriteLine($"   > Hittade: {pippi.Title} av {pippi.Author}");

            // 4. Allmän sökning (ISearchable)
            Console.WriteLine("\n[4] Allmän sökning på '1999':");
            catalog.DisplaySearchResults("1999");

            // 5. Polymorfisk sökning i medlemmar
            Console.WriteLine("\n[5] Sök medlem 'Anna':");
            var memberResults = members.Where(m => m.Matches("Anna")).ToList();
            Console.WriteLine($"   > Hittade {memberResults.Count} medlem(mar)");
            foreach (var m in memberResults)
            {
                Console.WriteLine($"   - {m.Name} ({m.Email})");
            }

            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("    SORTERINGSFUNKTIONER");
            Console.WriteLine(new string('=', 50));

            // 1. Sortera alfabetiskt
            catalog.DisplaySortedByTitle();

            // 2. Sortera efter år (äldst först)
            catalog.DisplaySortedByYear(ascending: true);

            // 3. Sortera efter år (nyast först)
            catalog.DisplaySortedByYear(ascending: false);

            // 4. Sortera böcker efter författare
            Console.WriteLine("\n=== Böcker sorterade efter författare ===");
            var booksByAuthor = catalog.GetBooksSortedByAuthor();
            foreach (var book in booksByAuthor)
            {
                Console.WriteLine($"- {book.Author}: {book.Title}");
            }

            // === SKAPA LÅNEAKTIVITET FÖR STATISTIK ===

            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("SKAPAR LÅNEAKTIVITET...");
            Console.WriteLine(new string('=', 50) + "\n");

            // Anna lånar 3 objekt
            loanManager.LoanItem(book1, member1);
            loanManager.LoanItem(book2, member1);
            loanManager.LoanItem(dvd1, member1);

            // Bengt lånar 2 objekt
            loanManager.LoanItem(book4, member2);
            loanManager.LoanItem(magazine1, member2);

            // Cecilia lånar 1 bok
            loanManager.LoanItem(book3, member3);

            // Anna returnerar en bok
            Console.WriteLine();
            loanManager.ReturnItem("978-91-0-012825-0", "M001");

            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("STATISTIK");
            Console.WriteLine(new string('=', 50));

            // Visa katalogstatistik
            catalog.DisplayStatistics();

            // Visa lånestatistik (inkl. mest aktiva låntagare)
            loanManager.DisplayStatistics(members);

            // Visa aktiva lån
            loanManager.DisplayActiveLoans();

            Console.WriteLine("\n\nDemonstration klar!");
          
        }
    }
}