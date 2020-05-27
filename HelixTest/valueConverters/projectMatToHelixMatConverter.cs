using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using HelixToolkit.Wpf.SharpDX;

namespace HelixTest
{
    /// <summary>
    /// Converter to convert project materials to helix materials
    /// </summary>
    [ValueConversion(typeof(object), typeof(Material))]
    public class projectMatToHelixMatConverter : IValueConverter
    {
        public static projectMatToHelixMatConverter instance = new projectMatToHelixMatConverter();

        public static object Convert(object value)
        {
            return new projectMatToHelixMatConverter().Convert(value, typeof(Material), null, CultureInfo.CurrentCulture);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {

                #region project materials - for use by user to assign to panels
                case projectMaterials.None:
                    return null;
                case projectMaterials.Default:
                    return DiffuseMaterials.White;
                case projectMaterials.Red:
                    return DiffuseMaterials.Red;
                case projectMaterials.Green:
                    return DiffuseMaterials.Green;
                case projectMaterials.Blue:
                    return DiffuseMaterials.Blue;
                case projectMaterials.Indigo:
                    return PhongMaterials.Indigo;
                case projectMaterials.Emerald:
                    return PhongMaterials.Emerald;
                case projectMaterials.Arrows:
                    return staticMaterialCollection.grainDirectionMaterial;

                #endregion

                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
