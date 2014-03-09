using System;
using SharpMASC.Extension;
using OpenTK;

namespace SharpMASC.Origami.Model
{
	public class Face
	{
		#region Propertry

		public int FaceId { get; set; }

		public Vertex[] Vertices { get; set; }

		public Matrix4d FoldingMap { get; set; }

		public bool PathFound { get; set; }

		public Face ParentFace { get; set; }

        public RigidGraphNode Node { get; set; }

		public Vector3d Normal {
			get { 
				var e2 = Vertices [2] - Vertices [0];
				var e1 = Vertices [1] - Vertices [0];
				var n = Vector3d.Cross (e2, e1);
                n.Normalize();
                return n;
			}
		}

		public Vector3d Center {
			get {
				return (this.Vertices [0].Position + this.Vertices [1].Position + this.Vertices [2].Position) / 3;
			}
		}

		#endregion

		public Face (int faceId)
		{
			this.FaceId = faceId;
			this.Vertices = new Vertex[3];
			this.FoldingMap = Matrix4d.Identity;
			this.ParentFace = null;
		}

		#region Public Methods

		/// <summary>
		/// Contains the specified v.
		/// </summary>
		/// <param name="v">V.</param>
		public bool Contains (Vertex v)
		{
			for (var i = 0; i < 3; ++i) {
				if (this.Vertices [i] == v)
					return true;
			}

			return false;
		}

		public bool Contains (Vertex v1, Vertex v2)
		{
			if (v1 == v2)
				return false;

			return this.Contains (v1) && this.Contains (v2);
		}

		/// <summary>
		/// Reverse the order of vertices
		/// </summary>
		public void Reverse ()
		{
			var t = this.Vertices [1];
			this.Vertices [1] = this.Vertices [2];

			this.Vertices [2] = t;
		}

		/// <summary>
		/// Check whether from this to f cross a crease CCW with the witness vertex v
		/// </summary>
		/// <returns><c>true</c>, if CC was crossed, <c>false</c> otherwise.</returns>
		/// <param name="v">V.</param>
		/// <param name="f">F.</param>
		public bool CrossCCW (Vertex v, Face f)
		{
			var v1 = (this.Center - v).ToVector2 ();
			var v2 = (f.Center - v).ToVector2 ();

			v1.Normalize ();
			v2.Normalize ();

			var cross = v1.X * v2.Y - v2.X * v1.Y;

			return cross > 0;
		}

        public void UpdateFoldingMap()
        {
            if (this.ParentFace == null)
            {
                this.FoldingMap = Matrix4d.Identity;
            }
            else
            {
                if(Node.Crease.IsAssistant)
                {
                    this.FoldingMap = this.ParentFace.FoldingMap;
                    return;   
                }                

                var B = Matrix4d.CreateTranslation(-Node.WitnessVertex.FlatPosition);
                var A = Matrix4d.CreateRotationZ(-Node.PlaneAngle);
                var C = Matrix4d.CreateRotationX(-Node.Crease.FoldingAngle);
                var AI = Matrix4d.Transpose(A);
                var BI = Matrix4d.CreateTranslation(Node.WitnessVertex.FlatPosition);


                var M = B * A * C * AI * BI;

                this.FoldingMap = M * ParentFace.FoldingMap;               
            }
        }

        public void ApplyFoldingMap()
        {                      
            for(var i=0;i<3;i++)
            {
                var v = this.Vertices[i].FlatPosition;
                this.Vertices[i].Position = Vector4d.Transform(v.ToVector4d(), this.FoldingMap).ToVector3d();                
            }         
        }

        public override int GetHashCode()
        {
            return this.FaceId.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Face {0}", FaceId);
        }

		#endregion
	}
}