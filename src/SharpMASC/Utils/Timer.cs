using System;
using System.Diagnostics;

namespace SharpMASC.Utils
{
	public class Timer
	{
		#region Field

		private TimeSpan _timeElapsed;
		private Stopwatch _stopwatch;

		#endregion

		#region Property

		public bool Started { get; private set; }

		/// <summary>
		/// Gets the time elapsed. (in million seconds)
		/// </summary>
		/// <value>The time elapsed.</value>
		public double TimeElapsed {
			get {
				return _timeElapsed.TotalMilliseconds;
			}
		}

		#endregion

		#region Constructor

		public Timer ()
		{
			this.Started = false;
			this._stopwatch = new Stopwatch ();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Start the timer
		/// </summary>
		public void Start ()
		{
			if (this.Started)
				return;

			this.Started = true;
			this._stopwatch.Reset ();
			this._stopwatch.Start ();
		}

		public void Reset ()
		{
			this.Started = false;
			this._timeElapsed = TimeSpan.FromMilliseconds (0);
		}

		/// <summary>
		/// Stop the timer
		/// </summary>
		public void Stop ()
		{
			if (!this.Started)
				return;

			this._stopwatch.Stop ();
			this._timeElapsed += this._stopwatch.Elapsed;
		}

		#endregion
	}
}

