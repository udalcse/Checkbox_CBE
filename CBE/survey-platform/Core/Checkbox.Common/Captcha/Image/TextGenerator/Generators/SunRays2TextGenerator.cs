using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class SunRays2TextGenerator : ITextGenerator
	{
	    public void DrawText(string text, Bitmap textDestination)
		{
			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, textDestination.Height);
		
			//int i;
			RandomClass rand=new RandomClass();

			RectangleF rectInner=new RectangleF(rectImage.Width*0.2f,rectImage.Height*0.2f,rectImage.Width*0.6f,rectImage.Height*0.6f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectImage,rectInner);

			
			//random font 
			Font font=new Font(rand.GetFontName(),200);
	
	

			int PixelCountConst=(int)(rectImage.Width*rectImage.Height*0.4f);

			//INSIDE THE TEXT 

			//create path 
			GraphicsPath path= Tools.TextIntoRectPath(text,font,rectFBorders);

			
			float linesAngle=1.2f;			 //PARAM -> the bigger value, the lines are more apart 
			float linesWidth=3.0f;			 //PARAM -> the bigger value, the lines are thicker 

					
			int maxColor=140;
			int minColor=0; 

			//the color of the letters is chosen randomly 
			Color lineColor=rand.GetColorBetween(minColor,maxColor,minColor,maxColor,minColor,maxColor); 
			//intersect color is the color complement for the letters color 
			Color backColor=Color.FromArgb(255, 255-lineColor.R,255-lineColor.G,255-lineColor.B);  
			
			//background 
			g.Clear(backColor);

			//the center of the radial lines 
			PointF ptCenter=rand.GetPointInsideRect(rectFBorders);

			g.SetClip(path);            
			
			//the pen for lines drawing 
			Pen pen= new Pen(lineColor,(float)linesWidth);

			linesAngle=linesAngle*1000*(1/(rectImage.Width*rectImage.Height));

			//angle is measured in radians 
			float fAngle;
			//draw a line from the center point to some second point far away,
			//the angle is increased every time a new line is drawn 
			for( fAngle=0;fAngle<Math.PI*2;fAngle=fAngle+linesAngle)
			{
				float lineLegth=888.0f; //it is an arbitrary value, it just has to be bigger than the picture 
				PointF ptSecondPoint=new PointF((float)Math.Sin(fAngle)*lineLegth+ptCenter.X,(float)Math.Cos(fAngle)*lineLegth+ptCenter.Y);
				g.DrawLine(pen,ptCenter,ptSecondPoint);
			}

			g.ResetClip();
				
			// Clean up.
			font.Dispose();
			g.Dispose();

		}
	}
}
