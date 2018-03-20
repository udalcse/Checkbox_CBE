using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class AncientMosaicTextGenerator : ITextGenerator
	{
	    public void DrawText(string text, Bitmap textDestination)
		{
			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, textDestination.Height);
		
			int i;
			RandomClass rand=new RandomClass();

			RectangleF rectInner=new RectangleF(rectImage.Width*0.1f,rectImage.Height*(-0.1f),rectImage.Width*0.85f,rectImage.Height*0.8f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectImage,rectInner);

			
			//random font 
			Font font=new Font(rand.GetFontName(),200);
	
			//background 
			g.Clear(Color.White);

			int PixelCountConst=(int)(rectImage.Width*rectImage.Height*0.4f);

			//INSIDE THE TEXT 

			//create path 
			CurvedText objCurvedText = CurvedTextShapes.GetCurvedText(CurvedTextShapes.CurvedTextShapeEnum.Shape1);			
			GraphicsPath path=objCurvedText.GetTextPath(rectFBorders,g,font,text);
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
			for( i=0;i<PixelCountConst/2;i++)
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
