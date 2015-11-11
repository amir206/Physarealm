﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Physarealm.Analysis
{
    public class FieldAnalysisMeshComponent :AbstractFieldAnalysisComponent
    {
        /// <summary>
        /// Initializes a new instance of the FieldAnalysisMeshComponent class.
        /// </summary>
        public FieldAnalysisMeshComponent()
            : base("Field AnalysisMesh", "FAMesh",
                "Description",
                null, "E1068216-44E9-4A4A-861E-E978A60378B1")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            base.RegisterInputParams(pManager);
            pManager.AddIntegerParameter("Z", "z", "Z", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Analysis Mesh", "AMesh", "Analysis Mesh", GH_ParamAccess.item);
        }
        protected override bool GetInputs(IGH_DataAccess da)
        {
            return true;
        }
        protected override void SetOutputs(IGH_DataAccess da)
        {

        }
    }
}