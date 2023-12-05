using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WADExplorer
{
    public enum Channel : short
    {
        Mono = 1,
        Stereo = 2
    }

    public static class WAV
    {
        // easy function
        public static byte[] FromSample(byte[] sample, int frequency, Channel numOfChannels, int bitsPerSample = 16)
        {
            byte[] buffer = new byte[46+sample.Length];
            MemoryStream data = new MemoryStream(buffer);
            data.SetLength(46 + sample.Length);

            byte[] bChunkID =  Encoding.ASCII.GetBytes("RIFF");
            byte[] bFormat =  Encoding.ASCII.GetBytes("WAVE");
            byte[] bSubChunkID =  Encoding.ASCII.GetBytes("fmt ");
            byte[] bSubChunkID2 =  Encoding.ASCII.GetBytes("data");
            int subChunkSize = 18; // 1 = PCM
            short audioFormat = 1;
            int chunkSize = 38 + sample.Length;
            int byteRate = frequency * (int)(numOfChannels) * (bitsPerSample / 8);

            var bw = new BinaryWriter(data, Encoding.ASCII, true);

            bw.Write(bChunkID);
            bw.Write(38 + sample.Length);
            // WAVE
            bw.Write(bFormat);
            // fmt
            bw.Write(bSubChunkID);
            bw.Write(subChunkSize); // sub chunk 1 size 16 for PCM
            bw.Write(audioFormat);
            bw.Write((short)numOfChannels);
            bw.Write(frequency); 
            bw.Write(byteRate); 
            bw.Write((short)((short)(numOfChannels)*bitsPerSample/8)); 
            bw.Write(bitsPerSample); 
            // data
            bw.Write(bSubChunkID2); 
            bw.Write(sample.Length);

            bw.Write(sample);

            buffer = data.ToArray();

            data.Dispose();
            return buffer;
        }
    }

    public class VAGDecoder
    {
        // PSX ADPCM coefficients
        private static readonly double[] K0 = { 0, 0.9375, 1.796875, 1.53125, 1.90625 };
        private static readonly double[] K1 = { 0, 0, -0.8125, -0.859375, -0.9375 };

        // PSX ADPCM decoding routine - decodes a single sample
        public static short VagToPCM(byte soundParameter, int soundData, ref double vagPrev1, ref double vagPrev2)
        {
            if (soundData > 7)
                soundData -= 16;

            var sp1 = (soundParameter >> 0) & 0xF;
            var sp2 = (soundParameter >> 4) & 0xF;

            var dTmp1 = soundData * Math.Pow(2.0, (12.0 - sp1));

            var dTmp2 = vagPrev1 * K0[sp2];
            var dTmp3 = vagPrev2 * K1[sp2];

            vagPrev2 = vagPrev1;
            vagPrev1 = dTmp1 + dTmp2 + dTmp3;

            var result = (int)Math.Round(vagPrev1);

            return (short)Math.Min(32767, Math.Max(-32768, result));
        }

        public static byte[] DecodeSound(byte[] buffer)
        {
            int numSamples = (buffer.Length >> 4) * 28; // PSX ADPCM data is stored in blocks of 16 bytes each containing 28 samples.

            int loopStart = 0;
            int loopLength = 0;

            var result = new byte[numSamples * 2];
            MemoryStream ms = new MemoryStream(result);
            BinaryWriter bw = new BinaryWriter(ms, Encoding.ASCII, true);

            byte sp = 0;

            double vagPrev1 = 0.0;
            double vagPrev2 = 0.0;

            int k = 0;

            byte[] r = result;
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (i % 16 == 0)
                    {
                        var ld1 = buffer[i];
                        var ld2 = buffer[i + 1];

                        sp = ld1;

                        if ((ld2 & 0xE) == 6)
                            loopStart = k;

                        if ((ld2 & 0xF) == 3 || (ld2 & 0xF) == 7)
                            loopLength = (k + 28) - loopStart;

                        i += 2;
                    }

                    for (int s = 0; s < 2; s++)
                    {
                        var sd = (buffer[i] >> (s * 4)) & 0xF;

                        bw.Write( VagToPCM(sp, sd, ref vagPrev1, ref vagPrev2) );
                    }
                }
            }

            result = ms.ToArray();

            ms.Close();

            return result;
        }

        public static byte[] GetWAVFromVagData(byte[] vagBuffer)
        {
            MemoryStream vagMS = new MemoryStream(vagBuffer);
            using (var br = new BinaryReader(vagMS, Encoding.ASCII, true))
            {
                vagMS.Position = 16;
                byte[] bFrequency = br.ReadBytes(4);
                Array.Reverse(bFrequency); // convert to little-endian

                vagMS.Position = 12;
                byte[] bSampleDataSize = br.ReadBytes(4);
                Array.Reverse(bSampleDataSize); // convert to little-endian

                int frequency = BitConverter.ToInt32(bFrequency,0);
                int sampleDataSize = BitConverter.ToInt32(bSampleDataSize, 0);
                
                vagMS.Position = 64;
                byte[] vag = br.ReadBytes(sampleDataSize);

                byte[] VAG2WAV = DecodeSound(vag);

                byte[] Sound = WAV.FromSample(VAG2WAV, frequency, Channel.Mono, 16);
                return Sound;
            }
        }
    }
}
