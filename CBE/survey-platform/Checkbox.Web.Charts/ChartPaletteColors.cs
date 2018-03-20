using System.Collections.Generic;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;

namespace Checkbox.Web.Charts
{
    /// <summary>
    /// Simple class for accessing colors specific to dundas palettes
    /// </summary>
    public static class ChartPaletteColors
    {
        private static readonly Dictionary<string, Color[]> _paletteColors;

        /// <summary>
        /// Private static constructor.  Initialize the palette colors.
        /// </summary>
        static ChartPaletteColors()
        {
            lock (typeof(ChartPaletteColors))
            {
                _paletteColors = new Dictionary<string, Color[]>();

                //Default
                _paletteColors["Default"] =
                    new[] {
                        Color.Green,
                        Color.Blue,
                        Color.Purple,
                        Color.Lime,
                        Color.Fuchsia,
                        Color.Teal,
                        Color.Yellow,
                        Color.Gray,
                        Color.Aqua,
                        Color.Navy,
                        Color.Maroon,
                        Color.Red,
                        Color.Olive,
                        Color.Silver,
                        Color.Tomato,
                        Color.Moccasin
                    };

                //EarthTones
                _paletteColors[ChartColorPalette.EarthTones.ToString()] =
                    new[] {
                        Color.FromArgb(255, 128, 0),
                        Color.DarkGoldenrod,
                        Color.FromArgb(192, 64, 0),
                        Color.OliveDrab,
                        Color.Peru,
                        Color.FromArgb(192, 192, 0),
                        Color.ForestGreen,
                        Color.Chocolate,
                        Color.Olive,
                        Color.LightSeaGreen,
                        Color.SandyBrown,
                        Color.FromArgb(0, 192, 0),
                        Color.DarkSeaGreen,
                        Color.Firebrick,
                        Color.SaddleBrown,
                        Color.FromArgb(192, 0, 0)
                    };

                //SemiTransparent
                _paletteColors[ChartColorPalette.SemiTransparent.ToString()] =
                    new[] {
                        Color.FromArgb(150, 255, 0, 0),
                        Color.FromArgb(150, 0, 255, 0),
                        Color.FromArgb(150, 0, 0, 255),
                        Color.FromArgb(150, 255, 255, 0),
                        Color.FromArgb(150, 0, 255, 255),
                        Color.FromArgb(150, 255, 0, 255),
                        Color.FromArgb(150, 170, 120, 20),
                        Color.FromArgb(80, 255, 0, 0),
                        Color.FromArgb(80, 0, 255, 0),
                        Color.FromArgb(80, 0, 0, 255),
                        Color.FromArgb(80, 255, 255, 0),
                        Color.FromArgb(80, 0, 255, 255),
                        Color.FromArgb(80, 255, 0, 255),
                        Color.FromArgb(80, 170, 120, 20),
                        Color.FromArgb(150, 100, 120, 50),
                        Color.FromArgb(150, 40, 90, 150)
                    };

                //Light
                _paletteColors[ChartColorPalette.Light.ToString()] =
                    new[] {
                        Color.Lavender,
                        Color.LavenderBlush,
                        Color.PeachPuff,
                        Color.LemonChiffon,
                        Color.MistyRose,
                        Color.Honeydew,
                        Color.AliceBlue,
                        Color.WhiteSmoke,
                        Color.AntiqueWhite,
                        Color.LightCyan
                    };

                //Excel
                _paletteColors[ChartColorPalette.Excel.ToString()] =
                    new[] {
                        Color.FromArgb(153,153,255),
                        Color.FromArgb(153,51,102),
                        Color.FromArgb(255,255,204),
                        Color.FromArgb(204,255,255),
                        Color.FromArgb(102,0,102),
                        Color.FromArgb(255,128,128),
                        Color.FromArgb(0,102,204),
                        Color.FromArgb(204,204,255),
                        Color.FromArgb(0,0,128),
                        Color.FromArgb(255,0,255),
                        Color.FromArgb(255,255,0),
                        Color.FromArgb(0,255,255),
                        Color.FromArgb(128,0,128),
                        Color.FromArgb(128,0,0),
                        Color.FromArgb(0,128,128),
                        Color.FromArgb(0,0,255)
                    };

                //Berry
                _paletteColors[ChartColorPalette.Berry.ToString()] =
                    new[] {
                        Color.BlueViolet,
                        Color.MediumOrchid,
                        Color.RoyalBlue,
                        Color.MediumVioletRed,
                        Color.Blue,
                        Color.BlueViolet,
                        Color.Orchid,
                        Color.MediumSlateBlue,
                        Color.FromArgb(192, 0, 192),
                        Color.MediumBlue,
                        Color.Purple
                    };

                //Chocolate
                _paletteColors[ChartColorPalette.Chocolate.ToString()] =
                    new[] {
                        Color.Sienna,
                        Color.Chocolate,
                        Color.DarkRed,
                        Color.Peru,
                        Color.Brown,
                        Color.SandyBrown,
                        Color.SaddleBrown,
                        Color.FromArgb(192, 64, 0),
                        Color.Firebrick,
                        Color.FromArgb(182, 92, 58)
                    };

                //Fire
                _paletteColors[ChartColorPalette.Fire.ToString()] =
                    new[] {
                        Color.Gold,
                        Color.Red,
                        Color.DeepPink,
                        Color.Crimson,
                        Color.DarkOrange,
                        Color.Magenta,
                        Color.Yellow,
                        Color.OrangeRed,
                        Color.MediumVioletRed,
                        Color.FromArgb(221, 226, 33)
                    };

                //SeaGreen
                _paletteColors[ChartColorPalette.SeaGreen.ToString()] =
                    new[] {
                        Color.SeaGreen,
                        Color.MediumAquamarine,
                        Color.SteelBlue,
                        Color.DarkCyan,
                        Color.CadetBlue,
                        Color.MediumSeaGreen,
                        Color.MediumTurquoise,
                        Color.LightSteelBlue,
                        Color.DarkSeaGreen,
                        Color.SkyBlue
                    };

                //Dundas
                _paletteColors["Dundas"] =
                    new[] {
                        Color.FromArgb(65, 140, 240),
                        Color.FromArgb(252, 180, 65),
                        Color.FromArgb(224, 64, 10),
                        Color.FromArgb(5, 100, 146),
                        Color.FromArgb(191, 191, 191),
                        Color.FromArgb(26, 59, 105),
                        Color.FromArgb(255, 227, 130),
                        Color.FromArgb(18, 156, 221),
                        Color.FromArgb(202, 107, 75),
                        Color.FromArgb(0, 92, 219),
                        Color.FromArgb(243, 210, 136),
                        Color.FromArgb(80, 99, 129),
                        Color.FromArgb(241, 185, 168),
                        Color.FromArgb(224, 131, 10),
                        Color.FromArgb(120, 147, 190)
                    };
            }
        }

        /// <summary>
        /// Get the colors for a predefined palette
        /// </summary>
        /// <param name="palette"></param>
        /// <returns></returns>
        public static Color[] GetColorList(string palette)
        {
            if (_paletteColors.ContainsKey(palette))
            {
                return _paletteColors[palette];
            }
            
            return new Color[] { };
        }
    }
}
