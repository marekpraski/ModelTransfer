using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelTransfer
{

        /// Class that implements IConvertible
        class Complex : IConvertible
        {
            double x;
            double y;

            public Complex(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public TypeCode GetTypeCode()
            {
                return TypeCode.Object;
            }

            bool IConvertible.ToBoolean(IFormatProvider provider)
            {
                if ((x != 0.0) || (y != 0.0))
                    return true;
                else
                    return false;
            }

            double GetDoubleValue()
            {
                return Math.Sqrt(x * x + y * y);
            }

            byte IConvertible.ToByte(IFormatProvider provider)
            {
                return Convert.ToByte(GetDoubleValue());
            }

            char IConvertible.ToChar(IFormatProvider provider)
            {
                return Convert.ToChar(GetDoubleValue());
            }

            DateTime IConvertible.ToDateTime(IFormatProvider provider)
            {
                return Convert.ToDateTime(GetDoubleValue());
            }

            decimal IConvertible.ToDecimal(IFormatProvider provider)
            {
                return Convert.ToDecimal(GetDoubleValue());
            }

            double IConvertible.ToDouble(IFormatProvider provider)
            {
                return GetDoubleValue();
            }

            short IConvertible.ToInt16(IFormatProvider provider)
            {
                return Convert.ToInt16(GetDoubleValue());
            }

            int IConvertible.ToInt32(IFormatProvider provider)
            {
                return Convert.ToInt32(GetDoubleValue());
            }

            long IConvertible.ToInt64(IFormatProvider provider)
            {
                return Convert.ToInt64(GetDoubleValue());
            }

            sbyte IConvertible.ToSByte(IFormatProvider provider)
            {
                return Convert.ToSByte(GetDoubleValue());
            }

            float IConvertible.ToSingle(IFormatProvider provider)
            {
                return Convert.ToSingle(GetDoubleValue());
            }

            string IConvertible.ToString(IFormatProvider provider)
            {
                return String.Format("({0}, {1})", x, y);
            }

            object IConvertible.ToType(Type conversionType, IFormatProvider provider)
            {
                return Convert.ChangeType(GetDoubleValue(), conversionType);
            }

            ushort IConvertible.ToUInt16(IFormatProvider provider)
            {
                return Convert.ToUInt16(GetDoubleValue());
            }

            uint IConvertible.ToUInt32(IFormatProvider provider)
            {
                return Convert.ToUInt32(GetDoubleValue());
            }

            ulong IConvertible.ToUInt64(IFormatProvider provider)
            {
                return Convert.ToUInt64(GetDoubleValue());
            }

        }

    }

