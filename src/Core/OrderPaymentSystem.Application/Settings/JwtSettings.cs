namespace OrderPaymentSystem.Application.Settings;

public class JwtSettings
{
    public string Issuer {  get; set; }

    public string Audience { get; set; }

    public string Authority { get; set; }

    public string JwtKey { get; set; }

    public string Lifetime { get; set; }

    public int RefreshTokenValidityInDays { get; set; }
    public int AccessTokenValidityInMinutes { get; set; }
}
