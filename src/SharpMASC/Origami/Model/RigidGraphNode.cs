﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMASC.Origami.Model
{
    public class RigidGraphNode
    {
        #region Property
        public Face FromFace { get; private set; }
        public Face TargetFace { get; private set; }
        public Crease Crease { get; private set; }
        public Vertex WitnessVertex { get; private set; }

        public double PlaneAngle { get; private set; }
        #endregion

        public RigidGraphNode(Crease c, Face fromFace, Face targetFace, Vertex witnessVertex)
        {
            this.Crease = c;

            this.FromFace = fromFace;
            this.TargetFace = targetFace;

            this.WitnessVertex = witnessVertex;

            this.PlaneAngle = c.GetPlaneAngle(witnessVertex);
        }

        
    }
}
