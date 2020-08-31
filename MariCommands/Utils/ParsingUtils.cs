using System;
using System.Collections.Generic;
using MariCommands.Models;

namespace MariCommands.Utils
{
    internal static class ParsingUtils
    {
        private static readonly IReadOnlyDictionary<Type, Delegate> _tryParseDelegates;

        static ParsingUtils()
        {
            _tryParseDelegates = new Dictionary<Type, Delegate>()
            {
                [typeof(char)] = (TryParseDelegate<char>)TryParseChar,
                [typeof(bool)] = (TryParseDelegate<bool>)bool.TryParse,
                [typeof(byte)] = (TryParseDelegate<byte>)byte.TryParse,
                [typeof(sbyte)] = (TryParseDelegate<sbyte>)sbyte.TryParse,
                [typeof(short)] = (TryParseDelegate<short>)short.TryParse,
                [typeof(ushort)] = (TryParseDelegate<ushort>)ushort.TryParse,
                [typeof(int)] = (TryParseDelegate<int>)int.TryParse,
                [typeof(uint)] = (TryParseDelegate<uint>)uint.TryParse,
                [typeof(long)] = (TryParseDelegate<long>)long.TryParse,
                [typeof(ulong)] = (TryParseDelegate<ulong>)ulong.TryParse,
                [typeof(float)] = (TryParseDelegate<float>)float.TryParse,
                [typeof(double)] = (TryParseDelegate<double>)double.TryParse,
                [typeof(decimal)] = (TryParseDelegate<decimal>)decimal.TryParse
            };
        }

        public static TryParseDelegate<T> GetParseDelegate<T>()
            where T : struct
        {
            return _tryParseDelegates[typeof(T)] as TryParseDelegate<T>;
        }

        private static bool TryParseChar(ReadOnlySpan<char> input, out char value)
        {
            if (input.Length == 1)
            {
                value = input[0];
                return true;
            }

            value = default;
            return false;
        }

        public static bool IsNullable(IParameter param)
            => IsNullable(param.ParameterInfo.ParameterType);

        public static bool IsNullable(Type type)
            => type.IsGenericType &&
               type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
}