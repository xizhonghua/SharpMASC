using System;
using OpenTK;

namespace SharpMASC.Extension
{
	public static class VectorExtension
	{
		public static Vector2d ToVector2 (this Vector3d self)
		{
			return new Vector2d (self.X, self.Y);
		}

		public static double Dot (this Vector3d v1, Vector3d v2)
		{
			return Vector3d.Dot (v1, v2);
		}
	}
}

