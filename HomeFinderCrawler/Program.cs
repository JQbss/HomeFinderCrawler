using DatabaseManager;

namespace HomeFinderCrawler // Note: actual namespace depends on the project name.
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            DbSql database = new DbSql("Data Source=:memory:");
            database.Connect();
            database.CreateMigration();
            Console.WriteLine(database.GetVerson());
            Console.WriteLine(database.GetTablesName());

        }
    }
}
