using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DBMaker
{
    public class PostegresConverter : IConverter
    {

        public string CreateDictionary(string name)
        {
            return $"CREATE TABLE IF NOT EXISTS {name} \n" +
                $"(\n" +
                $"id SERIAL PRIMARY KEY,\n" +
                $"name  VARCHAR(100)\n" +
                $");\n";
        }

        public string CreateFK(Foreign foreign)
        {
            return $"FOREIGN KEY ({foreign.Key}) REFERENCES {foreign.DataTable}({foreign.ForeignKey })";
        }

        public string CreateTable(string name, Type obj,string primaryKey, List<Foreign> foreign)
        {
            string result = $"CREATE TABLE IF NOT EXISTS {name} \n" +
                $"( \n";
            List<string> fields = new List<string>();
            bool keyUsed = false;
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            MemberInfo[] members = obj.GetFields(bindingFlags).Cast<MemberInfo>()
                .Concat(obj.GetProperties(bindingFlags)).ToArray();
            foreach (MemberInfo field in members)
            {
                string fieldData = "";
                string fieldName = field.Name.ToLower();
                Type fieldType = null;
                switch (field.MemberType)
                {
                    case MemberTypes.Property:
                        fieldType = obj.GetProperty(field.Name).PropertyType;
                        break;
                    case MemberTypes.Field:
                        fieldType = obj.GetField(field.Name).FieldType;
                        break;
                }
               
                fieldData += $"{fieldName}";
                if(fieldName == primaryKey.ToLower())
                {
                    fieldData += $" SERIAL PRIMARY KEY";
                    keyUsed = true;
                }
                else
                {
                    fieldData += $"    {MapType(fieldType)}";
                }
                fields.Add(fieldData);
            }
            if(keyUsed == false)
                fields.Add($"{primaryKey}   SERIAL  PRIMARY KEY");
            result += string.Join(",\n", fields);
            if (foreign != null)
            {
                string[] foreignKeys = foreign.Select(x => CreateFK(x)).ToArray();
                if (foreignKeys.Length > 0)
                {
                    result += ",\n";
                    result += string.Join(",\n", foreignKeys);
                }
            }
            result += "\n);\n";
            return result;
        }

        public string Insert(object obj)
        {
            return null;
        }

        public string MapType(Type type)
        {
            if (type.BaseType == typeof(object))
            {
                if (type.Name == "String")
                    return "VARCHAR(1000)";
                if (type.Name == "Gaussian")
                    return "Int";
            }
            if (type.BaseType == typeof(Array))
                return "Int";
            if(type.BaseType == typeof(ValueType))
            {
                if (type.Name == "DateTime")
                    return "date";
                if (type.Name == "Int32")
                    return "INT";
                if (type.Name == "Boolean")
                    return "BOOLEAN";
                if (type.Name == "Double")
                    return "FLOAT8";
            }
            else if (type.BaseType == typeof(Enum))
            {
                return "INT";
            }
            Console.WriteLine(type.Name);
            return "VARCHAR(1000)";
        }
    }
}
