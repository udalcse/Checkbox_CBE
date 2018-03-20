using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class SunAndWarmAirTextGenerator : ITextGenerator
	{
	    public void DrawText(string text, Bitmap textDestination)
		{
			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, textDestination.Height);
		
			//int i;
			RandomClass rand=new RandomClass();

			RectangleF rectInner=new RectangleF(rectImage.Width*0.1f,rectImage.Height*0.1f,rectImage.Width*0.75f,rectImage.Height*0.8f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectImage,rectInner);

			//random font 
			Font font=new Font(rand.GetFontName(),200,FontStyle.Bold);
	
			//create the text path 
			CurvedText objCurvedText = CurvedTextShapes.GetCurvedText(CurvedTextShapes.CurvedTextShapeEnum.Shape1);			
			GraphicsPath path=objCurvedText.GetTextPath(rectFBorders,g,font,text);
		

			float fRadiusIncrease=4.0f;			//PARAM -> the bigger value, the circles are more dense between

			float linesOutsideWidth=2.0f;		//PARAM -> the bigger value, the lines are thicker 
			float linesTextWidth=3.0f;			//PARAM -> the bigger value, the lines are thicker 


			//the text color value	
			Color textColor=Color.Black; 
			//the color of the circles outside the text 
			Color outsideColor=Color.White;  
		

			
			//the center of the circles 
			PointF ptCenter=rand.GetPointInsideRect(rectFBorders);


			//DRAW TEXT CIRCLES
			g.SetClip(path);            
			g.Clear(Color.Gray );
			
			//the pen for text lines drawing 
			Pen penText= new Pen(textColor,(float)linesTextWidth);

			float fRadius;
			for( fRadius=0;fRadius<888.0f;fRadius+=fRadiusIncrease)
			{
				RectangleF rectF=new RectangleF(ptCenter.X-fRadius,ptCenter.Y-fRadius,2*fRadius,2*fRadius);
				g.DrawEllipse(penText,rectF);
			}

			g.ResetClip();

			g.SetClip(path,CombineMode.Exclude);            
			//DRAW OUTSIDE CIRCLES

			//the pen for the drawing of the lines outside
			Pen penOutside= new Pen(outsideColor,(float)linesOutsideWidth);

			//draw a circle with a variable radius
			//the radius is increased every time a new circle is drawn 
		
			for( fRadius=0;fRadius<888.0f;fRadius+=fRadiusIncrease)
			{
				RectangleF rectF=new RectangleF(ptCenter.X-fRadius,ptCenter.Y-fRadius,2*fRadius,2*fRadius);
				g.DrawEllipse(penOutside,rectF);
			}
			g.ResetClip();
			
				
			// Clean up.
			font.Dispose();
			g.Dispose();

		}
	}
}
