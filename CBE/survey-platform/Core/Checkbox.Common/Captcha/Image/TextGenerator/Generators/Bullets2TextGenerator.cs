using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class Bullets2TextGenerator : ITextGenerator
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
			RectangleF rect0uter=new RectangleF(rectImage.Width*0.1f,rectImage.Height*0.1f,rectImage.Width*0.8f,rectImage.Height*0.8f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rect0uter,rectInner);

			//random font 
			Font font=new Font(rand.GetFontName(TextStyleEnum.Bullets),200,FontStyle.Bold);
			
			//minimal and maximal values of red, green and blue 
			//you can define the range the colors of the lines can ne between
			int rMin, rMax, gMin,gMax,bMin, bMax;
			rMin=0; //PARAM      
			rMax=100;	//PARAM 
			gMin=0;	//PARAM 
			gMax=100;	//PARAM 
			bMin=0;//PARAM 
			bMax=100;	//PARAM 

			Color lettersColor=rand.GetColorBetween(rMin, rMax, gMin,gMax,bMin, bMax); //the color of the text
			Color backColor=Color.FromArgb(255-lettersColor.R,255-lettersColor.G,255-lettersColor.B); //the color of the background and of the lines 
			Color holeColor=backColor;

			//background 
			g.Clear(backColor);

			float fHoleCountConst=8f; //PARAM the higher value -> more holes on the letters
			int HoleCount=(int)(fHoleCountConst*Tools.GetNonLinear(rectImage.Width*rectImage.Height));

			float fMaxRadiusConst=0.8f; //PARAM the higher value -> the bigger holes are  
			int iMaxRadius=(int)(fMaxRadiusConst*Tools.GetNonLinear(rectImage.Width*rectImage.Height));

			//INSIDE THE TEXT 

			//create path 
			GraphicsPath path= Tools.TextIntoRectPath(text,font,rectFBorders);

			g.FillPath(new SolidBrush(lettersColor),path);
			//g.SetClip(path);            
			
			//draw holes
			for( i=0;i<HoleCount;i++)
			{
				PointF pt=rand.GetPointInsideRect(rectFBorders);
				int iRadius=rand.BetweenTwo(0,iMaxRadius);
				g.FillEllipse(new SolidBrush(holeColor),new Rectangle((int)pt.X,(int)pt.Y,iRadius,iRadius));
			}

			//draw white points
			for( i=0;i<HoleCount/10;i++)
			{
				PointF pt=rand.GetPointInsideRect(rectFBorders);
				//g.FillEllipse(new SolidBrush(Color.White),new Rectangle((int)pt.X,(int)pt.Y,1,1));
			}

			//g.ResetClip();

			
			// Clean up.
			font.Dispose();
			
			g.Dispose();

		}
	}
}
