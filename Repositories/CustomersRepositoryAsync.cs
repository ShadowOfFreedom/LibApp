using LibApp.Data;
using LibApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibApp.Repositories {
    public class CustomersRepositoryAsync : BaseRepositoryAsync<Customer> {

        public CustomersRepositoryAsync(ApplicationDbContext context) : base(context) => _entities = context.Customers;

        public override async Task<List<Customer>> GetAll() =>
            await _entities.Include(c => c.MembershipType).ToListAsync();

        public override async Task<Customer> GetById(int id) =>
            await _entities.Include(c => c.MembershipType).FirstOrDefaultAsync(c => c.Id == id);
    }
}
