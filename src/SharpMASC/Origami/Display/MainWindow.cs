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
            this.DrawCreases();
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

        void DrawCreases()
        {
            GL.Begin(PrimitiveType.Lines);
            Origami.Creases.ForEach(DrawCrease);
            GL.End();
        }

        void DrawCrease(Crease c)
        {
            if (c.IsMountain)
            {
                GL.Color3(1.0f, 0, 0);
            }
            else if (c.IsValley)
            {
                GL.Color3(0, 0, 1.0f);
            }
            else
            {
                GL.Color3(0.5f, 0.5f, 0.5f);
            }

            GL.Vertex3(c.V1.Position);
            GL.Vertex3(c.V2.Position);
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

