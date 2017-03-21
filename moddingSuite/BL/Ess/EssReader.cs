using System.IO;

namespace moddingSuite.BL.Ess
{
    public class EssReader
    {
        private void Writewavheader(BinaryWriter bw, int channels, int samplerate, int bytes)
        {
            bw.Write("RIFF".ToCharArray());
            bw.Write(bytes + 36);
            bw.Write("WAVE".ToCharArray());
            bw.Write("fmt ".ToCharArray());
            bw.Write(16);
            bw.Write((short)1);
            bw.Write((short)channels);
            bw.Write(samplerate);
            bw.Write(samplerate * channels * 2);
            bw.Write((short)(channels * 2));
            bw.Write((short)16);
            bw.Write("data".ToCharArray());
            bw.Write(bytes);
        }

        private uint Swap(uint le)
        {
            uint b1 = le & 0xFF;
            uint b2 = (le >> 8) & 0xFF;
            uint b3 = (le >> 16) & 0xFF;
            uint b4 = (le >> 24) & 0xFF;
            return b1 * 16777216 + b2 * 65536 + b3 * 256 + b4;
        }

        public byte[] ReadEss(byte[] data)
        {
            using (var inMs = new MemoryStream(data))
            {
                var br = new BinaryReader(inMs);

                using (var ms = new MemoryStream())
                {
                    var bw = new BinaryWriter(ms);

                    br.ReadInt32(); // skip ESS version
                    byte unk = br.ReadByte();
                    int channels = br.ReadByte();
                    int samplerate = br.ReadByte()*256 + br.ReadByte();
                    int numSamples = (int) Swap(br.ReadUInt32());
                    br.ReadInt32();
                    br.ReadInt32(); // num_samples again

                    Writewavheader(bw, channels, samplerate, numSamples*channels*2);

                    int nfr = 1;
                    // dumb end search
                    while (inMs.Position + (int) Swap(br.ReadUInt32()) + 4 < inMs.Length) nfr++;
                    int datapos = (int) inMs.Position;

                    inMs.Seek(0x14, SeekOrigin.Begin);

                    int[] frs = new int[nfr + 1];
                    frs[0] = 0;
                    int[] audio = new int[1024];
                    int[] os = new int[1024];

                    for (int j = 0; j < nfr; j++) frs[j + 1] = (int) Swap(br.ReadUInt32());

                    for (int j = 0; j < nfr; j++)
                    {
                        int frSize = frs[j + 1] - frs[j] - 20*channels;
                        byte[] frame = new byte[frSize];
                        int frSamples = numSamples;
                        if (frSamples > 1024) frSamples = 1024;

                        int sample, sample1, sample2, input, output;
                        int vv10, vv20, vv21, vv22;

                        int v6, v16, v17, v18, v19, v26, v27, v31, v34, v35, v39 = 0;
                        int Bitvalue, Result, Bit16;
                        int C1, C2, C3;

                        inMs.Seek(datapos + frs[j] + 20*channels, SeekOrigin.Begin);
                        inMs.Read(frame, 0, frSize);
                        inMs.Seek(datapos + frs[j], SeekOrigin.Begin);

                        vv20 = br.ReadSByte() << 8 | br.ReadByte();
                        vv21 = br.ReadSByte() << 8 | br.ReadByte();
                        output = br.ReadSByte() << 8 | br.ReadByte();
                        vv22 = br.ReadSByte() << 8 | br.ReadByte();
                        sample1 = br.ReadSByte() << 8 | br.ReadByte();
                        sample2 = br.ReadSByte() << 8 | br.ReadByte();
                        vv10 = br.ReadSByte() << 8 | br.ReadByte();
                        C1 = br.ReadSByte() << 8 | br.ReadByte();
                        C2 = br.ReadSByte() << 8 | br.ReadByte();
                        C3 = br.ReadSByte() << 8 | br.ReadByte();

                        #region phase 1 left

                        int bb, bn = 0, bprev = -1;
                        var i = 0;
                        while (i < frSamples)
                        {
                            bb = ((frame[bn/8]) >> (7 - bn%8)) & 1;
                            if (bb > 0)
                            {
                                //Console.WriteLine(bn - bprev - 1);
                                v6 = 0;
                                Bitvalue = bn - bprev - 1;
                                bprev = bn;
                                if (Bitvalue >= 2)
                                {
                                    if (Bitvalue >= 4)
                                    {
                                        if (Bitvalue >= 6)
                                        {
                                            if (Bitvalue == 24) // take 16 bits from stream
                                            {
                                                Bit16 = 0;
                                                for (int k = 0; k < 16; k++)
                                                {
                                                    bn++;
                                                    Bit16 = Bit16*2 + (((frame[bn/8]) >> (7 - bn%8)) & 1); // 1 bit
                                                    bprev++;
                                                }
                                                Bitvalue = Bit16 + 24; // add them
                                            }
                                            v26 = C3;
                                            v27 = C2;
                                            v6 = C1 + v27 + (v26 + 1)*((Bitvalue >> 1) - 2);
                                            v39 = v26 + v6;
                                            C1 += 6*((C1 + 2048)/2048);
                                            C2 = v27 + 6*((v27 + 1024)/1024);
                                            C3 = v26 + 6*((v26 + 512)/512);
                                        }
                                        else
                                        {
                                            v18 = C2;
                                            v19 = C3;
                                            v6 = v18 + C1;
                                            v39 = v19 + v6;
                                            C1 += 6*((C1 + 2048)/2048);
                                            C2 = v18 + 6*((v18 + 1024)/1024);
                                            C3 = v19 - 2*((v19 + 510)/512);
                                        }
                                    }
                                    else
                                    {
                                        v6 = C1;
                                        v17 = C2;
                                        v39 = v17 + C1;
                                        C1 = v6 + 6*((v6 + 2048)/2048);
                                        C2 = v17 - 2*((v17 + 1022)/1024);
                                    }
                                    v16 = v39;
                                }
                                else
                                {
                                    v16 = C1;
                                    C1 -= 2*((C1 + 2046)/2048);
                                }
                                Result = (v16 + v6) >> 1;

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

                                v31 = v16 - v6;
                                if (v31 > 0x40) // ???
                                {
                                    v34 = v31/4 + 1;

                                    // take 2 bits from stream
                                    bn++;
                                    bb = ((frame[bn/8]) >> (7 - bn%8)) & 1; // 1st
                                    bn++;
                                    v35 = bb*2 + (((frame[bn/8]) >> (7 - bn%8)) & 1); // 2nd
                                    bprev += 2;

                                    Result = v6 + v34*v35;
                                }

                                if ((Bitvalue & 1) > 0) Result = ~Result; // sign

                                os[i++] = Result;
                            }
                            bn++;
                        }

                        #endregion

                        int v14, v28, v12;

                        #region phase 2 left

                        for (i = 0; i < frSamples; i++)
                        {
                            input = os[i];

                            v18 = output;

                            v14 = 2*(2*sample1 - vv10) - 5*sample2;
                            v12 = 2*output - vv22;
                            v28 = sample2;
                            sample2 = sample1;
                            sample1 = input + ((vv21*v14 + 128) >> 8);

                            // the meaning of this stuff: 
                            // d=2
                            // if (v14 < 0) d=-d;
                            // if (input < 0) d=-d;

                            if ((v14 | input) != 0) vv21 += ((v14 ^ input) & -0x20000001 | 0x40000000) >> 29;

                            sample = sample1 + ((vv20*v12 + 128) >> 8);
                            if (sample > 32767) sample = 32767;
                            if (sample < -32768) sample = -32768;

                            if ((sample1 | v12) != 0) vv20 += ((sample1 ^ v12) & -0x20000001 | 0x40000000) >> 29;

                            vv22 = v18;
                            //output = (output + 7 * sample) / 8;
                            output = sample;

                            audio[i] = output;

                            vv10 = v28;
                        }

                        #endregion

                        //for (i = 0; i < 512; i++) Console.WriteLine(audio[i].ToString("X8"));
                        for (i = 0; i < frSamples; i++) bw.Write((short) audio[i]);

                        numSamples -= frSamples;
                    }

                    bw.Close();
                    inMs.Close();

                    return ms.ToArray();
                }
            }
        }
    }
}
