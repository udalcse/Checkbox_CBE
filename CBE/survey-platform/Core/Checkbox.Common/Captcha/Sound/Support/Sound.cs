using System;
using System.IO;
using System.Reflection;

namespace Checkbox.Common.Captcha.Sound.Support
{
    /// <summary>
    /// Summary description for Sound.
    /// </summary>
    public class Sound
    {
        //all the chars that have its sound representation in the soundDATA resource		
        private const string strAllChars = "abcdefghijklmnopqrstuvwxyz1234567890";
        private string textToSpeak;

        //soundLOOKUP contains array of this structures for each char 
        private struct sound_lookup
        {
            public byte ch;  //the char 
            public int offset_in_file; // the offset of this char's sound data block in soundDATA 
            public int raw_data_size; // the size of this char's sound data block in soundDATA 

            public sound_lookup(byte ch, int offset_in_file, int raw_data_size)
            {
                this.ch = ch;
                this.offset_in_file = offset_in_file;
                this.raw_data_size = raw_data_size;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="format"></param>
        public void Save(Stream stream, SoundFormatEnum format)
        {

            //the names of the resource files
            string filenameSoundDATA = "Checkbox.Common.Captcha.Sound.Support.soundDATA";
            string filenameSoundLOOKUP = "Checkbox.Common.Captcha.Sound.Support.soundLOOKUP";
            string filenameSoundHEADER = "Checkbox.Common.Captcha.Sound.Support.soundHEADER";

            //read all raw sound data
            Stream inputSoundDATA = Assembly.GetExecutingAssembly().GetManifestResourceStream(filenameSoundDATA);
            byte[] bufferSoundDATA = new byte[inputSoundDATA.Length];
            inputSoundDATA.Read(bufferSoundDATA, 0, (int)inputSoundDATA.Length);

            //read all lookup data
            Stream inputSoundLOOKUP = Assembly.GetExecutingAssembly().GetManifestResourceStream(filenameSoundLOOKUP);
            byte[] bufferSoundLOOKUP = new byte[inputSoundLOOKUP.Length];
            inputSoundLOOKUP.Read(bufferSoundLOOKUP, 0, (int)inputSoundLOOKUP.Length);

            //read the header data 
            Stream inputSoundHEADER = Assembly.GetExecutingAssembly().GetManifestResourceStream(filenameSoundHEADER);
            byte[] bufferSoundHEADER = new byte[inputSoundHEADER.Length];
            inputSoundHEADER.Read(bufferSoundHEADER, 0, (int)inputSoundHEADER.Length);

            //convert the text to lowercase 
            textToSpeak = textToSpeak.ToLower();

            //find out the size for raw sound data array 
            int i;
            int nTextRawDataSize = 0;
            //take the size of the sound data of each letter and sum them 
            for (i = 0; i < textToSpeak.Length; i++)
            {
                sound_lookup lookup = FindSoundLookupByChar(textToSpeak[i], bufferSoundLOOKUP);

                if (lookup.offset_in_file == -1)
                {
                    throw new Exception(@"Letter '" + textToSpeak[i] + @"' not found in the lookup table !");
                }
                nTextRawDataSize += lookup.raw_data_size;
            }

            //allocate the space for raw sound data	
            byte[] arrTextRawData = new byte[nTextRawDataSize];

            //fill text raw data (letter by letter)

            int iOffset = 0; //reset the pointer to the dest array 
            for (i = 0; i < textToSpeak.Length; i++) //letter by letter
            {
                //find out the offset of the wanted sound block within the bufferSoundDATA and the size of this sound data block
                sound_lookup lookup = FindSoundLookupByChar(textToSpeak[i], bufferSoundLOOKUP);
                //copy this sound block to the dest array
                CopyBytesFromTo(arrTextRawData, iOffset, bufferSoundDATA, lookup.offset_in_file, lookup.raw_data_size);
                //move the pointer to the dest array by the size of the copied block  
                iOffset += lookup.raw_data_size;
            }

            //header
            //change some data in the header
            //the raw audio data size has to be updated 
            CopyBytesFromTo(bufferSoundHEADER, 4, ConvertIntToBytes(nTextRawDataSize + 36), 0, 4);
            CopyBytesFromTo(bufferSoundHEADER, bufferSoundHEADER.Length - 4, ConvertIntToBytes(nTextRawDataSize), 0, 4);

            //allocate the space for all the data (raw sound and header)	--> a wave file 
            byte[] arrWaveFile = new byte[nTextRawDataSize + bufferSoundHEADER.Length];

            //merge raw sound text data and header
            //copy header
            CopyBytesFromTo(arrWaveFile, 0, bufferSoundHEADER, 0, bufferSoundHEADER.Length);
            //copy raw sound text data 
            CopyBytesFromTo(arrWaveFile, bufferSoundHEADER.Length, arrTextRawData, 0, arrTextRawData.Length);

            //copy to the output stream finally
            stream.Write(arrWaveFile, 0, (int)arrWaveFile.Length);
        }


        private sound_lookup FindSoundLookupByChar(char ch, byte[] arrSoundLookup)
        {
            //it should be 12
            int sizeOfSingleStruct = arrSoundLookup.Length / strAllChars.Length;

            int i;
            for (i = 0; i < strAllChars.Length; i++)
            {
                int iBegin = i * sizeOfSingleStruct;
                if (Convert.ToByte(ch) == arrSoundLookup[iBegin])
                {
                    return new sound_lookup(arrSoundLookup[iBegin], ConvertBytesToInt(arrSoundLookup, iBegin + 4), ConvertBytesToInt(arrSoundLookup, iBegin + 8));
                }
            }
            //not found, return some error 
            return new sound_lookup(Convert.ToByte(ch), -1, -1);

        }

        // copying the bytes from one byte array to another 
        private void CopyBytesFromTo(byte[] arrDest, int posDest, byte[] arrSource, int posSource, int howMany)
        {
            int i;
            for (i = 0; i < howMany; i++)
            {
                arrDest[posDest + i] = arrSource[posSource + i];
            }

        }

        //
        private static int ConvertBytesToInt(byte[] arr, int pos)
        {
            //the first byte is the least important byte !!!
            return (arr[pos + 0] | arr[pos + 1] << 8 | arr[pos + 2] << 16 | arr[pos + 3] << 24);
        }


        private static byte[] ConvertIntToBytes(int nNum)
        {
            byte[] arr = new byte[4];
            arr[0] = (byte)(nNum % 256);   //the first byte is the least important byte !!!
            arr[1] = (byte)((nNum % 65536) / 256);
            arr[2] = (byte)((nNum % 16777216) / 65536);
            arr[3] = (byte)((nNum % 4294967296) / 16777216);
            return arr;
        }

        internal void Speak(string textToSpeak)
        {
            this.textToSpeak = textToSpeak;
        }
    }
}
