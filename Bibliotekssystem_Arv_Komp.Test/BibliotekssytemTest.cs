using Bibliotekssystem_Arv_Komp.Models;
using Bibliotekssystem_Arv_Komp.Services;

namespace Bibliotekssystem_Arv_Komp.Test
{
    public class BibliotekssytemTest
    {
        // === Book-klassen: 3 tester ===

        public class BookTests
        {
            [Fact]
            public void Constructor_ShouldSetPropertiesCorrectly()
            {
                // Arrange & Act
                var book = new Book("978-91-0-012345-6", "Testbok", "Testförfattare", 2024, 300);

                // Assert
                Assert.Equal("978-91-0-012345-6", book.ISBN);
                Assert.Equal("Testbok", book.Title);
                Assert.Equal("Testförfattare", book.Author);
                Assert.Equal(2024, book.PublishedYear);
                Assert.Equal(300, book.Pages);
                Assert.True(book.IsAvailable);
            }

            [Fact]
            public void IsAvailable_ShouldBeTrueForNewBook()
            {
                // Arrange & Act
                var book = new Book("978-91-0-012345-6", "Testbok", "Testförfattare", 2024);

                // Assert
                Assert.True(book.IsAvailable);
            }

            [Fact]
            public void GetInfo_ShouldReturnFormattedString()
            {
                // Arrange
                var book = new Book("978-91-0-012345-6", "Testbok", "Testförfattare", 2024, 200);

                // Act
                var info = book.GetInfo();

                // Assert
                Assert.Contains("[BOK]", info);
                Assert.Contains("ISBN: 978-91-0-012345-6", info);
                Assert.Contains("Titel: Testbok", info);
                Assert.Contains("Författare: Testförfattare", info);
                Assert.Contains("Sidor: 200", info);
                Assert.Contains("Tillgänglig", info);
            }
        }

        // === Loan-klassen: 3 tester ===

        public class LoanTests
        {
            [Fact]
            public void IsOverdue_ShouldReturnFalse_WhenDueDateIsInFuture()
            {
                // Arrange
                var book = new Book("123", "Test", "Author", 2024);
                var member = new Member("M001", "Test Person", "test@test.com");
                var loan = new Loan(book, member, 14);

                // Act & Assert
                Assert.False(loan.IsOverdue);
            }

            [Fact]
            public void IsOverdue_ShouldReturnTrue_WhenDueDateHasPassed()
            {
                // Arrange — skapa lån med 0 dagars lånetid så det förfaller direkt
                var book = new Book("123", "Test", "Author", 2024);
                var member = new Member("M001", "Test Person", "test@test.com");
                var loan = new Loan(book, member, -1);

                // Act & Assert
                Assert.True(loan.IsOverdue);
            }

            [Fact]
            public void ReturnItem_ShouldSetReturnDateAndMakeItemAvailable()
            {
                // Arrange
                var book = new Book("123", "Test", "Author", 2024);
                var member = new Member("M001", "Test Person", "test@test.com");
                var loan = new Loan(book, member, 14);
                book.IsAvailable = false;

                // Act
                loan.ReturnItem();

                // Assert
                Assert.True(loan.IsReturned);
                Assert.NotNull(loan.ReturnDate);
                Assert.True(book.IsAvailable);
            }
        }

        // === Sökning (ISearchable): 2 tester ===

        public class SearchTests
        {
            [Theory]
            [InlineData("Tolkien", true)]
            [InlineData("tolkien", true)]
            [InlineData("Rowling", false)]
            public void Book_Matches_ShouldFindByAuthor(string searchTerm, bool expected)
            {
                // Arrange
                var book = new Book("123", "Sagan om ringen", "J.R.R. Tolkien", 1954);

                // Act
                var result = book.Matches(searchTerm);

                // Assert
                Assert.Equal(expected, result);
            }

            [Fact]
            public void Catalog_Search_ShouldReturnMatchingItems()
            {
                // Arrange
                var catalog = new ItemCatalog();
                var book1 = new Book("111", "Harry Potter", "J.K. Rowling", 1997);
                var book2 = new Book("222", "Sagan om ringen", "J.R.R. Tolkien", 1954);
                var dvd = new DVD("DVD001", "Inception", "Christopher Nolan", 2010, 148, "Sci-Fi");
                catalog.AddItem(book1);
                catalog.AddItem(book2);
                catalog.AddItem(dvd);

                // Act
                var results = catalog.Search("Harry");

                // Assert
                Assert.Single(results);
                Assert.Equal("Harry Potter", results[0].Title);
            }
        }

        // === Statistik/algoritmer: 2 tester ===

        public class LibraryStatisticsTests
        {
            [Fact]
            public void GetStatistics_ShouldReturnCorrectCounts()
            {
                // Arrange
                var catalog = new ItemCatalog();
                catalog.AddItem(new Book("111", "Bok 1", "Författare 1", 2020));
                catalog.AddItem(new Book("222", "Bok 2", "Författare 2", 2021));
                catalog.AddItem(new Magazine("333", "Tidskrift 1", 1, "Utgivare", 2022, 5));
                catalog.AddItem(new DVD("DVD001", "Film 1", "Regissör", 2023, 120, "Drama"));

                // Act
                var stats = catalog.GetStatistics();

                // Assert
                Assert.Equal(4, stats.TotalItems);
                Assert.Equal(2, stats.TotalBooks);
                Assert.Equal(1, stats.TotalMagazines);
                Assert.Equal(1, stats.TotalDVDs);
                Assert.Equal(4, stats.AvailableItems);
                Assert.Equal(0, stats.LoanedItems);
            }

            [Fact]
            public void GetMostActiveMembers_ShouldReturnMemberWithMostLoans()
            {
                // Arrange
                var loanManager = new LoanManager(maxLoansPerMember: 5);
                var member1 = new Member("M001", "Anna", "anna@test.com");
                var member2 = new Member("M002", "Bengt", "bengt@test.com");
                var members = new List<Member> { member1, member2 };

                var book1 = new Book("111", "Bok 1", "Författare", 2020);
                var book2 = new Book("222", "Bok 2", "Författare", 2021);
                var book3 = new Book("333", "Bok 3", "Författare", 2022);

                loanManager.LoanItem(book1, member1);
                loanManager.LoanItem(book2, member1);
                loanManager.LoanItem(book3, member2);

                // Act
                var topMembers = loanManager.GetMostActiveMembers(members, 1);

                // Assert
                Assert.Single(topMembers);
                Assert.Equal("Anna", topMembers[0].Member.Name);
                Assert.Equal(2, topMembers[0].TotalLoans);
            }
        }
    }
}
