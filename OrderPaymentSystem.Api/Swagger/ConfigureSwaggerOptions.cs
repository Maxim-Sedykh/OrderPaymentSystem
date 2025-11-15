using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OrderPaymentSystem.Api.Swagger
{
    /// <summary>
    /// Настройки конфигурации Swagger
    /// </summary>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        /// <summary>
        /// Конструктор настроек
        /// </summary>
        /// <param name="provider">Провайдер API описания</param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Выполнить конфигурацию Swagger
        /// </summary>
        /// <param name="options">Настройки Swagger gen</param>
        public void Configure(SwaggerGenOptions options)
        {
            var contactUri = new Uri("https://vk.com/maximsedykh2000");

            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo
                    {
                        Version = description.ApiVersion.ToString(),
                        Title = "OrderPaymentSystem.API",
                        Description = $"This is description of version {description.ApiVersion}",
                        TermsOfService = contactUri,
                        Contact = new OpenApiContact
                        {
                            Name = "Test contact",
                            Url = contactUri
                        }
                    });
            }
        }
    }
}
