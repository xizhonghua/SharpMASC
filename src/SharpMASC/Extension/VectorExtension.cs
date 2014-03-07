using System;
using OpenTK;

namespace SharpMASC.Extension
{
	public static class VectorExtension
	{
		public static Vector2 ToVector2 (this Vector3 self)
		{
			return new Vector2 (self.X, self.Y);
		}

		public static double Dot (this Vector3 v1, Vector3 v2)
		{
			return Vector3.Dot (v1, v2);
		}
	}
}

