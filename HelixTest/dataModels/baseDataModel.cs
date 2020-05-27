using System;
using PropertyChanged;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf.SharpDX.Model;
using Geometry3D = HelixToolkit.Wpf.SharpDX.Geometry3D;
using HelixToolkit.Wpf.SharpDX;
using DiffuseMaterial = HelixToolkit.Wpf.SharpDX.DiffuseMaterial;

namespace HelixTest
{
    /// <summary>
    /// Base data model for all Helix Geometry classes
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class baseDataModel : ObservableObject
    {
        public Geometry3D helixGeometry { get; set; }
        public TranslateTransform3D transform { get; set; } 
        public projectMaterials material { get; set; }
    }
}