using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SQLProto.Data;
using SQLProto.Schema;
using Decimal = SQLProto.Data.Decimal;

namespace SQLProto.Storage
{
    public class TableStorage
    {
        public const string Directory = "./dbdata";

        public TableStorage(Table table)
        {
            this.Stream = new System.IO.FileStream(Path.Join(Directory, table.Database.Name, table.Name, "data.bin"),
                FileMode.Open);
            this.LoadTask = Task.Run(() =>
            {
                lock (Stream)
                {
                    Data = new List<IValue[]>();
                    Stream.Position = 0;
                    var reader = new BinaryReader(Stream);
                    while (Stream.Position<Stream.Length)
                    {
                        var type = (ObjectTypes) reader.ReadByte();
                        var length = reader.ReadInt32();
                        if (type == ObjectTypes.Row)
                        {
                            var row = new IValue[table.Columns.Count()];
                            int i = 0;
                            foreach (var col in table.Columns)
                            {
                                switch (col.Type.Type)
                                {
                                    case DataType.Types.Decimal:
                                        row[i] = Decimal.Deserialize(Stream);
                                        break;
                                    case DataType.Types.Integer:
                                        row[i] = Integer.Deserialize(Stream);
                                        break;
                                    default:
                                        throw new Exception("bad format");
                                }

                                i++;
                            }
                            Data.Add(row);
                        }
                    }
                }
            });
        }

        public List<IValue[]> Data = null;

        public Task LoadTask { get; set; }

        public FileStream Stream { get; set; }

        public async Task Insert(IValue[] row)
        {
            await LoadTask;
            Data.Add(row);
            lock (Stream)
            {
                var mem = new MemoryStream();
                foreach (var x in row)
                {
                    x.Write(mem);
                }

                var writer = new BinaryWriter(Stream);
                writer.Write((byte) ObjectTypes.Row);
                writer.Write((int) mem.Position);
                mem.Position = 0;
                mem.CopyTo(Stream);
                Stream.Flush();
            }
        }

        enum ObjectTypes : byte
        {
            Empty = 0,
            Row = 1
        }

        public async Task<IEnumerable<IEnumerable<IValue>>> GetAllData()
        {
            await LoadTask;
            return Data;
        }
    }
}