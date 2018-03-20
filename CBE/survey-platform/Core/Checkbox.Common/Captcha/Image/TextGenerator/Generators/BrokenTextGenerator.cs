using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Checkbox.Common.Captcha.Image.Support;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for BrokenTextGenerator.
	/// </summary>
	internal class BrokenTextGenerator : ITextGenerator
	{
	    #region ITextGenerator Members

		public void DrawText(string text, Bitmap textDestinationBitmap)
		{
			// calculating spacing variables
			int spacingWidth = Math.Min(10, textDestinationBitmap.Height / 10);
			int verticalSpacingCount = Math.Max(2, textDestinationBitmap.Height / 25);

			#region Creating rectangles
			// Create a graphics object for drawing destination picture.
			Graphics g = Graphics.FromImage(textDestinationBitmap);
			g.SmoothingMode = SmoothingMode.AntiAlias;

			// Set size for drawing rectangles
			Rectangle mainRect = new Rectangle(0, 0, textDestinationBitmap.Width, textDestinationBitmap.Height);
			
			int textRectY = textDestinationBitmap.Height - verticalSpacingCount*spacingWidth;
			int spacingPeriod = (textDestinationBitmap.Height + spacingWidth) / (verticalSpacingCount+1);
			int horizontalSpacingCount = (textDestinationBitmap.Width + spacingWidth) / spacingPeriod;
			int textRectX = textDestinationBitmap.Width - horizontalSpacingCount*spacingWidth;

			Rectangle textRect = new Rectangle(0, textRectY*15/100, textRectX, textRectY-textRectY*15/100);
			#endregion

			#region Setting font size
			SizeF fontSize;
			float tempVerticalFontSize = textRect.Height + 1;
			Font font;
			
			// Adjust the font size until the text fits within the image.
			do
			{
				tempVerticalFontSize--;
				font = new Font("Courier New", tempVerticalFontSize, FontStyle.Bold);
				fontSize = g.MeasureString(text, font);
			} while (fontSize.Width > textRect.Width);
			g.Dispose();
			#endregion

			#region Set up the text format.
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			#endregion
			
			#region Draw Text
			// Draw the black text on white background.
			System.Drawing.Image b = new Bitmap(mainRect.Width, mainRect.Height);
			Graphics g2 = Graphics.FromImage(b);
			g2.SmoothingMode = SmoothingMode.AntiAlias;
			HatchBrush hatchBrush = new HatchBrush(HatchStyle.Percent50, Color.White, Color.LightGray);
			g2.FillRectangle(hatchBrush, mainRect);
			hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.Black, Color.Black);
			g2.DrawString(text, font, hatchBrush, textRect, format);
			g2.DrawImage(b, mainRect);
			#endregion
			
			// Draw chess-like image
			g = Graphics.FromImage(textDestinationBitmap);

			// Draw background
			hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.White, Color.White);
			g.FillRectangle(hatchBrush, mainRect);

			int sliceWidth = textRectY / (verticalSpacingCount+1);

			// Copy slices from textRect to destination picture
			for (int i = 0; i < horizontalSpacingCount+1; i++)
				for (int j = 0; j < verticalSpacingCount+1; j++)
				{
					Rectangle textPartRect = new Rectangle(
						i * sliceWidth,
						j * sliceWidth, 
						sliceWidth, sliceWidth);

					g.DrawImage(b, 
						i * spacingPeriod, 
						j * spacingPeriod, 
						textPartRect, GraphicsUnit.Pixel);
				}

			// Clean up.
			font.Dispose();
			hatchBrush.Dispose();
			g.Dispose();
		}

		#endregion
	}
}
