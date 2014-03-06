using NUnit.Framework;
using System;
using SharpMASC.Utils;
using System.IO;

namespace test
{
	[TestFixture ()]
	public class Test
	{
		[Test ()]
		public void TestCase ()
		{
			var timer = new Timer ();

			var result = 0;
			timer.Start ();
			using (var fs = new StreamWriter ("test.txt")) {
				for (var i = 0; i < 100000; i++) {
					result += i;
					fs.WriteLine (result);
				}
			}

			timer.Stop ();

			Console.WriteLine ("result = {0} totalTime = {1}", result, timer.TimeElapsed);

			Assert.IsTrue (result > 0);
			Assert.IsTrue (timer.TimeElapsed > 0);
		}
	}
}

