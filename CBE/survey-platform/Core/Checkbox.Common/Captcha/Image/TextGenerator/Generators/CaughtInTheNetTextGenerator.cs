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
	internal class CaughtInTheNetTextGenerator : ITextGenerator
	{
	    public void DrawText(string text, Bitmap textDestination)
		{

			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, (float)textDestination.Height);
			
			int i;
			RandomClass rand=new RandomClass();

			RectangleF rectInner=new RectangleF(rectImage.Width*0.0f,rectImage.Height*0.0f,rectImage.Width*0.9f,rectImage.Height*0.9f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectImage,rectInner);
			
			
			//LINES
			ArrayList arrLines1=new ArrayList();
			GenerateLines(textDestination,arrLines1);

			//TEXT
			//random font 
			Font font=new Font(rand.GetFontName(),200,System.Drawing.FontStyle.Bold);


		
			//generate  the text region
			//create path 
			CurvedText objCurvedText = CurvedTextShapes.GetCurvedText(CurvedTextShapes.CurvedTextShapeEnum.Shape1);			
			GraphicsPath pathLetters=objCurvedText.GetTextPath(rectFBorders,g,font,text);
		

			//generate  the text region
			Region rgnLetters=new Region(pathLetters);

			Color backColor=Color.FromArgb(0,0,150); //the color of the text
			Color letterColor=Color.FromArgb(255-backColor.R,255-backColor.G,255-backColor.B); //the color of the background and of the lines  //the color of the background and of the lines 

			float linesThick=1.5f; //PARAM -> the bigger value, the lines will be more thick
			float circlesThick=1.5f; //PARAM -> the bigger value, the lines will be more thick

			//the center of the web 
			PointF ptCenter=rand.GetPointInsideRect(rectFBorders);
			float fRadiusIncrease=5.0f; //PARAM -> the bigger value, the circles will be more apart

			//DRAWING
			
			//draw the background  
			g.Clear(backColor);

			SolidBrush lettersBrush=new SolidBrush(letterColor);
			g.FillPath(lettersBrush,pathLetters);

			g.SetClip(rgnLetters,CombineMode.Intersect);

			//draw the lines inside the letters 
			
			for( i=0;i<arrLines1.Count;i++)
			{	
				Line clrLine= (Line)arrLines1[i];
				Pen pen=new Pen(backColor,linesThick); 
				g.DrawLine(pen,((Line)arrLines1[i]).x1,((Line)arrLines1[i]).x2);
			}
			
			//draw the circles inside the letters 
			
			
			float fRadius;
			for( fRadius=0;fRadius<888.0f;fRadius+=fRadiusIncrease)
			{
				RectangleF rectF=new RectangleF(ptCenter.X-fRadius,ptCenter.Y-fRadius,2*fRadius,2*fRadius);
				Pen pen= new Pen(backColor,(float)circlesThick);
				g.DrawEllipse(pen,rectF);
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

		
			// lines
			float lines1Const=3.0f;    //PARAM -> the bigger value, more lines (more dense on the screen)
			int lines1Count=(int)(lines1Const*Tools.GetNonLinear(rectImage.Width*rectImage.Height));
			
			
			//create and store lines
			for( i=0;i<lines1Count;i++)
			{	
				Color lineColor=Color.White; //the dummy color 			
				RectangleF rectUpper=new RectangleF(rectImage.Left,rectImage.Top,rectImage.Width,0.1f*rectImage.Height);
				RectangleF rectLower=new RectangleF(rectImage.Left,rectImage.Top+0.9f*rectImage.Height,rectImage.Width,0.1f*rectImage.Height);
				arrLines1.Add(new Line(lineColor,rand.GetPointInsideRect(rectUpper),rand.GetPointInsideRect(rectLower)));				
			}	
					
		}
	}		
}