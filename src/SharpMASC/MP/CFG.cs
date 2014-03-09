using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMASC.MP
{
    public class CFG
    {
        #region Property
        public double this[int index]
        {
            get { 
                return data[index];
            }
            set
            {
                data[index] = value;
            }
        }

        public double NormalSqr
        {
            get
            {
                var nsqr = 0.0;
                foreach (var c in this.data)
                {
                    nsqr += c * c;
                }
                return nsqr;
            }
        }

        public double Normal
        {
            get
            {
                return Math.Sqrt(this.NormalSqr);
            }
        }

        public int DOF { get; private set;}
        
        #endregion

        #region Fields
        protected double[] data;
        #endregion

        #region Constructor
        public CFG(int dof)
        {
            this.data = new double[dof];
            this.DOF = dof;
        }               
        
        public CFG(CFG cfg)
        {
            this.data = new double[cfg.DOF];
            cfg.data.CopyTo(this.data, 0);
        }

        public CFG(IEnumerable<double> cfg)
        {
            this.data = cfg.ToArray();
            this.DOF = data.Length;
        }

        public CFG(IEnumerable<string> cfg)
        {
            var array = cfg.ToArray();
            this.DOF = array.Length;
            this.data = new double[this.DOF];
            for (var i = 0; i < this.DOF; ++i)
                this.data[i] = double.Parse(array[i]);
        }

        #endregion

        #region Public Methods
        public CFG Clone()
        {
            return new CFG(this);
        }

        public List<double> ToList()
        {
            var list = new List<double>(this.data);
            return list;
        }

        public double[] ToArray()
        {
            var array = new double[this.DOF];
            this.data.CopyTo(array, 0);
            return array;
        }
        #endregion

        #region Static Methods

        public static CFG operator -(CFG cfg)
        {
            var c = new CFG(cfg);
            for(var i=0;i<c.data.Length;i++)
                c.data[i] = - c.data[i];
            return c;
        }

        public static CFG operator +(CFG l, CFG r)
        {        
            if(l.DOF != r.DOF)
                throw new ArgumentException(string.Format("DOF does not match! l = {0} r = {1}", l.DOF, r.DOF));

            var c = new CFG(l.DOF);

            for(var i=0;i<l.DOF;i++)
                c.data[i] = l.data[i] + r.data[i];

            return c;
        }

        public static CFG operator -(CFG l, CFG r)
        {
            if (l.DOF != r.DOF)
                throw new ArgumentException(string.Format("DOF does not match! l = {0} r = {1}", l.DOF, r.DOF));

            var c = new CFG(l.DOF);

            for (var i = 0; i < l.DOF; i++)
                c.data[i] = l.data[i] - r.data[i];

            return c;
        }

        public static CFG operator *(CFG cfg, double s)
        {
            var c = new CFG(cfg.DOF);
            for(var i=0;i<c.DOF;i++)
                c[i]*=s;
            return c;        
        }

        public static CFG operator *(double s, CFG cfg)
        {
            return cfg*s;
        }

        public static CFG operator /(CFG cfg, double s)
        {
            if(s!=0)
            {
                var t = 1.0/s;
                return cfg*t;
            }
            return new CFG(cfg.DOF);
        }

        #endregion
    }
}
