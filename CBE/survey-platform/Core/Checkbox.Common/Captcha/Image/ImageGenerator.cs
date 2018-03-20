using System;
using System.IO;
using System.Drawing;

using Checkbox.Common.Captcha.Image.TextGenerators;

namespace Checkbox.Common.Captcha.Image
{
	/// <summary>
	/// Summary description for ImageGenerator.
	/// </summary>
    public class ImageGenerator
	{
		private Bitmap _image = null;
		
        /// <summary>
        /// 
        /// </summary>
		public Bitmap Image
		{
			get
			{
				System.Diagnostics.Debug.Assert(null != _image);
				return _image;
			}
		}

        /// <summary>
        /// Get the image bytes
        /// </summary>
        public byte[] ImageBytes
        {
            get
            {
                MemoryStream s = new MemoryStream();
                Image.Save(s, System.Drawing.Imaging.ImageFormat.Gif);

                return s.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textToRender"></param>
        /// <param name="textStyle"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
		public ImageGenerator(string textToRender, TextStyleEnum textStyle, int width, int height)
		{
            Size size = new Size(width, height);
			GenerateSurface(size);
			GenerateImage(textToRender, textStyle);
		}

		private void GenerateImage(string textToRender, TextStyleEnum textStyle)
		{
			ITextGenerator textGenerator = TextGeneratorFactory.CreateGenerator(textStyle);
			textGenerator.DrawText(textToRender, Image);
		}

		private void GenerateSurface(Size size)
		{
			_image = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
		}
	}
}
