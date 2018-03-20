using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class WavyColorLettersTextGenerator : ITextGenerator
	{
	    public void DrawText(string text, Bitmap textDestination)
		{
			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, textDestination.Height);
			//int i;
			RandomClass rand=new RandomClass();

			RectangleF rectInner=new RectangleF(rectImage.Width*0.1f,rectImage.Height*0.0f,rectImage.Width*0.85f,rectImage.Height*0.8f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectImage,rectInner);

			
			//random font 
			Font font=new Font(rand.GetFontName(),200);
	
			CurvedText objCurvedText = CurvedTextShapes.GetCurvedText(CurvedTextShapes.CurvedTextShapeEnum.Shape1);			

			int maxColor=120;
			int minColor=0; 

			//the color of the letters is chosen randomly 
			Color lineColor=rand.GetColorBetween(minColor,maxColor,minColor,maxColor,minColor,maxColor); 
			//the fill color is the color complement for the letters color 
			Color backColor=Color.FromArgb(255-lineColor.R,255-lineColor.G,255-lineColor.B);  

			g.Clear(backColor);
			objCurvedText.DrawText(rectFBorders,g,font,text,null,new Pen(lineColor,3.0f));
				
			// Clean up.
			font.Dispose();
			g.Dispose();

		}
	
	}
}
