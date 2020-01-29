namespace SQLProto.Parser.Queries
{
    public class CreateDatabase: IQuery
    {
        public string Name { get; set; }

        public CreateDatabase(string name)
        {
            Name = name;
        }
    }
}