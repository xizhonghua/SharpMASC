using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SharpMASC.Display;
using SharpMASC.Origami.Model;

namespace SharpMASC.Origami.Display
{
	public class MainWindow: WindowBase
	{
		#region implemented abstract members of WindowBase

		protected override void Render (FrameEventArgs e)
		{
			// Write your render code here

			GL.PushMatrix ();

			GL.Translate (-this.Origami.COM);
			this.DrawAll ();

			GL.PopMatrix ();
		}

		#endregion

		#region propetry

		public RigidOrigami Origami { get; private set; }

		#endregion

		#region Private

		void DrawAll ()
		{
			this.DrawFaces ();
		}

		void DrawFaces ()
		{
			GL.Enable (EnableCap.PolygonOffsetFill);
			GL.PolygonOffset (0.5f, 0.5f);

			GL.Enable (EnableCap.CullFace);

			GL.CullFace (CullFaceMode.Front);
			GL.Color3 (1.0f, 1.0f, 1.0f);
			GL.Begin (PrimitiveType.Triangles);
			this.Origami.Faces.ForEach (this.DrawFace);
			GL.End ();

			GL.CullFace (CullFaceMode.Back);

			GL.Color3 (0.8f, 0.8f, 0.8f);
			GL.Begin (PrimitiveType.Triangles);
			this.Origami.Faces.ForEach (this.DrawFace);
			GL.End ();

			GL.Disable (EnableCap.PolygonOffsetFill);
			GL.Disable (EnableCap.CullFace);
		}

		void DrawFace (Face f)
		{
			for (var i = 0; i < 3; i++)
				GL.Vertex3 (f.Vertices [i].Position);
		}

		#endregion

		public MainWindow (RigidOrigami origami)
		{
			this.Origami = origami;
		}

		protected override void Init ()
		{
			base.Init ();

			// change the backgroun color
			this.BackgroundColor = Color.Gray;
		}
	}
}

