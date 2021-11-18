using Microsoft.Data.Sqlite;

namespace DatabaseManager
{
    public class DbSql
    {
        private string _dataSourceString = String.Empty;
        SqliteConnection _sqlConnection;
        public DbSql(string DataSourceString)
        {
            _dataSourceString = DataSourceString;
            _sqlConnection = new SqliteConnection(_dataSourceString);
        }

        public void Connect()
        {
            if(_sqlConnection is not null)
            {
                _sqlConnection.Open();
            }
        }

        public string GetVerson()
        {
            var command = _sqlConnection.CreateCommand();
            command.CommandText = @"select sqlite_version()";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader.GetString(0);

                    return ($"SQLite version: {name}!");
                }
            }

            return String.Empty;
        }

        //TODO: Return table list
        public string GetTablesName()
        {
            var command = _sqlConnection.CreateCommand();
            command.CommandText = @"select distinct t.name as tbl_name
                                    from sqlite_master AS t
                                    where t.type = 'table'; ";

            string tablesName = string.Empty;
            using var reader = command.ExecuteReader();
            while (reader.HasRows)
            {
                while (reader.Read())
                    tablesName += reader.GetString(0) + Environment.NewLine;

                reader.NextResult();
            }
            return tablesName;
        }

        //TODO
        public string GetColumnsName(string tableName)
        {
            return string.Empty;
        }

        //TODO: Test
        public SqliteDataReader ExecuteQuery(string queryString)
        {
            var command = _sqlConnection.CreateCommand();
            command.CommandText = queryString;
            return command.ExecuteReader();
        }

        //TODO: Universal columns show
        public string ShowQuery(string queryString)
        {
            if (queryString is null) return string.Empty;

            using SqliteDataReader reader = ExecuteQuery(queryString);
            string tablesName = string.Empty;
            while (reader.HasRows)
            {
                while (reader.Read())
                    tablesName += reader.GetString(0) + Environment.NewLine;
                reader.NextResult();
            }
            return tablesName;

        }

        public void CreateMigration()
        {
            //TODO: Migration from file
            string createAnnouncementTable = @"create table  announcement (
                                               id integer primary key,
                                               describe text not null);";

            string createImagesTable = @"create table images (
                                         id integer primaty key, 
                                         announcement_id id integer, 
                                         url text);";


            ExecuteQuery(createAnnouncementTable);
            ExecuteQuery(createImagesTable);
        }

        public void Close()
        {
            if(_sqlConnection is not null )
            {
                _sqlConnection.Close();
            }
        }
    }
}
