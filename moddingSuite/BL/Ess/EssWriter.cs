using System.Collections.Generic;
using System.IO;

namespace moddingSuite.BL.Ess
{
    public class EssWriter
    {
        private uint Swap(uint le)
        {
            uint b1 = le & 0xFF;
            uint b2 = (le >> 8) & 0xFF;
            uint b3 = (le >> 16) & 0xFF;
            uint b4 = (le >> 24) & 0xFF;
            return b1 * 16777216 + b2 * 65536 + b3 * 256 + b4;
        }

        public byte[] WriteEss(byte[] data)
        {
            int channels, samplerate;

            using (var fs = new MemoryStream(data))
            {
                var br = new BinaryReader(fs);

                uint id = br.ReadUInt32();
                if (id != 0x46464952)
                {
                    throw new InvalidDataException("Expected a valid WAV file.");
                }
                br.ReadUInt32();
                br.ReadUInt32();
                br.ReadUInt32();
                id = br.ReadUInt32();
                if (id == 16) // old wav
                {
                    id = br.ReadUInt16();
                    if (id != 1)
                    {
                        throw new InvalidDataException($"Unknown or unsupported WAV format was given Level. 1: ({id}).");
                    }
                    channels = br.ReadUInt16();
                    samplerate = br.ReadInt32();
                    br.ReadUInt32();
                    br.ReadUInt16();
                    id = br.ReadUInt16();
                    if (id != 16)
                    {
                        throw new InvalidDataException($"Only 16 bit WAV supported. {id} was given.");
                    }
                }
                else if (id == 40) // new wav
                {
                    id = br.ReadUInt16();

                    if (id != 0xfffe)
                        throw new InvalidDataException($"Unknown or unsupported WAV format was given. Level : ({id}).");

                    channels = br.ReadUInt16();
                    samplerate = br.ReadInt32();
                    br.ReadUInt32();
                    br.ReadUInt16();
                    id = br.ReadUInt16();

                    if (id != 16)
                        throw new InvalidDataException("Only 16 bit WAV supported.");

                    br.ReadUInt32();
                    br.ReadUInt32();
                    br.ReadUInt32();
                    br.ReadUInt32();
                    br.ReadUInt32();
                    br.ReadUInt32();
                }
                else
                {
                    throw new InvalidDataException($"Unknown or unsupported WAV format was given. Level 3: ({id}).");
                }
                id = br.ReadUInt32();

                if (id != 0x61746164)
                    throw new InvalidDataException("Data not found.");

                if (channels != 1)
                    throw new InvalidDataException("Only mono files supported.");

                int nsamples = br.ReadInt32()/channels/2;

                int frsize = 1024;
                int[] audio = new int[frsize];
                int[] oaudio = new int[frsize];
                int[] k = new int[frsize];
                int[] bb = new int[frsize];
                int[] bfrac = new int[frsize];
                int i, predict;

                int k1, k2;

                int sample, sample1 = 0, sample2 = 0, sample3, input, output = 0;
                int pred1 = 0, pred2 = 0, vv10 = 0, vv22 = 0;

                int r1, r2, v18;
                int Bitvalue, Result, d, dk, sign, diff;
                int C1 = 0, C2 = 0, C3 = 0;

                using (var ms = new MemoryStream())
                {
                    var bw = new BinaryWriter(ms);
                    bw.Write(0x2020001);
                    bw.Write((short) 0x101);
                    ms.WriteByte((byte) (samplerate >> 8));
                    ms.WriteByte((byte) samplerate);
                    bw.Write(Swap((uint) nsamples));
                    bw.Write((int) 0);
                    bw.Write(Swap((uint) nsamples));
                    int frn = nsamples/frsize;
                    ms.Seek(frn*4, SeekOrigin.Current);

                    long datastart = ms.Position;

                    int[] frs = new int[frn];

                    for (int f = 0; f < frn; f++)
                    {
                        ms.WriteByte((byte) (pred1 >> 8));
                        ms.WriteByte((byte) pred1);
                        ms.WriteByte((byte) (pred2 >> 8));
                        ms.WriteByte((byte) pred2);
                        ms.WriteByte((byte) (output >> 8));
                        ms.WriteByte((byte) output);
                        ms.WriteByte((byte) (vv22 >> 8));
                        ms.WriteByte((byte) vv22);
                        ms.WriteByte((byte) (sample1 >> 8));
                        ms.WriteByte((byte) sample1);
                        ms.WriteByte((byte) (sample2 >> 8));
                        ms.WriteByte((byte) sample2);
                        ms.WriteByte((byte) (vv10 >> 8));
                        ms.WriteByte((byte) vv10);
                        ms.WriteByte((byte) (C1 >> 8));
                        ms.WriteByte((byte) C1);
                        ms.WriteByte((byte) (C2 >> 8));
                        ms.WriteByte((byte) C2);
                        ms.WriteByte((byte) (C3 >> 8));
                        ms.WriteByte((byte) C3);

                        for (i = 0; i < frsize; i++) oaudio[i] = br.ReadInt16();
                        /////////////////////////////////
                        // encode
                        for (i = 0; i < frsize; i++)
                        {
                            v18 = output;

                            k2 = 2*(2*sample1 - vv10) - 5*sample2;
                            k1 = 2*output - vv22;
                            sample3 = sample2;
                            sample2 = sample1;

                            predict = ((pred2*k2 + 128) >> 8) + ((pred1*k1 + 128) >> 8);

                            //////////////////////////
                            // phase 2
                            sign = 0;
                            dk = oaudio[i] - predict;
                            if (dk < 0)
                            {
                                sign = 1;
                                dk = ~dk;
                            }

                            bfrac[i] = 0;
                            Bitvalue = 0;
                            if (dk > 0)
                            {
                                if (dk >= C1 + C2)
                                {
                                    if (C3 > 0)
                                    {
                                        d = (dk - C1 - C2)/C3;
                                        Bitvalue = d + 2;
                                        bfrac[i] = (dk - C1 - C2 - C3*d)*4/C3;
                                    }
                                    else Bitvalue = 3;
                                }
                                else if (dk >= C1)
                                {
                                    Bitvalue = 1;
                                    if (C2 > 0) bfrac[i] = (dk - C1)*4/C2;
                                }
                                else if (C1 > 0) bfrac[i] = (dk)*4/C1;
                            }

                            Bitvalue <<= 1;
                            Bitvalue += sign;
                            bb[i] = Bitvalue;

                            //////////////////////////////////
                            // test decode

                            #region test decode

                            r1 = 0;
                            Bitvalue = bb[i];
                            if (Bitvalue > 1)
                            {
                                if (Bitvalue > 3)
                                {
                                    if (Bitvalue > 5)
                                    {
                                        r1 = C1 + C2 + (C3 + 1)*((Bitvalue >> 1) - 2);
                                        r2 = C3 + r1;
                                        C1 += 6*((C1 + 2048)/2048);
                                        C2 += 6*((C2 + 1024)/1024);
                                        C3 += 6*((C3 + 512)/512);
                                    }
                                    else
                                    {
                                        r1 = C2 + C1;
                                        r2 = C3 + r1;
                                        C1 += 6*((C1 + 2048)/2048);
                                        C2 += 6*((C2 + 1024)/1024);
                                        C3 -= 2*((C3 + 510)/512);
                                    }
                                }
                                else
                                {
                                    r1 = C1;
                                    r2 = C2 + C1;
                                    C1 += 6*((C1 + 2048)/2048);
                                    C2 -= 2*((C2 + 1022)/1024);
                                }
                            }
                            else
                            {
                                r2 = C1;
                                C1 -= 2*((C1 + 2046)/2048);
                            }
                            Result = (r2 + r1) >> 1;

                            // C1-2-3 correction
                            if (Result > C1)
                            {
                                if (Result > C2)
                                {
                                    if (Result <= C3) C3 = C3 - 2*((C3 + 510)/512);
                                }
                                else
                                    C2 = C2 - 2*((C2 + 1022)/1024);
                            }
                            else
                                C1 -= 2*((C1 + 2046)/2048);

                            if (r2 - r1 > 0x40)
                            {
                                diff = (r2 - r1)/4 + 1;
                                Result = r1 + diff*bfrac[i];
                            }
                            else bfrac[i] = -1;

                            if ((Bitvalue & 1) > 0) Result = ~Result; // sign

                            k[i] = Result;

                            #endregion

                            input = k[i];

                            // phase 1
                            sample1 = input + ((pred2*k2 + 128) >> 8);
                            if ((k2 | input) != 0) pred2 += ((k2 ^ input) & -0x20000001 | 0x40000000) >> 29;

                            sample = sample1 + ((pred1*k1 + 128) >> 8);
                            if (sample > 32767) sample = 32767;
                            if (sample < -32768) sample = -32768;

                            if ((sample1 | k1) != 0) pred1 += ((sample1 ^ k1) & -0x20000001 | 0x40000000) >> 29;

                            vv22 = v18;
                            //output = (output + 7 * sample) / 8;   // this is for another encoding method
                            output = sample;
                            vv10 = sample3;
                        }

                        ///////////////////////////////////////
                        // output to bitstream
                        List<bool> esstr = new List<bool>();
                        for (i = 0; i < frsize; i++)
                        {
                            d = bb[i];
                            if (d > 23)
                            {
                                for (int j = 0; j < 24; j++) esstr.Add(false);
                                esstr.Add(true);
                                Bitvalue = d - 24;
                                for (int j = 0; j < 16; j++)
                                {
                                    if ((Bitvalue & 0x8000) == 0) esstr.Add(false);
                                    else esstr.Add(true);
                                    Bitvalue <<= 1;
                                }
                            }
                            else
                            {
                                for (int j = 0; j < d; j++) esstr.Add(false);
                                esstr.Add(true);
                            }
                            if (bfrac[i] != -1)
                            {
                                if (bfrac[i] < 2) esstr.Add(false);
                                else esstr.Add(true);
                                if ((bfrac[i] & 1) == 0) esstr.Add(false);
                                else esstr.Add(true);
                            }
                        }
                        if ((esstr.Count%8) != 0) for (int j = 0; j < 8; j++) esstr.Add(false);

                        for (i = 0; i < esstr.Count/8; i++)
                        {
                            Bitvalue = 0;
                            for (int j = 0; j < 8; j++)
                            {
                                Bitvalue *= 2;
                                if (esstr[i*8 + j]) Bitvalue++;
                            }
                            ms.WriteByte((byte) Bitvalue);
                        }
                        frs[f] = (int) (ms.Position - datastart);
                    }

                    ms.Seek(0x14, SeekOrigin.Begin);
                    for (int f = 0; f < frn; f++) bw.Write(Swap((uint) frs[f]));

                    return ms.ToArray();
                }
            }
        }
    }
}