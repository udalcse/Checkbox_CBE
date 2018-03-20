using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Checkbox.Common.Captcha.Image.Support
{
    /// <summary>
    /// 
    /// </summary>
	public class Tools
	{
		/// <summary>
		/// 
		/// </summary>
		public Tools()
		{
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
		public  static float GetNonLinear(float num)
		{
			return (float)Math.Log((double)num,2.0);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <param name="base_number"></param>
        /// <returns></returns>
		public  static float GetNonLinear(float num,float base_number)
		{
			return (float)Math.Log((double)num,(double)base_number);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rectFBounds"></param>
        /// <returns></returns>
		public static Rectangle ToRectangle(RectangleF rectFBounds)
		{
			return new Rectangle((int) rectFBounds.Left,(int)rectFBounds.Top,(int)rectFBounds.Width,(int)rectFBounds.Height);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rectFBounds"></param>
        /// <returns></returns>
		public static PointF GetMiddle(RectangleF rectFBounds)
		{
			return new PointF((rectFBounds.Left+rectFBounds.Right)/2,(rectFBounds.Top+rectFBounds.Bottom)/2);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="font"></param>
        /// <param name="rectF"></param>
        /// <returns></returns>
		public static  GraphicsPath TextIntoRectPath(String str, Font font,RectangleF rectF)
		{
			GraphicsPath path=new GraphicsPath();

			path.AddString(str,font.FontFamily,(int)font.Style,font.Size,new Point(0,0),new StringFormat());
			RectangleF rectFBounds=path.GetBounds();
			//PointF[] aptfDest={new PointF(0,0),new PointF(rectF.Width,0),new PointF(0,rectF.Height)};
			//Matrix matrix=new Matrix(rectFBounds,aptfDest);

			Matrix matrix=new Matrix();
			matrix.Reset();
			matrix.Translate(-rectFBounds.Left,-rectFBounds.Top);
			path.Transform(matrix);

			matrix.Reset();
			matrix.Scale(rectF.Width/rectFBounds.Width ,rectF.Height/rectFBounds.Height);
			path.Transform(matrix);

			matrix.Reset();
			matrix.Translate(rectF.Left,rectF.Top);
			path.Transform(matrix);	

			return path;
		}

//		public static void DrawBitmapInvert(Graphics grfxDest,int xDest, int yDest, Bitmap srcImage)
//		{
//			DrawBitmapROP( grfxDest, xDest, yDest, srcImage, Win32Support.TernaryRasterOperations.SRCINVERT);
//		}

//		private static void DrawBitmapROP(Graphics grfxDest,int xDest, int yDest, Bitmap srcImage, Win32Support.TernaryRasterOperations dwRop)
//		{  
//
//			try
//			{
//				
//				IntPtr hBmp1,hBmp2;
//				IntPtr oldBmp1,oldBmp2;
//				IntPtr memDC1, memDC2; 
//		    
//				//create a temp bitmap
//				Bitmap bmp = new Bitmap(srcImage.Width, srcImage.Height);
//
//				
//				//get bitmap handlers 
//				hBmp1=srcImage.GetHbitmap();
//				hBmp2=bmp.GetHbitmap();
//
//				//craete mem DCs
//				memDC1 = Win32Support.CreateCompatibleDC(grfxDest.GetHdc());
//				memDC2 = Win32Support.CreateCompatibleDC(memDC1);
//
//				//select bitmaps
//				oldBmp1 = Win32Support.SelectObject(memDC1, hBmp1);
//				oldBmp2 = Win32Support.SelectObject(memDC2, hBmp2);
//
//				Win32Support.BitBlt(memDC2, 0, 0, srcImage.Width, srcImage.Height, memDC1, 0, 0, Win32Support.TernaryRasterOperations.PATPAINT);
//
//				Win32Support.SelectObject(memDC1, oldBmp1);
//				Win32Support.SelectObject(memDC2, oldBmp2);
//
//				Win32Support.DeleteDC(memDC1);
//				Win32Support.DeleteDC(memDC2);
//
//				grfxDest.DrawImage(bmp,xDest,yDest);
//				Win32Support.DeleteObject(hBmp2);
//			}
//			catch(Exception ex)
//			{
//				int ii=0;
//			}
//		}

	}

    /// <summary>
    /// 
    /// </summary>
	class ColorLine
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_color"></param>
        /// <param name="_coord"></param>
		public ColorLine(Color _color,float _coord)
		{
			color=_color;
			coord=_coord;
		}

        /// <summary>
        /// 
        /// </summary>
		public Color color;

        /// <summary>
        /// 
        /// </summary>
		public float coord;
	}

    /// <summary>
    /// 
    /// </summary>
	class Line
	{
		public Line(Color _color,PointF _x1,PointF _x2)
		{
			color=_color;
			x1=_x1;
			x2=_x2;
		}

        /// <summary>
        /// 
        /// </summary>
		public Color color;

        /// <summary>
        /// 
        /// </summary>
		public PointF x2;	

        /// <summary>
        /// 
        /// </summary>
		public PointF x1;
	}
}

//
//
//
//
//
//
//
//			Graphics grfxSource=Graphics.FromImage(srcImage);
//			IntPtr dcSource = grfxSource.GetHdc();
//
//			Bitmap MyImage = new Bitmap(srcImage.Width,srcImage.Height);
//						
//			IntPtr dcDest = Win32Support.CreateCompatibleDC(dcSource);
//			Win32Support.SelectObject(dcDest, MyImage.GetHbitmap());
//			Graphics grfxDest2=Graphics.FromHdc(dcDest);
//			IntPtr dcDest2=grfxDest2.GetHdc();
//
//			Win32Support.BitBlt(dcDest2, 0,0, srcImage.Width,srcImage.Height, dcSource, 0, 0, Win32Support.TernaryRasterOperations.WHITENESS);
//
//			grfxSource.ReleaseHdc(dcSource);
//			grfxDest2.ReleaseHdc(dcDest);
//			
//			
//
//			srcImage.Save(@"c:\sourcejakov.jpg", ImageFormat.Jpeg);
//			MyImage.Save(@"c:\destjakov.jpg", ImageFormat.Jpeg);

//            IntPtr dcDest = grfxDest.GetHdc();
//
//			Graphics grfxSource=Graphics.FromImage(srcImage);
//			IntPtr dcSource = grfxSource.GetHdc();
//
//			Win32Support.BitBlt(dcDest, xDest,yDest, srcImage.Width,srcImage.Height, dcSource, 0, 0, dwRop);
//
//			grfxDest.ReleaseHdc(dcDest);
//			grfxSource.ReleaseHdc(dcSource);
//		}
//	
//	}
//}
