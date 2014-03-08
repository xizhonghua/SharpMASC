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

        public List<Vertex> RealVertices { get; private set; }

		public Vector3d COM {
			get { 
				var com = new Vector3d ();
				this.Faces.ForEach (f => com += f.Center);
				com /= this.Faces.Count;
				return com;
			}
		}

		#endregion

		#region Fields

		private int vid;
		private int fid;
		private int cid;

        private Dictionary<Face, List<RigidGraphNode>> pathGraph;

        private List<Face> orderedFaceList;

		#endregion

        #region Constructor
        public RigidOrigami ()
		{
			this.Creases = new List<Crease> ();
			this.Vertices = new List<Vertex> ();
			this.Faces = new List<Face> ();
            this.RealVertices = new List<Vertex>();
		}

        #endregion

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

            this.RealVertices = this.Vertices.Where(v => v.IsRealVertex).ToList();

            this.ComputePlaneAngles();

            this.ProcessRealVertices();

            this.BuildPathGraph();

            this.FindFoldingMapPath();


            Console.WriteLine("Vertices = {0}/{1}", this.Vertices.Count, this.RealVertices.Count);
			Console.WriteLine ("Faces = {0}", this.Faces.Count);
			Console.WriteLine ("Creases = {0}", this.Creases.Count);

            Faces.ForEach(f =>
            {
                Console.WriteLine("fid = {0} parent = {1}", f.FaceId, f.ParentFace == null ? -1 : f.ParentFace.FaceId);
            });
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

            var c = new Crease(this.cid++)
            {
                CreaseType = creaseType,
                V1 = this.Vertices [vid1],
                V2 = this.Vertices [vid2]
            };

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

        void ComputePlaneAngles()
        {
            Creases.ForEach(c => c.UpdatePlaneAngles());
        }

        void ProcessRealVertices()
        {
            RealVertices.ForEach(v =>
                {
                    v.Creases.AddRange(Creases.Where(c => c.V1 == v || c.V2 == v));
                    v.SortCreases();
                });
        }

        void BuildPathGraph()
        {
            this.pathGraph = new Dictionary<Face, List<RigidGraphNode>>();

            Creases.ForEach(c =>
                {
                    if(!this.pathGraph.ContainsKey(c.F1))
                    {
                        this.pathGraph[c.F1] = new List<RigidGraphNode>();
                    }

                    if(!this.pathGraph.ContainsKey(c.F2))
                    {
                        this.pathGraph[c.F2] = new List<RigidGraphNode>();
                    }

                    var ccw =  c.F1.CrossCCW(c.V1, c.F2);
                    var fromFace = ccw ? c.F1 : c.F2;
                    var targetFace = ccw ? c.F2 : c.F1;
                    if (c.V1.IsRealVertex)
                        this.pathGraph[fromFace].Add(new RigidGraphNode(c, fromFace, targetFace, c.V1));
                    if (c.V2.IsRealVertex)
                        this.pathGraph[fromFace].Add(new RigidGraphNode(c, fromFace, targetFace, c.V2));
                });
        }

        void FindFoldingMapPath()
        {
            this.orderedFaceList = new List<Face>();

            Faces.ForEach(f => { f.PathFound = false; });

            var q = new Queue<Face>();

            var f0 = Faces[0];
            f0.PathFound = true;            

            q.Enqueue(f0);

            while(q.Count !=0)
            {
                var f = q.Dequeue();

                Console.WriteLine("fid = {0}", f.FaceId);
                orderedFaceList.Add(f);

                var nodes = pathGraph[f];

                nodes.ForEach(node =>
                {
                    var targetFace = node.TargetFace;
                    if (targetFace.PathFound) return;
                    targetFace.PathFound = true;
                    targetFace.ParentFace = f;
                    q.Enqueue(targetFace);
                });
            }
        }

		#endregion
	}
}

