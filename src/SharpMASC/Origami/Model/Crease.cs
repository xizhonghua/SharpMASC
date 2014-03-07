using System;
using System.Collections.Generic;
using OpenTK;
using SharpMASC.Extension;

namespace SharpMASC.Origami.Model
{
	public enum CreaseType
	{
		Boundary = 0,
		Mountain = 1,
		Valley = 2,
		Assistant = 3
	}

	public class Crease
	{
		#region Propetry

		public CreaseType CreaseType { get; set; }

		public int CreaseId { get ; set; }

		public bool IsBoundary { get { return this.CreaseType == CreaseType.Boundary; } }

		public bool IsMountain { get { return this.CreaseType == CreaseType.Mountain; } }

		public bool IsValley { get { return this.CreaseType == CreaseType.Valley; } }

		public bool IsAssistant { get { return this.CreaseType == CreaseType.Assistant; } }

		public Face F1 { get; set; }

		public Face F2 { get; set; }

		public Vertex V1 { get; set; }

		public Vertex V2 { get; set; }

		public Crease Refer { get; set; }

		public bool IsSameTypeToRefer {
			get {
				if (Refer == null)
					return false;
				return this.CreaseType == Refer.CreaseType;
			}
		}

		public double PlaneAngle1 { get; set; }

		public double PlaneAngle2 { get; set; }

		public double FoldingAngle { get; set; }

		public List<double> GoalFoldingAngles { get; private set; }

		#endregion

		#region Fields

		bool crossed1;
		bool crossed2;

		#endregion

		#region Constructor

		public Crease (int creaseId)
		{
			this.CreaseId = creaseId;
			this.CreaseType = CreaseType.Boundary;
			this.Refer = null;
			this.GoalFoldingAngles = new List<double> ();
		}

		#endregion

		#region Public Methods

		public void UpdatePlaneAngles ()
		{
			var x = new Vector3 (1, 0, 0);
			var v1 = (this.V1 - this.V2);
			v1.Normalize ();
			var v2 = -v1;
			double pa1 = Math.Acos (v1.Dot (x));
			double pa2 = Math.Acos (v2.Dot (x));

			this.PlaneAngle1 = v1.Y < 0 ? 2 * Math.PI - pa1 : pa1;
			this.PlaneAngle2 = v2.Y < 0 ? 2 * Math.PI - pa2 : pa2;
		}

		public bool IsCrossed (int witnessId)
		{
			if (witnessId != this.V1.VertexId && witnessId != this.V2.VertexId)
				throw new ArgumentException ("witnessId");

			if (witnessId == this.V1.VertexId)
				return crossed1;
			else
				return crossed2;
		}

		public void Cross (int witnessId)
		{
			if (witnessId != this.V1.VertexId && witnessId != this.V2.VertexId)
				throw new ArgumentException ("witnessId");

			if (witnessId == this.V1.VertexId)
				crossed1 = true;

			if (witnessId == this.V2.VertexId)
				crossed2 = true;
		}

		public void UnCross (int witnessId)
		{
			if (witnessId != this.V1.VertexId && witnessId != this.V2.VertexId)
				throw new ArgumentException ("witnessId");

			if (witnessId == this.V1.VertexId)
				crossed1 = false;

			if (witnessId == this.V2.VertexId)
				crossed2 = false;
		}

		public double ComputeFoldingAngle ()
		{
			var n1 = this.F1.Normal;
			var n2 = this.F2.Normal;

			var dot = Vector3.Dot (n1, n2);
			var alpha = Math.Acos (dot);

			double dot2 = Vector3.Dot ((F1.Center - F2.Center), n2);

			if (Math.Abs (alpha) < 0.01)
				alpha = 0;

			if (dot2 < 0)
				alpha = -alpha;

			return alpha;
		}

		#endregion
	}
}

