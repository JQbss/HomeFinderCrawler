using System.Text;

namespace RequestsServices
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly string _logFilePath = string.Empty;
        private bool _loggingEnabled = false;
        private StringBuilder _buffer = new();

        public ErrorLogService(string lofFilePath) => _logFilePath = lofFilePath;
        public void StartLog() => _loggingEnabled = true;
        public void StopLog() => _loggingEnabled = false;

        public void AddLog(string logMessage)
        {
            if (_loggingEnabled && _logFilePath != string.Empty)
            {
                _buffer.Append(DateTime.Now + ": " + logMessage + Environment.NewLine);
                File.AppendAllText(_logFilePath,_buffer.ToString());
                _buffer.Clear();
            }
        }
    }
}
