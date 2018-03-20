using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for SnowTextGenerator.
	/// </summary>
	internal class SnowTextGenerator : ITextGenerator
	{
	    #region ITextGenerator Members

		public void DrawText(string text, Bitmap textDestination)
		{
			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			Rectangle rect = new Rectangle(0, 0, textDestination.Width, textDestination.Height);

			// Fill in the background.
			HatchBrush hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.LightGray, Color.White);
			g.FillRectangle(hatchBrush, rect);

			// Set up the text font.
			SizeF size;
			float fontSize = rect.Height + 1;
			Font font;
			// Adjust the font size until the text fits within the image.
			do
			{
				fontSize--;
				font = new Font("Arial", fontSize, FontStyle.Bold);
				size = g.MeasureString(text.ToLower(), font);
			} while (size.Width > rect.Width);

			// Set up the text format.
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;

			// Create a path using the text and warp it randomly.

			// For generating random numbers.
			Random random = new Random();

			GraphicsPath path = new GraphicsPath();
			path.AddString(text.ToLower(), font.FontFamily, (int) font.Style, font.Size, rect, format);
			float v = 4F;
			PointF[] points =
			{
				new PointF(random.Next(rect.Width) / v, random.Next(rect.Height) / v),
				new PointF(rect.Width - random.Next(rect.Width) / v, random.Next(rect.Height) / v),
				new PointF(random.Next(rect.Width) / v, rect.Height - random.Next(rect.Height) / v),
				new PointF(rect.Width - random.Next(rect.Width) / v, rect.Height - random.Next(rect.Height) / v)
			};
			Matrix matrix = new Matrix();
			matrix.Translate(0F, 0F);
			path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);

			// Add some random noise.
			hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.LightGray, Color.DarkGray);
			int m = Math.Max(rect.Width, rect.Height);
			for (int i = 0; i < (int) (rect.Width * rect.Height / 30F); i++)
			{
				int x = random.Next(rect.Width);
				int y = random.Next(rect.Height);
				int w = random.Next(m / 50);
				int h = random.Next(m / 50);
				g.FillEllipse(hatchBrush, x, y, w, h);
			}

			// Draw the text.
			hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.LightGray, Color.Black);
			g.FillPath(hatchBrush, path);

			// Clean up.
			font.Dispose();
			hatchBrush.Dispose();
			g.Dispose();
		}

		#endregion
	}
}
