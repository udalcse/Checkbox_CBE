using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class CrossShadowTextGenerator : ITextGenerator
	{
	    public void DrawText(string text, Bitmap textDestination)
		{
			

			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, (float)textDestination.Height);
			
			//int i;
			RandomClass rand=new RandomClass();

			//inner rect (the text can not be smaller than this rectangle)
			RectangleF rectInner=new RectangleF(rectImage.Width*0.2f,rectImage.Height*0.2f,rectImage.Width*0.6f,rectImage.Height*0.6f);

			//outer rect (the text can not be larger than this rectangle)
			RectangleF rectOuter=new RectangleF(rectImage.Width*0.0f,rectImage.Height*0.0f,rectImage.Width*0.9f,rectImage.Height*0.9f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectOuter,rectInner);

			//random font 
			Font font=new Font(rand.GetFontName(),200,FontStyle.Bold);

			Color lettersColor=Color.Green; //PARAM background and intersect color 
			Color shadowColor=Color.LightGreen;
			Color intersectColor=Color.Red;  //PARAM letters and circles color 

			g.Clear(intersectColor);

			//the shadow offset - how much  the shadow is right and down 
			float fShadowSize=0.04f; //PARAM the bigger value, the larger the shadow offset

			//generate  the text region
			GraphicsPath pathLetters = Tools.TextIntoRectPath(text,font,rectFBorders);
			Region rgnLetters=new Region(pathLetters);
			
			//generate the shadow region
			GraphicsPath pathShadow = (GraphicsPath)pathLetters.Clone();
			Region rgnShadow=new Region(pathShadow);
			rgnShadow.Translate(textDestination.Width*fShadowSize,textDestination.Height*fShadowSize);
		
						
			g.FillRegion(new SolidBrush(lettersColor),rgnLetters);
			g.FillRegion(new SolidBrush(shadowColor),rgnShadow);

			Region rgnIntersect=new Region();
			rgnIntersect=rgnLetters.Clone();
			rgnIntersect.Intersect(rgnShadow);
			
		

			g.FillRegion(new SolidBrush(intersectColor),rgnIntersect);

			//g.FillPath(new SolidBrush(mainColor),pathLetters);
			// Clean up.
			font.Dispose();
			g.Dispose();


		}

		
	}
}