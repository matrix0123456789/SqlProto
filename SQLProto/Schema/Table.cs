using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLProto.Data;
using SQLProto.Storage;

namespace SQLProto.Schema
{
    public class Table
    {
        /// <summary>
        /// deserialization
        /// </summary>
        private Table()
        {
        }

        public Table(Database database, string name, IEnumerable<NamedType> columns)
        {
            this.Database = database;
            this.Name = name;
            this.Columns = columns;
        }

        public IEnumerable<NamedType> Columns { get; set; }

        public string Name { get; set; }
        [JsonIgnore] public Database Database { get; set; }
        [JsonIgnore] public TableStorage Storage { get; set; }

        void InitStorage()
        {
            if (Storage == null)
                Storage = new TableStorage(this);
        }

        public async Task Insert(IValue[] values)
        {
            InitStorage();
            await Storage.Insert(values);
        }

        public async Task<List<IValue[]>> GetAllData()
        {
            InitStorage();
            return await Storage.GetAllData();
        }
    }
}