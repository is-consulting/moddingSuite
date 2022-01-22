﻿using System;
using moddingSuite.BL.Utils;

namespace moddingSuite.Model.Ndfbin.Types.AllTypes
{
    public class NdfLocalisationHash : NdfFlatValueWrapper
    {
        public NdfLocalisationHash(byte[] value)
            : base(NdfType.LocalisationHash, value)
        {
        }

        public new byte[] Value
        {
            get { return (byte[]) base.Value; }
            set
            {
                base.Value = value;
                OnPropertyChanged(() => Value);
            }
        }

        public override byte[] GetBytes()
        {
            return Value;
        }

        public override string ToString()
        {
            return StdUtils.ByteArrayToBigEndianHexByteString(Value);
        }

        public override byte[] GetNdfText()
        {
            throw new NotImplementedException();
        }
    }
}