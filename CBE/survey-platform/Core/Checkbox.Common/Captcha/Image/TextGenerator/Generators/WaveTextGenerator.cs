using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for WaveTextGenerator.
	/// </summary>
	internal class WaveTextGenerator : ITextGenerator
	{
	    #region ITextGenerator Members

		public void DrawText(string textSource, Bitmap textDestination)
		{
			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			Rectangle rect = new Rectangle(0, 0, textDestination.Width, textDestination.Height);
			HatchBrush hatchBrush = new HatchBrush(HatchStyle.LargeConfetti, Color.White, Color.White);
			
			// For generating random numbers.
			Random random = new Random();
								
			// Fill in the background.
			g.FillRectangle(hatchBrush, 0, 0, rect.Width, rect.Height);
			
			// Create some noise
			int NumPoints = rect.Width * rect.Height / 7;
			PointF[] points = new PointF[NumPoints];
			for(int i=0; i<NumPoints; i++)
			{
				points[i] = new PointF(random.Next(rect.Width), random.Next(rect.Height));
			};
			Bitmap bm = new Bitmap(1, 1);
			bm.SetPixel(0, 0, Color.Black);
			for(int i=0; i<NumPoints; i++)
				g.DrawImageUnscaled(bm, (int)points[i].X, (int)points[i].Y);

			// Set up the text font.
			SizeF size;
			Font font;
			float fontSize = rect.Height + 1;

			// Adjust the font size until the text fits within the image.
			string text = textSource.ToLower();
			fontSize=fontSize/(float)1.5;
			do
			{
				fontSize--;
				font = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
				size = g.MeasureString(text, font);
			} while (size.Width > rect.Width || size.Height > rect.Height);

			// Set up the text format.
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
	
			// Draw the text.
			hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.Black, Color.Black);
			for(int i=0; i<text.Length; i++)
			{
				int randomHeight = random.Next(rect.Height-(int)font.Size-5);

				Rectangle currentRect = new Rectangle(i*rect.Width/text.Length, randomHeight, rect.Width/text.Length, (int)font.Size + 5);
				g.DrawString(text.Substring(i,1),font,hatchBrush,currentRect,format);
			}

			// Clean up.
			font.Dispose();
			hatchBrush.Dispose();
			g.Dispose();
		}
		#endregion
	}
}
