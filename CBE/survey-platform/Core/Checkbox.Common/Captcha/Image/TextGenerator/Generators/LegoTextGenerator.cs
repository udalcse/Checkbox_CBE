using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class LegoTextGenerator : ITextGenerator
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
			RectangleF rectOuter=new RectangleF(rectImage.Width*0.1f,rectImage.Height*0.1f,rectImage.Width*0.8f,rectImage.Height*0.8f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectOuter,rectInner);
			
			//random font 
			Font font=new Font(rand.GetFontName(),200,FontStyle.Bold);
	
			
			//how many times an original rectangle will be blown out (min value)
			float fInflateMin=1.0f; //PARAM the greater number -> the larger single rectangles 
			//how many times an original rectangle will be blown out (max value)
			float fInflateMax=3.0f; //PARAM the greater number -> the larger single rectangles 


			//this parameter determines how many times the blowing up will be bigger in the vertical than in tha horizontal direction 
			float fVerticalBlowing=2.5f; //PARAM  the greater number -> the more square-like a single rectangle is


			//some math to take the size of the picture into condsideration 
			fInflateMin=(rectImage.Width*rectImage.Height*0.00003f*fInflateMin);
			fInflateMax=(rectImage.Width*rectImage.Height*0.00003f*fInflateMax);

			
			//create path with text
			GraphicsPath path= Tools.TextIntoRectPath(text,font,rectFBorders);

			//get the rectangles the path consists of 
			Region rgn=new Region(path); 
			Matrix mx=new Matrix();
            RectangleF[] rgnRectangles=rgn.GetRegionScans(mx); //the rectangles the path consists of 

			
			//minimal and maximal values of red, green and blue 
			//you can define the range the colors for the single rectangles
			int rMin, rMax, gMin,gMax,bMin, bMax;
			rMin=0; //PARAM      
			rMax=200;	//PARAM 
			gMin=0;	//PARAM 
			gMax=200;	//PARAM 
			bMin=0;//PARAM 
			bMax=200;	//PARAM 

			//DRAWING BACKGROUND
			g.Clear(rand.GetColorBetween(rMax,255,gMax,255,bMax,255));  

			//DRAWING  TEXT

			Color color=Color.Black;
			for(i=0;i<rgnRectangles.Length;i++)
			{
				RectangleF rectSingle=rgnRectangles[i];

				//the color will be changed for every 30th rectangle 
				if (i%30==0) //PARAM 
				{
					color=rand.GetColorBetween(rMin, rMax, gMin,gMax,bMin, bMax);
				}

				//only the every other rectangle will be blown out 
				if(i%2==0) 
				{
					rectSingle.Inflate(rand.BetweenTwo(fInflateMin,fInflateMax),rand.BetweenTwo(fInflateMin*fVerticalBlowing,fInflateMax*fVerticalBlowing));
				}					
				
				g.FillRectangle( new SolidBrush(color),Tools.ToRectangle(rectSingle));
				
			}

				
			// Clean up.
			font.Dispose();
			g.Dispose();

		}
	}
}
