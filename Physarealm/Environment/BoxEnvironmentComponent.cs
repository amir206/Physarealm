﻿using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Physarealm.Environment
{
    public class BoxEnvironmentComponent : AbstractEnvironmentComponent
    {
        private int x_count;
        private int y_count;
        private int z_count;
        private Box box;
        /// <summary>
        /// Initializes a new instance of the BoxEnvironmentComponent class.
        /// </summary>
        public BoxEnvironmentComponent()
            : base("Box Environment", "BoxEnvir",
                "Description",null
                , "CBE2B1BA-1C6E-473E-8D4A-F724290A9BED")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("XResolution", "xres", "subdivision on x-axis", GH_ParamAccess.item, 100);
            pManager.AddIntegerParameter("YResolution", "yres", "subdivision on y-axis", GH_ParamAccess.item, 100);
            pManager.AddIntegerParameter("ZResolution", "zres", "subdivision on z-axis", GH_ParamAccess.item, 100);
            pManager.AddBoxParameter("Box", "B", "Box", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            base.RegisterOutputParams(pManager);
            pManager.AddPointParameter("pts", "pt", "pt", GH_ParamAccess.list);
            //pManager.AddGenericParameter("BoxEnvironment", "BoxEnv", "Box Environment", GH_ParamAccess.item);
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>

        protected override bool GetInputs(IGH_DataAccess da)
        {
            if (!da.GetData(nextInputIndex++, ref x_count)) return false;
            if (!da.GetData(nextInputIndex++, ref y_count)) return false;
            if (!da.GetData(nextInputIndex++, ref z_count)) return false;
            if (!da.GetData(nextInputIndex++, ref box)) return false;
            return true;
        }

        protected override void SetOutputs(IGH_DataAccess da)
        {
            BoxEnvironmentType environment = new BoxEnvironmentType(box.BoundingBox, x_count, y_count, z_count);
            //environment.setContainer(null);
            da.SetData(nextOutputIndex++, environment);
            da.SetDataList(1, environment.positions);
        }
    }
}