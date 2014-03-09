using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMASC.Origami.Display
{
    public class DisplayConfig
    {
        #region Fields
        private int currentFrame;
        #endregion

        #region Property

        public bool ShowNumbers { get; set; }
        public bool ShowAssistantCreases { get; set; }

        public int CurrentFrame { 
        get {
            return currentFrame;
        } set {
            if(value < 0) currentFrame = 0;
            else if(value > this.TotalFrame) currentFrame = TotalFrame;
            else currentFrame = value;
        }}

        public int TotalFrame { get; set; }

        public double Percent
        {
            get
            {
                return CurrentFrame *1.0 / TotalFrame;
            }
        }
        #endregion

        public DisplayConfig()
        {             
            this.currentFrame = 0;
            this.TotalFrame=120;
        }
    }
}
