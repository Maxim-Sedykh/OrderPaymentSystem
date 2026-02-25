using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OrderPaymentSystem.Api.HealthChecks;
using OrderPaymentSystem.Api.Swagger;
using OrderPaymentSystem.Application.DependencyInjection;
using OrderPaymentSystem.Application.Settings;
using OrderPaymentSystem.Application.Validations.FluentValidations.Auth;
using OrderPaymentSystem.DAL.DependencyInjection;
using OrderPaymentSystem.Domain.Settings;
using Prometheus;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace OrderPaymentSystem.Api.Extensions;

/// <summary>
/// Здесь будет вся регистрация зависимостей (DI).
/// </summary>
public static class ServiceCollectionExtensions
{
    public static void AddApiInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.Configure<RedisSettings>(configuration.GetSection(nameof(RedisSettings)));
        services.Configure<AdminSettings>(configuration.GetSection(nameof(AdminSettings)));
        services.Configure<ElasticsearchSettings>(configuration.GetSection(ElasticsearchSettings.SectionName));

        services.AddEndpointsApiExplorer();
        services.AddControllers();

        services.UseHttpClientMetrics();

        services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
        services.AddFluentValidationAutoValidation();

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

    public static void AddHealthChecksConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();

        services.AddHealthChecks()
            .AddNpgSql(
                configuration.GetConnectionString("PostgresSQL")
                    ?? throw new InvalidOperationException("Postgres connection string not found"),
                name: "postgres",
                tags: ["db", "sql"])
            .AddRedis(
                redisConnectionString: configuration.GetSection(nameof(RedisSettings))[nameof(RedisSettings.Url)]
                    ?? throw new InvalidOperationException("Redis URL not found"),
                name: "redis",
                tags: ["cache"])
            .AddCheck<CustomElasticsearchHealthCheck>(
                "elasticsearch-custom-url",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["logging", "search"]
            );
    }
}