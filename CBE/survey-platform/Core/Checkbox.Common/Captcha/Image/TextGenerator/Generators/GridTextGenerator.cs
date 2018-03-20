using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for GridTextGenerator.
	/// </summary>
	internal class GridTextGenerator : ITextGenerator
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
			HatchBrush hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.White, Color.White);
			g.FillRectangle(hatchBrush, rect);						
			
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
			hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.Black, Color.Black);
//			g.DrawString(text, font, hatchBrush, rect, format);
			g.DrawString(text, font, hatchBrush, rectForText, format);
			Pen pen = new Pen(hatchBrush, 1);

			// Draw grid
			int v_lines = rect.Width/7;			// svakih 7 pixela
			int h_lines = rect.Height/7;

			for (int i = 0; i < h_lines; i++)
			{
				g.DrawLine(pen, 0, i*rect.Height/h_lines, rect.Width, i*rect.Height/h_lines);
			}
			for (int i = 0; i < v_lines; i++)
			{
				g.DrawLine(pen, i*rect.Width/v_lines, 0, i*rect.Width/v_lines, rect.Height);
			}

			// Clean up.
			font.Dispose();
			hatchBrush.Dispose();
			g.Dispose();
		}

		#endregion
	}
}
