﻿using System;
using OpenTK;
using System.Collections.Generic;

namespace SharpMASC.Origami.Model
{
	public class Vertex
	{
		#region Propertry

		public int VertexId { get; set; }

		public Vector3d Position { get; set; }

        public Vector3d FlatPosition { get; set; }

		public List<Crease> Creases { get; private set; }

		public bool IsRealVertex { get; set; }

		public bool IsImportantVertex { get; set; }

		#endregion

		public Vertex ()
		{
			this.Creases = new List<Crease> ();
			this.IsRealVertex = true;
			this.IsImportantVertex = false;
		}

		#region Public Methods

		public void SortCreases ()
		{
			this.Creases.Sort ((c1, c2) => {
				return c1.GetPlaneAngle (this).CompareTo (c1.GetPlaneAngle (this));
			});
		}

        public override string ToString()
        {
            return string.Format("Vertex = {0} Pos = ({1})", this.VertexId, this.Position);
        }

        public Matrix4d GetLoopMatrix()
        {
            var M = Matrix4d.Identity;

            Creases.ForEach(c =>
                {
                    var A = Matrix4d.CreateRotationZ(c.GetPlaneAngle(this));
                    var C = Matrix4d.CreateRotationX(c.FoldingAngle);
                    var AI = Matrix4d.Transpose(A);

                    var T = A * C * AI;
                    M = M * T;
                });

            return M;
        }



        #endregion

        #region Operators

        public static Vector3d operator  - (Vertex l, Vertex r)
		{
			return l.Position - r.Position;
		}

		public static Vector3d operator  - (Vertex l, Vector3d r)
		{
			return l.Position - r;
		}

		public static Vector3d operator  - (Vector3d l, Vertex r)
		{
			return l - r.Position;
		}

		public static Vector3d operator  + (Vertex l, Vertex r)
		{
			return l.Position + r.Position;
		}

		public static Vector3d operator  + (Vector3d l, Vertex r)
		{
			return l + r.Position;
		}

		public static Vector3d operator  + (Vertex l, Vector3d r)
		{
			return l.Position + r;
		}

		#endregion
	}
}

