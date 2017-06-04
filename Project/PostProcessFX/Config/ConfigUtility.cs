using System;

using System.Xml.Serialization;
using System.IO;

namespace PostProcessFX.Config
{
    class ConfigUtility
    {
        public static void Serialize<T>(String filename, object instance)
        {
            TextWriter writer = null;

            try
            {
                writer = new StreamWriter(filename);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(writer, instance);
            }
            catch (Exception ex)
            {
                PPFXUtility.logException("EffectConfig: Failed to save config " + filename, ex);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        public static T Deserialize<T>(String filename)  where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filename);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                PPFXUtility.logException("EffectConfig: Failed to load config " + filename, ex);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return new T();
        }
    }
}
