using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class BlackOverlapTextGenerator : ITextGenerator
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

			//inner rect (the text can not be smaller tahn this rectangle)
			RectangleF rectInner=new RectangleF(rectImage.Width*0.1f,rectImage.Height*0.1f,rectImage.Width*0.5f,rectImage.Height*0.5f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectImage,rectInner);

			// frame the rectangle 
			Pen pen2= new Pen(Color.GreenYellow,(float)1);
			g.DrawRectangle(pen2,Tools.ToRectangle(rectFBorders));		
		
			//random font 
			Font font=new Font(rand.GetFontName(TextStyleEnum.BlackOverlap),200);
		
			//overlaps
			float[] arrOverLaps=new float[text.Length]; 
			for( i=0;i<arrOverLaps.Length;i++)
			{
				arrOverLaps[i]=rand.BetweenTwo(0.03f,0.13f);
			}
			//sum overlapps 
			float overlapSum=0.0f;
			for( i=0;i<arrOverLaps.Length-1;i++)
			{
				overlapSum += arrOverLaps[i];
			}

			//oversized rectangle
			RectangleF rectFBordersOver= rectFBorders;	
			rectFBordersOver.Width=rectFBorders.Width*(1.0f+overlapSum);
		
			//
			float[] arrLetterWidths=new float[text.Length]; 

			for( i=0;i<text.Length;i++)
			{
				//get letter width 
				GraphicsPath path=new GraphicsPath();
				path.AddString(text,font.FontFamily,(int)font.Style,font.Size,new Point(0,0),new StringFormat());
				RectangleF rectFBounds=path.GetBounds();
				arrLetterWidths[i]=rectFBounds.Width;
			}

			//normalize widths 
			float widthSum=0.0f;
			for( i=0;i<text.Length;i++)
			{
				widthSum+=arrLetterWidths[i];
			}
			for( i=0;i<text.Length;i++)
			{
				arrLetterWidths[i]/=widthSum;
			}

			//draw background 
			g.Clear(Color.White);

			//draw all the text 
			float offset=0.0f;
			for( i=0;i<text.Length;i++)
			{
				//one letter rectangle
				RectangleF rectF=new RectangleF(rectFBorders.Left+offset,rectFBorders.Top,arrLetterWidths[i]*rectFBordersOver.Width,rectFBorders.Height);
				GraphicsPath path= Tools.TextIntoRectPath(text.Substring(i,1),font,rectF);
	

				g.FillPath(new SolidBrush(Color.Black),path);
				
				offset+=arrLetterWidths[i]*rectFBordersOver.Width-arrOverLaps[i]*rectFBorders.Width;

			}

			// Clean up.
			font.Dispose();
			g.Dispose();
		}

		#endregion
	}
}