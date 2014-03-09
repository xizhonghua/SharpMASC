using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using SharpMASC.Display;
using SharpMASC.Origami.Model;
using System.IO;

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

		public DisplayConfig DC { get; private set; }

		public Config Config { get; private set; }

		#endregion

		#region Fields

		public int currentModelIndex;

		#endregion

		#region Private Drawing

		void DrawAll ()
		{
			this.DrawFaces ();
			this.DrawCreases ();
			this.DrawBoundarys ();
			this.DrawFaceIds ();
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

		void DrawCreases ()
		{
			GL.Begin (PrimitiveType.Lines);
			Origami.Creases.ForEach (DrawCrease);
			GL.End ();
		}

		void DrawCrease (Crease c)
		{
			GL.LineWidth (2.0f);
			if (c.IsMountain) {
				GL.Color3 (1.0f, 0, 0);
			} else if (c.IsValley) {
				GL.Color3 (0, 0, 1.0f);
			} else {
				GL.Color3 (0.5f, 0.5f, 0.5f);
				GL.LineWidth (1.0f);
			}

			if (c.IsAssistant && !this.DC.ShowAssistantCreases)
				return;

			GL.Vertex3 (c.V1.Position);
			GL.Vertex3 (c.V2.Position);
		}

		void DrawBoundarys ()
		{
			GL.Begin (PrimitiveType.Lines);
			GL.Color3 (0.3, 0.3, 0.3);
			GL.LineWidth (2.0f);

			Origami.Faces.ForEach (f => {
				var v0 = f.Vertices [0];
				var v1 = f.Vertices [1];
				var v2 = f.Vertices [2];

				if (!v0.IsRealVertex && !v1.IsRealVertex) {
					GL.Vertex3 (v0.Position);
					GL.Vertex3 (v1.Position);
				}
				if (!v1.IsRealVertex && !v2.IsRealVertex) {
					GL.Vertex3 (v1.Position);
					GL.Vertex3 (v2.Position);
				}
				if (!v2.IsRealVertex && !v0.IsRealVertex) {
					GL.Vertex3 (v2.Position);
					GL.Vertex3 (v0.Position);
				}
			});
			GL.End ();
		}

		void DrawFaceIds ()
		{
            
		}

		#endregion

		public MainWindow (Config config)
		{
			this.Config = config;
		}

		#region Overloaded Protected Methods

		protected override void OnUpdateFrame (FrameEventArgs e)
		{
			// your code here

			if (Keyboard [Key.Comma]) {
				DC.CurrentFrame++;
			}
			if (Keyboard [Key.Period]) {
				DC.CurrentFrame--;
			}

			base.OnUpdateFrame (e);
		}

		protected override void OnKeyPress (KeyPressEventArgs e)
		{
			switch (e.KeyChar) {
			case 'a':
				DC.ShowAssistantCreases = !DC.ShowAssistantCreases;
				break;
			case 't':
				Origami.FoldToGoal ();
				break;
			case 'r':
				Origami.FoldToInital ();
				break;
			case '[':
				DC.CurrentFrame++;
				break;
			case ']':
				DC.CurrentFrame--;
				break;
			case '?':
				this.PrintGUIKeys ();
				break;
			case 'M':
				var filename = string.Format ("{0}_{1}_{2}.obj", Path.GetFileNameWithoutExtension (Origami.Config.ModelPath), DC.CurrentFrame, DC.TotalFrame);
				Origami.DumpObj (filename);
				Console.WriteLine ("Obj file saved to: {0}", filename);
				break;
			case 'n':
				if (this.currentModelIndex < Config.Models.Count - 1) {
					this.currentModelIndex++;
					this.LoadModel ();
				}
				break;
			case 'p':
				if (this.currentModelIndex > 0) {
					this.currentModelIndex--;
					this.LoadModel ();
				}
				break;
			}
			base.OnKeyPress (e);
		}

		protected override void Init ()
		{
			base.Init ();

			// change the backgroun color
			this.BackgroundColor = Color.Gray;
			this.LoadModel ();
		}

		#endregion

		void HandleCurrentFrameChanged (object sender, EventArgs e)
		{
			this.Origami.FoldToPercent (DC.Percent);
			this.UpdateWindownTitle ();
		}

		void LoadModel ()
		{
			var entry = this.Config.Models [currentModelIndex];

			this.DC = new DisplayConfig ();

			this.Origami = new RigidOrigami (entry);

			this.Origami.Build (entry.ModelPath);

			if (!string.IsNullOrEmpty (entry.TrajectoriesPath)) {
				this.Origami.LoadTrajectories (entry.TrajectoriesPath);
				this.DC.TotalFrame = this.Origami.FoldingPath.Count;
			}

			this.DC.CurrentFrameChanged += HandleCurrentFrameChanged;

			this.UpdateWindownTitle ();
			this.ResetCamera ();
		}

		void UpdateWindownTitle ()
		{
			this.Title = string.Format ("[{0}/{1}] Origami {2} Steps:{3}/{4}", currentModelIndex + 1, Config.Models.Count, Origami.Config.ModelPath, DC.CurrentFrame, DC.TotalFrame);
		}

		void PrintGUIKeys ()
		{
			Console.WriteLine ("-- Origami --");
			Console.WriteLine ("? : Display this message");
			Console.WriteLine ("n: Next model");
			Console.WriteLine ("p: Previous model");
			Console.WriteLine ("-- Folding --");
			Console.WriteLine (", : Fold");
			Console.WriteLine (". : Unfold");
			Console.WriteLine ("[ : Fold (one step)");
			Console.WriteLine ("] : Unfold (one step)");
			Console.WriteLine ("t : Folding to goal state");
			Console.WriteLine ("r : Folding to initial state");
			Console.WriteLine ("-- Display --");
			Console.WriteLine ("a : Toggle showing assistant creases");
			Console.WriteLine ("-- Dumping --");
			Console.WriteLine ("M: Dump current state to obj file");
			//.WriteLine ("D: Dump deformation");
		}
	}
}