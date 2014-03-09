using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace SharpMASC.Origami
{
	[Serializable]
	public class ConfigEntry
	{
		public string TrajectoriesPath { get; set; }

		public string ModelPath { get; set; }
	}

	[Serializable]
	public class Config
	{
		public List<ConfigEntry> Models { get; set; }

		public Config ()
		{
			this.Models = new List<ConfigEntry> ();
		}

		public static Config Load (string path)
		{
			var ser = new XmlSerializer (typeof(Config));
			Config config;
			using (var sr = new StreamReader (path)) {
				config = (Config)ser.Deserialize (sr);
			}
			return config;
		}

		public static void GenerateTemplate ()
		{
			var config = new Config ();
			config.Models.Add (new ConfigEntry { 
				ModelPath = "a.obj",
				TrajectoriesPath = "a.trj"
			});
			config.Models.Add (new ConfigEntry { 
				ModelPath = "b.obj",
				TrajectoriesPath = "b.trj"
			}
			);

			var ser = new XmlSerializer (typeof(Config));

			using (var sw = new StreamWriter ("template.xml")) {
				ser.Serialize (sw, config);
			}
		}
	}
}
