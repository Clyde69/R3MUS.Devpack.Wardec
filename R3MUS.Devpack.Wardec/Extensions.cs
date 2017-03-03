using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R3MUS.Devpack.Wardec
{
    static class Extensions
    {
        /// <summary>
        ///     Formats the given string with the provided paramaters
        /// </summary>
        /// <param name="value">The string</param>
        /// <param name="args">The parameters</param>
        public static string FormatWith(this string value, params object[] args)
        {
            return value == null
                ? null
                : string.Format(value, args);
        }

    }
}
