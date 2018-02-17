using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public abstract class CLIParser<T>
    {
        List<string> keys = new List<string>();

        Dictionary<string, string> namedParam = new Dictionary<string, string>();

        public List<string> Keys { get => keys; protected set => keys = value; }
        public Dictionary<string, string> NamedParam { get => namedParam; protected set => namedParam = value; }


        public virtual void Run(string[] args)
        {
            Init(args);
        }
        
        protected virtual void Init(string[] args)
        {
            foreach(string arg in args)
            {
                string[] tryParse = arg.Split('=');
                if (tryParse.Length == 2)
                    NamedParam[tryParse[0]] = tryParse[1];
                else
                    Keys.Add(arg);
            }
        }

        public abstract void Validate();

        protected abstract object ReadData();

        protected abstract T ParseData(object data);

        protected abstract void OpenSource(string path);

        protected abstract void Save(string name,object obj, Type t);

    }
}
