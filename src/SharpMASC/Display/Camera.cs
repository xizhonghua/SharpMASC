using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SharpMASC.Display
{
	public class Camera
	{
		#region Fields

		Vector3 eye;
		Vector3 up;
		float tilt;
		float spin;
		float fov;
		float zNear;
		float zFar;
		float aspect;

		#endregion

		#region Property

		public Vector3 Eye {
			get { return this.eye; }
		}

		public Vector3 Target {
			get { return this.eye + this.Forward (); }
		}

		public Vector3 Up {
			get { return this.up; }
		}

		public float Tilt {
			get { return this.tilt; }
			set { this.tilt = value; }
		}

		public float Spin {
			get { return this.spin; }
			set { this.spin = value; }
		}

		public float FOV {
			get { return this.fov; }
			set {
				this.fov = value;
				this.updatePerspective ();
			}
		}

		public float ZNear {
			get {
				return this.zNear;
			}
			set {
				this.zNear = value;
				this.updatePerspective ();
			}
		}

		public float ZFar {
			get { return this.zFar; }
			set {
				this.zFar = value;
				this.updatePerspective ();
			}
		}

		public float Aspect {
			get { return this.aspect; }
			set {
				this.aspect = value;
				this.updatePerspective ();
			}
		}

		#endregion

		public Camera ()
		{
			this.eye = new Vector3 (0, 0, 16);
			this.fov = (float)Math.PI / 4;	// 45 degree
			this.up = new Vector3 (0, 1, 0);
			this.aspect = 1.0f;
			this.zNear = 0.1f;
			this.zFar = 10000f;
		}

		#region Public Methods

		public void updatePerspective ()
		{
			Matrix4 projection_matrix;
			Matrix4.CreatePerspectiveFieldOfView (this.FOV, this.Aspect, this.ZNear, this.ZFar, out projection_matrix);

			GL.MatrixMode (MatrixMode.Projection);
			GL.LoadMatrix (ref projection_matrix);
		}

		public void Move (float dx, float dy, float dz)
		{
			this.eye.X += dx;
			this.eye.Y += dy;
			this.eye.Z += dz;
		}

		public void UpdateZ (float z)
		{
			this.eye.Z = z;
		}

		public void Turn (float dTile, float dSpin)
		{
			this.tilt += dTile;
			this.spin += dSpin;
		}

		public Vector2 Heading ()
		{
			return new Vector2 ((float)Math.Cos (spin), (float)Math.Sin (spin));
		}

		public Vector3 Forward ()
		{
			var heading = Heading ();
			var forward = new Vector3 (heading.Y, 0, -heading.X);
			forward.Y = (float)Math.Tan (tilt);
			forward.Normalize ();
			return forward;
		}

		#endregion
	}
}
	