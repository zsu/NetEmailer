using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetEmailer.Extension;
namespace NetEmailer.Common
{


    public class Check
    {
        [DebuggerStepThrough]
        public static void IsNotEmpty(string argument, string argumentName)
        {
            if (string.IsNullOrEmpty((argument ?? string.Empty).Trim()))
            {
                throw new ArgumentException(NetEmailer.Argument_Cannot_Be_Empty.FormatWith(argumentName), argumentName);
            }
        }
    }
}