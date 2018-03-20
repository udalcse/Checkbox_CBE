using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for DistortionTextGenerator.
	/// </summary>
	internal class DistortionTextGenerator : ITextGenerator
	{
	    #region ITextGenerator Members

		public void DrawText(string text, Bitmap textDestination)
		{
			// Create a graphics object for drawing.
			Graphics g = Graphics.FromImage(textDestination);
			g.SmoothingMode = SmoothingMode.AntiAlias;

			Rectangle rect = new Rectangle(0, 0, textDestination.Width, textDestination.Height);
			
			// Fill in the background.
			HatchBrush hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.Black, Color.Black);			// Set up the text font.
			
			SizeF size;
			float fontSize = rect.Height + 1;
			Font font;

			// Adjust the font size until the text fits within the image.
			fontSize = fontSize / (float)1.5;
			do
			{
				fontSize--;
				font = new Font("Arial", fontSize, FontStyle.Regular);
				size = g.MeasureString(text, font);
			} while (size.Width > rect.Width);
			g.Dispose();
			
			// Set up the text format.
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			
			// Draw the black text on white background.
			Bitmap b = new Bitmap(rect.Width, rect.Height);
			Graphics g1 = Graphics.FromImage(b);
			g1.SmoothingMode = SmoothingMode.AntiAlias;
			hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.White, Color.White);
			g1.FillRectangle(hatchBrush, rect);
			hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.Black, Color.Black);
			g1.DrawString(text, font, hatchBrush, rect, format);
			g1.DrawImage(b, rect);

			Bitmap copy = new Bitmap(rect.Width, rect.Height);
			Graphics g2 = Graphics.FromImage(copy);
			g2.SmoothingMode = SmoothingMode.AntiAlias;
			hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.White, Color.White);
			g2.FillRectangle(hatchBrush, rect);
			hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.Black, Color.Black);
			g2.DrawString(text, font, hatchBrush, rect, format);
			g2.DrawImage(copy, rect);
			
			// Distortion
			double distortion = 5;
			for (int y = 0; y < rect.Height; y++)
			{
				for (int x = 0; x < rect.Width; x++)
				{
					int newX = (int)(x + (distortion * Math.Sin(Math.PI * y / 32.0)));
					int newY = (int)(y + (distortion * Math.Cos(Math.PI * x / 32.0)));
					if (newX <= 0 || newX >= rect.Width) newX = 1;
					if (newY <= 0 || newY >= rect.Height) newY = 1;
					b.SetPixel(x, y, copy.GetPixel(newX, newY));
				}
			}

			// Draw final image to destination
			g = Graphics.FromImage(textDestination);
			g.DrawImage(b, rect);

			// Clean up.
			font.Dispose();
			hatchBrush.Dispose();
			g.Dispose();
		}

		#endregion
	}
}
