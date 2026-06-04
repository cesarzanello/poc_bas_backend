using Microsoft.Extensions.Configuration;
using NetEscapades.Configuration.Yaml;

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
            builder.Configuration.AddYamlFile(fileName, optional, reloadOnChange);
            return builder;
        }
    }
}
