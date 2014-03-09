using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using System.Linq;
using SharpMASC.Utils;
using SharpMASC.MP;
using SharpMASC.Extension;
using System.Reflection;

namespace SharpMASC.Origami.Model
{
	public class RigidOrigami
	{
		#region Property

		public ConfigEntry Config { get; private set; }

		public List<Crease> Creases { get; private set; }

		public List<Vertex> Vertices { get; private set; }

		public List<Face> Faces { get; private set; }

		public List<Vertex> RealVertices { get; private set; }

		public Vector3d COM { get; private set; }

		public double R { get; private set; }

		public List<CFG> FoldingPath { get; private set; }

		#endregion

		#region Fields

		private int vid;
		private int fid;
		private int cid;
		private int gid;
		private Dictionary<Face, List<RigidGraphNode>> pathGraph;
		private List<Face> orderedFaceList;
		private List<Crease> importantCreases;

		#endregion

		#region Constructor

		public RigidOrigami (ConfigEntry config)
		{
			this.Config = config;
			this.Creases = new List<Crease> ();
			this.Vertices = new List<Vertex> ();
			this.Faces = new List<Face> ();
			this.RealVertices = new List<Vertex> ();
			this.importantCreases = new List<Crease> ();
		}

		#endregion

		#region Public Methods

