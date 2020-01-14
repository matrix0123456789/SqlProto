namespace SQLProto.Parser.Queries
{
    public class SourceTable
    {
        public string DatabaseName;
        public string TableName;

        public SourceTable(string name, string database = null)
        {
            TableName = name;
            DatabaseName = database;
        }
    }
}