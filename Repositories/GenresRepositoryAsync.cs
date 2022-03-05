using LibApp.Data;
using LibApp.Models;

namespace LibApp.Repositories {
    public class GenresRepositoryAsync : BaseRepositoryAsync<Genre> {
        public GenresRepositoryAsync(ApplicationDbContext context) : base(context) => _entities = context.Genre;
    }
}
