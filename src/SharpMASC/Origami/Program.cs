using System;
using SharpMasc.Origami.Display;

namespace SharpMasc.Origami
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

			using (var w = new MainWindow ()) {
				w.Title = "Origami";
				w.Run (60d);
			}

		}
	}
}
