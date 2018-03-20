using System;

using Checkbox.Common.Captcha.Sound.Support;

namespace Checkbox.Common.Captcha.Sound
{
	/// <summary>
	/// Summary description for SoundGenerator.
	/// </summary>
	public class SoundGenerator
	{
		private Captcha.Sound.Support.Sound _sound;

        /// <summary>
        /// 
        /// </summary>
        public Captcha.Sound.Support.Sound Sound
		{
			get
			{
				System.Diagnostics.Debug.Assert(null != _sound);
				return _sound;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textToSpeak"></param>
		public SoundGenerator(string textToSpeak)
		{
			GenerateSound(textToSpeak);
		}

		private void GenerateSound(string textToSpeak)
		{
			_sound = new Captcha.Sound.Support.Sound();

			_sound.Speak(textToSpeak);
		}
	}
}
