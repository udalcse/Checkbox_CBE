using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class CrossShadow2TextGenerator : ITextGenerator
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


			float fLighter=1.5f; //PARAM the bigger number, the shadow color is lighter  

			int maxColor=(int)((256.0f/fLighter)*0.8f); //max color 
			int minColor=1; //must not be 0

			//the color of the letters is chosen randomly 
			Color lettersColor=rand.GetColorBetween(minColor,maxColor,minColor,maxColor,minColor,maxColor); 
			//the color of the shadow is a bit lighter 
			Color shadowColor=Color.FromArgb((int)(lettersColor.R*fLighter),(int)(lettersColor.G*fLighter),(int)(lettersColor.B*fLighter));  
			//intersect color is the color complement for the letters color 
			Color intersectColor=Color.FromArgb(256-lettersColor.R,256-lettersColor.G,256-lettersColor.B);  

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