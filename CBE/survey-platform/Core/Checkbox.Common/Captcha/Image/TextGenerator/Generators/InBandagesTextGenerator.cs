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
	internal class InBandagesTextGenerator : ITextGenerator
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

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectImage,rectInner);
			
			
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

			float lineInThick=3.1f; //PARAM -> the bigger value, the lines will be more thick, and letters more "eaten"			
						
			int maxColor=100;
			int minColor=0; 

			//the color of the letters is chosen randomly 
			Color letterColor=rand.GetColorBetween(minColor,maxColor,minColor,maxColor,minColor,maxColor); 
			//the fill color is the color complement for the letters color 
			Color backColor=Color.FromArgb(255-letterColor.R,255-letterColor.G,255-letterColor.B);  
		
			//DRAWING

			//draw the background  
			g.Clear(backColor);

			SolidBrush lettersBrush=new SolidBrush(letterColor);
			g.FillPath(lettersBrush,pathLetters);

			//g.SetClip(rgnLetters,CombineMode.Intersect);

			//draw the lines inside the letters 
			
			for( i=0;i<arrLines1.Count;i++)
			{	
				Line clrLine= (Line)arrLines1[i];
				Pen pen=new Pen(backColor,lineInThick); 
				g.DrawLine(pen,((Line)arrLines1[i]).x1,((Line)arrLines1[i]).x2);
			}
			
			
			//g.ResetClip();			

			g.Dispose();


		}

		private void GenerateLines( Bitmap textDestination,ArrayList arrLines1)
		{
			// Create a graphics object for drawing.
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, (float)textDestination.Height);

			int i;
			RandomClass rand=new RandomClass();
			
		
			// lines
			float lines1Const=1.1f;    //PARAM -> the bigger value, more lines (more dense on the screen)
			int lines1Count=(int)(lines1Const*Tools.GetNonLinear(rectImage.Width*rectImage.Height));
			
			
			//create and store lines
			for( i=0;i<lines1Count;i++)
			{	
				Color lineColor=Color.White;
				RectangleF rectUpper=new RectangleF(rectImage.Left,rectImage.Top,rectImage.Width,0.1f*rectImage.Height);
				RectangleF rectLower=new RectangleF(rectImage.Left,rectImage.Top+0.9f*rectImage.Height,rectImage.Width,0.1f*rectImage.Height);
				arrLines1.Add(new Line(lineColor,rand.GetPointInsideRect(rectUpper),rand.GetPointInsideRect(rectLower)));				
			}	
					
		}
	}		
}
