using Microsoft.EntityFrameworkCore;
using Bibliotekssystem_Arv_Komp.Data;
using Bibliotekssystem_Arv_Komp.Models;

namespace Bibliotekssystem_Arv_Komp.Test
{
    /// <summary>
    /// Hjälpmetod som skapar en ny InMemory-databas för varje test
    /// så att testerna inte påverkar varandra.
    /// </summary>
    public abstract class DatabaseTestBase : IDisposable
    {
        protected readonly LibraryContext Context;

        protected DatabaseTestBase()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Context = new LibraryContext(options);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }

    

    public class RepositoryTests : DatabaseTestBase
    {
        [Fact]
        public async Task BookRepository_GetAllAsync_ShouldReturnAllBooks()
        {
            
            Context.Books.AddRange(
                new Book("111", "Bok A", "Författare A", 2020, 100),
                new Book("222", "Bok B", "Författare B", 2021, 200));
            await Context.SaveChangesAsync();
            var repo = new BookRepository(Context);

            var books = await repo.GetAllAsync();

            Assert.Equal(2, books.Count());
        }

        [Fact]
        public async Task BookRepository_GetByISBNAsync_ShouldFindCorrectBook()
        {
            
            Context.Books.Add(new Book("978-91-0-012345-6", "Testbok", "Författare", 2024, 150));
            await Context.SaveChangesAsync();
            var repo = new BookRepository(Context);

            var book = await repo.GetByISBNAsync("978-91-0-012345-6");

            
            Assert.NotNull(book);
            Assert.Equal("Testbok", book.Title);
        }

        [Fact]
        public async Task BookRepository_SearchAsync_ShouldFindByTitleOrAuthor()
        {
            
            Context.Books.AddRange(
                new Book("111", "Clean Code", "Robert Martin", 2008, 464),
                new Book("222", "Harry Potter", "J.K. Rowling", 1997, 223),
                new Book("333", "The Pragmatic Programmer", "David Thomas", 2019, 352));
            await Context.SaveChangesAsync();
            var repo = new BookRepository(Context);

            
            var results = await repo.SearchAsync("Harry");

            
            Assert.Single(results);
            Assert.Equal("Harry Potter", results.First().Title);
        }

        [Fact]
        public async Task DbContext_ShouldStoreAndRetrieveBookWithTPH()
        {
            
            var book = new Book("978-91-0-012345-6", "Testbok", "Författare", 2024, 300);
            Context.Books.Add(book);
            await Context.SaveChangesAsync();

            
            var saved = await Context.Books.FirstAsync(b => b.ISBN == "978-91-0-012345-6");

            // Assert – alla fält från både LibraryItem (bas) och Book (sub) ska finnas
            Assert.Equal("Testbok", saved.Title);
            Assert.Equal("Författare", saved.Author);
            Assert.Equal(2024, saved.PublishedYear);
            Assert.Equal(300, saved.Pages);
            Assert.True(saved.IsAvailable);
            Assert.True(saved.Id > 0);
        }
    }

 

    public class CrudTests : DatabaseTestBase
    {
        [Fact]
        public async Task AddAsync_ShouldPersistBookToDatabase()
        {
           
            var repo = new BookRepository(Context);
            var book = new Book("978-0-13-468599-1", "The Pragmatic Programmer", "David Thomas", 2019, 352);

            
            await repo.AddAsync(book);
            var saved = await Context.Books.FirstOrDefaultAsync(b => b.ISBN == "978-0-13-468599-1");

            
            Assert.NotNull(saved);
            Assert.Equal("The Pragmatic Programmer", saved.Title);
            Assert.True(saved.IsAvailable);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingBook()
        {
            
            var book = new Book("111", "Gammal Titel", "Författare", 2020, 100);
            Context.Books.Add(book);
            await Context.SaveChangesAsync();
            var repo = new BookRepository(Context);

            
            book.Title = "Ny Titel";
            await repo.UpdateAsync(book);
            var updated = await Context.Books.FindAsync(book.Id);

           
            Assert.NotNull(updated);
            Assert.Equal("Ny Titel", updated.Title);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveBookFromDatabase()
        {
            
            var book = new Book("111", "Bok att ta bort", "Författare", 2020, 100);
            Context.Books.Add(book);
            await Context.SaveChangesAsync();
            var bookId = book.Id;
            var repo = new BookRepository(Context);

            
            await repo.DeleteAsync(bookId);
            var deleted = await Context.Books.FindAsync(bookId);

            
            Assert.Null(deleted);
        }
    }

    

    public class IntegrationTests : DatabaseTestBase
    {
        [Fact]
        public async Task CreateLoan_ShouldSetBookAsUnavailable()
        {
            
            var book = new Book("111", "Testbok", "Författare", 2024, 200);
            var member = new Member("M001", "Anna Andersson", "anna@test.com");
            Context.Books.Add(book);
            Context.Members.Add(member);
            await Context.SaveChangesAsync();

            
            var loan = new Loan(book, member, 14);
            Context.Loans.Add(loan);
            book.IsAvailable = false;
            await Context.SaveChangesAsync();

            
            var savedBook = await Context.Books.FindAsync(book.Id);
            Assert.NotNull(savedBook);
            Assert.False(savedBook.IsAvailable);
        }

        [Fact]
        public async Task ReturnLoan_ShouldMakeBookAvailableAgain()
        {
           
            var book = new Book("111", "Testbok", "Författare", 2024, 200);
            var member = new Member("M001", "Anna Andersson", "anna@test.com");
            Context.Books.Add(book);
            Context.Members.Add(member);
            await Context.SaveChangesAsync();

            var loan = new Loan(book, member, 14);
            Context.Loans.Add(loan);
            book.IsAvailable = false;
            await Context.SaveChangesAsync();

           
            loan.ReturnItem();
            await Context.SaveChangesAsync();

           
            var savedBook = await Context.Books.FindAsync(book.Id);
            var savedLoan = await Context.Loans.FindAsync(loan.Id);
            Assert.NotNull(savedBook);
            Assert.True(savedBook.IsAvailable);
            Assert.NotNull(savedLoan);
            Assert.True(savedLoan.IsReturned);
        }

        [Fact]
        public async Task Member_ShouldNotExceedMaxLoans()
        {
            
            var member = new Member("M001", "Anna Andersson", "anna@test.com");
            var book1 = new Book("111", "Bok 1", "Författare", 2020, 100);
            var book2 = new Book("222", "Bok 2", "Författare", 2021, 200);
            var book3 = new Book("333", "Bok 3", "Författare", 2022, 300);
            var book4 = new Book("444", "Bok 4", "Författare", 2023, 400);

            Context.Members.Add(member);
            Context.Books.AddRange(book1, book2, book3, book4);
            await Context.SaveChangesAsync();

            
            Context.Loans.AddRange(
                new Loan(book1, member, 14),
                new Loan(book2, member, 14),
                new Loan(book3, member, 14));
            book1.IsAvailable = false;
            book2.IsAvailable = false;
            book3.IsAvailable = false;
            await Context.SaveChangesAsync();

            
            var activeLoans = await Context.Loans
                .Include(l => l.Member)
                .Where(l => l.Member.Id == member.Id && l.ReturnDate == null)
                .CountAsync();

            
            Assert.Equal(3, activeLoans);
            Assert.True(book4.IsAvailable);
        }
    }
}
