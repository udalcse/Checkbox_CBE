using System;
using System.IO;
using System.Text;

namespace Checkbox.Common.Captcha.Sound.Support
{
	/// <summary>
	/// Summary description for WavFile.
	/// </summary>
	public class WavFile
	{
		private MemoryStream _stream = null;
		private BinaryWriter _writer = null;
		private BinaryReader _reader = null;

        /// <summary>
        /// 
        /// </summary>
		public void Create()
		{
			_stream = new MemoryStream();
			_writer = new BinaryWriter(_stream);

			WriteHeader();
			WriteFormat();
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public MemoryStream Save()
		{
			return _stream;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
		public void Open(MemoryStream input)
		{
			_stream = input;
			_reader = new BinaryReader(_stream);
		}

		#region WAV File Definitions

		// PCM = 1
        /// <summary>
        /// 
        /// </summary>
		protected short _audioFormat = 1;

		/// <summary>
		/// PCM = 1
		/// </summary>
		public short AudioFormat 
		{
			get	
			{
				return _audioFormat;
			}

			set	
			{
				_audioFormat = value;
			}
		}
		
		// Mono = 1, Stereo = 2, etc..
        /// <summary>
        /// 
        /// </summary>
		protected short _channels = 2;

		/// <summary>
		/// Mono = 1, Stereo = 2, etc..
		/// </summary>
		 public short NumChannels 
		{
			get 
			{
				return _channels;
			}

			set 
			{
				_channels = value;
			}
		}
		
		// Sample rate in Hz
        /// <summary>
        /// 
        /// </summary>
		protected int _sampleRate = 38000;

		/// <summary>
		/// Sample rate in Hz
		/// </summary>
		public int SampleRate 
		{
			get	
			{
				return _sampleRate;
			}

			set	
			{ 
				_sampleRate = value;	
			}
		}
		
		// 8 bits = 8, 16 bits = 16...
        /// <summary>
        /// 
        /// </summary>
		protected short _bitsPerSample = 8;

		/// <summary>
		/// 8 bits = 8, 16 bits = 16...
		/// </summary>
		public short BitsPerSample 
		{
			get
			{
				return _bitsPerSample;	
			}

			set	
			{
				_bitsPerSample = value;
			}
		}
		
		/// <summary>
		/// Calculated byte rate
		/// </summary>
		public int ByteRate
		{
			get 
			{ 
				return SampleRate * NumChannels * (BitsPerSample / 8); 
			}
		}

		/// <summary>
		/// WAV block alginment
		/// </summary>
		public short BlockAlign
		{
			get 
			{ 
				return (short)(NumChannels * (BitsPerSample / 8));
			}
		}

		/// <summary>
		/// Size of the data chunk area
		/// </summary>
		public int DataChunkSize
		{
			get 
			{ 
				return SampleNumber * NumChannels * (BitsPerSample / 8);
			}
		}

		/// <summary>
		/// Size of the format chunk area
		/// </summary>
		public int FormatChunkSize
		{
			get 
			{ 
				return 16; 
			}
		}

		/// <summary>
		/// Total chunk size
		/// </summary>
		public int ChunkSize
		{
			get 
			{ 
				return 4 + (8 + FormatChunkSize) + (8 + DataChunkSize); 
			}
		}
        
		/// <summary>
        /// // Number of samples to include in this wave file
		/// </summary>
		protected int _sampleNumber = 200;

		/// <summary>
		/// Number of samples to include in this wave file
		/// </summary>
		public int SampleNumber
		{
			get 
			{ 
				return _sampleNumber;
			}

			set
			{ 
				_sampleNumber = value;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		protected int _sampleMaxVal = 255;

		/// <summary>
		/// Maximum value of sample
		/// </summary>
		 public int SampleHigh
		{
			get
			{ 
				return _sampleMaxVal; 
			}

			set 
			{
				_sampleMaxVal = value; 
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		protected int _sampleMinVal = 0;

		/// <summary>
		/// Minimum value of sample
		/// </summary>
		public int SampleLow
		{
			get 
			{ 
				return _sampleMinVal;
			}

			set 
			{ 
				_sampleMinVal = value; 
			}
		}

		#endregion

		#region WAV File methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
		public void Write(byte[] buffer)
		{
			_writer.Write(buffer);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringValue"></param>
		public void WriteString(string stringValue)
		{
			byte[] baBytes = Encoding.ASCII.GetBytes(stringValue.ToCharArray());
			_writer.Write(baBytes);	
		}

        /// <summary>
        /// 
        /// </summary>
		private void WriteHeader()
		{
			WriteString("RIFF");
			_writer.Write(ChunkSize);
			WriteString("WAVE");
		}

        /// <summary>
        /// 
        /// </summary>
		private void WriteFormat()
		{
			WriteString("fmt ");
			_writer.Write(FormatChunkSize);
			_writer.Write(AudioFormat);
			_writer.Write(NumChannels);
			_writer.Write(SampleRate);
			_writer.Write(ByteRate);
			_writer.Write(BlockAlign);
			_writer.Write(BitsPerSample);
		}
		#endregion
	}
}
