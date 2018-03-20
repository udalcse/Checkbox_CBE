using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
namespace Checkbox.Common.Captcha.Image.Support
{
    /// <summary>
    /// 
    /// </summary>
    public class Win32Support
	{
		/// <summary>
		/// Enumeration to be used for those Win32 function that return BOOL
		/// </summary>
		public enum Bool 
		{
            /// <summary>
            /// 
            /// </summary>
			False= 0,

            /// <summary>
            /// 
            /// </summary>
			True
		};

		/// <summary>
		/// Enumeration for the raster operations used in BitBlt.
		/// In C++ these are actually #define. But to use these
		/// constants with C#, a new enumeration type is defined.
		/// </summary>
		public enum TernaryRasterOperations
		{
            /// <summary>
            /// 
            /// </summary>
			SRCCOPY     = 0x00CC0020, /* dest = source                   */

            /// <summary>
            /// 
            /// </summary>
            SRCPAINT = 0x00EE0086, /* dest = source OR dest           */

            /// <summary>
            /// 
            /// </summary>
            SRCAND = 0x008800C6, /* dest = source AND dest          */
            /// <summary>
            /// 
            /// </summary>
            SRCINVERT = 0x00660046, /* dest = source XOR dest          */
            /// <summary>
            /// 
            /// </summary>
            SRCERASE = 0x00440328, /* dest = source AND (NOT dest )   */
            /// <summary>
            /// 
            /// </summary>
            NOTSRCCOPY = 0x00330008, /* dest = (NOT source)             */
            /// <summary>
            /// 
            /// </summary>
            NOTSRCERASE = 0x001100A6, /* dest = (NOT src) AND (NOT dest) */
            /// <summary>
            /// 
            /// </summary>
            MERGECOPY = 0x00C000CA, /* dest = (source AND pattern)     */
            /// <summary>
            /// 
            /// </summary>
            MERGEPAINT = 0x00BB0226, /* dest = (NOT source) OR dest     */
            /// <summary>
            /// 
            /// </summary>
            PATCOPY = 0x00F00021, /* dest = pattern                  */
            /// <summary>
            /// 
            /// </summary>
            PATPAINT = 0x00FB0A09, /* dest = DPSnoo                   */
            /// <summary>
            /// 
            /// </summary>
            PATINVERT = 0x005A0049, /* dest = pattern XOR dest         */
            /// <summary>
            /// 
            /// </summary>
            DSTINVERT = 0x00550009, /* dest = (NOT dest)               */
            /// <summary>
            /// 
            /// </summary>
            BLACKNESS = 0x00000042, /* dest = BLACK                    */
            /// <summary>
            /// 
            /// </summary>
            WHITENESS = 0x00FF0062, /* dest = WHITE                    */
		};

//		/// <summary>
//		/// CreateCompatibleDC
//		/// </summary>
//		[DllImport("gdi32.dll", ExactSpelling=true, SetLastError=true)]
//		public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
//
//		/// <summary>
//		/// DeleteDC
//		/// </summary>
//		[DllImport("gdi32.dll", ExactSpelling=true, SetLastError=true)]
//		public static extern Bool DeleteDC(IntPtr hdc);
//
//		/// <summary>
//		/// SelectObject
//		/// </summary>
//		[DllImport("gdi32.dll", ExactSpelling=true)]
//		public static extern IntPtr SelectObject(IntPtr hDC, IntPtr
//			hObject);
//
//		/// <summary>
//		/// DeleteObject
//		/// </summary>
//		[DllImport("gdi32.dll", ExactSpelling=true, SetLastError=true)]
//		public static extern Bool DeleteObject(IntPtr hObject);
//
//		/// <summary>
//		/// CreateCompatibleBitmap
//		/// </summary>
//		[DllImport("gdi32.dll", ExactSpelling=true, SetLastError=true)]
//		public static extern IntPtr CreateCompatibleBitmap(IntPtr hObject,
//			int width, int height);
//
//		/// <summary>
//		/// BitBlt
//		/// </summary>
//		/// 
//		[DllImport("gdi32.dll", ExactSpelling=true, SetLastError=true)]
//		public static extern Bool BitBlt(IntPtr hObject, int nXDest, int
//			nYDest, int nWidth, int nHeight, IntPtr hObjSource, int nXSrc, int
//			nYSrc, TernaryRasterOperations dwRop);
	}
}