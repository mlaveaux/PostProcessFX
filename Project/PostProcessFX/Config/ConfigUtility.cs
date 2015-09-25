using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
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
				Utility.log("EffectConfig: Failed to save config " + ex.Message);
			}
			finally
			{
				if (writer != null)
				{
					writer.Close();
				}
			}
		}

		public static T Deserialize<T>(String filename)
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
				Utility.log("EffectConfig: " + ex.Message);
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
				}
			}

			return default(T);
		}
	}
}
