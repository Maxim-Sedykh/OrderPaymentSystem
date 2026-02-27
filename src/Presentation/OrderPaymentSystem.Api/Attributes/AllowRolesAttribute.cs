using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace OrderPaymentSystem.Api.Attributes;

/// <summary>
/// Атрибут, для того чтобы передавать массив ролей
/// Если пользователь имеет хоть одну роль из списка, то валидация проходит
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowRolesAttribute : Attribute, IAuthorizationFilter
{
    private readonly HashSet<string> _roles;

    /// <summary>
    /// Конструктор атрибута
    /// </summary>
    /// <param name="roles">Список ролей к которым надо проверить доступ у пользователя</param>
    public AllowRolesAttribute(params string[] roles) => _roles = [.. roles];

    /// <inheritdoc/>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        bool authorized = user.Claims.Any(c => c.Type == ClaimTypes.Role 
            && _roles.Contains(c.Value, StringComparer.OrdinalIgnoreCase));

        if (!authorized)
        {
            context.Result = new ForbidResult();
        }
    }
}
