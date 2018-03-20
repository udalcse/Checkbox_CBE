using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class CollageTextGenerator : ITextGenerator
	{
	    public void DrawText(string text, Bitmap textDestination)
		{

			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, (float)textDestination.Height);
			
			int i;
			RandomClass rand=new RandomClass();

			RectangleF rectInner=new RectangleF(rectImage.Width*0.2f,rectImage.Height*0.2f,rectImage.Width*0.6f,rectImage.Height*0.6f);
			RectangleF rectOuter=new RectangleF(rectImage.Width*0.05f,rectImage.Height*0.05f,rectImage.Width*0.9f,rectImage.Height*0.9f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectOuter,rectInner);
			
			
			//LINES
			ArrayList arrLines1=new ArrayList();
			GenerateLines(textDestination,arrLines1);

			//TEXT
			//random font 
			Font font=new Font(rand.GetFontName(),200,System.Drawing.FontStyle.Bold);

			//create the text path 
			GraphicsPath pathLetters= Tools.TextIntoRectPath(text,font,rectFBorders);

			//generate  the text region
			Region rgnLetters=new Region(pathLetters);

			//DRAWING
			
			//draw the background  
			g.Clear(Color.White);

			g.SetClip(rgnLetters,CombineMode.Intersect);
			
			//draw the lines inside the letters 
			float lineInThick=5.0f; //PARAM -> the bigger value, the lines will be more thick			
			for( i=0;i<arrLines1.Count;i++)
			{	
				Line clrLine= (Line)arrLines1[i];
				Pen pen=new Pen(clrLine.color,lineInThick); 
				g.DrawLine(pen,((Line)arrLines1[i]).x1,((Line)arrLines1[i]).x2);
			}
		
			g.ResetClip();			

			g.Dispose();


		}

		private void GenerateLines( Bitmap textDestination,ArrayList arrLines1)
		{
			// Create a graphics object for drawing.
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, (float)textDestination.Height);

			int i;
			RandomClass rand=new RandomClass();
			
			//minimal and maximal values of red, green and blue 
			//you can define the range the colors of the lines can ne between
			int rMin, rMax, gMin,gMax,bMin, bMax;
			rMin=50; //PARAM      
			rMax=255;	//PARAM 
			gMin=50;	//PARAM 
			gMax=255;	//PARAM 
			bMin=50;//PARAM 
			bMax=255;	//PARAM 

			// lines
			float lines1Const=42.1f;    //PARAM -> the bigger value, more lines (more dense on the screen)
			int lines1Count=(int)(lines1Const*Tools.GetNonLinear(rectImage.Width*rectImage.Height));
			
			//create and store lines
			for( i=0;i<lines1Count;i++)
			{	
				Color lineColor=rand.GetColorBetween(rMin, rMax, gMin,gMax,bMin, bMax);
				RectangleF rectUpper=new RectangleF(rectImage.Left,rectImage.Top,rectImage.Width,0.1f*rectImage.Height);
				RectangleF rectLower=new RectangleF(rectImage.Left,rectImage.Top+0.9f*rectImage.Height,rectImage.Width,0.1f*rectImage.Height);
				arrLines1.Add(new Line(lineColor,rand.GetPointInsideRect(rectUpper),rand.GetPointInsideRect(rectLower)));				
			}
		
			
		}
	}		
}
