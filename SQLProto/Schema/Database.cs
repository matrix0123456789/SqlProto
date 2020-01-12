using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using SQLProto.Storage;

namespace SQLProto.Schema
{
    public class Database
    {
        public static Dictionary<string, Database> AllDatabases = new Dictionary<string, Database>();
        public Dictionary<string, Table> Tables = new Dictionary<string, Table>();
        public string Name;

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
                return table;
            }
        }
    }
}