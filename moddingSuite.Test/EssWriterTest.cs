using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace moddingSuite.Test
{
    [TestClass]
    public class EssWriterTest
    {
        private uint swap(uint le)
        {
            uint b1 = le & 0xFF;
            uint b2 = (le >> 8) & 0xFF;
            uint b3 = (le >> 16) & 0xFF;
            uint b4 = (le >> 24) & 0xFF;
            return b1 * 16777216 + b2 * 65536 + b3 * 256 + b4;
        }

        //[TestMethod]
        public void TestWriter()
        {
            var file = @"C:\Users\enohka\Desktop\teststuff\beep.ess.wav";

            int channels, samplerate;

            var fs = new FileStream(file, FileMode.Open); var br = new BinaryReader(fs);
            uint id = br.ReadUInt32();
            if (id != 0x46464952) { Console.WriteLine("I need WAV file."); return; }
            br.ReadUInt32(); br.ReadUInt32(); br.ReadUInt32();
            id = br.ReadUInt32();
            if (id == 16) // old wav
            {
                id = br.ReadUInt16();
                if (id != 1) { Console.WriteLine("Unknown WAV format."); return; }
                channels = br.ReadUInt16();
                samplerate = br.ReadInt32();
                br.ReadUInt32(); br.ReadUInt16();
                id = br.ReadUInt16();
                if (id != 16) { Console.WriteLine("Only 16 bit WAV supported."); return; }
            }
            else if (id == 40) // new wav
            {
                id = br.ReadUInt16();
                if (id != 0xfffe) { Console.WriteLine("Unknown WAV format."); return; }
                channels = br.ReadUInt16();
                samplerate = br.ReadInt32();
                br.ReadUInt32(); br.ReadUInt16();
                id = br.ReadUInt16();
                if (id != 16) { Console.WriteLine("Only 16 bit WAV supported."); return; }
                br.ReadUInt32(); br.ReadUInt32(); br.ReadUInt32();
                br.ReadUInt32(); br.ReadUInt32(); br.ReadUInt32();
            }
            else { Console.WriteLine("Unknown WAV format."); return; }
            id = br.ReadUInt32();
            if (id != 0x61746164) { Console.WriteLine("Data not found."); return; }

            if (channels != 1) { Console.WriteLine("Only mono files supported."); return; }
            int nsamples = br.ReadInt32() / channels / 2;

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

            fs = new FileStream(Path.GetFileNameWithoutExtension(file) + ".ess", FileMode.Create);
            var bw = new BinaryWriter(fs);
            bw.Write(0x2020001);
            bw.Write((short)0x101);
            fs.WriteByte((byte)(samplerate >> 8)); fs.WriteByte((byte)samplerate);
            bw.Write(swap((uint)nsamples));
            bw.Write((int)0);
            bw.Write(swap((uint)nsamples));
            int frn = nsamples / frsize;
            fs.Seek(frn * 4, SeekOrigin.Current);

            long datastart = fs.Position;

            int[] frs = new int[frn];

            for (int f = 0; f < frn; f++)
            {
                fs.WriteByte((byte)(pred1 >> 8)); fs.WriteByte((byte)pred1);
                fs.WriteByte((byte)(pred2 >> 8)); fs.WriteByte((byte)pred2);
                fs.WriteByte((byte)(output >> 8)); fs.WriteByte((byte)output);
                fs.WriteByte((byte)(vv22 >> 8)); fs.WriteByte((byte)vv22);
                fs.WriteByte((byte)(sample1 >> 8)); fs.WriteByte((byte)sample1);
                fs.WriteByte((byte)(sample2 >> 8)); fs.WriteByte((byte)sample2);
                fs.WriteByte((byte)(vv10 >> 8)); fs.WriteByte((byte)vv10);
                fs.WriteByte((byte)(C1 >> 8)); fs.WriteByte((byte)C1);
                fs.WriteByte((byte)(C2 >> 8)); fs.WriteByte((byte)C2);
                fs.WriteByte((byte)(C3 >> 8)); fs.WriteByte((byte)C3);

                for (i = 0; i < frsize; i++) oaudio[i] = br.ReadInt16();
                /////////////////////////////////
                // encode
                for (i = 0; i < frsize; i++)
                {
                    v18 = output;

                    k2 = 2 * (2 * sample1 - vv10) - 5 * sample2;
                    k1 = 2 * output - vv22;
                    sample3 = sample2;
                    sample2 = sample1;

                    predict = ((pred2 * k2 + 128) >> 8) + ((pred1 * k1 + 128) >> 8);

                    //////////////////////////
                    // phase 2
                    sign = 0;
                    dk = oaudio[i] - predict;
                    if (dk < 0) { sign = 1; dk = ~dk; }

                    bfrac[i] = 0;
                    Bitvalue = 0;
                    if (dk > 0)
                    {
                        if (dk >= C1 + C2)
                        {
                            if (C3 > 0)
                            {
                                d = (dk - C1 - C2) / C3;
                                Bitvalue = d + 2;
                                bfrac[i] = (dk - C1 - C2 - C3 * d) * 4 / C3;
                            }
                            else Bitvalue = 3;
                        }
                        else if (dk >= C1)
                        {
                            Bitvalue = 1;
                            if (C2 > 0) bfrac[i] = (dk - C1) * 4 / C2;
                        }
                        else if (C1 > 0) bfrac[i] = (dk) * 4 / C1;
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
                                r1 = C1 + C2 + (C3 + 1) * ((Bitvalue >> 1) - 2);
                                r2 = C3 + r1;
                                C1 += 6 * ((C1 + 2048) / 2048);
                                C2 += 6 * ((C2 + 1024) / 1024);
                                C3 += 6 * ((C3 + 512) / 512);
                            }
                            else
                            {
                                r1 = C2 + C1;
                                r2 = C3 + r1;
                                C1 += 6 * ((C1 + 2048) / 2048);
                                C2 += 6 * ((C2 + 1024) / 1024);
                                C3 -= 2 * ((C3 + 510) / 512);
                            }
                        }
                        else
                        {
                            r1 = C1;
                            r2 = C2 + C1;
                            C1 += 6 * ((C1 + 2048) / 2048);
                            C2 -= 2 * ((C2 + 1022) / 1024);
                        }
                    }
                    else
                    {
                        r2 = C1;
                        C1 -= 2 * ((C1 + 2046) / 2048);
                    }
                    Result = (r2 + r1) >> 1;

                    // C1-2-3 correction
                    if (Result > C1)
                    {
                        if (Result > C2)
                        {
                            if (Result <= C3) C3 = C3 - 2 * ((C3 + 510) / 512);
                        }
                        else
                            C2 = C2 - 2 * ((C2 + 1022) / 1024);
                    }
                    else
                        C1 -= 2 * ((C1 + 2046) / 2048);

                    if (r2 - r1 > 0x40)
                    {
                        diff = (r2 - r1) / 4 + 1;
                        Result = r1 + diff * bfrac[i];
                    }
                    else bfrac[i] = -1;

                    if ((Bitvalue & 1) > 0) Result = ~Result;  // sign

                    k[i] = Result;
                    #endregion

                    input = k[i];

                    // phase 1
                    sample1 = input + ((pred2 * k2 + 128) >> 8);
                    if ((k2 | input) != 0) pred2 += ((k2 ^ input) & -0x20000001 | 0x40000000) >> 29;

                    sample = sample1 + ((pred1 * k1 + 128) >> 8);
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
                            if ((Bitvalue & 0x8000) == 0) esstr.Add(false); else esstr.Add(true);
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
                        if (bfrac[i] < 2) esstr.Add(false); else esstr.Add(true);
                        if ((bfrac[i] & 1) == 0) esstr.Add(false); else esstr.Add(true);
                    }
                }
                if ((esstr.Count % 8) != 0) for (int j = 0; j < 8; j++) esstr.Add(false);

                for (i = 0; i < esstr.Count / 8; i++)
                {
                    Bitvalue = 0;
                    for (int j = 0; j < 8; j++)
                    {
                        Bitvalue *= 2;
                        if (esstr[i * 8 + j]) Bitvalue++;
                    }
                    fs.WriteByte((byte)Bitvalue);
                }
                frs[f] = (int)(fs.Position - datastart);
            }

            fs.Seek(0x14, SeekOrigin.Begin);
            for (int f = 0; f < frn; f++) bw.Write(swap((uint)frs[f]));

            fs.Close();

        }
    }
}
