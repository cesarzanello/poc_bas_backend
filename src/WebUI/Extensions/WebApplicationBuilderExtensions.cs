using Microsoft.Extensions.Configuration;

namespace WebUI.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddYamlConfiguration(
           this WebApplicationBuilder builder,
           string fileName = "appsettings.yml",
           bool optional = false,
           bool reloadOnChange = true)
        {
            builder.Configuration.AddXmlFile(fileName, optional, reloadOnChange);
            return builder;
        }
    }
}
