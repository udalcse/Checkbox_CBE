using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class RoughTextGenerator : ITextGenerator
	{
	    public void DrawText(string text, Bitmap textDestination)
		{
			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, textDestination.Height);
		
			int i;
			RandomClass rand=new RandomClass();

			RectangleF rectInner=new RectangleF(rectImage.Width*0.2f,rectImage.Height*0.2f,rectImage.Width*0.6f,rectImage.Height*0.6f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectImage,rectInner);

			
			//random font 
			Font font=new Font(rand.GetFontName(TextStyleEnum.Rough),200);
	
			//background 
			g.Clear(Color.White);

			int PixelCountConst=(int)(rectImage.Width*rectImage.Height*0.4f);

			//INSIDE THE TEXT 

			//create path 
			GraphicsPath path= Tools.TextIntoRectPath(text,font,rectFBorders);
			g.SetClip(path);            
			
			//draw black points 
			for( i=0;i<PixelCountConst;i++)
			{
				PointF pt=rand.GetPointInsideRect(rectFBorders);
				g.FillEllipse(new SolidBrush(Color.Black),new Rectangle((int)pt.X,(int)pt.Y,1,1));
			}

			//draw white points
			for( i=0;i<PixelCountConst/10;i++)
			{
				PointF pt=rand.GetPointInsideRect(rectFBorders);
				g.FillEllipse(new SolidBrush(Color.White),new Rectangle((int)pt.X,(int)pt.Y,1,1));
			}

			g.ResetClip();

			//ALONG THE TEXT LINES 

			Pen pen= new Pen(Color.White,(float)3);
			path.Widen(pen);
			g.SetClip(path);
			//draw black points 
			for( i=0;i<PixelCountConst/10;i++)
			{
				PointF pt=rand.GetPointInsideRect(rectFBorders);
				g.FillEllipse(new SolidBrush(Color.Black),new Rectangle((int)pt.X,(int)pt.Y,2,2));
			}

			//draw white points
			for( i=0;i<PixelCountConst/7;i++)
			{
				PointF pt=rand.GetPointInsideRect(rectFBorders);
				g.FillEllipse(new SolidBrush(Color.White),new Rectangle((int)pt.X,(int)pt.Y,2,2));
			}
			g.ResetClip();

			//AROUND THE TEXT 

			//draw black points 
			for( i=0;i<PixelCountConst/10;i++)
			{
				PointF pt=rand.GetPointInsideRect(rectImage);
				g.FillEllipse(new SolidBrush(Color.Black),new Rectangle((int)pt.X,(int)pt.Y,1,1));
			}

			//draw white points
			for( i=0;i<PixelCountConst/7;i++)
			{
				PointF pt=rand.GetPointInsideRect(rectImage);
				g.FillEllipse(new SolidBrush(Color.White),new Rectangle((int)pt.X,(int)pt.Y,1,1));
			}
			
				
			// Clean up.
			font.Dispose();
			g.Dispose();

		}
	}
}
