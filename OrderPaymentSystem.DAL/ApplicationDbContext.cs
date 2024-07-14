using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.DAL.Interceptors;
using System.Reflection;

namespace OrderPaymentSystem.DAL
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            optionsBuilder.AddInterceptors(new AuditInterceptor(httpContextAccessor));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
