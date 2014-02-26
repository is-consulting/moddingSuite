using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.BL.ImageService.BlockDXT
{
    [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 1)]
    public struct AlphaBlockDXT5
    {
        [FieldOffset(0)]
        private ulong alpha0_i;	// 8

        public ulong alpha0
        {
            get { return ((alpha0_i >> 56) & 0xFF); }
            set { alpha0_i = ((alpha0_i & ~(0xFF << 56)) | (value & 0xFF) << 56); }
        }

        [FieldOffset(0)]
        private ulong alpha1_i;	// 16

        public ulong alpha1
        {
            get { return ((alpha1_i >> 48) & 0xFF); }
            set { alpha1_i = ((alpha1_i & ~(0xFFU << 48)) | (value & 0xFFU) << 48); }
        }

        [FieldOffset(0)]
        ulong bits0_i;	// 3 - 19

        public ulong bits0
        {
            get { return ((bits0_i >> 45) & 0x7U); }
            set { bits0_i = ((bits0_i & ~(0x7U << 45)) | (value & 0x7U) << 45); }
        }

        [FieldOffset(0)]
        ulong bits1_i; 	// 6 - 22

        public ulong bits1
        {
            get { return ((bits1_i >> 42) & 0x7U); }
            set { bits1_i = ((bits1_i & ~(0x7U << 42)) | (value & 0x7U) << 42); }
        }

        [FieldOffset(0)]
        ulong bits2_i; 	// 9 - 25

        public ulong bits2
        {
            get { return ((bits2_i >> 39) & 0x7U); }
            set { bits2_i = ((bits2_i & ~(0x7U << 39)) | (value & 0x7U) << 39); }
        }

        [FieldOffset(0)]
        ulong bits3_i;	// 12 - 28

        public ulong bits3
        {
            get { return ((bits3_i >> 36) & 0x7U); }
            set { bits3_i = ((bits3_i & ~(0x7U << 36)) | (value & 0x7U) << 36); }
        }

        [FieldOffset(0)]
        ulong bits4_i;	// 15 - 31

        public ulong bits4
        {
            get { return ((bits4_i >> 33) & 0x7U); }
            set { bits4_i = ((bits4_i & ~(0x7U << 33)) | (value & 0x7U) << 33); }
        }

        [FieldOffset(0)]
        ulong bits5_i;	// 18 - 34

        public ulong bits5
        {
            get { return ((bits5_i >> 30) & 0x7U); }
            set { bits5_i = ((bits5_i & ~(0x7U << 30)) | (value & 0x7U) << 30); }
        }

        [FieldOffset(0)]
        ulong bits6_i;	// 21 - 37

        public ulong bits6
        {
            get { return ((bits6_i >> 27) & 0x7U); }
            set { bits6_i = ((bits6_i & ~(0x7U << 27)) | (value & 0x7U) << 27); }
        }

        [FieldOffset(0)]
        ulong bits7_i;	// 24 - 40

        public ulong bits7
        {
            get { return ((bits7_i >> 24) & 0x7U); }
            set { bits7_i = ((bits7_i & ~(0x7U << 24)) | (value & 0x7U) << 24); }
        }

        [FieldOffset(0)]
        ulong bits8_i;	// 27 - 43

        public ulong bits8
        {
            get { return ((bits8_i >> 21) & 0x7U); }
            set { bits8_i = ((bits8_i & ~(0x7U << 21)) | (value & 0x7U) << 21); }
        }

        [FieldOffset(0)]
        ulong bits9_i; 	// 30 - 46

        public ulong bits9
        {
            get { return ((bits9_i >> 18) & 0x7U); }
            set { bits9_i = ((bits9_i & ~(0x7U << 18)) | (value & 0x7U) << 18); }
        }

        [FieldOffset(0)]
        ulong bitsA_i; 	// 33 - 49

        public ulong bitsA
        {
            get { return ((bitsA_i >> 15) & 0x7U); }
            set { bitsA_i = ((bitsA_i & ~(0x7U << 15)) | (value & 0x7U) << 15); }
        }

        [FieldOffset(0)]
        ulong bitsB_i;	// 36 - 52

        public ulong bitsB
        {
            get { return ((bitsB_i >> 12) & 0x7U); }
            set { bitsB_i = ((bitsB_i & ~(0x7U << 12)) | (value & 0x7U) << 12); }
        }

        [FieldOffset(0)]
        ulong bitsC_i;	// 39 - 55

        public ulong bitsC
        {
            get { return ((bitsC_i >> 9) & 0x7U); }
            set { bitsC_i = ((bitsC_i & ~(0x7U << 9)) | (value & 0x7U) << 9); }
        }

        [FieldOffset(0)]
        ulong bitsD_i;	// 42 - 58

        public ulong bitsD
        {
            get { return ((bitsD_i >> 6) & 0x7U); }
            set { bitsD_i = ((bitsD_i & ~(0x7U << 6)) | (value & 0x7U) << 6); }
        }

        [FieldOffset(0)]
        ulong bitsE_i;	// 45 - 61

        public ulong bitsE
        {
            get { return ((bitsE_i >> 3) & 0x7U); }
            set { bitsE_i = ((bitsE_i & ~(0x7U << 3)) | (value & 0x7U) << 3); }
        }

        [FieldOffset(0)]
        ulong bitsF_i;	// 48 - 64

        public ulong bitsF
        {
            get { return (bitsF_i & 0x7U); }
            set { bitsF_i = ((bitsF_i & ~0x7U) | (value & 0x7U)); }
        }

        [FieldOffset(0)]
        ulong u;



        public void evaluatePalette(ref byte[] alpha)
        {
            if (alpha.Length > 8)
                throw new ArgumentException("alpha");

            if (alpha0 > alpha1)
            {
                evaluatePalette8(ref alpha);
            }
            else
            {
                evaluatePalette6(ref alpha);
            }
        }

        public void evaluatePalette8(ref byte[] alpha)
        {
            if (alpha.Length != 8)
                throw new ArgumentException("alpha");
            // 8-alpha block:  derive the other six alphas.
            // Bit code 000 = alpha0, 001 = alpha1, others are interpolated.
            alpha[0] = (byte)alpha0;
            alpha[1] = (byte)alpha1;
            alpha[2] = (byte)((6 * alpha[0] + 1 * alpha[1]) / 7);	// bit code 010
            alpha[3] = (byte)((5 * alpha[0] + 2 * alpha[1]) / 7);	// bit code 011
            alpha[4] = (byte)((4 * alpha[0] + 3 * alpha[1]) / 7);	// bit code 100
            alpha[5] = (byte)((3 * alpha[0] + 4 * alpha[1]) / 7);	// bit code 101
            alpha[6] = (byte)((2 * alpha[0] + 5 * alpha[1]) / 7);	// bit code 110
            alpha[7] = (byte)((1 * alpha[0] + 6 * alpha[1]) / 7);	// bit code 111
        }

        public void evaluatePalette6(ref byte[] alpha)
        {
            if (alpha.Length != 8)
                throw new ArgumentException("alpha");
            // 6-alpha block.
            // Bit code 000 = alpha0, 001 = alpha1, others are interpolated.
            alpha[0] = (byte)alpha0;
            alpha[1] = (byte)alpha1;
            alpha[2] = (byte)((4 * alpha[0] + 1 * alpha[1]) / 5);	// Bit code 010
            alpha[3] = (byte)((3 * alpha[0] + 2 * alpha[1]) / 5);	// Bit code 011
            alpha[4] = (byte)((2 * alpha[0] + 3 * alpha[1]) / 5);	// Bit code 100
            alpha[5] = (byte)((1 * alpha[0] + 4 * alpha[1]) / 5);	// Bit code 101
            alpha[6] = 0x00;							// Bit code 110
            alpha[7] = 0xFF;							// Bit code 111
        }

        public void indices(ref byte[] index_array)
        {
            if (index_array.Length != 16)
                throw new ArgumentException("index_array");

            index_array[0x0] = (byte)bits0;
            index_array[0x1] = (byte)bits1;
            index_array[0x2] = (byte)bits2;
            index_array[0x3] = (byte)bits3;
            index_array[0x4] = (byte)bits4;
            index_array[0x5] = (byte)bits5;
            index_array[0x6] = (byte)bits6;
            index_array[0x7] = (byte)bits7;
            index_array[0x8] = (byte)bits8;
            index_array[0x9] = (byte)bits9;
            index_array[0xA] = (byte)bitsA;
            index_array[0xB] = (byte)bitsB;
            index_array[0xC] = (byte)bitsC;
            index_array[0xD] = (byte)bitsD;
            index_array[0xE] = (byte)bitsE;
            index_array[0xF] = (byte)bitsF;
        }

        public uint index(uint index)
        {
            if (index > 16)
                throw new ArgumentException("index");

            int offset = (int)(3 * index + 16);
            return (uint)((u >> offset) & 0x7);
        }

        public void setIndex(uint index, uint value)
        {
            if (index > 16)
                throw new ArgumentException("index");
            if (value > 8)
                throw new ArgumentException("value");

            int offset = (int)(3 * index + 16);
            ulong mask = (ulong)0x7 << offset;
            u = (u & ~mask) | ((ulong)value << offset);
        }

        public void decodeBlock(ColorBlock block)
        {
            if (block == null)
                throw new ArgumentException("block");

            byte[] alpha_array = new byte[8];
            evaluatePalette(ref alpha_array);

            byte[] index_array = new byte[16];
            indices(ref index_array);

            for (uint i = 0; i < 16; i++)
            {
                block.Data[i].a = alpha_array[index_array[i]];
            }
        }

        public void flip4()
        {
            // @@ The masks might have to be byte swapped.
            ulong tmp = (u & (0x000000000000FFFFUL));
            tmp |= (u & 0x000000000FFF0000UL) << 36;
            tmp |= (u & 0x000000FFF0000000UL) << 12;
            tmp |= (u & 0x000FFF0000000000UL) >> 12;
            tmp |= (u & 0xFFF0000000000000UL) >> 36;

            u = tmp;
        }

        public void flip2()
        {
            // @@ The masks might have to be byte swapped.
            uint tmp = ((uint)u & 0xFF000000);
            tmp |= ((uint)u & 0x00000FFF) << 12;
            tmp |= ((uint)u & 0x00FFF000) >> 12;

            u = tmp;
        }

    }
}
