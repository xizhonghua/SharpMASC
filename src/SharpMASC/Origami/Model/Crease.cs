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

        public int GroupId { get; set; }

        public bool SameType
        {
            get
            {
                return this.CreaseType == Refer.CreaseType;
            }
        }

		public bool IsSameTypeToRefer {
			get {
				if (Refer == null)
					return false;
				return this.CreaseType == Refer.CreaseType;
			}
		}

		public double PlaneAngle1 { get; set; }

		public double PlaneAngle2 { get; set; }

		public double FoldingAngle {
            get
            {                
                var angle = this.Refer.foldingAngle;
                if (IsSameTypeToRefer) return angle;
                return -angle;
            }
            set
            {
                foldingAngle = Math.Abs(value);
                if (this.IsValley) foldingAngle = -foldingAngle;
            }
        }

		public List<double> GoalFoldingAngles { get; private set; }

		#endregion

		#region Fields	
        double foldingAngle;

		#endregion

		#region Constructor

		public Crease (int creaseId)
		{
			this.CreaseId = creaseId;
			this.CreaseType = CreaseType.Boundary;
			this.Refer = this;
			this.GoalFoldingAngles = new List<double> ();
		}

		#endregion

		#region Public Methods

		public void UpdatePlaneAngles ()
		{
			var x = new Vector3d (1, 0, 0);
            var v1 = (this.V2 - this.V1);
			v1.Normalize ();
			var v2 = -v1;
			double pa1 = Math.Acos (v1.Dot (x));
			double pa2 = Math.Acos (v2.Dot (x));

			this.PlaneAngle1 = v1.Y < 0 ? 2 * Math.PI - pa1 : pa1;
			this.PlaneAngle2 = v2.Y < 0 ? 2 * Math.PI - pa2 : pa2;
		}	

		public double ComputeFoldingAngle ()
		{
			var n1 = this.F1.Normal;
			var n2 = this.F2.Normal;

			var dot = Vector3d.Dot (n1, n2);
			var alpha = Math.Acos (dot);

			double dot2 = Vector3d.Dot ((F1.Center - F2.Center), n2);

			if (Math.Abs (alpha) < 0.01)
				alpha = 0;

			if (dot2 < 0)
				alpha = -alpha;

			return alpha;
		}

		public double GetPlaneAngle (Vertex witnessVertex)
		{
            if (witnessVertex == this.V1)
				return this.PlaneAngle1;
            if (witnessVertex == this.V2)
				return this.PlaneAngle2;

			throw new ArgumentException ("witnessId");
		}

        public override string ToString()
        {
            return string.Format("Crease {0}", this.CreaseId);
        }

		#endregion
	}
}

