namespace NetEmailer.Extension
{
    using System;
    using System.Diagnostics;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Globalization;

    public static class StringExtension
    {
        [DebuggerStepThrough]
        public static string FormatWith(this string target, params object[] args)
        {
            if (string.IsNullOrEmpty((target ?? string.Empty).Trim()))
            {
                throw new ArgumentException(NetEmailer.Argument_Cannot_Be_Empty.FormatWith(target), "target");
            }
            return string.Format(CultureInfo.CurrentCulture, target, args);
        }
    }
}