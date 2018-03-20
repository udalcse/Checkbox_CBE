using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class Graffiti2TextGenerator : ITextGenerator
	{
	    public void DrawText(string text, Bitmap textDestination)
		{
			

			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, textDestination.Height);
		
			int i;
			RandomClass rand=new RandomClass();

			RectangleF rectInner=new RectangleF(rectImage.Width*0.1f,rectImage.Height*0.0f,rectImage.Width*0.85f,rectImage.Height*0.8f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectImage,rectInner);
			
			int PixelCountConst=(int)(rectImage.Width*rectImage.Height*0.5f); //PARAM
			
			g.Clear(Color.White);

			SolidBrush fillBrush=new SolidBrush(Color.Green);
			Pen pen=new Pen(Color.Red,2.0f);

			//draw black points 
			for( i=0;i<PixelCountConst;i++)
			{
				PointF pt=rand.GetPointInsideRect(rectImage);
				g.FillEllipse(fillBrush,new Rectangle((int)pt.X,(int)pt.Y,1,1));
			}
			
			//random font 
			Font font=new Font(rand.GetFontName(),200,FontStyle.Bold);

			//create path 
			CurvedText objCurvedText = CurvedTextShapes.GetCurvedText(CurvedTextShapes.CurvedTextShapeEnum.Shape1);			
			objCurvedText.DrawText(rectFBorders,g,font,text,null,pen);
		

			// Clean up.
			g.Dispose();

		}
		
	}
}
