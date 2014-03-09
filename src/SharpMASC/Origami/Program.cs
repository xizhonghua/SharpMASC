using System;
using SharpMASC.Origami.Display;
using SharpMASC.Origami.Model;

namespace SharpMASC.Origami
{
	class MainClass
	{
		private static bool ParseArgs (string[] args, out Config config)
		{
			config = new Config ();

			if (args.Length == 0)
				return false;            

			var entry = new ConfigEntry ();

			config.Models.Add (entry);

			var i = 0;
			while (i < args.Length) {
				var arg = args [i];
				if (arg == "-traj") {
					entry.TrajectoriesPath = args [++i];
				} else if (arg == "-c") {
					var configPath = args [++i];
					config = Config.Load (configPath);
				} else {
					entry.ModelPath = arg;
				}
				++i;
			}

			return config.Models.Count > 0;
		}

		private static void PrintUsage ()
		{
			Console.WriteLine ("[mono] Origami.exe [options] [*.obj]");
			Console.WriteLine ("Options:");
			Console.WriteLine ("    -c : Config file");
			Console.WriteLine (" -traj : Trajectories file");
			Console.WriteLine ("Examples:");
			Console.WriteLine ("  [mono] Origami.exe -c models.xml");
			Console.WriteLine ("  [mono] Origami.exe comp/miura_3_3.obj -traj comp/miura_3_3_md_0.0001_us.trj");
		}

		public static void Main (string[] args)
		{
			Config config;
			if (!ParseArgs (args, out config)) {
				PrintUsage ();
				return;
			}

			using (var w = new MainWindow (config)) {
				w.Title = "Origami " + args [0];
				w.Run (30d);
			}

		}
	}
}
