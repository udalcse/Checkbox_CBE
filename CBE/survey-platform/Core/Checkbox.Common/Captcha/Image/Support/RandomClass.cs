using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing.Drawing2D;

namespace Checkbox.Common.Captcha.Image.Support
{
    /// <summary>
    /// 
    /// </summary>
    public class RandomClass
	{
		private Random generator;

        /// <summary>
        /// 
        /// </summary>
		public RandomClass()
		{
			generator = new Random();
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="outerRect"></param>
        /// <param name="innerRect"></param>
        /// <returns></returns>
		public RectangleF GetBetweenRectangle(RectangleF outerRect, RectangleF innerRect)
		{
			float Top=BetweenTwo(outerRect.Top,innerRect.Top);
			float Left=BetweenTwo(outerRect.Left,innerRect.Left);	
			float Right=BetweenTwo(innerRect.Right,outerRect.Right);	
			float Bottom=BetweenTwo(innerRect.Bottom,outerRect.Bottom);	
			
			return new RectangleF(Left,Top,Right-Left,Bottom-Top);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outerRect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
		public RectangleF GetSameSizeRectangle(RectangleF outerRect, float width,float height)
		{
			//the rect inside which  topleft coord of the random rect can be placed
			RectangleF insideRect=new RectangleF(outerRect.Left,outerRect.Top,outerRect.Width-width,outerRect.Height-height); 
			PointF ptTopLeft=this.GetPointInsideRect(insideRect);
            return  new RectangleF(ptTopLeft,new SizeF(width,height));

		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="number"></param>
        /// <returns></returns>
		public float[] ArrayWithOffset(float min, float max,int number)
		{
			float[] arrNumbers =new float[number];
			int i;

			float offset=0;
			for(i=0; i<number;i++)
			{
				float num=BetweenTwo(min,max);
				offset+=num;

				arrNumbers[i]=offset;
				
			}
			return  arrNumbers;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public FontStyle FontStyle()
		{
			
			FontStyle fontstyle=System.Drawing.FontStyle.Regular;
			
			
			int num=generator.Next(1,4);
			switch (num)
			{
				case 1:
					fontstyle=System.Drawing.FontStyle.Bold;
					break;
				case 2:
					fontstyle=System.Drawing.FontStyle.Italic;
					break;
				case 3:
					fontstyle=System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold;
					break;
				case 4:
					break;
			}
			return fontstyle;			
		}
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public Color GetColor()
		{
			return Color.FromArgb(generator.Next(256),generator.Next(256),generator.Next(256));
			
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rMin"></param>
        /// <param name="rMax"></param>
        /// <param name="gMin"></param>
        /// <param name="gMax"></param>
        /// <param name="bMin"></param>
        /// <param name="bMax"></param>
        /// <returns></returns>
		public Color GetColorBetween(int rMin,int rMax,int gMin,int gMax,int bMin,int bMax)
		{
			return Color.FromArgb(BetweenTwo(rMin,rMax),BetweenTwo(gMin,gMax),BetweenTwo(bMin,bMax));
			
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public char GetAlphaNumeric()
		{
			string alphanumeric = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
						
			int index = generator.Next(0,alphanumeric.Length-1);
			
			return alphanumeric[index];
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
		public float BetweenTwo(float min, float max)
		{
						
			double d=generator.NextDouble();
			return (max-min)*(float)d +min;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
		public int BetweenTwo(int min, int max)
		{
			return generator.Next(min,max);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Rect"></param>
        /// <returns></returns>
		public PointF GetPointInsideRect(RectangleF Rect)
		{
						
			return new PointF(BetweenTwo(Rect.Left,Rect.Right),BetweenTwo(Rect.Top,Rect.Bottom));
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
		public PointF GetPointAroundPoint(PointF pt,float radius)
		{
						
            return GetPointInsideRect(new RectangleF(pt.X-radius/2,pt.Y-radius/2,radius,radius));
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="pt3"></param>
        /// <param name="pt4"></param>
		public void GetFourPointsInRect(RectangleF Rect,out PointF pt1,out PointF pt2,out PointF pt3,out PointF pt4 )
		{
			pt1=GetPointInsideRect(Rect);
			pt2=GetPointInsideRect(Rect);
			pt3=GetPointInsideRect(Rect);
			pt4=GetPointInsideRect(Rect);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public string GetFontName()
		{
			return "Times New Roman";			
		}  

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textStyle"></param>
        /// <returns></returns>
		public string GetFontName(TextStyleEnum textStyle)
		{
			StringCollection fonts = new StringCollection();
			switch(textStyle) 
			{
				case TextStyleEnum.Stitch:
					fonts.Add("Arial");
					fonts.Add("Verdana");
					fonts.Add("Times New Roman");
					fonts.Add("Tahoma");
					fonts.Add("Courier New");
					break;
				case TextStyleEnum.WantedCircular:
					fonts.Add("Times New Roman");
					break;

			default:
				fonts.Add("Arial");
				fonts.Add("Verdana");
				fonts.Add("Times New Roman");
				fonts.Add("Tahoma");
				fonts.Add("Courier New");
				break;
			}
			int ran = generator.Next(fonts.Count);
			return fonts[ran];
		}  


	}
}
