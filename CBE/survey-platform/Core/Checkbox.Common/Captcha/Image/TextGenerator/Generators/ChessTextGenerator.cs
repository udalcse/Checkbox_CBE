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
	internal class ChessTextGenerator : ITextGenerator
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
			fontSize = fontSize / (float)1.2;
			do
			{
				fontSize--;
				font = new Font("Courier New", fontSize, FontStyle.Bold);
				size = g.MeasureString(text, font);
			} while (size.Width > rect.Width);
			g.Dispose();
			
			// Set up the text format.
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			
			Rectangle trect = new Rectangle(5, 5, textDestination.Width, textDestination.Height);
			// Draw the white text on black background.
			System.Drawing.Image b1 = new Bitmap(rect.Width, rect.Height);
			Graphics g1 = Graphics.FromImage(b1);
			g1.SmoothingMode = SmoothingMode.AntiAlias;
			hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.Black, Color.Black);
			g1.FillRectangle(hatchBrush, rect);
			hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.White, Color.White);
			g1.DrawString(text, font, hatchBrush, trect, format);
			g1.DrawImage(b1,rect);

			// Draw the black text on white background.
			System.Drawing.Image b2 = new Bitmap(rect.Width, rect.Height);
			Graphics g2 = Graphics.FromImage(b2);
			g2.SmoothingMode = SmoothingMode.AntiAlias;
			hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.White, Color.White);
			g2.FillRectangle(hatchBrush, rect);
			hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.Black, Color.Black);
			g2.DrawString(text, font, hatchBrush, trect, format);
			g2.DrawImage(b2,rect);
			
			// Draw chess-like image
			int v_lines = rect.Width/30;			// dijelimo s razmakom u pikselima
			int h_lines = rect.Height/25;			// dijelimo s razmakom u pikselima
			g = Graphics.FromImage(textDestination);

			for (int i = 0; i < h_lines; i++)
				for (int j = 0; j < v_lines; j++)
				{
					Rectangle currentRect = new Rectangle(j * rect.Width / v_lines, i * rect.Height / h_lines, rect.Width / v_lines, rect.Height / h_lines);
					if((i+j)%2 == 0)
					{
						g.DrawImage(b1, j * rect.Width / v_lines, i * rect.Height / h_lines, currentRect, GraphicsUnit.Pixel);
					}
					else
					{
						g.DrawImage(b2, j * rect.Width / v_lines, i * rect.Height / h_lines, currentRect, GraphicsUnit.Pixel);
					}
				}

			// Clean up.
			font.Dispose();
			hatchBrush.Dispose();
			g.Dispose();
		}

		#endregion
	}
}
