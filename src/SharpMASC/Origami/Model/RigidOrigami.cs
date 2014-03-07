using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using System.Linq;

namespace SharpMASC.Origami.Model
{
	public class RigidOrigami
	{
		#region Property

		public List<Crease> Creases { get; private set; }

		public List<Vertex> Vertices { get; private set; }

		public List<Face> Faces { get; private set; }

		#endregion

		#region Fields

		private int vid;
		private int fid;
		private int cid;

		#endregion

		public RigidOrigami ()
		{
			this.Creases = new List<Crease> ();
			this.Vertices = new List<Vertex> ();
			this.Faces = new List<Face> ();
		}

		#region Public Methods

		public void Build (string path)
		{
			Console.WriteLine ("Building Origami form {0}", path);

			using (var sr = new StreamReader (path)) {
				while (!sr.EndOfStream) {
					var line = sr.ReadLine ();
					if (string.IsNullOrWhiteSpace (line))
						continue;

					if (line.Trim () == "#")
						continue;

					char t = line [0];
					line = line.Substring (1);

					if (t == 'v' && line [0] != 't') {
						this.CreateVertex (line);
					} else if (t == 'f') {
						this.CreateFace (line);
					} else if (t == 'c') { 
						this.CreateCrease (line);
					} else if (t == '#' && line [0] == 'e') {
						this.CreateCrease (line.Substring (1));
					}
				}
			}

			Console.WriteLine ("Vertices = {0}", this.Vertices.Count);
			Console.WriteLine ("Faces = {0}", this.Faces.Count);
			Console.WriteLine ("Creases = {0}", this.Creases.Count);
		}

		#endregion

		#region Private Methods

		void CreateVertex (string line)
		{
			var items = line.Split (new []{ ' ' }, StringSplitOptions.RemoveEmptyEntries);

			var vertex = new Vertex {
				VertexId = this.vid++,
				Position = new Vector3d (double.Parse (items [0]), double.Parse (items [1]), double.Parse (items [2]))
			};

			this.Vertices.Add (vertex);
		}

		void CreateFace (string line)
		{
			var items = line.Split (new []{ ' ' }, StringSplitOptions.RemoveEmptyEntries);

			var f = new Face (this.fid++);

			for (int i = 0; i < 3; i++) {
				var item = items [i];
				f.Vertices [i] = this.Vertices [int.Parse (item) - 1];
			}

			this.Faces.Add (f);
		}

		void CreateCrease (string line)
		{
			var items = line.Split (new []{ ' ' }, StringSplitOptions.RemoveEmptyEntries);

			var vid1 = int.Parse (items [0]) - 1;
			var vid2 = int.Parse (items [1]) - 1;
			var type = int.Parse (items [2]);

			var creaseType = (CreaseType)type;

			if (creaseType == CreaseType.Boundary) {
				this.Vertices [vid1].IsRealVertex = false;
				this.Vertices [vid2].IsRealVertex = false;
				return;
			}

			var c = new Crease (this.cid++);

			c.V1 = this.Vertices [vid1];
			c.V2 = this.Vertices [vid2];

			for (var i = 3; i < items.Length; ++i) {
				var goalAngle = Math.Abs (double.Parse (items [i]));
				if (c.CreaseType == CreaseType.Valley)
					goalAngle = -goalAngle;
				c.GoalFoldingAngles.Add (goalAngle);
			}

			var fs = this.Faces.Where (f => {
				return f.Contains (c.V1, c.V2);
			}).ToList ();

			if (fs.Count != 2)
				throw new Exception ("fs.Count != 2");

			c.F1 = fs [0];
			c.F2 = fs [1];

			c.FoldingAngle = c.ComputeFoldingAngle ();

			c.GoalFoldingAngles.Insert (0, c.FoldingAngle);

			this.Creases.Add (c);
		}

		#endregion
	}
}

