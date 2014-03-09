using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Input;
using OpenTK.Graphics;

namespace SharpMASC.Display
{
	public abstract class WindowBase : GameWindow
	{
		#region Propertry

		public Camera Camera { get; protected set; }

		public Color BackgroundColor { get; protected set; }

		protected bool MouseButtonDown { get; set; }

		protected MouseButton MouseButton { get; set; }

		protected Point MouseDownPosition { get; set; }

		protected Vector3 Rotation { get; set; }

		protected Vector3 Translation { get; set; }
		// center of mass
		protected Vector3 COM { get; set; }
		// radius of the model
		protected float R { get; set; }

		#endregion

		public WindowBase () : base (800, 600, new GraphicsMode (32, 24, 0, 8))
		{

		}

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
			this.Camera = new Camera ();

			this.COM = new Vector3 ();
			this.R = 1.0f;

			this.ResetCamera ();

			this.BackgroundColor = Color.MidnightBlue;

			this.Mouse.ButtonDown += HandleButtonDown;
			this.Mouse.ButtonUp += HandleButtonUp;
			this.Mouse.Move += HandleMouseMove;
		}

		protected virtual void ResetCamera ()
		{
			this.Camera.UpdateZ (this.R * 2.5f);
			this.Translation = Vector3.Zero;
			this.Rotation = new Vector3 (0.01f, 0, 0);
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

			if (Keyboard [OpenTK.Input.Key.Left]) {
				this.Translation += new Vector3 (-0.01f, 0, 0);
			}

			if (Keyboard [OpenTK.Input.Key.Right]) {
				this.Translation += new Vector3 (0.01f, 0, 0);
			}

			if (Keyboard [OpenTK.Input.Key.Up]) {
				this.Translation += new Vector3 (0, 0.01f, 0);
			}

			if (Keyboard [OpenTK.Input.Key.Down]) {
				this.Translation += new Vector3 (0, -0.01f, 0);
			}
				
		}

		#endregion

		#region OnRenderFrame

		protected override void OnRenderFrame (FrameEventArgs e)
		{
			Matrix4 lookat = Matrix4.LookAt (this.Camera.Eye, this.Camera.Target, this.Camera.Up);

			GL.MatrixMode (MatrixMode.Modelview);
			GL.LoadMatrix (ref lookat);

			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.PushMatrix ();

			GL.Translate (this.Translation);

			GL.Rotate (this.Rotation.X, 1, 0, 0);
			GL.Rotate (this.Rotation.Y, 0, 1, 0);

			this.Render (e);

			GL.PopMatrix ();

			SwapBuffers ();
		}

		#endregion

		#region OnResize

		protected override void OnResize (EventArgs e)
		{
			base.OnResize (e);

			GL.Viewport (ClientRectangle);

			float aspect = this.ClientSize.Width / (float)this.ClientSize.Height;

			this.Camera.Aspect = aspect;
		}

		#endregion

		#region Protected methods

		#region MouseEvents

		protected virtual void HandleButtonDown (object sender, MouseButtonEventArgs e)
		{
			this.MouseButtonDown = true;
			this.MouseButton = e.Button;
			this.MouseDownPosition = e.Position;
		}

		protected virtual void HandleButtonUp (object sender, MouseButtonEventArgs e)
		{
			this.MouseButtonDown = false;
		}

		protected virtual void HandleMouseMove (object sender, MouseMoveEventArgs e)
		{
			if (!this.MouseButtonDown)
				return;

			switch (this.MouseButton) {

			case MouseButton.Left:
				this.Rotation += new Vector3 (e.YDelta / 2.0f, e.XDelta / 2.0f, 0);
				break;
			case MouseButton.Right:
				this.Camera.Move (0, 0, e.YDelta / 10.0f);
				break;
			case MouseButton.Middle:
				break;
			}
		}

		#endregion

		#endregion
	}
}

