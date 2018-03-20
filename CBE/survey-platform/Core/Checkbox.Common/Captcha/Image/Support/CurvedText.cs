using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing.Drawing2D;
using System.IO;

namespace Checkbox.Common.Captcha.Image.Support
{
	/// <summary>
	/// Summary description for CurvedText.
	/// </summary>
    public class CurvedText
	{
		private PointF[] arrTop;
		private PointF[] arrBottom;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_arrTop"></param>
        /// <param name="_arrBottom"></param>
		public CurvedText(PointF[] _arrTop,PointF[] _arrBottom)
		{
			arrTop=_arrTop;
			arrBottom=_arrBottom;																			  
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="g"></param>
        /// <param name="font"></param>
        /// <param name="strText"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
		public  void DrawText(RectangleF rect, Graphics g, Font font, string strText, Brush brush,Pen pen)
		{
			AdjustToNewFrame(rect);
			TextEffect( g , strText, font, brush, pen);
		}
	
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="g"></param>
        /// <param name="font"></param>
        /// <param name="strText"></param>
        /// <returns></returns>
		public GraphicsPath GetTextPath(RectangleF rect, Graphics g, Font font, string strText)
		{
			AdjustToNewFrame(rect);
			return TextEffect(g , strText, font);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newFrame"></param>
		private void  AdjustToNewFrame(RectangleF newFrame)
		{
			RectangleF originalFrame=FrameGuidlines( arrTop, arrBottom);
			float scaleX=newFrame.Width/originalFrame.Width;
			float scaleY=newFrame.Height/originalFrame.Height;

			int i;
			for (i=0; i<arrTop.Length; i++) 
			{
				arrTop[i].X-=originalFrame.Left;
				arrTop[i].X*=scaleX;
				arrTop[i].X+=newFrame.Left;

				arrTop[i].Y-=originalFrame.Top;
				arrTop[i].Y*=scaleY;
				arrTop[i].Y+=newFrame.Top;
			}

			for (i=0; i<arrBottom.Length; i++) 
			{
				arrBottom[i].X-=originalFrame.Left;
				arrBottom[i].X*=scaleX;
				arrBottom[i].X+=newFrame.Left;

				arrBottom[i].Y-=originalFrame.Top;
				arrBottom[i].Y*=scaleY;
				arrBottom[i].Y+=newFrame.Top;
			}			
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

		private RectangleF FrameGuidlines(PointF[] arrTop,PointF[] arrBottom)
		{
			int i;		

			float right=arrTop[0].X,left=arrTop[0].X,top=arrTop[0].Y,bottom=arrTop[0].Y;

			for (i=0; i<arrTop.Length; i++) 
			{
				if (arrTop[i].X >right)
				{
					right = arrTop[i].X;
				}
				if (arrTop[i].X < left) 
				{
					left=arrTop[i].X;
				}
				if (arrTop[i].Y < top)
				{
					top = arrTop[i].Y;
				}
				if (arrTop[i].Y > bottom) 
				{
					bottom=arrTop[i].Y;
				}
			}

			for (i=0; i<arrBottom.Length; i++) 
			{
				if (arrBottom[i].X > right)
				{
					right = arrBottom[i].X;
				}
				if (arrBottom[i].X < left) 
				{
					left=arrBottom[i].X;
				}
				if (arrBottom[i].Y < top)
				{
					top = arrBottom[i].Y;
				}
				if (arrBottom[i].Y > bottom) 
				{
					bottom=arrBottom[i].Y;
				}
			}
			
			return new RectangleF(left,top,right-left,bottom-top);
																			 
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="strFileName"></param>
		/// <returns></returns>
		public static PointF[] GetPointsFromFile(string strFileName ) 
		{
			ArrayList arr=new ArrayList();

			StreamReader sr = new StreamReader(strFileName); 			 
			String line;
			while ((line = sr.ReadLine()) != null) 
			{
				char[] ch={','};
				string[] coords=line.Split(ch);
				float x=float.Parse( coords[0]);
				float y=float.Parse( coords[1]);
				arr.Add(new PointF(x,y));
			}
			PointF[] arrCoords=new PointF[arr.Count];
			int i=0;
			for (i=0;i<arrCoords.Length;i++)  
			{
				arrCoords[i]=(PointF)arr[i];
			}
			return arrCoords;
		}

		void RenderPathPoints(Graphics g ,PointF[] arrPathPoints, byte[] arrPathTypes, Brush brush,Pen pen) 
		{
			g.SmoothingMode=SmoothingMode.AntiAlias;
			GraphicsPath path=new GraphicsPath(arrPathPoints,arrPathTypes);
			
			// Draw the path
			if (pen != null)
			{
				// draw the outline
				g.DrawPath(pen,path);		
			}

			if (brush != null)  			
			{   
				// fill the interior
				g.FillPath(brush,path);
			} 			
		}

		private void TextEffect(Graphics g, string strText,Font font, Brush brush,Pen pen) 
		{
			int i, iNumPts;
			SizeF size ;		  // Text size info
			int iTopInd, iBottomInd;	  // Guide array indices
			float fXScale, fYScale;	  // Scaling values

			GraphicsPath path=new GraphicsPath();
			
			path.AddString(strText,font.FontFamily,(int)font.Style,font.Size,new Point(0,0),new StringFormat());

			// Get the points and types from the current path
			PointF[] arrPathPoints=path.PathPoints; 
			byte[] arrPathTypes=path.PathTypes; 
			iNumPts=arrPathPoints.Length;

			// Get the exact extents of our text 
			
			size=GetRealTextExtent( arrPathPoints);

			// Relocate the points in the path based on the guide lines
			
			for (i=0; i < iNumPts; i++) 
			{
				// How far along is this point on the x-axis
				fXScale = (float)arrPathPoints[i].X / (float)size.Width;

				// What point on the top guide does this coorespond to
				iTopInd = (int)(fXScale * (arrTop.Length-1));
				// What point on the bottom guide does this coorespond to
				iBottomInd = (int)(fXScale * (arrBottom.Length-1));

				// How far along is this point on the y-axis
				fYScale = (float)arrPathPoints[i].Y / (float)size.Height;

				// Scale the points to their new locations
				arrPathPoints[i].X = (int)((arrBottom[iBottomInd].X* fYScale) + (arrTop[iTopInd].X * (1.0f-fYScale)));
				arrPathPoints[i].Y = (int)((arrBottom[iBottomInd].Y * fYScale) + (arrTop[iTopInd].Y * (1.0f-fYScale)));
			}

			// Draw the new path 
			RenderPathPoints(g, arrPathPoints, arrPathTypes, brush, pen);	
		}

		private GraphicsPath TextEffect(Graphics g, string strText,Font font) 
		{
			int i, iNumPts;
			SizeF size ;		  // Text size info
			int iTopInd, iBottomInd;	  // Guide array indices
			float fXScale, fYScale;	  // Scaling values

			GraphicsPath path=new GraphicsPath();
			
			path.AddString(strText,font.FontFamily,(int)font.Style,font.Size,new Point(0,0),new StringFormat());

			// Get the points and types from the current path
			PointF[] arrPathPoints=path.PathPoints; 
			byte[] arrPathTypes=path.PathTypes; 
			iNumPts=arrPathPoints.Length;

			// Get the exact extents of our text 
			
			size=GetRealTextExtent( arrPathPoints);

			// Relocate the points in the path based on the guide lines
			
			for (i=0; i < iNumPts; i++) 
			{
				// How far along is this point on the x-axis
				fXScale = (float)arrPathPoints[i].X / (float)size.Width;

				// What point on the top guide does this coorespond to
				iTopInd = (int)(fXScale * (arrTop.Length-1));
				// What point on the bottom guide does this coorespond to
				iBottomInd = (int)(fXScale * (arrBottom.Length-1));

				// How far along is this point on the y-axis
				fYScale = (float)arrPathPoints[i].Y / (float)size.Height;

				// Scale the points to their new locations
				arrPathPoints[i].X = (int)((arrBottom[iBottomInd].X* fYScale) + (arrTop[iTopInd].X * (1.0f-fYScale)));
				arrPathPoints[i].Y = (int)((arrBottom[iBottomInd].Y * fYScale) + (arrTop[iTopInd].Y * (1.0f-fYScale)));
			}

			// Draw the new path 
			return new GraphicsPath(arrPathPoints,arrPathTypes);
			
		}

		private SizeF GetRealTextExtent(PointF[] arrPathPoints)
		{
			int i;
	
			SizeF size=new SizeF(0.0f,0.0f);
			for (i=0; i<arrPathPoints.Length; i++) 
			{
				if (arrPathPoints[i].X > size.Width)
				{
					size.Width = arrPathPoints[i].X;
				}
				if (arrPathPoints[i].Y > size.Height) 
				{
					size.Height = arrPathPoints[i].Y;
				}
			}
			return size;
		}
	}
}
