﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace moddingSuite.BL.Utils
{
    public static class StdUtils
    {
        public static string ByteArrayToBigEndianHexByteString(byte[] data)
        {
            if (data == null)
                return string.Empty;

            var stringBuilderb = new StringBuilder();

            stringBuilderb.Append(string.Empty);

            foreach (var b in data)
                stringBuilderb.Append(string.Format("{0:X2}", b));

            return stringBuilderb.ToString();
        }

        public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return stuff;
        }

        public static string ReadString(Stream fs)
        {
            var b = new StringBuilder();
            int c;

            do
            {
                c = fs.ReadByte();
                b.Append((char)c);
            } while (c != '\0');

            return StripString(b.ToString());
        }

        public static string StripString(string s)
        {
            return s.Split('\0')[0].TrimEnd();
        }

        public static bool ByteArrayCompare(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }


        public static int RoundToNextDivBy4(int number)
        {
            while (number % 4 != 0)
                number++;

            return number;
        }

        public static void Swap<T>(T a, T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static bool IsValueType(object obj)
        {
            return obj != null && obj.GetType().IsValueType;
        }

        public static byte[] StructToBytes(object str)
        {
            if (!IsValueType(str))
                throw new ArgumentException("str");

            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        public static byte[] CreateLocalisationHash(string text, int maxSize = 8)
        {
            long hash = 0;
            for (int i = 0; i < maxSize; ++i)
            {
                int value;
                ushort chr = text[i];

                if (chr == 0)
                    break;

                if ('0' <= chr && chr <= '9')
                    value = 1 + chr - '0';
                else if ('A' <= chr && chr <= 'Z')
                    value = 2 + '9' - '0' + chr - 'A';
                else if (chr == '_')
                    value = 3 + '9' - '0' + 'Z' - 'A';
                else if ('a' <= chr && chr <= 'z')
                    value = 4 + '9' - '0' + 'Z' - 'A' + chr - 'a';
                else
                    throw new InvalidDataException("");

                hash = (hash << 6) | value;
            }

            return BitConverter.GetBytes(hash);
        }
    }
}
