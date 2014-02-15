using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.Model.Common
{
    public struct Md5Hash
    {
        public byte B_1;
        public byte B_2;
        public byte B_3;
        public byte B_4;
        public byte B_5;
        public byte B_6;
        public byte B_7;
        public byte B_8;
        public byte B_9;
        public byte B_10;
        public byte B_11;
        public byte B_12;
        public byte B_13;
        public byte B_14;
        public byte B_15;
        public byte B_16;
        
        public static Md5Hash GetHash(byte[] arr) 
        {
            Md5Hash h = new Md5Hash();

            h.B_1 = arr[0];
            h.B_2 = arr[1];
            h.B_3 = arr[2];
            h.B_4 = arr[3];
            h.B_5 = arr[4]; 
            h.B_6 = arr[5];
            h.B_7 = arr[6];
            h.B_8 = arr[7];
            h.B_9 = arr[8];
            h.B_10 = arr[9];
            h.B_11 = arr[10];
            h.B_12 = arr[11];
            h.B_13 = arr[12];
            h.B_14 = arr[13];
            h.B_15 = arr[14];
            h.B_16 = arr[15];

            return h;
        }

        public static byte[] GetBytes(Md5Hash hash)
        {
            List<byte> l = new List<byte>();

            l.Add(hash.B_1);
            l.Add(hash.B_2);
            l.Add(hash.B_3);
            l.Add(hash.B_4);
            l.Add(hash.B_5);
            l.Add(hash.B_6);
            l.Add(hash.B_7);
            l.Add(hash.B_8);
            l.Add(hash.B_9);
            l.Add(hash.B_10);
            l.Add(hash.B_11);
            l.Add(hash.B_12);
            l.Add(hash.B_13);
            l.Add(hash.B_14);
            l.Add(hash.B_15);
            l.Add(hash.B_16);

            return l.ToArray();
        }
    }
}
