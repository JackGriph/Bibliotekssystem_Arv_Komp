using Microsoft.EntityFrameworkCore;
using Bibliotekssystem_Arv_Komp.Models;

namespace Bibliotekssystem_Arv_Komp.Data
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryContext _context;

        public BookRepository(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<Book?> GetByISBNAsync(string isbn)
        {
            return await _context.Books
                .FirstOrDefaultAsync(b => b.ISBN == isbn);
        }

        public async Task AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Book>> SearchAsync(string searchTerm)
        {
            return await _context.Books
                .Where(b => b.Title.Contains(searchTerm) ||
                            b.Author.Contains(searchTerm) ||
                            b.ISBN.Contains(searchTerm))
                .ToListAsync();
        }
    }
}
