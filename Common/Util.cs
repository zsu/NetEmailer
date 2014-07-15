using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Reflection;

namespace NetEmailer.Common
{
    public static class Util
    {
        #region Fields
        #endregion

        #region Methods
        /// <summary>
        /// Resolve path into absolute physical path.
        /// </summary>
        /// <param name="sPath">Relative path to the webroot or absolute physical path of file</param>
        /// <returns>Absolute Physical full path of the file</returns>
        public static string GetFullPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;
            string pathRoot = Path.GetPathRoot(path);
            if (pathRoot == string.Empty || pathRoot == "\\")
                if (System.Web.HttpContext.Current != null)
                    return Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~"), path);
                else
                    if (Assembly.GetEntryAssembly() != null)
                        return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), path);
                    else
                        return Path.GetFullPath(path);
            else
                return path;
        }
        #endregion
    }
}
