using System;
using SharpMASC.Origami.Display;
using SharpMASC.Origami.Model;

namespace SharpMASC.Origami
{
	class MainClass
	{
		private static bool ParseArgs (string[] args)
		{
			if (args.Length == 0)
				return false;
			return true;
		}

		private static void PrintUsage ()
		{
			Console.WriteLine ("[mono] Origami.exe [options] *.obj");
		}

		public static void Main (string[] args)
		{
			if (!ParseArgs (args)) {
				PrintUsage ();
				return;
			}

			var origami = new RigidOrigami ();

			origami.Build (args [0]);

			using (var w = new MainWindow (origami)) {
				w.Title = "Origami " + args [0];
				w.Run (60d);
			}

		}
	}
}
