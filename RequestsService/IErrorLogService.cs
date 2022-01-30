namespace RequestsServices
{
    public interface IErrorLogService
    {
        void AddLog(string logMessage);
        void StartLog();
        void StopLog();
    }
}