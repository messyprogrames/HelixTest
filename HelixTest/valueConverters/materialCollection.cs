using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf.SharpDX;
using Color = System.Windows.Media.Color;
using Color4 = SharpDX.Color4;
using Colors = System.Windows.Media.Colors;
using Element3D = HelixToolkit.Wpf.SharpDX.Element3D;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Vector3 = SharpDX.Vector3;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using SharpDX.Direct3D11;
using System.Globalization;
using System.Windows.Data;
using System.IO;
using SharpDX;
using System.IO;
using System.Windows.Resources;
using System.Windows;
using System.Resources;
using System.Reflection;
using SharpDX.Direct2D1;
using SharpDX.Direct2D1.Effects;
using BitmapSource = System.Windows.Media.Imaging.BitmapSource;
using DeviceContext = SharpDX.Direct2D1.DeviceContext;

namespace HelixTest
{

    /// <summary>
    /// Materials used by the user and assigned to panels.
    /// </summary>
    public enum projectMaterials
    {
        None,
        Default,
        Red,
        Green,
        Blue,
        Indigo,
        Emerald,
        Arrows
    }

    /// <summary>
    /// Materials used by application.
    /// </summary>
    public enum appMaterials
    {
        defaultPanel,
        defaultFace,
        selectedFace
    }

    static class staticMaterialCollection
    {
        #region app materials

        public static DiffuseMaterial diffuseHighlight = new DiffuseMaterial() { DiffuseColor = new Color4(91, 198, 208, 255) };

        public static Material defaultPanelMaterial = new DiffuseMaterial { DiffuseColor = new Color4(235, 235, 235, 255) };
        public static Material defaultFaceMaterial = DiffuseMaterials.LightGray;
        public static Material selectedFaceMaterial = DiffuseMaterials.BlanchedAlmond;

        #endregion

        #region project materials - selected by user

        #region panel direction material

        //private static MemoryStream mstr = new MemoryStream();
        private static Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("HelixTest.Resources.test.png");

        //str.CopyTo(mstr);
        private static TextureModel tm = new TextureModel(str);
        //string directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); // set temporary directory
        //string texture = directory + "\\" + "test.png"; // set temporary filename

        //mstr = baseViewModel.loadFileToMemory(texture);

        public static Material grainDirectionMaterial = new PhongMaterial
        {
            AmbientColor = Colors.BlueViolet.ToColor4(),
            DiffuseColor = Colors.BlueViolet.ToColor4(),
            SpecularColor = Colors.BlueViolet.ToColor4(),
            SpecularShininess = 100f,
            DiffuseMap = tm
        };

        #endregion 
        #endregion

        #region materials used by curves from revit

        public static DiffuseMaterial panelMaterial { get; private set; } = new DiffuseMaterial() { DiffuseColor = new Color4(0, 0, 255, 255) };
        public static DiffuseMaterial cornerMaterial { get; private set; } = new DiffuseMaterial() { DiffuseColor = new Color4(250, 183, 0, 255) };
        public static DiffuseMaterial fasciaMaterial { get; private set; } = new DiffuseMaterial() { DiffuseColor = new Color4(255, 0, 0, 255) };
        public static DiffuseMaterial openingMaterial { get; private set; } = new DiffuseMaterial() { DiffuseColor = new Color4(0, 255, 0, 255) };
        public static DiffuseMaterial baseMaterial { get; private set; } = new DiffuseMaterial() { DiffuseColor = new Color4(171, 0, 250, 255) };

        #endregion

        #region colors

        public static Color red = Color.FromRgb(200, 0, 0);
        public static Color edgeColorByDefault = Color.FromRgb(20, 20, 20);
        public static Color edgeColorOnSelection = Colors.OrangeRed;
        public static Color transparent = Colors.Transparent;

        #endregion
    }

}
