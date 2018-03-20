using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for NegativeTextGenerator.
	/// </summary>
	internal class NegativeTextGenerator : ITextGenerator
	{
	    #region ITextGenerator Members

		public void DrawText(string text, Bitmap textDestination)
		{
			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			Rectangle rect = new Rectangle(0, 0, textDestination.Width, textDestination.Height);
			Rectangle rectForText = new Rectangle(0, textDestination.Height*15/100, textDestination.Width, textDestination.Height-textDestination.Height*15/100);
			
			// Fill in the background.
			HatchBrush hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.Black, Color.Black);
			g.FillRectangle(hatchBrush, rect);
			
			// For generating random numbers.
			Random random = new Random();

			// Create some noise
			int NumPoints = rect.Width * rect.Height / 2;
			PointF[] points = new PointF[NumPoints];
			for(int i=0; i<NumPoints; i++)
			{
				points[i] = new PointF(random.Next(rect.Width), random.Next(rect.Height));
			};
			Bitmap bm = new Bitmap(1, 1);
			bm.SetPixel(0, 0, Color.White);
			for(int i=0; i<NumPoints; i++)
				g.DrawImageUnscaled(bm, (int)points[i].X, (int)points[i].Y);
			
			// Set up the text font.
			SizeF size;
			float fontSize = rect.Height + 1;
			Font font;
			
			// Adjust the font size until the text fits within the image.
			fontSize = fontSize / (float)1.2;
			do
			{
				fontSize--;
				font = new Font("Courier New", fontSize, FontStyle.Bold);
				size = g.MeasureString(text, font);
			} while (size.Width > rect.Width);
			
			// Set up the text format.
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;

			// Draw the text.
			hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.White, Color.White);
		//	g.DrawString(text, font, hatchBrush, rect, format);
			g.DrawString(text, font, hatchBrush, rectForText, format);

			// Clean up.
			font.Dispose();
			hatchBrush.Dispose();
			g.Dispose();
		}

		#endregion
	}
}
