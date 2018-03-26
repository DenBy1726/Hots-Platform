using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMaker
{
    interface IConverter
    {
        string Insert(object obj);
        string CreateTable(string name, Type obj, string primaryKey, List<Foreign> foreign);
        string CreateDictionary(string name);
        string CreateFK(Foreign foreign);
        string MapType(Type type);
    }

    public enum ForeignType
    {
        OneToOne,OneToMany,ManyTomany
    }

    public class Foreign
    {
        public ForeignType Type;
        public string Key;
        public string ForeignKey;
        public string DataTable;
    }
}
