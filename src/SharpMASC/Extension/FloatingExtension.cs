using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMASC.Extension
{
    public static class FloatingExtension
    {
        public static double ToRad(this double self)
        {
            return self / 180.0 * Math.PI;
        }

        public static double ToDegree(this double self)
        {
            return self / Math.PI * 180.0;
        }
    }
}
