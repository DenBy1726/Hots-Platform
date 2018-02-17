using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;

namespace HoTS_Forecaster_form
{
    //диспетчер исключений
    public class ExceptionThrower
    {
        public static void Throw(int code,Exception e,params object[] data)
        {
            switch (code)
            {
                //файл не найден
                case 404:
                    throw new FileNotFoundException("Не найден файл " + data[0] + ".Без данного файла работа " +
                     "приложения не предусмотренна в данной версии",e);
                //ошибка сериализации
                case 405:
                    throw new SerializationException("Не удается разобрать файл " + data[0],e);
            }
        } 

    }
}
