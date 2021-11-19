using DatabaseManager;

namespace HomeFinderCrawler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DatabaseService db = new(new DataContext("DataSource=file::memory:?cache=shared"));
            db.Test();
        }
    }
}
