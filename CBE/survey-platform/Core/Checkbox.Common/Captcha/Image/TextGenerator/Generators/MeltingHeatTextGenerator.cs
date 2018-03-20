using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class MeltingHeatTextGenerator : ITextGenerator
	{
	    public void DrawText(string text, Bitmap textDestination)
		{
			
			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, (float)textDestination.Height);
			
			//int i;
			RandomClass rand=new RandomClass();

		

			//inner rect (the text can not be smaller than this rectangle)
			RectangleF rectInner=new RectangleF(rectImage.Width*0.1f,rectImage.Height*(0.0f),rectImage.Width*0.85f,rectImage.Height*0.8f);
			
			//inner rect (the text can not be smaller than this rectangle)
			RectangleF rectOuter=new RectangleF(rectImage.Width*0.1f,rectImage.Height*0.1f,rectImage.Width*0.85f,rectImage.Height*0.8f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectOuter,rectInner);
			
			//random font 
			Font font=new Font(rand.GetFontName(),200);


			float fRatio=0.19f; //PARAM  the smaller value -> the larger halo 
			
			Color colorHalo=Color.Red;
			Color colorText=Color.Black;

			//the pen for the halo drawing 
			Pen penHallo=new Pen(colorHalo,1);
			penHallo.Alignment=PenAlignment.Outset;

			//the brush to fill the text 
			SolidBrush fillText=new SolidBrush(colorText);

			//the brush to fill the background
			Color fillBack=colorText;

			//create the text path 
			CurvedText objCurvedText = CurvedTextShapes.GetCurvedText(CurvedTextShapes.CurvedTextShapeEnum.Shape1);			
			GraphicsPath path=objCurvedText.GetTextPath(rectFBorders,g,font,text);

			//Create a bitmap in a fixed ratio to the original drawing area.
			Bitmap bmSmall=new Bitmap((int)(textDestination.Width*fRatio), (int)(textDestination.Height *fRatio));

			//create the small text path 
			Matrix mx=new Matrix();
			mx.Scale(fRatio,fRatio);
			GraphicsPath pathSmall=(GraphicsPath )path.Clone();
			pathSmall.Transform(mx);

			//Get the graphics object for the image.  
			Graphics grfxSmall=Graphics.FromImage(bmSmall);

			grfxSmall.Clear(fillBack);

			//Draw around the outline of the path
			grfxSmall.DrawPath(penHallo,pathSmall);
			grfxSmall.FillPath(new SolidBrush(penHallo.Color),pathSmall);

			//and then fill in for good measure.  
			//smallGrfx.FillPath(Brushes.Yellow,path);

			//We no longer need this graphics object 
			grfxSmall.Dispose();

			//setup the smoothing mode for path drawing
			g.SmoothingMode=SmoothingMode.AntiAlias;

			//and the interpolation mode for the expansion of the halo bitmap 
			g.InterpolationMode=InterpolationMode.HighQualityBilinear;

			//expand the halo making the edges nice and fuzzy. 
			Rectangle rectDest=new Rectangle(0,0,(int)textDestination.Width,(int)textDestination.Height);
			Rectangle rectSource=new Rectangle(0,0,(int)bmSmall.Width,(int)bmSmall.Height);
			g.DrawImage(bmSmall,rectDest,rectSource,GraphicsUnit.Pixel);


			//Redraw the original text 
			g.FillPath(fillText,path);

			//---------------

			font.Dispose();
			g.Dispose();


		}
		
	}
}
