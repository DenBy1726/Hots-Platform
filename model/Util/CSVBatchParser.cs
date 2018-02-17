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
    public class CSVBatchParser : IDisposable
    {
        System.IO.StreamReader stream;

        string[] files;

        int id;

        public void Dispose()
        {
            if (stream != null)
                stream.Close();
        }

        /// <summary>
        /// Загружает пакет файлов. формат имени файла: name_number
        /// number - порядок обработки файлов
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public IDisposable Load(string[] file)
        {
            id = 0;
            files = new string[file.Length];
            foreach(string path in file)
            {
                string name = Path.GetFileNameWithoutExtension(path);
                int id = Int32.Parse(name.Split('_').Last());
                files[id] = path;
            }
            stream = new System.IO.StreamReader(files[0],Encoding.Default);
            return this;
        }

        /// <summary>
        /// возвращает следующую порцию данных
        /// </summary>
        /// <returns></returns>
        public string[] Next(int skip = 0)
        {
            if(stream.EndOfStream == true)
            {
                if(id == files.Length - 1)
                    return null;
                else
                {
                    stream.Close();
                    stream = new StreamReader(files[++id], Encoding.Default);
                    return Next();
                }
            }
            else
            {
                string data = stream.ReadLine();
                return data.Split(new char[] { ',' });
            }
        }
    }
}
