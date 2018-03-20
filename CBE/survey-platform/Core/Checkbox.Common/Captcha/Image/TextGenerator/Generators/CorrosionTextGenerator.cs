using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class CorrosionTextGenerator : ITextGenerator
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

			Color mainColor=Color.OrangeRed;
			Color backColor=Color.Silver;
			float randomRectWidth=rectImage.Width/30;
			float randomRectHeight=rectImage.Height/30;

			//random font 
			Font font=new Font("Arial",200,FontStyle.Regular );
	
			//background 
			g.Clear(backColor);

			int CurvesCountConst=(int)(rectImage.Width*rectImage.Height*0.2f);

			//INSIDE THE TEXT 

			//create path 
			GraphicsPath path= Tools.TextIntoRectPath(text,font,rectFBorders);
			g.SetClip(path);            
			
			//draw black points 
			for( i=0;i<CurvesCountConst;i++)
			{
				PointF pt1,pt2,pt3,pt4;
				RectangleF rect=rand.GetSameSizeRectangle(rectImage,randomRectWidth,randomRectHeight);
				rand.GetFourPointsInRect(rect,out pt1,out pt2,out pt3,out pt4);
				g.DrawBezier(new Pen(mainColor,1.0f),pt1,pt2,pt3,pt4);
			}

			//draw white points
			for( i=0;i<CurvesCountConst/10;i++)
			{
				PointF pt1,pt2,pt3,pt4;
				RectangleF rect=rand.GetSameSizeRectangle(rectImage,randomRectWidth,randomRectHeight);
				rand.GetFourPointsInRect(rect,out pt1,out pt2,out pt3,out pt4);
				g.DrawBezier(new Pen(backColor,1.0f),pt1,pt2,pt3,pt4);
			}

			g.ResetClip();

			//ALONG THE TEXT LINES 

			Pen pen= new Pen(Color.White,(float)3);
			path.Widen(pen);
			g.SetClip(path);

			//draw black points 
			for( i=0;i<CurvesCountConst/1;i++)
			{
				PointF pt1,pt2,pt3,pt4;
				RectangleF rect=rand.GetSameSizeRectangle(rectImage,randomRectWidth,randomRectHeight);
				rand.GetFourPointsInRect(rect,out pt1,out pt2,out pt3,out pt4);
				g.DrawBezier(new Pen(mainColor,1.0f),pt1,pt2,pt3,pt4);
			}

//			//draw white points
//			for( i=0;i<CurvesCountConst/7;i++)
//			{
//				PointF pt=rand.GetPointInsideRect(rectFBorders);
//				g.FillEllipse(new SolidBrush(Color.White),new Rectangle((int)pt.X,(int)pt.Y,2,2));
//			}
			g.ResetClip();


			//AROUND THE TEXT 

			//draw black points 
			for( i=0;i<CurvesCountConst/10;i++)
			{
				PointF pt1,pt2,pt3,pt4;
				RectangleF rect=rand.GetSameSizeRectangle(rectImage,randomRectWidth,randomRectHeight);
				rand.GetFourPointsInRect(rect,out pt1,out pt2,out pt3,out pt4);
				g.DrawBezier(new Pen(mainColor,1.0f),pt1,pt2,pt3,pt4);
			}

			//draw white points
			for( i=0;i<CurvesCountConst/2;i++)
			{
				PointF pt1,pt2,pt3,pt4;
				RectangleF rect=rand.GetSameSizeRectangle(rectImage,randomRectWidth,randomRectHeight);
				rand.GetFourPointsInRect(rect,out pt1,out pt2,out pt3,out pt4);
				g.DrawBezier(new Pen(backColor,1.0f),pt1,pt2,pt3,pt4);
			}
			
				
			// Clean up.
			font.Dispose();
			g.Dispose();

		}
	}
}
