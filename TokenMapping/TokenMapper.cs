#region License
// Copyright 2006 Zhicheng Su
//
//Licensed under the Apache License, Version 2.0 (the "License"); 
//you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 

//http://www.apache.org/licenses/LICENSE-2.0 

//Unless required by applicable law or agreed to in writing, software 
//distributed under the License is distributed on an "AS IS" BASIS, 
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and 
//limitations under the License. 
#endregion
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace NetEmailer.TokenMapping
{
    /// <summary>
    /// Substitue parameters with the value provided by <see cref="IParameterProvider"/>.
    /// </summary>
    internal static class TokenMapper
    {
        #region CodeBlock Constants
        private const string CodeBlock = @"{=(\w+)}";
        private const string DateTime = "DateTime";
        private const string DateTimeFormat = "DateTimeFormat";
        private const string Newline = "Newline";
        private const string Tab = "Tab";
        private const string WhiteSpace = "WhiteSpace";
        private const string DoubleQuote = "DoubleQuote";
        private const string SingleQuote = "SingleQuote";
        #endregion
        /// <summary>
        /// Substitue paramerters in source string with values provided by the <see cref="IParameterProvider"/>. 
        /// The followings are predefined parameters:
        /// {=DateTime}: Current datetime value. Default format is "MMMM d, yyyy \"at\" HH:mm ET". 
        /// <see cref="IParameterProvider"/> can provide a custom format using the key "DateTimeFormat";
        /// {=Newline}: New line character.
        /// {=Tab}: Tab.
        /// {=WhiteSpace}: White space.
        /// {=DoubleQuote}: Double quote.
        /// {SingleQuote}: Single quote.
        /// </summary>
        /// <param name="sSource">Source string with parameters defined in the format of {=paramertername}</param>
        /// <param name="oParameterProvider">Paramerters value provider</param>
        /// <returns>String with matched parameters replaced</returns>
        static public string ParseParameterizedString(string source, IParameterProvider parameterProvider)
        {
            string dateTimeFormat;
            string pre, post = String.Empty;
            int begin = 0, end = 0;
            StringBuilder strBuilder = new StringBuilder();
            MatchCollection matches = Regex.Matches(source, TokenMapper.CodeBlock);
            if (matches.Count <= 0)
            { return source; }
            foreach (Match match in matches)
            {
                end = match.Index;
                if (begin < source.Length)
                {
                    pre = source.Substring(begin, end - begin);
                    begin = end + match.Length;
                    post = source.Substring(begin);
                    strBuilder.Append(pre);
                }
                try
                {
                    switch (match.Result("$1").Trim())
                    {
                        case TokenMapper.DateTime:
                            dateTimeFormat = parameterProvider.GetProperty(DateTimeFormat);
                            if (dateTimeFormat != null && dateTimeFormat.Trim() != String.Empty)
                                try
                                {
                                    strBuilder.Append(System.DateTime.Now.ToString(dateTimeFormat));
                                }
                                catch
                                {
                                    throw new System.ArgumentException("DateTimeFormat is not valid.");
                                }
                            else
                                strBuilder.Append(System.DateTime.Now.ToString("MMMM d, yyyy \"at\" HH:mm ET"));
                            break;
                        case TokenMapper.Newline:
                            strBuilder.AppendLine();
                            break;
                        case TokenMapper.Tab:
                            strBuilder.Append("\t");
                            break;
                        case TokenMapper.WhiteSpace:
                            strBuilder.Append(" ");
                            break;
                        case TokenMapper.DoubleQuote:
                            strBuilder.Append("\"");
                            break;
                        case TokenMapper.SingleQuote:
                            strBuilder.Append("'");
                            break;
                        default:
                            strBuilder.Append(parameterProvider.GetProperty(match.Result("$1").Trim()));
                            break;
                    }

                }
                catch (Exception exception)
                {
                    throw new Exception(string.Format("Failed to retrieve value for key {0}.", match.Result("$1").Trim()), exception);
                }
            }

            strBuilder.Append(post);
            return strBuilder.ToString();
        }


    }
}
