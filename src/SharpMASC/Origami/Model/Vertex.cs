using System;
using OpenTK;

namespace SharpMASC.Origami.Model
{
	public class Vertex
	{
		#region Propertry

		public int VertexId { get; set; }

		public Vector3 Position { get; set; }

		#endregion

		public Vertex ()
		{
		}

		#region Operators

		public static Vector3 operator  - (Vertex l, Vertex r)
		{
			return l.Position - r.Position;
		}

		public static Vector3 operator  - (Vertex l, Vector3 r)
		{
			return l.Position - r;
		}

		public static Vector3 operator  - (Vector3 l, Vertex r)
		{
			return l - r.Position;
		}

		public static Vector3 operator  + (Vertex l, Vertex r)
		{
			return l.Position + r.Position;
		}

		public static Vector3 operator  + (Vector3 l, Vertex r)
		{
			return l + r.Position;
		}

		public static Vector3 operator  + (Vertex l, Vector3 r)
		{
			return l.Position + r;
		}

		#endregion
	}
}

