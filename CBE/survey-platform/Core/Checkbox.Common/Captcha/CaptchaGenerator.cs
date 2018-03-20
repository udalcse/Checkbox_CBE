using System;
using System.Collections.Generic;

using Checkbox.Common.Captcha.Image;
using Prezza.Framework.Data;

namespace Checkbox.Common.Captcha
{
    /// <summary>
    /// Captcha image and code generator.
    /// </summary>
    public static class CaptchaGenerator
    {
        /// <summary>
        /// Generate a CAPTCHA code of the specified type.
        /// </summary>
        /// <param name="codeType">Type of code to produce.</param>
        /// <param name="minLength">Minimum length of the code.</param>
        /// <param name="maxLength">Maximum length of the code.</param>
        /// <returns>A code meeting the specified parameters.  Please note that for alphanumeric codes, 1, l, O, and 0 are not used to avoid potential confusion.</returns>
        public static string GenerateCode(CodeType codeType, int minLength, int maxLength)
        {
            string possibleValues;

            if (codeType == CodeType.Alpha)
            {
                possibleValues = "ABCDEFGHIJKLMNOPQRSTUV";
            }
            else if (codeType == CodeType.Numeric)
            {
                possibleValues = "0123456789";
            }
            else
            {
                //Note a subset of alphanum is used to prevent O, 0, l, 1 confusion
                possibleValues = "2345689ABCDEFGHJKMNPQRSTUVWXYZ";
            }

            string code = string.Empty;

            //Determine the length
            Random r = new Random();

            int length = r.Next(minLength, maxLength + 1);

            for (int i = 0; i < length; i++)
            {
                code += possibleValues[r.Next(possibleValues.Length)];
            }

            return code;            
        }

        /// <summary>
        /// Generate and store a captcha image and return the ID for the image.
        /// </summary>
        /// <param name="code">Code to display.</param>
        /// <param name="imageFormat">Format of the image.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="textStyles"></param>
        /// <returns>ID of image to display.</returns>
        public static int GenerateAndStoreImage(string code, ImageFormatEnum imageFormat, int imageWidth, int imageHeight, List<TextStyleEnum> textStyles)
        {
            ImageGenerator generator = new ImageGenerator(code, GetTextStyle(textStyles), imageWidth, imageHeight);

            byte[] imageBytes = generator.ImageBytes;

            int imageID = DbUtility.SaveImage(imageBytes, "image/gif", null, "tmpCaptcha_" + DateTime.Now.Ticks.ToString(), Guid.NewGuid().ToString(), DateTime.Now, true);
            DbUtility.DeleteTempImages();

            return imageID;
        }

        /// <summary>
        /// Choose the text style based on the list of possible styles.
        /// </summary>
        /// <returns>Text style to use.</returns>
        private static TextStyleEnum GetTextStyle(IList<TextStyleEnum> possibleStyles)
        {
            Random r = new Random();

            if (possibleStyles.Count > 0)
            {
                return possibleStyles[r.Next(possibleStyles.Count)];
            }
            else
            {
                return TextStyleEnum.AncientMosaic;
            }
        }
    }
}
