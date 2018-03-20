using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class CirclesTextGenerator : ITextGenerator
	{
	    public void DrawText(string text, Bitmap textDestination)
		{
			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, (float)textDestination.Height);
			
			RandomClass rand=new RandomClass();

			//inner rect (the text can not be smaller than this rectangle)
			RectangleF rectInner=new RectangleF(rectImage.Width*0.2f,rectImage.Height*0.2f,rectImage.Width*0.5f,rectImage.Height*0.5f);

			//rectangle to fill with text
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectImage,rectInner);

			//random font 
			Font font=new Font(rand.GetFontName(TextStyleEnum.Circles),200);

			//draw background 
			g.Clear(Color.White);

			//generate  the text region
			GraphicsPath pathLetters = Tools.TextIntoRectPath(text,font,rectFBorders);
			Region rgnLetters=new Region(pathLetters);

			//generate the circles region
			GraphicsPath pathCircles=DrawBackgroundIntoPath(textDestination);
			Region rgnCircles=new Region(pathCircles);

			Color mainColor=Color.Black;  //PARAM letters and circles color 
			Color backColor=Color.White; //PARAM background and intersect color 
			g.Clear(backColor);

			g.FillRegion(new SolidBrush(mainColor),rgnLetters);
			g.FillRegion(new SolidBrush(mainColor),rgnCircles);

			Region rgnIntersect=new Region();
			rgnIntersect=rgnLetters.Clone();
			rgnIntersect.Intersect(rgnCircles);
			
			g.FillRegion(new SolidBrush(backColor),rgnIntersect);

			//g.FillPath(new SolidBrush(mainColor),pathLetters);
			// Clean up.
			font.Dispose();
			g.Dispose();


		}

		private GraphicsPath DrawBackgroundIntoPath( Bitmap textDestination)
		{
			
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, (float)textDestination.Height);
			
			int i;
			float middle=(rectImage.Width+ rectImage.Height)/2.0f;

			RandomClass rand=new RandomClass();
			float CircleRadiusMin=middle*0.05f; //PARAM the min radius of a circle 
			float CircleRadiusMax=middle*0.20f; //PARAM the max radius of a circle 
			
			float CircleCountMin=middle*0.04f; //PARAM the min number of circles 
			float CircleCountMax=middle*0.08f; //PARAM the max number of circles 

			float[] arrOffsets=rand.ArrayWithOffset(2*CircleRadiusMin,2*CircleRadiusMax,(int)rand.BetweenTwo(CircleCountMin,CircleCountMax));

			//insert 0 as the first element and copy the rest 
			float[] arrOffsetsWithZero=new float[arrOffsets.Length+1];
			arrOffsetsWithZero[0]=0.0f;
			for( i=0;i<arrOffsets.Length;i++)
			{
				arrOffsetsWithZero[i+1]=arrOffsets[i];
			}

			//draw into path 
			GraphicsPath path=new GraphicsPath();
			for( i=0;i<arrOffsets.Length;i++)
			{
				int radius=(int)rand.BetweenTwo(CircleRadiusMin,CircleRadiusMax);
				path.AddEllipse((int)((arrOffsetsWithZero[i+1]+arrOffsetsWithZero[i])/2),(int)rand.BetweenTwo(0,rectImage.Height-CircleRadiusMax*2),radius*2,radius*2);
			}

			return path;
		}

	}
}