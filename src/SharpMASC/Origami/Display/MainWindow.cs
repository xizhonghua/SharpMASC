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
			GL.Translate (-0.5, -0.5, -0.5);


			#region Draw a Cube

			GL.Begin (PrimitiveType.LineLoop);
			GL.Vertex3 (0, 0, 0);
			GL.Vertex3 (1, 0, 0);
			GL.Vertex3 (1, 1, 0);
			GL.Vertex3 (0, 1, 0);
			GL.End ();

			GL.Begin (PrimitiveType.LineLoop);
			GL.Vertex3 (0, 0, 1);
			GL.Vertex3 (1, 0, 1);
			GL.Vertex3 (1, 1, 1);
			GL.Vertex3 (0, 1, 1);
			GL.End ();

			GL.Begin (PrimitiveType.Lines);
			GL.Vertex3 (0, 0, 0);
			GL.Vertex3 (0, 0, 1);

			GL.Vertex3 (1, 0, 0);
			GL.Vertex3 (1, 0, 1);

			GL.Vertex3 (1, 1, 0);
			GL.Vertex3 (1, 1, 1);

			GL.Vertex3 (0, 1, 0);
			GL.Vertex3 (0, 1, 1);
			GL.End ();

			#endregion


			GL.PopMatrix ();
		}

		#endregion

		public RigidOrigami Origami { get; private set; }

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

