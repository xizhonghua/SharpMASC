using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace SharpMASC.Window
{
	public abstract class WindowBase : GameWindow
	{
		#region Propertry

		public float EyeX { get; protected set; }

		public float EyeY { get; protected set; }

		public float EyeZ { get; protected set; }

		public float TargetX { get; protected set; }

		public float TargetY { get; protected set; }

		public float TargetZ { get; protected set; }

		public float UpX { get; protected set; }

		public float UpY { get; protected set; }

		public float UpZ { get; protected set; }

		public float FOVY { get; protected set; }

		public float ZNear { get; protected set; }

		public float ZFar { get; protected set; }

		public Color BackgroundColor { get; protected set; }

		#endregion

		#region Abstract Methods

		protected abstract void Render (FrameEventArgs e);

		#endregion

		#region Virtual Methods

		/// <summary>
		/// Will be called afer OnLoad
		/// Be sure to call base.Init() after override
		/// </summary>
		protected virtual void Init ()
		{
			this.EyeX = this.EyeY = 0;
			this.EyeZ = 5;

			this.TargetX = this.TargetY = this.TargetZ = 0;

			this.UpX = 0;
			this.UpY = 1;
			this.UpZ = 0;

			this.FOVY = (float)Math.PI / 4;
			this.ZNear = 0.1f;
			this.ZFar = 6400f;


			this.BackgroundColor = Color.MidnightBlue;
		}

		#endregion

		#region OnLoad

		protected override void OnLoad (EventArgs e)
		{
			this.Init ();

			GL.ClearColor (this.BackgroundColor);
			GL.Enable (EnableCap.DepthTest);

			GL.MatrixMode (MatrixMode.Modelview);
			GL.LoadIdentity ();

		}

		#endregion

		#region OnUpdateFrame

		/// <summary>
		/// Called when the frame is updated.
		/// Default action:
		/// 	Escape : return
		/// </summary>
		/// <param name="e">Contains information necessary for frame updating.</param>
		protected override void OnUpdateFrame (FrameEventArgs e)
		{
			if (Keyboard [OpenTK.Input.Key.Escape]) {
				this.Exit ();
			}
		}

		#endregion

		#region OnRenderFrame

		protected override void OnRenderFrame (FrameEventArgs e)
		{
			Matrix4 lookat = Matrix4.LookAt (
				                 this.EyeX, this.EyeY, this.EyeZ, 
				                 this.TargetX, this.TargetY, this.TargetZ, 
				                 this.UpX, this.UpY, this.UpZ);

			GL.MatrixMode (MatrixMode.Modelview);
			GL.LoadMatrix (ref lookat);

			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			this.Render (e);

			SwapBuffers ();
		}

		#endregion

		#region OnResize

		protected override void OnResize (EventArgs e)
		{
			GL.Viewport (ClientRectangle);

			float aspect = this.ClientSize.Width / (float)this.ClientSize.Height;

			Matrix4 projection_matrix;
			Matrix4.CreatePerspectiveFieldOfView (this.FOVY, aspect, this.ZNear, this.ZFar, out projection_matrix);

			GL.MatrixMode (MatrixMode.Projection);
			GL.LoadMatrix (ref projection_matrix);
		}

		#endregion

		#region Protected methods

		/// <summary>
		/// Call after you changed, fovy, z-near/z-far
		/// </summary>
		protected void UpdatePerspectiveFieldOfView ()
		{
			this.OnResize (null);
		}

		#endregion
	}
}

