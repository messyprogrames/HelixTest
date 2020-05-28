using System;
using System.Collections.Generic;
using System.Linq;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;

namespace HelixTest
{
    public class cubeDataModel : baseDataModel
    {
        // test - makes cubes
        public cubeDataModel(Vector3 center, double size)
        {
            material = projectMaterials.Blue;

            // create mesh
            MeshBuilder mb = new MeshBuilder(true, true, false);
            mb.AddBox(center, size, size, size);

            helixGeometry = mb.ToMeshGeometry3D();
        }
    }
}