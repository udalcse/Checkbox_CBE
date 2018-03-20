using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class GhostlyTextGenerator : ITextGenerator
	{
	    public void DrawText(string text, Bitmap textDestination)
		{
			

			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, textDestination.Height);
		
			int i;
			RandomClass rand=new RandomClass();

			RectangleF rectInner=new RectangleF(rectImage.Width*0.1f,rectImage.Height*0.0f,rectImage.Width*0.85f,rectImage.Height*0.8f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectImage,rectInner);

			
			//random font 
			Font font=new Font(rand.GetFontName(),200);
	
			CurvedText objCurvedText = CurvedTextShapes.GetCurvedText(CurvedTextShapes.CurvedTextShapeEnum.Shape1);			

			int maxColor=250; //PARAM 
			int minColor=170; //PARAM 

			//the color of the letters is chosen randomly 
			Color lineColor=rand.GetColorBetween(minColor,maxColor,minColor,maxColor,minColor,maxColor); 
			int lineGreen=lineColor.G;
			int lineBlue=lineColor.B;
			int lineRed=lineColor.R;

			//the fill color is the color complement for the letters color 
			Color backColor=Color.FromArgb(255-lineColor.R,255-lineColor.G,255-lineColor.B);  
			int backGreen=backColor.G;
			int backBlue=backColor.B;
			int backRed=backColor.R;

			g.Clear(backColor);

			//fill interor of the letters 
			objCurvedText.DrawText(rectFBorders,g,font,text,new SolidBrush(lineColor),new Pen(lineColor,1));
			
			float fLayersThickness=2.7f; //PARAM the greater the number -> the fewer the layers there are 
			int layersTotal=(int)(Math.Min((float)rectImage.Width,(float)rectImage.Height)/fLayersThickness);

			//we draw the layers from the most outer one to the most inner one
			//the outer layers are wider and their color is more similar to the background color 
			//the inner layers are thinner and their color is more similar to the letter color 
			for(i=layersTotal;i>0;i--)
			{
				int partGreen=lineGreen+((backGreen-lineGreen)/layersTotal)*i;
				int partBlue=lineBlue+((backBlue-lineBlue)/layersTotal)*i;
				int partRed=lineRed+((backRed-lineRed)/layersTotal)*i;
				Pen pen= new Pen(Color.FromArgb(partRed,partGreen,partBlue),(float)i);				

				//draw outline
				objCurvedText.DrawText(rectFBorders,g,font,text,null,pen);
			}
				
			// Clean up.
			font.Dispose();
			g.Dispose();

		}
	
	}
}
