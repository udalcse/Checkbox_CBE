using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class FingerPrintsTextGenerator : ITextGenerator
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
	
			//create the text path 
			GraphicsPath path= Tools.TextIntoRectPath(text,font,rectFBorders);

			float fRadiusIncrease=3.0f;			//PARAM -> the bigger value, the circles are more dense between
			float linesTextWidth=1.0f;			//PARAM -> the bigger value, the lines are thicker 

			int maxColor=180;
			int minColor=1; 

			//the text color value	
			Color textColor=rand.GetColorBetween(minColor,maxColor,minColor,maxColor,minColor,maxColor); 
			
			//background 
			g.Clear(Color.White);

			//the center of the circles 
			PointF ptCenter=rand.GetPointInsideRect(rectFBorders);


			//DRAW TEXT CIRCLES
			g.SetClip(path);            
			
			//the pen for text lines drawing 
			Pen penText= new Pen(textColor,(float)linesTextWidth);

			float fRadius;
			for( fRadius=0;fRadius<888.0f;fRadius+=fRadiusIncrease)
			{
				RectangleF rectF=new RectangleF(ptCenter.X-fRadius,ptCenter.Y-fRadius,2*fRadius,2*fRadius);
				g.DrawEllipse(penText,rectF);
			}

			g.ResetClip();
		
				
			// Clean up.
			font.Dispose();
			g.Dispose();

		}
	}
}
