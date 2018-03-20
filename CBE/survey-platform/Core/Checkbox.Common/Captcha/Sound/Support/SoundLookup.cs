using System;
using System.IO;
using System.Reflection;

namespace Checkbox.Common.Captcha.Sound.Support
{
    /// <summary>
    /// 
    /// </summary>
    public struct sound_lookup
	{
        /// <summary>
        /// 
        /// </summary>
		public char ch;

        /// <summary>
        /// 
        /// </summary>
		public long offset_in_file;

        /// <summary>
        /// 
        /// </summary>
		public long raw_data_size;
	}

	/// <summary>
	/// Summary description for SoundLookup.
	/// </summary>
    public class SoundLookup
	{
        /// <summary>
        /// 
        /// </summary>
		public SoundLookup()
		{
		
		}

		//private sound_lookup[] _lookup = null;

		private void GetSoundLookup()
		{
            string filename = "Checkbox.Common.Captcha.Sound.Support.soundDATA";

			MemoryStream stream =
                (MemoryStream)Assembly.GetExecutingAssembly().GetManifestResourceStream(filename);

			long length = stream.Length;

			while(stream.Position < stream.Length)
			{
				//stream.Read();
			}

			// 

//			WavFile lookupFile = new WavFile();
//
//			lookupFile.Open(stream);
//
//			stream.
//
//			_lookup = new sound_lookup[100];
		}
	}
}