		public void Build (string path)
		{
			var timer = new Timer ();
			timer.Start ();

			Console.WriteLine ("-----------------------------------------------------------");
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
					} else if (t == 's') {
						// reset gid
						if (gid == this.Creases.Count) {
							gid = 0;
							importantCreases.Clear ();
						}
						this.CreateSymmetry (line);
					}
				}
			}

			this.RealVertices = this.Vertices.Where (v => v.IsRealVertex).ToList ();

			this.ComputePlaneAngles ();

			this.ProcessRealVertices ();

			this.BuildPathGraph ();

			this.FindFoldingMapPath ();       

			this.ComputeCOM_R ();

			timer.Stop ();

			Console.WriteLine ("COM = {0} R = {1}", this.COM, this.R);
			Console.WriteLine ("Vertices = {0}/{1}", this.Vertices.Count, this.RealVertices.Count);
			Console.WriteLine ("Faces = {0}", this.Faces.Count);
			Console.WriteLine ("Creases = {0}/{1}", this.Creases.Count, this.importantCreases.Count);
			Console.WriteLine ("Rigid origami built in {0}ms", timer.TimeElapsed);
		}

		public void LoadTrajectories (string path)
		{
			var timer = new Timer ();
			timer.Start ();

			this.FoldingPath = new List<CFG> ();
			using (var sr = new StreamReader (path)) {
				while (!sr.EndOfStream) {
					var items = sr.ReadLine ().Split (new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					var cfg = new CFG (items);
					for (var i = 0; i < cfg.DOF; ++i)
						cfg [i] = cfg [i].ToRad ();
					this.FoldingPath.Add (cfg);
				}
			}

			timer.Stop ();
			Console.WriteLine ("Folding Path loaded form {0}! Total {1} steps in {2}ms", path, this.FoldingPath.Count, timer.TimeElapsed);
		}

		public void FoldToGoal ()
		{
			Creases.ForEach (c => {
				c.FoldingAngle = c.GoalFoldingAngles [c.GoalFoldingAngles.Count - 1];
			});

			this.FoldToCurrent ();            
		}

		public void FoldToInital ()
		{
			importantCreases.ForEach (c => {
				c.FoldingAngle = c.GoalFoldingAngles [0];
			});

			this.FoldToCurrent ();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="percent">0~1</param>
		public void FoldToPercent (double percent)
		{
			if (this.FoldingPath != null && this.FoldingPath.Count > 0) {
				var index = (int)(percent * (this.FoldingPath.Count - 1));
				importantCreases.ForEach (c => {
					c.FoldingAngle = this.FoldingPath [index] [c.GroupId];
				});
			} else {
				importantCreases.ForEach (c => {
					c.FoldingAngle = percent * c.GoalFoldingAngles [c.GoalFoldingAngles.Count - 1] + (1.0 - percent) * c.GoalFoldingAngles [0];
				});
			}

			FoldToCurrent ();
		}

		public void DumpObj (string path)
		{
			using (var sw = new StreamWriter (path)) {
				sw.WriteLine ("# Dumped by Origami  v{0} {1}\n", Assembly.GetExecutingAssembly ().GetName ().Version, DateTime.Now);
				sw.WriteLine ("# Number of Vertices = {0}\n", Vertices.Count);
				Vertices.ForEach (v => sw.WriteLine ("v {0} {1} {2}", v.Position [0], v.Position [1], v.Position [2]));
				sw.WriteLine ();
				sw.WriteLine ("# Number of Faces = {0}\n", Faces.Count);
				Faces.ForEach (f => sw.WriteLine ("f {0} {1} {2}", f.Vertices [0].VertexId + 1, f.Vertices [1].VertexId + 1, f.Vertices [2].VertexId + 1));
			}
		}

		#endregion

		#region Private Methods

		void FoldToCurrent ()
		{
			var t = new Timer ();

			t.Start ();

			UpdateFoldingMap ();            

			Faces.ForEach (f => {
				f.ApplyFoldingMap ();
			});

			t.Stop ();
		}

		void CreateVertex (string line)
		{
			var items = line.Split (new []{ ' ' }, StringSplitOptions.RemoveEmptyEntries);

			var vertex = new Vertex {
				VertexId = this.vid++,
				Position = new Vector3d (double.Parse (items [0]), double.Parse (items [1]), double.Parse (items [2]))
			};

			vertex.FlatPosition = vertex.Position;

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

			if (f.Normal [2] < 0)
				f.Reverse ();

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

			var c = new Crease (this.cid++) {
				CreaseType = creaseType,
				V1 = this.Vertices [vid1],
				V2 = this.Vertices [vid2],
				GroupId = this.gid++
			};

			this.importantCreases.Add (c);

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

		void CreateSymmetry (string line)
		{
			var items = line.Split (new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			var parentCreaseId = int.Parse (items [0]) - 1;

			var parentCrease = Creases.FirstOrDefault (c => c.CreaseId == parentCreaseId);
			parentCrease.GroupId = gid++;
			importantCreases.Add (parentCrease);            

			for (var i = 1; i < items.Length; ++i) {
				var childCreaseId = int.Parse (items [i]) - 1;
				var childCrease = Creases.FirstOrDefault (c => c.CreaseId == childCreaseId);
				childCrease.Refer = parentCrease;
			}
		}

		void ComputePlaneAngles ()
		{
			Creases.ForEach (c => c.UpdatePlaneAngles ());
		}

		void ProcessRealVertices ()
		{
			RealVertices.ForEach (v => {
				v.Creases.AddRange (Creases.Where (c => c.V1 == v || c.V2 == v));
				v.SortCreases ();
			});
		}

		void BuildPathGraph ()
		{
			this.pathGraph = new Dictionary<Face, List<RigidGraphNode>> ();

			Creases.ForEach (c => {
				if (!this.pathGraph.ContainsKey (c.F1)) {
					this.pathGraph [c.F1] = new List<RigidGraphNode> ();
				}

				if (!this.pathGraph.ContainsKey (c.F2)) {
					this.pathGraph [c.F2] = new List<RigidGraphNode> ();
				}

				var ccw = c.F1.CrossCCW (c.V1, c.F2);
				var fromFace = ccw ? c.F1 : c.F2;
				var targetFace = ccw ? c.F2 : c.F1;
				if (c.V1.IsRealVertex)
					this.pathGraph [fromFace].Add (new RigidGraphNode (c, fromFace, targetFace, c.V1));
				if (c.V2.IsRealVertex)
					this.pathGraph [targetFace].Add (new RigidGraphNode (c, targetFace, fromFace, c.V2));
			});
		}

		void FindFoldingMapPath ()
		{
			this.orderedFaceList = new List<Face> ();

			Faces.ForEach (f => {
				f.PathFound = false;
			});

			var q = new Queue<Face> ();

			var f0 = Faces [0];
			f0.PathFound = true;            

			q.Enqueue (f0);

			while (q.Count != 0) {
				var f = q.Dequeue ();
                
				orderedFaceList.Add (f);

				var nodes = pathGraph [f];

				nodes.ForEach (node => {
					var targetFace = node.TargetFace;
					if (targetFace.PathFound)
						return;
					targetFace.PathFound = true;
					targetFace.ParentFace = f;
					targetFace.Node = node;
					q.Enqueue (targetFace);
				});
			}
		}

		void UpdateFoldingMap ()
		{
			this.Faces [0].FoldingMap = Matrix4d.Identity;

			orderedFaceList.ForEach (f => {                
				f.UpdateFoldingMap ();
			});
		}

		void ComputeCOM_R ()
		{
			var bvs = Vertices.Where (v => {
				return !v.IsRealVertex;
			}).ToList ();

			var com = new Vector3d ();

			bvs.ForEach (v => {
				com += v.Position;
			});

			com /= bvs.Count;

			var r = 0.0;

			bvs.ForEach (v => {
				if ((v - com).Length > r)
					r = (v - com).Length;
			});

			this.COM = com;
			this.R = r;

		}

		#endregion
	}
}

