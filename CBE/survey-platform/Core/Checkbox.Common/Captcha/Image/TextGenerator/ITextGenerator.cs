using System;
using System.Drawing;

namespace Checkbox.Common.Captcha.Image.TextGenerators
{
	/// <summary>
	/// Summary description for ITextGenerator.
	/// </summary>
    public interface ITextGenerator
	{
        /// <summary>
        /// Draw the text to a bitmap.
        /// </summary>
        /// <param name="text">Text to draw.</param>
        /// <param name="textDestination">Bitmap to render text to.</param>
		void DrawText(string text, Bitmap textDestination);
	}
}
