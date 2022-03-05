using LibApp.Data;
using LibApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibApp.Repositories {
    public class RentalsRepositoryAsync : BaseRepositoryAsync<Rental> {
        public RentalsRepositoryAsync(ApplicationDbContext context) :
            base(context) => _entities = context.Rentals;

        public override async Task<List<Rental>> GetAll() =>
            await _entities.Include(r => r.Book).Include(r => r.Customer).ToListAsync();

        public override async Task<Rental> GetById(int id) =>
            await _entities.Include(r => r.Book).Include(r => r.Customer).FirstOrDefaultAsync(r => r.Id == id);
    }
}
