using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using SQLProto.Storage;

namespace SQLProto.Schema
{
    public class Database
    {
        public static Dictionary<string, Database> AllDatabases = new Dictionary<string, Database>();
        public Dictionary<string, Table> Tables { get; set; } = new Dictionary<string, Table>();
        public string Name { get; set; }

        /// <summary>
        /// Deserialization
        /// </summary>
        private Database()
        {
        }

        private Database(string name)
        {
            this.Name = name;
        }

        public static Database Create(string name)
        {
            lock (AllDatabases)
            {
                var regex = new Regex("^[a-zA-Z_0-9]+$");
                if (!regex.IsMatch(name))
                    throw new Exception("BadName");
                if (AllDatabases.ContainsKey(name))
                    throw new Exception("Database already exist");

                var db = new Database(name);
                AllDatabases.Add(name, db);

                var dataDir = new DirectoryInfo(TableStorage.Directory);
                if (!dataDir.Exists)
                    dataDir.Create();

                dataDir.CreateSubdirectory(name);
                db.UpdateSchema();
                return db;
            }
        }

        public Table CreateTable(string name, IEnumerable<NamedType> columns)
        {
            lock (Tables)
            {
                var regex = new Regex("^[a-zA-Z_0-9]+$");
                if (!regex.IsMatch(name))
                    throw new Exception("BadName");
                if (Tables.ContainsKey(name))
                    throw new Exception("Table already exist");

                var dataDir = new DirectoryInfo(Path.Join(TableStorage.Directory, this.Name, name));
                if (!dataDir.Exists)
                    dataDir.Create();

                var table = new Table(this, name, columns);
                Tables.Add(name, table);
                UpdateSchema();
                return table;
            }
        }

        protected void UpdateSchema()
        {
            var dataDir = new DirectoryInfo(Path.Join(TableStorage.Directory, this.Name));
            if (!dataDir.Exists)
                dataDir.Create();
            var schemaFile = new FileInfo(Path.Join(dataDir.FullName, "schema.json"));
            var stream = new System.IO.StreamWriter(schemaFile.FullName);
            stream.Write(Newtonsoft.Json.JsonConvert.SerializeObject(this));
            stream.Close();
        }

        public static void Load()
        {
            var mainDir = new DirectoryInfo(TableStorage.Directory);
            var databases = mainDir
                .GetDirectories()
                .Select(x => new FileInfo(Path.Join(x.FullName, "schema.json")))
                .Where(x => x.Exists).Select(x =>
                {
                    var reader = new StreamReader(x.FullName);
                    var db = Newtonsoft.Json.JsonConvert.DeserializeObject<Database>(reader.ReadToEnd());
                    reader.Close();
                    return db;
                });
            AllDatabases = databases.ToDictionary(x => x.Name);
        }
        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            foreach (var (key,value) in Tables)
            {
                value.Database = this;
            }
        }
    }
}