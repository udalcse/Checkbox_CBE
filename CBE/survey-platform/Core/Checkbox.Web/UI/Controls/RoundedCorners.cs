using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Prezza.Framework.Data;
using Checkbox.Web.UI.Utility;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Container control that supports framing it's children in a table with rounded corners
    /// </summary>
    [ParseChildren(ChildrenAsProperties = false)]
    public class RoundedCorners : CompositeControl
    {
        /// <summary>
        /// Corner type enumeration
        /// </summary>
        protected enum Corners
        {
            /// <summary>
            /// Upper left
            /// </summary>
            UpperLeft,

            /// <summary>
            /// Upper right
            /// </summary>
            UpperRight,

            /// <summary>
            /// Bottom right
            /// </summary>
            BottomRight,

            /// <summary>
            /// Bottom left
            /// </summary>
            BottomLeft
        }

        #region Member Variables

        private BorderStyle _borderStyle;
        private Unit _borderWidth;
        private Color _borderColor;
        private Color _backColor;
        private bool _isInDesignTime;

        #endregion


        #region Public Properties

        /// <summary>
        /// Specify whether a rounded bottom should be displayed
        /// </summary>
        [Bindable(true),
         Category("Appearance"),
         DefaultValue(true),
         Description("Determines whether a rounded bottom should be displayed.")]
        public bool RoundedBottom { get; set; }

        /// <summary>
        /// Specify whether a rounded bottom should be displayed
        /// </summary>
        [Bindable(true),
         Category("Appearance"),
         DefaultValue(true),
         Description("Determines whether a rounded top should be displayed.")]
        public bool RoundedTop { get; set; }


        /// <summary>
        /// Background color
        /// </summary>
        [Bindable(true),
         Category("Appearance"),
         Description("The color of the background behind the control."),
         NotifyParentProperty(true)]
        public Color BackgroundBackColor { get; set; }

        /// <summary>
        /// Get the corner height
        /// </summary>
        protected static int CornerHeight
        {
            get { return 13; }
        }

        /// <summary>
        /// Get the corner width
        /// </summary>
        protected static int CornerWidth
        {
            get { return 13; }
        }

        #endregion

        #region Overridden WebControl Methods

        /// <summary>
        /// Override add attributes to render
        /// </summary>
        /// <param name="writer"></param>
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            // Save the border & backcolor information and clear it out
            _borderStyle = BorderStyle;
            _borderWidth = BorderWidth;
            _borderColor = BorderColor;
            _backColor = BackColor;

            BorderStyle = BorderStyle.None;
            BorderWidth = Unit.Empty;
            BorderColor = Color.Empty;
            BackColor = Color.Empty;

            // Add the base attributes
            base.AddAttributesToRender(writer);

            writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0px");
            writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0px");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            _isInDesignTime = Site != null && Site.DesignMode;

            base.RenderBeginTag(writer);

            string styleForSides;

            /****************** Add Top Row *****************/
            if (RoundedTop)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, CornerHeight + "px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, CornerWidth + "px");

                writer.RenderBeginTag(HtmlTextWriterTag.Td);

                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                writer.AddAttribute(HtmlTextWriterAttribute.Src, CreateCorner(Corners.UpperLeft));
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, CornerHeight + "px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, CornerWidth + "px");
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, "");	// Mark
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();

                writer.RenderEndTag();

                if (_backColor != Color.Empty)
                   writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorToHex(_backColor));

                writer.AddAttribute(HtmlTextWriterAttribute.Style, string.Format("border-top-style: {0};border-top-color: {1};border-top-width: {2};", _borderStyle, ColorToHex(_borderColor), _borderWidth));
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(Create1x1Gif());
                writer.RenderEndTag();

                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, CornerHeight + "px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, CornerWidth + "px");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);

                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                writer.AddAttribute(HtmlTextWriterAttribute.Src, CreateCorner(Corners.UpperRight));
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, CornerHeight + "px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, CornerWidth + "px");
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, "");	// Mark
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderEndTag();
            }
            else
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, (CornerHeight - _borderWidth.Value) + "px");
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                if (_backColor != Color.Empty)
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorToHex(_backColor));
                }
                styleForSides = string.Format("border-left-style: {0};border-left-color: {1};border-left-width: {2};", _borderStyle, ColorToHex(_borderColor), _borderWidth);
                styleForSides += string.Format("border-top-style: {0};border-top-color: {1};border-top-width: {2};", _borderStyle, ColorToHex(_borderColor), _borderWidth);
                writer.AddAttribute(HtmlTextWriterAttribute.Style, styleForSides);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(Create1x1Gif());
                writer.RenderEndTag();

                if (_backColor != Color.Empty)
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorToHex(_backColor));
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Style, string.Format("border-top-style: {0};border-top-color: {1};border-top-width: {2};", _borderStyle, ColorToHex(_borderColor), _borderWidth));
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(Create1x1Gif());
                writer.RenderEndTag();

                if (_backColor != Color.Empty)
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorToHex(_backColor));
                }
                styleForSides = string.Format("border-right-style: {0};border-right-color: {1};border-right-width: {2};", _borderStyle, ColorToHex(_borderColor), _borderWidth);
                styleForSides += string.Format("border-top-style: {0};border-top-color: {1};border-top-width: {2};", _borderStyle, ColorToHex(_borderColor), _borderWidth);
                writer.AddAttribute(HtmlTextWriterAttribute.Style, styleForSides);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(Create1x1Gif());
                writer.RenderEndTag();

                writer.RenderEndTag();
            }
            /************************************************/

            /****************** Add Middle Row *****************/
            styleForSides = string.Format("border-left-style: {0};border-left-color: {1};border-left-width: {2};", _borderStyle, ColorToHex(_borderColor), _borderWidth);

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            if (!Height.IsEmpty)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, (Height.Value - (2 * CornerHeight)) + "px");
            }
            // ----------------------------------------------------------------------------------------------

            if (_backColor != Color.Empty)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorToHex(_backColor));
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Style, styleForSides);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(Create1x1Gif());
            writer.RenderEndTag();

            styleForSides = string.Format("padding:{0};", Unit.Empty.Value + "px");
            if (_backColor != Color.Empty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorToHex(_backColor));
            if (ForeColor != Color.Empty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.Color, ColorToHex(ForeColor));

            writer.AddAttribute(HtmlTextWriterAttribute.Style, styleForSides);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            /************************************************/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void RenderEndTag(HtmlTextWriter writer)
        {
            // end the td
            writer.RenderEndTag();

            if (_backColor != Color.Empty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorToHex(_backColor));
            string styleForSides = string.Format("border-right-style: {0};border-right-color: {1};border-right-width: {2};", _borderStyle, ColorToHex(_borderColor), _borderWidth);
            // Bottom border is added regardless of RoundedBottom setting (SKM, 2/20/07)
            //if (!RoundedBottom)	// add bottom border
            //	styleForSides += string.Format("border-bottom-style: {0};border-bottom-color: {1};border-bottom-width: {2};", _borderStyle.ToString(), ColorToHex(_borderColor), _borderWidth.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Style, styleForSides);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(Create1x1Gif());
            writer.RenderEndTag();

            writer.RenderEndTag();

            /****************** Add Bottom Row *****************/
            if (RoundedBottom)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, CornerHeight + "px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, CornerWidth + "px");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);

                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                writer.AddAttribute(HtmlTextWriterAttribute.Src, CreateCorner(Corners.BottomLeft));
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, CornerHeight + "px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, CornerWidth + "px");
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, "");	// Mark
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();

                writer.RenderEndTag();

                if (_backColor != Color.Empty)
                    writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorToHex(_backColor));
                writer.AddAttribute(HtmlTextWriterAttribute.Style, string.Format("border-bottom-style: {0};border-bottom-color: {1};border-bottom-width: {2};", _borderStyle, ColorToHex(_borderColor), _borderWidth));
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(Create1x1Gif());
                writer.RenderEndTag();

                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, CornerHeight + "px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, CornerWidth + "px");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);

                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                writer.AddAttribute(HtmlTextWriterAttribute.Src, CreateCorner(Corners.BottomRight));
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, CornerHeight + "px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, CornerWidth + "px");
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, "");	// Mark
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();

                writer.RenderEndTag();

                writer.RenderEndTag();
            }
            else
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, (CornerHeight - _borderWidth.Value) + "px");
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                if (_backColor != Color.Empty)
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorToHex(_backColor));
                }
                styleForSides = string.Format("border-left-style: {0};border-left-color: {1};border-left-width: {2};", _borderStyle, ColorToHex(_borderColor), _borderWidth);
                styleForSides += string.Format("border-bottom-style: {0};border-bottom-color: {1};border-bottom-width: {2};", _borderStyle, ColorToHex(_borderColor), _borderWidth);
                writer.AddAttribute(HtmlTextWriterAttribute.Style, styleForSides);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(Create1x1Gif());
                writer.RenderEndTag();

                if (_backColor != Color.Empty)
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorToHex(_backColor));
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Style, string.Format("border-bottom-style: {0};border-bottom-color: {1};border-bottom-width: {2};", _borderStyle, ColorToHex(_borderColor), _borderWidth));
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(Create1x1Gif());
                writer.RenderEndTag();

                if (_backColor != Color.Empty)
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorToHex(_backColor));
                }
                styleForSides = string.Format("border-right-style: {0};border-right-color: {1};border-right-width: {2};", _borderStyle, ColorToHex(_borderColor), _borderWidth);
                styleForSides += string.Format("border-bottom-style: {0};border-bottom-color: {1};border-bottom-width: {2};", _borderStyle, ColorToHex(_borderColor), _borderWidth);
                writer.AddAttribute(HtmlTextWriterAttribute.Style, styleForSides);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(Create1x1Gif());
                writer.RenderEndTag();

                writer.RenderEndTag();
            }
            /************************************************/

            base.RenderEndTag(writer);
        }
        /// <summary>
        /// 
        /// </summary>
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Table;
            }
        }
        #endregion

        #region Corner Creating / Saving-Related Methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string Create1x1Gif()
        {
            // Do not generate the image if we are in the Designer
            if (_isInDesignTime)
            {
                return string.Empty;
            }

            List<int> imageIDs = DbUtility.FindImage("roundedCorner_1x1.gif", true);

            int imageID = imageIDs.Count == 0 ? Create1x1GifImage("roundedCorner_1x1.gif") : imageIDs[0];

            string imageUrl = Management.ApplicationManager.ApplicationRoot + "/ViewContent.aspx?ImageID=" + imageID;

            return string.Format("<img style=\"border-width:1px;height:1px;width:1px;\" src=\"{0}\" alt=\"\" />", imageUrl);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageName"></param>
        protected virtual int Create1x1GifImage(string imageName)
        {
            // Create the palette - used for quantization and transparency
            // The first palette color - 49/64/81 - is the transparent color.
            ArrayList palette = new ArrayList {Color.FromArgb(49, 64, 81)};

            // otherwise enter an arbitrary color

            // Create the corner bitmap
            Bitmap smallGif = new Bitmap(1, 1);
            Graphics gpx = Graphics.FromImage(smallGif);

            // Clear out the palette with the transparent or BackgroundBackColor color
            gpx.Clear((Color)palette[0]);

            // Quantize the GIF image
            PaletteQuantizer quantizer = new PaletteQuantizer(palette, false);
            Bitmap quantizedGif = quantizer.Quantize(smallGif);

            // save the image (if not in the designer)
            MemoryStream stream = new MemoryStream();

            quantizedGif.Save(stream, ImageFormat.Gif);

            int imageID = DbUtility.SaveImage(stream.ToArray(), "image/gif", null, imageName, Guid.NewGuid().ToString(),DateTime.Now, true);

            gpx.Dispose();
            smallGif.Dispose();
            quantizedGif.Dispose();

            return imageID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected virtual string CreateCorner(Corners c)
        {
            // Do not generate the image if we are in the Designer
            if (_isInDesignTime) return string.Empty;

            // compute the file name
            StringBuilder imageName = new StringBuilder(string.Empty, 75);

            // start with the corner abbreviation
            switch (c)
            {
                case Corners.UpperLeft:
                    imageName.Append("ul");

                    // now add the backcolor
                    imageName.AppendFormat(string.Concat(".", ColorToHex(_backColor).Replace("#", "")));
                    break;
                case Corners.UpperRight:
                    imageName.Append("ur");

                    // now add the backcolor
                    imageName.AppendFormat(string.Concat(".", ColorToHex(_backColor).Replace("#", "")));
                    break;
                case Corners.BottomLeft:
                    imageName.Append("bl");

                    // now add the backcolor
                    imageName.AppendFormat(string.Concat(".", ColorToHex(_backColor).Replace("#", "")));
                    break;
                case Corners.BottomRight:
                    imageName.Append("br");

                    // now add the backcolor
                    imageName.AppendFormat(string.Concat(".", ColorToHex(_backColor).Replace("#", "")));
                    break;
            }

            // add in the BackgroundBackColor
            imageName.Append(string.Concat(".", ColorToHex(BackgroundBackColor).Replace("#", "")));

            // now add in the borderColor
            imageName.AppendFormat(string.Concat(".", ColorToHex(_borderColor).Replace("#", "")));

            // add in the borderStyle
            imageName.Append(string.Concat(".", _borderStyle.ToString()));

            // add in the corner width and height
            imageName.AppendFormat(".{0}-{1}", CornerWidth, CornerHeight);

            // finally add in the borderWidth
            imageName.Append(".").Append(_borderWidth).Append(".gif");

            List<int> imageIDs = DbUtility.FindImage(imageName.ToString(), true);

            int imageID = imageIDs.Count > 0 ? imageIDs[0] : CreateRoundedCorner(c, imageName.ToString());

            return Management.ApplicationManager.ApplicationRoot + "/ViewContent.aspx?ImageID=" + imageID;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected virtual Color PickRandomColorBetween(Color c1, Color c2, int offset)
        {
            int r = Math.Min(Math.Min(c1.R, c2.R) + offset, 255);
            int g = Math.Min(Math.Min(c1.G, c2.G) + offset, 255);
            int b = Math.Min(Math.Min(c1.B, c2.B) + offset, 255);

            return Color.FromArgb(r, g, b);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="imageName"></param>
        protected virtual int CreateRoundedCorner(Corners c, string imageName)
        {
            // Create the palette - used for quantization and transparency
            // The first palette color - 49/64/81 - is the transparent color.
            ArrayList palette = new ArrayList();

            // if we have a specific BackgroundBackColor, use that
            if (BackgroundBackColor != Color.Empty)
                palette.Add(BackgroundBackColor);
            else
                // otherwise enter an arbitrary color
                palette.Add(Color.FromArgb(49, 64, 81));

            palette.Add(_borderColor);
            palette.Add(_backColor);

            // Create the corner bitmap
            Bitmap corner = new Bitmap(CornerWidth, CornerHeight);
            Graphics gpx = Graphics.FromImage(corner);

            // Set mode to anti-alias if we have a specific BackgroundBackColor, and add some additional
            // palette colors that are a mix between the _backColor, _borderColor, and BackgroundBackColor
            if (BackgroundBackColor != Color.Empty)
            {
                gpx.SmoothingMode = SmoothingMode.HighQuality;

                Random rnd = new Random();
                for (int i = 0; i < 50; i++)
                {
                    palette.Add(PickRandomColorBetween(_backColor, _borderColor, rnd.Next(20)));
                    palette.Add(PickRandomColorBetween(_backColor, BackgroundBackColor, rnd.Next(20)));
                    palette.Add(PickRandomColorBetween(BackgroundBackColor, _borderColor, rnd.Next(20)));
                }

                // add grey colors
                for (int i = 0; i < 100; i++)
                    palette.Add(Color.FromArgb(i * 2, i * 2, i * 2));
            }

            // Clear out the palette with the transparent or BackgroundBackColor color
            gpx.Clear((Color)palette[0]);

            Pen fc = new Pen(_borderColor, (float)_borderWidth.Value);
            switch (BorderStyle)
            {
                case BorderStyle.Dashed:
                    fc.DashStyle = DashStyle.Dash;
                    break;
                case BorderStyle.Dotted:
                    fc.DashStyle = DashStyle.Dot;
                    break;
                default:
                    fc.DashStyle = DashStyle.Solid;
                    break;
            }

            SolidBrush bc = null;
            const int SCALE_FACTOR = 3;
            int x = 0, y = 0;
            switch (c)
            {
                case Corners.UpperLeft:
                    x = y = 0;

                    // Adjust for border width
                    x += Convert.ToInt32(_borderWidth.Value) / SCALE_FACTOR;
                    y += Convert.ToInt32(_borderWidth.Value) / SCALE_FACTOR;

                    bc = new SolidBrush(_backColor);
                    break;
                case Corners.UpperRight:
                    x = -Convert.ToInt32(CornerWidth) - 1;
                    y = 0;

                    // Adjust for border width
                    x -= Convert.ToInt32(_borderWidth.Value) / SCALE_FACTOR;
                    y += Convert.ToInt32(_borderWidth.Value) / SCALE_FACTOR;

                    bc = new SolidBrush(_backColor);
                    break;
                case Corners.BottomLeft:
                    x = 0;
                    y = -Convert.ToInt32(CornerHeight) - 1;

                    // Adjust for border width
                    x += Convert.ToInt32(_borderWidth.Value) / SCALE_FACTOR;
                    y -= Convert.ToInt32(_borderWidth.Value) / SCALE_FACTOR;

                    bc = new SolidBrush(_backColor);
                    break;
                case Corners.BottomRight:
                    x = -Convert.ToInt32(CornerWidth) - 1;
                    y = -Convert.ToInt32(CornerHeight) - 1;

                    // Adjust for border width
                    x -= Convert.ToInt32(_borderWidth.Value) / SCALE_FACTOR;
                    y -= Convert.ToInt32(_borderWidth.Value) / SCALE_FACTOR;

                    bc = new SolidBrush(_backColor);
                    break;
            }

            if (bc != null)
            {
                gpx.FillEllipse(bc, x, y, 2 * Convert.ToInt32(CornerWidth), 2 * Convert.ToInt32(CornerHeight));
            }

            gpx.DrawArc(fc, x, y, 2 * Convert.ToInt32(CornerWidth), 2 * Convert.ToInt32(CornerHeight), 0, 360);

            // Quantize the GIF image
            PaletteQuantizer quantizer = new PaletteQuantizer(palette, BackgroundBackColor != Color.Empty);

            Bitmap quantizedGif = quantizer.Quantize(corner);

            MemoryStream stream = new MemoryStream();

            quantizedGif.Save(stream, ImageFormat.Gif);

            int imageID = DbUtility.SaveImage(stream.ToArray(), "image/gif", null, imageName, Guid.NewGuid().ToString(), DateTime.Now, true);


            if (bc != null)
            {
                bc.Dispose();
            }

            fc.Dispose();
            gpx.Dispose();
            corner.Dispose();
            quantizedGif.Dispose();

            return imageID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pobjColor"></param>
        /// <returns></returns>
        private static string ColorToHex(Color pobjColor)
        {
            if (pobjColor == Color.Empty || pobjColor == Color.Transparent)
            {
                return "transparent";
            }
            return "#" + string.Format("{0:X2}{1:X2}{2:X2}", pobjColor.R, pobjColor.G, pobjColor.B).ToLower();
        }

        #endregion
    }
}
