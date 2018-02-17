using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    /// <summary>
    /// Парсер для разбора файла, где строки разделены '\r\n' а столбцы ','
    /// </summary>
    public class CSVParser : IDisposable
    {
        System.IO.StreamReader stream;
        System.IO.StreamWriter writer;
        public void Dispose()
        {
            if(stream != null)
                stream.Close();
            if (writer != null)
                writer.Close();
        }

        /// <summary>
        /// Загружает файл
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public IDisposable Load(string file)
        {
            stream = new System.IO.StreamReader(file,Encoding.UTF8);
            return this;
        }

        public void Save(string file,List<string[]> value)
        {
            using (writer = new StreamWriter(file))
            {
                foreach (var it in value)
                {
                    writer.WriteLine(string.Join(",", it));
                }
            }
        }

        public void Save(string file, List<SByte[]> value)
        {
            using (writer = new StreamWriter(file))
            {
                foreach (var it in value)
                {
                    writer.WriteLine(string.Join(",", it));
                }
            }
        }

        public void WriteLine(string[] items)
        {
            writer.WriteLine(string.Join(",",items));
        }

        public void WriteLine(string items)
        {
            writer.WriteLine(items);
        }

        public CSVParser Save(string file)
        {
            writer = new StreamWriter(file);
            return this;
        }


        /// <summary>
        /// возвращает следующую порцию данных
        /// </summary>
        /// <returns></returns>
        public string[] Next()
        {
            if(stream.EndOfStream == true)
            {
                return null;
            }
            else
            {
                string data = stream.ReadLine();
                return data.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}
