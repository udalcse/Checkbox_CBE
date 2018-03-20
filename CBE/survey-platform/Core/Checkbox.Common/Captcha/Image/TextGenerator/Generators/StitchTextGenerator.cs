using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class StitchTextGenerator : ITextGenerator
	{
	    #region ITextGenerator Members

		public void DrawText(string text, Bitmap textDestination)
		{

			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.Default;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, (float)textDestination.Height);
			
			//int i;
			RandomClass rand=new RandomClass();

			//inner rect (the text can not be smaller than this rectangle)
			RectangleF rectInner=new RectangleF(rectImage.Width*0.1f,rectImage.Height*0.1f,rectImage.Width*0.5f,rectImage.Height*0.5f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectImage,rectInner);

			//random font 
			Font font=new Font(rand.GetFontName(TextStyleEnum.Stitch),200);

			//draw background 
			g.Clear(Color.White);

			//draw  the text 
			GraphicsPath path= Tools.TextIntoRectPath(text,font,rectFBorders);

			Pen pen=new Pen(Color.White,1);
			pen.Alignment=PenAlignment.Inset;
			pen.DashStyle=DashStyle.Dot;
			path.Widen(pen,new Matrix());
			
			g.FillPath(new SolidBrush(Color.Black),path);
			
			// Clean up.
			font.Dispose();
			g.Dispose();
		}

		#endregion
	}
}
