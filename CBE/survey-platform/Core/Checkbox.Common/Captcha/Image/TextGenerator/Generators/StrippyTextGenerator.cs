using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class StrippyTextGenerator : ITextGenerator
	{
	    public void DrawText(string text, Bitmap textDestination)
		{
			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, textDestination.Height);
		
			int i;
			RandomClass rand=new RandomClass();

			RectangleF rectInner=new RectangleF(rectImage.Width*0.2f,rectImage.Height*0.2f,rectImage.Width*0.7f,rectImage.Height*0.7f);
			RectangleF rectOuter=new RectangleF(rectImage.Width*0.1f,rectImage.Height*0.1f,rectImage.Width*0.8f,rectImage.Height*0.8f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectOuter,rectInner);
			
			//random font 
			Font font=new Font(rand.GetFontName(),200,FontStyle.Bold);
	
			//background 
			Color backColor=Color.White;  //PARAM the background color 

			//how many times an original rectangle will be blown out (in vertical direction)
			float fInflateHor=4.0f; //PARAM the greater number -> the larger single rectangles 
			//how many times an original rectangle will be blown out (in horizontal direction)
			float fInflateVert=4.0f; //PARAM the greater number -> the larger single rectangles 
			
			int iColorChange=40; //PARAM the smaller number -> the colors are mmore often changed 


			//some math to take the size of the picture into condsideration 
			fInflateVert=(rectImage.Width*rectImage.Height*0.00003f*fInflateVert);
			fInflateHor=(rectImage.Width*rectImage.Height*0.00003f*fInflateHor);

			
			//create path with text
			GraphicsPath pathLetters= Tools.TextIntoRectPath(text,font,rectFBorders);

			//get the rectangles the path consists of 
			Region rgnLetters=new Region(pathLetters); 
			
			Matrix mx=new Matrix();
			RectangleF[] rgnRectangles=rgnLetters.GetRegionScans(mx); //the rectangles the letters consist of 

			//DRAW BACKGROUND
			g.Clear(backColor);

			//DRAWING  TEXT
			Color color=Color.Black;
			for(i=0;i<rgnRectangles.Length;i++)
			{
				RectangleF rectSingle=rgnRectangles[i];

				//the color will be changed for every fColorChage-th rectangle 
				if (i%iColorChange==0) 
				{
					color=rand.GetColor();
				}
				rectSingle.Inflate(fInflateHor,fInflateVert);

				g.FillRectangle( new SolidBrush(color),Tools.ToRectangle(rectSingle));
			
			}

				
			// Clean up.
			font.Dispose();
			g.Dispose();

		}
	}
}
