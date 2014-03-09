using System;
using SharpMASC.Origami.Display;
using SharpMASC.Origami.Model;

namespace SharpMASC.Origami
{
	class MainClass
	{
		private static bool ParseArgs (string[] args, out Config config)
		{
            config = new Config();

			if (args.Length == 0)            
				return false;            

            var i = 0;
            while(i<args.Length)
            {
                var arg = args[i];
                if(arg == "-traj")
                {
                    config.HasTrajectories = true;
                    config.TrajectoriesPath = args[++i];
                }
                else
                {
                    config.ModelPath = arg;
                }
                ++i;
            }

            return !string.IsNullOrWhiteSpace(config.ModelPath);
		}

		private static void PrintUsage ()
		{
			Console.WriteLine ("[mono] Origami.exe [options] *.obj");
		}

		public static void Main (string[] args)
		{
            Config config;
			if (!ParseArgs (args, out config)) {
				PrintUsage ();
				return;
			}

            var origami = new RigidOrigami(config);

            origami.Build(config.ModelPath);

            if (config.HasTrajectories)
                origami.LoadTrajectories(config.TrajectoriesPath);

			using (var w = new MainWindow (origami)) {
				w.Title = "Origami " + args [0];
				w.Run (60d);
			}

		}
	}
}
