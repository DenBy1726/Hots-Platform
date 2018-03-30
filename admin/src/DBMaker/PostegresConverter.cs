using HoTS_Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DBMaker
{

    class Config
    {
        public string DateFormat = "yyyy-MM-dd";
    }
    public class PostegresConverter : IConverter
    {

        Dictionary<string, string> customNameMapper = new Dictionary<string, string>();
        Dictionary<string, int> idStorage = new Dictionary<string, int>();

        private int getId(string key)
        {
            if (idStorage.ContainsKey(key) == false)
                idStorage[key] = 0;
            else
                idStorage[key] = idStorage[key] + 1;
            return idStorage[key];
        }

        Config config = new Config();

        public Dictionary<string, string> CustomNameMapper { get => customNameMapper; set => customNameMapper = value; }
        internal Config Config { get => config; set => config = value; }

        public string CreateDictionary(string name)
        {
            return $"CREATE TABLE IF NOT EXISTS {name} \n" +
                $"(\n" +
                $"id BIGSERIAL UNIQUE PRIMARY KEY NOT NULL,\n" +
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
                if (CustomNameMapper.ContainsKey(fieldName))
                    fieldName = CustomNameMapper[fieldName];
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
                    fieldData += $" BIGSERIAL UNIQUE PRIMARY KEY NOT NULL";
                    keyUsed = true;
                }
                else
                {
                    fieldData += $"    {MapType(fieldType)}";
                }
                fields.Add(fieldData);
            }
            if(keyUsed == false)
                fields.Add($"{primaryKey}   BIGSERIAL UNIQUE PRIMARY KEY NOT NULL");
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

        public string Insert(string table,object obj)
        {
            Type type = obj.GetType();
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            MemberInfo[] members = type.GetFields(bindingFlags).Cast<MemberInfo>()
                .Concat(type.GetProperties(bindingFlags)).ToArray();
            string result = $"INSERT INTO {table}(";
            string result2 = "VALUES (";
            List<Tuple<string, string>> dataMap = new List<Tuple<string, string>>();
            foreach (MemberInfo field in members)
            {
                object value = "";
                Type fieldType = null;
                switch (field.MemberType)
                {
                    case MemberTypes.Property:
                        value = type.GetProperty(field.Name).GetValue(obj);
                        fieldType = type.GetProperty(field.Name).PropertyType;
                        break;
                    case MemberTypes.Field:
                        value = type.GetField(field.Name).GetValue(obj);
                        fieldType = type.GetField(field.Name).FieldType;
                        break;
                }
                string pgValue = MapValue(fieldType, value);
                string fieldName = field.Name.ToLower();
                if (CustomNameMapper.ContainsKey(fieldName))
                    fieldName = CustomNameMapper[fieldName];
                dataMap.Add(new Tuple<string, string>(fieldName, pgValue));
            }
            result += String.Join(",", dataMap.Select(x => x.Item1).ToArray()) + ")\n";
            result2 += String.Join(",", dataMap.Select(x => x.Item2).ToArray()) + ");\n";
            return result + result2;
        }

        public string InsertDictionary(Type type)
        {
            return Insert(type.Name, Enum.GetNames(type)
                .Select((x,index)=>new { name=x,id = index }));
        }

        public string Insert(string table,IEnumerable<object> obj)
        {
            string result = "";
            foreach(var it in obj)
            {
                result += Insert(table,it) + "\n";
            }
            return result;
        }

        public string MapType(Type type)
        {
            if (type.BaseType == typeof(object))
            {
                if (type.Name == "String")
                    return "VARCHAR(1000)";
                if (type.Name == "Gaussian")
                    return "INT";
                if (type.Name == "Json")
                    return "JSONB";
            }
            if (type.BaseType == typeof(Array))
                return "INT";
            if(type.BaseType == typeof(ValueType))
            {
                if (type.Name == "DateTime")
                    return "date";
                if (type.Name == "Int32")
                    return "INT";
                if (type.Name == "Int64")
                    return "BIGINT";
                if (type.Name == "Boolean")
                    return "BOOLEAN";
                if (type.Name == "Double")
                    return "FLOAT8";
            }
            else if (type.BaseType == typeof(Enum))
            {
                return "INT";
            }
            Console.WriteLine($"Cannot map type {type.Name}");
            return "INT";
        }

        public string MapValue(Type type,object obj)
        {
            if (type.BaseType == typeof(object))
            {
                if (type.Name == "String")
                    return $"\'{obj.ToString().Replace(@"'",@"''")}\'";
                if (type.Name == "Gaussian")
                    return getId(type.FullName).ToString();
                if (type.Name == "Json")
                    return $"\'{((Json)obj).data.Replace(@"'", @"''")}\'";
            }
            if (type.BaseType == typeof(Array))
                return getId(type.FullName).ToString();
            if (type.BaseType == typeof(ValueType))
            {
                if (type.Name == "DateTime")
                {
                    DateTime date = ((DateTime)obj);
                    return $"\'{date.ToString(Config.DateFormat)}\'";
                }
                   
                if (type.Name == "Int32")
                    return obj.ToString();
                if (type.Name == "Int64")
                    return obj.ToString();
                if (type.Name == "Boolean")
                    return obj.ToString().ToLower();
                if (type.Name == "Double")
                    return obj.ToString().Replace(",",".");
            }
            else if (type.BaseType == typeof(Enum))
            {
                return ((int)obj).ToString();
            }
            Console.WriteLine($"Cannot map type {type.Name}");
            return "INT";
        }
    }
}
