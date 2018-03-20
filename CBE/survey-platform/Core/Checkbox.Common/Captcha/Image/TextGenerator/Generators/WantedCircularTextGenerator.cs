using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class WantedCircularTextGenerator : ITextGenerator
	{
	    public void DrawText(string text, Bitmap textDestination)
		{
			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.None;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, textDestination.Height);
		
			int i;
			RandomClass rand=new RandomClass();

			RectangleF rectInner=new RectangleF(rectImage.Width*0.2f,rectImage.Height*0.2f,rectImage.Width*0.6f,rectImage.Height*0.6f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectImage,rectInner);

			//the color of a front seagull 
			Pen mainPen=new Pen(Color.Black,1.0f);
			//the color of a back seagull or background as well
			Pen backPen=new Pen(Color.WhiteSmoke,1.0f);

			//the width of the seagull
			float randomSeagullWidth=rectImage.Width/60;
			//the height of the seagull
			float randomSeagullHeight=rectImage.Height/60;

			//a saegull consists of 7 points, they are in a specific relationship to each other
			//this number makes fixed points in the seagull algorithm go a bit left or rigth (down or up)
			float randomSeagullOff=randomSeagullWidth/3;

			//random font 
			Font font=new Font(rand.GetFontName(TextStyleEnum.WantedCircular),200,FontStyle.Bold );
	
			//background 
			g.Clear(Color.WhiteSmoke);

			int SeagullsCountConst=(int)(rectImage.Width*rectImage.Height*0.25f);
			
			//INSIDE THE TEXT 

			//create path 
			GraphicsPath path= Tools.TextIntoRectPath(text,font,rectFBorders);
			g.SetClip(path);            
			
			//draw main seagulls
			for( i=0;i<SeagullsCountConst/1;i++)
			{
				RectangleF rect=rand.GetSameSizeRectangle(rectImage,randomSeagullWidth,randomSeagullHeight);
				DrawRandomSeaGull(g,mainPen,rect,randomSeagullOff,rand);				
			}

			//draw back segulls
			for( i=0;i<SeagullsCountConst/10;i++)
			{
				RectangleF rect=rand.GetSameSizeRectangle(rectImage,randomSeagullWidth,randomSeagullHeight);
				DrawRandomSeaGull(g,backPen,rect,randomSeagullOff,rand);				
			}

			g.ResetClip();

			//AROUND THE TEXT 

			Region rgn=new Region(rectImage);
			g.SetClip(path);
			g.SetClip(rgn,CombineMode.Xor);

			//draw main seagulls
			for( i=0;i<SeagullsCountConst*0.8;i++)
			{
				RectangleF rect=rand.GetSameSizeRectangle(rectImage,randomSeagullWidth,randomSeagullHeight);
				DrawRandomSeaGull(g,mainPen,rect,randomSeagullOff,rand);				
			}

			//draw back segulls
			for( i=0;i<SeagullsCountConst*2;i++)
			{
				RectangleF rect=rand.GetSameSizeRectangle(rectImage,randomSeagullWidth,randomSeagullHeight);
				DrawRandomSeaGull(g,backPen,rect,randomSeagullOff,rand);				
			}		
				
			// Clean up.
			font.Dispose();
			g.Dispose();

		}
		void DrawRandomSeaGull(Graphics grfx, Pen pen, RectangleF rectFrame,float radius,RandomClass rand)
		{
			float width=rectFrame.Width;
			float height=rectFrame.Height;
			float left=rectFrame.Left;
			float bottom=rectFrame.Bottom;
			PointF pt1= rand.GetPointAroundPoint(new PointF(left,bottom),radius);
			PointF pt2=rand.GetPointAroundPoint(new PointF(left+width*0.2f,bottom-height*1.1f),radius);
			PointF pt3=rand.GetPointAroundPoint(new PointF(left+width*0.3f,bottom-height*1.1f),radius);
			PointF pt4=rand.GetPointAroundPoint(new PointF(left+width*0.5f,bottom),radius);

			PointF pt5=pt4;
			PointF pt6=rand.GetPointAroundPoint(new PointF(left+width*0.7f,bottom-height*1.1f),radius);
			PointF pt7=rand.GetPointAroundPoint(new PointF(left+width*0.8f,bottom-height*1.1f),radius);
			PointF pt8=rand.GetPointAroundPoint(new PointF(left+width,bottom),radius);
						
			grfx.DrawBezier(pen,pt1,pt2,pt3,pt4);
			grfx.DrawBezier(pen,pt5,pt6,pt7,pt8);
		}
	}
}
