using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OrderPaymentSystem.Api.Swagger;
using OrderPaymentSystem.Application.Settings;
using OrderPaymentSystem.Application.Validations.FluentValidations.Auth;
using OrderPaymentSystem.Domain.Settings;
using Prometheus;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using OrderPaymentSystem.DAL.DependencyInjection;
using OrderPaymentSystem.Application.DependencyInjection;

namespace OrderPaymentSystem.Api.Extensions;

/// <summary>
/// Здесь будет вся регистрация зависимостей (DI).
/// </summary>
public static class ServiceCollectionExtensions
{
    public static void AddApiInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Settings
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.Configure<RedisSettings>(configuration.GetSection(nameof(RedisSettings)));

        // Common API
        services.AddEndpointsApiExplorer();
        services.AddControllers();

        // Metrics
        services.UseHttpClientMetrics();

        // Validations
        services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
        services.AddFluentValidationAutoValidation();

        // Custom Layers
        services.AddDataAccessLayer(configuration);
        services.AddApplication();
    }

    public static void AddAuthConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()
                             ?? throw new InvalidOperationException("JwtSettings not found");

            o.Authority = jwtSettings.Authority;
            o.RequireHttpsMetadata = false;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.JwtKey)),
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
    }

    public static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = []
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);
        });
    }
}
