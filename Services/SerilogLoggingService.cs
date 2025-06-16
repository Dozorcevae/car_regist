using System;
using Serilog;

namespace CarManagment.Services
{
    public class SerilogLoggingService : ILoggingService
    {
        private readonly Serilog.ILogger _logger;

        public SerilogLoggingService(Serilog.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.Information(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.Warning(message, args);
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            _logger.Error(ex, message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }
    }
} 