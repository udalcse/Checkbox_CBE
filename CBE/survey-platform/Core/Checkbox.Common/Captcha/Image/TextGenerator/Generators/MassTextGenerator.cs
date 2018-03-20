using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class MassTextGenerator : ITextGenerator
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
			int ColorMax=120;   //PARAM  the bigger-> the letter is lighter

			//the range a letter can rotate 
			float RotateMaxRight=40f; //PARAM   the bigger -> the letter more rotated to the right
			float RotateMaxLeft=-40f; //PARAM	the bigger -> the letter more rotated to the left

			//the max size of a letter box to host a single letter
			float boxWidth=rectFBorders.Width/(float)text.Length *(float)1.0;                
			float boxHeight=rectFBorders.Height/(float)text.Length * (float)2.2;                         
			float boxReduct=0.8f; //PARAM the bigger -> the letter is bigger

			//draw the background
			DrawBackground(textDestination,new SizeF(boxWidth,boxHeight));

			for( i=0;i<text.Length;i++)
			{		
				//random font 
				Font font=new Font(rand.GetFontName(TextStyleEnum.Mass),200,rand.FontStyle()); //ignore 200

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

		private void DrawBackground( Bitmap textDestination, SizeF BigLetter)
		{
			
			//SizeF BigLetter=the size of big letters (the main letters to validate )

			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, (float)textDestination.Height);
			
			int i;
			RandomClass rand=new RandomClass();

			//let the small letter rotate at any angle 
			float RotateMaxLeft=180f;    //PARAM   the bigger -> the letter more rotated to the right
			float RotateMaxRight=-180f;  //PARAM   the bigger -> the letter more rotated to the right
			
			//small letter size range is given over BigLetter size 
			SizeF smallLetterSizeMin=new SizeF(BigLetter.Width*0.3f,BigLetter.Height*0.3f); //PARAM the bigger numbers -> the bigger letters 
			SizeF smallLetterSizeMax=new SizeF(BigLetter.Width*0.7f,BigLetter.Height*0.7f); //PARAM the bigger numbers -> the bigger letters 


			//the red and blue and green part of the color are same  
			//the range the blue(red, green) color part can be set to 
			int ColorMin=170;     //PARAM  the bigger-> the letter is lighter
			int ColorMax=256;     //PARAM  the bigger-> the letter is lighter

			//the number of small letters 
			int LettersCountConst=(int)(rectImage.Width*rectImage.Height*0.007f); //PARAM the bigger number ->more letters are generated (the control of density)

			
			//background 
			g.Clear(Color.LightYellow);

			//draw small letters 
			for( i=0;i<LettersCountConst;i++)
			{
				//random font 
				Font font=new Font(rand.GetFontName(),2,rand.FontStyle()); //ignore 2

				SizeF letterSize=new SizeF(rand.BetweenTwo(smallLetterSizeMin.Width,smallLetterSizeMax.Width),rand.BetweenTwo(smallLetterSizeMin.Height,smallLetterSizeMax.Height));
				PointF letterPos=rand.GetPointInsideRect(rectImage);
                RectangleF rectLetter=new RectangleF(letterPos,letterSize);

				GraphicsPath path= Tools.TextIntoRectPath(rand.GetAlphaNumeric().ToString(),font,rectLetter);

				Matrix matrix=new Matrix();
				matrix.RotateAt(rand.BetweenTwo(RotateMaxLeft,RotateMaxRight),Tools.GetMiddle(rectLetter));
				path.Transform(matrix);

				g.FillPath(new SolidBrush(rand.GetColorBetween(ColorMin,ColorMax,ColorMin,ColorMax,ColorMin,ColorMax)),path);
				
			}
			
			// Clean up.
			g.Dispose();
		}
		#endregion
	}
}
