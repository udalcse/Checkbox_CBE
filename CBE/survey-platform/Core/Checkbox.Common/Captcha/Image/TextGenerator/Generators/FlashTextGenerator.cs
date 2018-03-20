using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class FlashTextGenerator : ITextGenerator
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
			

			//the red and blue and green part of the color are same  
			//the range the blue(red, green) color part can be set to 
			int ColorMin=0;     //PARAM  the bigger-> the letter is lighter
			int ColorMax=150;   //PARAM  the bigger-> the letter is lighter

			//the range a letter can rotate 
			float RotateMaxRight=40f; //PARAM   the bigger -> the letter more rotated to the right
			float RotateMaxLeft=-40f; //PARAM	the bigger -> the letter more rotated to the left

			//the max size of a letter box to host a single letter
			float boxWidth=rectFBorders.Width/(float)text.Length *(float)1.0;                
			float boxHeight=rectFBorders.Height/(float)text.Length * (float)2.2;                         
			float boxReduct=0.8f; //PARAM the bigger -> the letter is bigger

			//draw the background
			DrawBackground(textDestination);

			for( i=0;i<text.Length;i++)
			{		
				//random font 
				Font font=new Font(rand.GetFontName(TextStyleEnum.Flash),200,rand.FontStyle()); //ignore 200

				RectangleF rectLetter= new RectangleF(boxWidth*i,rand.BetweenTwo(0,rectFBorders.Height-boxHeight),boxWidth*boxReduct,boxHeight*boxReduct);

				GraphicsPath path= Tools.TextIntoRectPath(text.Substring(i,1),font,rectLetter);

				Matrix matrix=new Matrix();
				matrix.RotateAt(rand.BetweenTwo(RotateMaxLeft,RotateMaxRight),Tools.GetMiddle(rectLetter));
				path.Transform(matrix);

				matrix.Reset();
				matrix.Translate(rectFBorders.Left,rectFBorders.Top);
				path.Transform(matrix);
				
				g.FillPath(new SolidBrush(rand.GetColorBetween(ColorMin,ColorMax,ColorMin,ColorMax,ColorMin,ColorMax)),path);
			}
			// Clean up.
			g.Dispose();
		}
		#endregion

		private void DrawBackground( Bitmap textDestination)
		{
			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, (float)textDestination.Height);

			int i;
			RandomClass rand=new RandomClass();
			
			g.Clear(Color.LightYellow);

			//minimal and maximal values of red, green and blue 
			//you can define the range the colors of the lines can ne between
			int rMin, rMax, gMin,gMax,bMin, bMax;
			rMin=0; //PARAM 
			rMax=256;	//PARAM 
			gMin=0;	//PARAM 
			gMax=256;	//PARAM 
			bMin=0;//PARAM 
			bMax=256;	//PARAM 

			float lineThick=1.0f; //PARAM -> the bigger value, the lines will be more thick

			//vertical lines
			float linesConstVert=1.1f;    //PARAM -> the bigger value, more lines (more dense on the screen)
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
			float linesConstHor=1.1f;    //PARAM -> the bigger value, more lines (more dense on the screen)
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


			//draw lines
			for( i=0;i<linesNumberVert;i++)
			{	
				Color lineColor=rand.GetColorBetween(rMin, rMax, gMin,gMax,bMin, bMax);
				Pen pen=new Pen(lineColor,lineThick); 
				g.DrawLine(pen,arrVertLinesCoords[i],0,arrVertLinesCoords[i],rectImage.Bottom);
			}
			for( i=0;i<linesNumberHor;i++)
			{	
				Color lineColor=rand.GetColorBetween(rMin, rMax, gMin,gMax,bMin, bMax);
				Pen pen=new Pen(lineColor,lineThick); 
				g.DrawLine(pen,0,arrHorLinesCoords[i],rectImage.Right,arrHorLinesCoords[i]);
			}
			
			// Clean up.
			g.Dispose();
		}
	}
}
