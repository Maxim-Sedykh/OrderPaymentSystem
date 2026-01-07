using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OrderPaymentSystem.Api.Controllers.Abstract;

/// <summary>
/// Базовый контроллер для контроллеров, которым нужны данные о текущем пользователе
/// </summary>
public abstract class PrincipalAccessController : ControllerBase
{
    /// <summary>
    /// Идентификатор авторизованного пользователя
    /// </summary>
    protected Guid AuthorizedUserId => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
        ? userId
        : Guid.Empty;
}
