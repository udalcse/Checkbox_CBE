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
	internal class ThickThinLines2TextGenerator : ITextGenerator
	{
	    #region ITextGenerator Members

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
			
			
			//the max size of a letter box to host a single letter
			float boxWidth=rectFBorders.Width/text.Length;
			float boxHeight=rectFBorders.Height/text.Length;
			//float boxReduct=0.9f; //PARAM the bigger -> the letter is bigger
		
			//LINES
			ArrayList arrHor=new ArrayList();
			ArrayList arrVert=new ArrayList();

			GenerateLines(textDestination,arrHor,arrVert);

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
			float lineInThick=4.0f; //PARAM -> the bigger value, the lines will be more thick			
			for( i=0;i<arrHor.Count;i++)
			{	
				ColorLine clrLine= (ColorLine)arrHor[i];
				Pen pen=new Pen(clrLine.color,lineInThick); 
				g.DrawLine(pen,0,clrLine.coord,rectImage.Right,clrLine.coord);
			}
			
			for( i=0;i<arrVert.Count;i++)
			{	
				ColorLine clrLine= (ColorLine)arrVert[i];
				Pen pen=new Pen(clrLine.color,lineInThick); 
				g.DrawLine(pen,clrLine.coord,0,clrLine.coord,rectImage.Bottom);	
			}
			g.ResetClip();			

			g.SetClip(rgnLetters,CombineMode.Exclude);

			//draw the lines outside the letters
			float lineOutThick=1.0f; //PARAM -> the bigger value, the lines will be more thick			
			for( i=0;i<arrHor.Count;i++)
			{	
				ColorLine clrLine= (ColorLine)arrHor[i];
				Pen pen=new Pen(clrLine.color,lineOutThick); 
				g.DrawLine(pen,0,clrLine.coord,rectImage.Right,clrLine.coord);
			}
			
			for( i=0;i<arrVert.Count;i++)
			{	
				ColorLine clrLine= (ColorLine)arrVert[i];
				Pen pen=new Pen(clrLine.color,lineOutThick); 
				g.DrawLine(pen,clrLine.coord,0,clrLine.coord,rectImage.Bottom);	
			}			
			// Clean up.
			g.Dispose();


		}
		#endregion

	

		private void GenerateLines( Bitmap textDestination,ArrayList arrHor,ArrayList arrVert)
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

			//vertical lines
			float linesConstVert=4.1f;    //PARAM -> the bigger value, more lines (more dense on the screen)
			float minDistValueVert=0.7f; //PARAM -> the bigger value, the lines can go more left from the average distance  //always <1.0f
			float maxDistValueVert=1.3f; //PARAM -> the bigger value, the lines can go more right from the average distance //always >1.0f
			
			//num of lines
			int linesNumberVert=(int)(linesConstVert* Tools.GetNonLinear(rectImage.Width)); 
			//average distance between two adjacent lines 
			float averageDistVert=rectImage.Width/linesNumberVert; 
			//maximal distance between two adjacent lines 
			float minDistVert=averageDistVert*minDistValueVert; 
			//minimal distance between two adjacent lines 
			float maxDistVert=averageDistVert*maxDistValueVert;	

			//get the coords of the vertical lines 
			float[] arrVertLinesCoords=rand.ArrayWithOffset(minDistVert,maxDistVert,linesNumberVert);

			//Horzontal lines
			float linesConstHor=4.1f;    //PARAM -> the bigger value, more lines (more dense on the screen)
			float minDistValueHor=0.7f; //PARAM -> the bigger value, the lines can go more up from the average distance  //always <1.0f
			float maxDistValueHor=1.3f; //PARAM -> the bigger value, the lines can go more down from the average distance //always >1.0f
			
			//num of lines
			int linesNumberHor=(int)(linesConstHor* Tools.GetNonLinear(rectImage.Height)); 
			//average distance between two adjacent lines 
			float averageDistHor=rectImage.Height/linesNumberHor; 
			//maximal distance between two adjacent lines 
			float minDistHor=averageDistHor*minDistValueHor; 
			//minimal distance between two adjacent lines 
			float maxDistHor=averageDistHor*maxDistValueHor;	

			//get the coords of the Horizontal lines 
			float[] arrHorLinesCoords=rand.ArrayWithOffset(minDistHor,maxDistHor,linesNumberHor);

			//create and store lines
			for( i=0;i<linesNumberVert;i++)
			{	
				Color lineColor=rand.GetColorBetween(rMin, rMax, gMin,gMax,bMin, bMax);
				arrVert.Add(new ColorLine(lineColor,arrVertLinesCoords[i]));				
			}
		
			//create and store lines
			for( i=0;i<linesNumberHor;i++)
			{	
				Color lineColor=rand.GetColorBetween(rMin, rMax, gMin,gMax,bMin, bMax);
				arrHor.Add(new ColorLine(lineColor,arrHorLinesCoords[i]));								
			}
			
		}
	}
}
