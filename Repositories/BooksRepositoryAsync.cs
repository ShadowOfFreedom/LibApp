using LibApp.Data;
using LibApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibApp.Repositories {
    public class BooksRepositoryAsync : BaseRepositoryAsync<Book> {

        public BooksRepositoryAsync(ApplicationDbContext context) : base(context) => _entities = context.Books;

        public override async Task<List<Book>> GetAll() =>
            await _entities.Include(b => b.Genre).ToListAsync();

        public override async Task<Book> GetById(int id) =>
            await _entities.Include(b => b.Genre).FirstOrDefaultAsync(b => b.Id == id);
    }
}
