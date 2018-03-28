using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMaker
{
    interface IConverter
    {
        string Insert(string table,object obj);
        string Insert(string table,IEnumerable<object> obj);
        string InsertDictionary(Type type);
        string CreateTable(string name, Type obj, string primaryKey, List<Foreign> foreign);
        string CreateDictionary(string name);
        string CreateFK(Foreign foreign);
        string MapType(Type type);
    }

    public class Foreign
    {
        public string Key;
        public string ForeignKey;
        public string DataTable;
    }
}
