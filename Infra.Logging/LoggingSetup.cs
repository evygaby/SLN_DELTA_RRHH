using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Config;

namespace Infra.Logging
{
    public static class LoggingSetup
    {
        public static ILoggingBuilder AddCustomLogging(this ILoggingBuilder builder, string logDirectory = null)
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NLog.config");
            var config = new XmlLoggingConfiguration(configPath);

            // Asigna la variable de ruta si se pasa
            if (!string.IsNullOrWhiteSpace(logDirectory))
            {
                config.Variables["logDirectory"] = logDirectory;
            }

            LogManager.Configuration = config;
            LogManager.ReconfigExistingLoggers();

     

            return builder.AddNLog();
        }
    }
}
