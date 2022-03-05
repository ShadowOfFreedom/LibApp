using LibApp.Data;
using LibApp.Models;

namespace LibApp.Repositories {
    public class MembershipTypesRepositoryAsync : BaseRepositoryAsync<MembershipType> {
        public MembershipTypesRepositoryAsync(ApplicationDbContext context) : base(context) => _entities = context.MembershipTypes;
    }
}
