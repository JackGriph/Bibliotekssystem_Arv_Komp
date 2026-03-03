using Bibliotekssystem_Arv_Komp.Models;

namespace Bibliotekssystem_Arv_Komp.Data.Context;

public static class DbSeeder
{
    public static void Seed(LibraryContext context)
    {
        context.Database.EnsureCreated();

        var addedAny = false;

        // Seed books: add any missing books (idempotent by ISBN)
        var seedBooks = new List<Book>
        {
            new Book("978-0-13-468599-1", "The Pragmatic Programmer", "David Thomas", 2019, 352),
            new Book("978-0-596-51774-8", "JavaScript: The Good Parts", "Douglas Crockford", 2008, 176),
            new Book("978-0-13-235088-4", "Clean Code", "Robert C. Martin", 2008, 464),
            new Book("978-91-29-72348-2", "Mio, min Mio", "Astrid Lindgren", 1954, 198),
            new Book("978-91-29-66634-5", "Bröderna Lejonhjärta", "Astrid Lindgren", 1973, 256),
            new Book("978-0-201-63361-0", "Design Patterns: Elements of Reusable Object-Oriented Software", "Erich Gamma", 1994, 395),
            new Book("978-0-201-48567-9", "Refactoring: Improving the Design of Existing Code", "Martin Fowler", 1999, 448),
            new Book("978-0-262-03384-8", "Introduction to Algorithms", "Thomas H. Cormen", 2009, 1312),
            new Book("978-0-596-00797-3", "Head First Design Patterns", "Eric Freeman & Elisabeth Robson", 2004, 694),
            new Book("978-0-321-35668-0", "Structure and Interpretation of Computer Programs", "Harold Abelson & Gerald Jay Sussman", 1996, 657),
            new Book("978-1-4919-1889-0", "You Don't Know JS: Up & Going", "Kyle Simpson", 2015, 88),
            new Book("978-0-201-53082-7", "The Mythical Man-Month", "Frederick P. Brooks Jr.", 1975, 322),
            new Book("978-0-452-28423-4", "1984", "George Orwell", 1949, 328),
            new Book("978-0-14-044913-6", "Crime and Punishment", "Fyodor Dostoevsky", 1866, 671)
        };

        foreach (var book in seedBooks)
        {
            if (!context.Books.Any(x => x.ISBN == book.ISBN))
            {
                context.Books.Add(book);
                addedAny = true;
            }
        }

        // Seed members: only if none exist
        if (!context.Members.Any())
        {
            context.Members.AddRange(
                new Member("M001", "Anna Andersson", "anna@example.com"),
                new Member("M002", "Erik Eriksson", "erik@example.com"),
                new Member("M003", "Maria Johansson", "maria@example.com")
            );
            addedAny = true;
        }

        if (addedAny)
        {
            context.SaveChanges();
        }
    }
}
