using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OrderPaymentSystem.Domain.Interfaces.Entities;
using System.Security.Claims;

namespace OrderPaymentSystem.DAL.Interceptors
{
    public class AuditInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var userId = GetUserId();

            var dbContext = eventData.Context;
            if (dbContext == null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            var entries = dbContext.ChangeTracker.Entries<IAuditable>()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            DateTime currentTimeUtc = DateTime.UtcNow;

            foreach ( var entry in entries )
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property(x => x.CreatedAt).CurrentValue = currentTimeUtc;
                    entry.Property(x => x.CreatedBy).CurrentValue = userId;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property(x => x.UpdatedAt).CurrentValue = currentTimeUtc;
                    entry.Property(x => x.UpdatedBy).CurrentValue = userId;

                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private long GetUserId()
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
                {
                    return userId;
                }
            }

            return 0; // В поле UpdatedBy и CreatedBy, если пользователь не авторизован, ставится 0 - значение по умолчанию
        }
    }
}
