using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for ChessTextGenerator.
	/// </summary>
	internal class Chess3DTextGenerator : ITextGenerator
	{
	    #region ITextGenerator Members

		public void DrawText(string text, Bitmap textDestination)
		{
			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF rectImage = new RectangleF(0, 0, (float)textDestination.Width, (float)textDestination.Height);

			//the text rectangle will be squized between these two rectangles
			RectangleF rectInner=new RectangleF(rectImage.Width*0.2f,rectImage.Height*0.2f,rectImage.Width*0.5f,rectImage.Height*0.5f);
			RectangleF rectOuter=new RectangleF(rectImage.Width*0.05f,rectImage.Height*0.05f,rectImage.Width*0.8f,rectImage.Height*0.8f);
			
			RandomClass rand=new RandomClass();

			//text rectangle 
			RectangleF rectFBorders=rand.GetBetweenRectangle(rectOuter,rectInner);

			//two rows and two columns at least
			int horLinesCount=Math.Max(3,(int)(rectImage.Height/20.0)); //the number of rows
			int vertLinesCount=Math.Max(3,(int)(rectImage.Width/20.0)); //the number of columns

			Color mainColor=Color.Black;  //PARAM 
			Color backColor=Color.White; //PARAM 
			
			float constSize=rectImage.Width; //
			
			float leftUpMin=constSize*0.1f; //see attached picture 
			float leftUpMax=constSize*0.2f;

			float rightUpMin=constSize*0.1f;
			float rightUpMax=constSize*0.2f;

			float leftDownMin=constSize*1.0f;
			float leftDownMax=constSize*1.5f;

			float rightDownMin=constSize*1.0f;
			float rightDownMax=constSize*1.5f;

			GraphicsPath pathMain=new GraphicsPath(); //receives main fields of the chessboard
			GraphicsPath pathBack=new GraphicsPath(); //receives back fields of the chessboard

			//random font 
			Font font=new Font(rand.GetFontName(TextStyleEnum.Chess3D), 200, FontStyle.Bold); //ignore 200

			//generate  the text region
			GraphicsPath pathLetters = Tools.TextIntoRectPath(text, font, rectFBorders); //get the path of the text of the same size of the passed rect
		
		
			//the points 
			PointF[] aptfDest=new PointF[4];
			aptfDest[0]=new PointF(rand.BetweenTwo(0-leftUpMin,0-leftUpMax),0);
			aptfDest[1]=new PointF(rand.BetweenTwo(rectImage.Width+ rightUpMin,rectImage.Width+rightUpMax),0);
			aptfDest[2]=new PointF(rand.BetweenTwo(0-leftDownMin,0-leftDownMax),rectImage.Height);
			aptfDest[3]=new PointF(rand.BetweenTwo(rectImage.Width+rightDownMin,rectImage.Width+rightDownMax),rectImage.Height);
	

			//get the paths of the checkboard fields 
			//they will be grouped into two different paths (regions later)
			DrawBackgroundIntoPaths(new SizeF(rectImage.Width,rectImage.Height),horLinesCount,vertLinesCount,pathMain,pathBack);

			//create wrapped paths of checkboard fields
			GraphicsPath pathMainWarped =(GraphicsPath)pathMain.Clone();
			GraphicsPath pathBackWarped =(GraphicsPath)pathBack.Clone();

			WarpMode wm = WarpMode.Perspective;

			pathMainWarped.Warp(aptfDest,pathMain.GetBounds(),new Matrix(),wm);
			pathBackWarped.Warp(aptfDest,pathBack.GetBounds(),new Matrix(),wm);

			// get regions out of the paths 
			Region rgnLetters=new Region(pathLetters);
			Region rgnMain=new Region(pathMainWarped);
			Region rgnBack=new Region(pathBackWarped);

			//drawing

			g.FillRegion(new SolidBrush(mainColor),rgnMain); //fill "main" field with main color 
			g.FillRegion(new SolidBrush(backColor),rgnBack); //fill "back" field with back color 
			g.FillRegion(new SolidBrush(mainColor),rgnLetters); //fill letters with main color 

			Region rgnIntersect=new Region(); //get the intersection of the main fields and letter area
			rgnIntersect=rgnLetters.Clone();
			rgnIntersect.Intersect(rgnMain);

			g.FillRegion(new SolidBrush(backColor),rgnIntersect); //fill the intersection with back color
			
			// Clean up.
			g.Dispose();
	
		}

		
		private void DrawBackgroundIntoPaths( SizeF sizeChess,int horLinesCount, int vertLinesCount,GraphicsPath pathMain,GraphicsPath pathBack)
		{

			//the filed size is determined with the number of the lines (fields)
			float fieldWidth=sizeChess.Width/vertLinesCount;
			float fieldHeight=sizeChess.Height/horLinesCount;
						
			int i,j;

			//add fields (rects) to one or the other path 
			for( i=0;i<vertLinesCount;i++)
			{
				for( j=0;j<horLinesCount;j++)
				{	
					//all the rects are of the same size
					RectangleF rectF=new RectangleF(i*fieldWidth,j*fieldHeight,fieldWidth,fieldHeight);
					// odd numbers mean taht this rect goes to one path (color), the even numbers mean 
					//that this rect gose to the other path (color)
					if ((i+j)%2==0) 
					{
						pathMain.StartFigure();
						pathMain.AddRectangle(rectF);
						pathMain.CloseFigure();
					}
					else
					{
						pathBack.StartFigure();
						pathBack.AddRectangle(rectF);
						pathBack.CloseFigure();
					}
				}				
			}
			
		}

		#endregion
	}
}

