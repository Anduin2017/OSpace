using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace Xml
{
    class XmlHelper
    {
        public static T DeSerialize<T>(StreamReader data)
        {
            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                T o = (T)formatter.Deserialize(data);
                return o;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static void Serialize<T>(T o, string filePath)
        {
            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T));
                StreamWriter sw = new StreamWriter(filePath, false);
                formatter.Serialize(sw, o);
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString()); 
            }
        }
    }
}
