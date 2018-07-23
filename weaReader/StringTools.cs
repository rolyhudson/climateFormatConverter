using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weaReader
{
    class StringTools
    {
        static public string getExt(string filename)
        {
           
            int lastdot = filename.LastIndexOf(".");
            string ext = filename.Substring(lastdot);
            return ext;
        }
        static public string stripExt(string filename)
        {
            int lastdot = filename.LastIndexOf(".");
            string name = filename.Substring(0,lastdot);
            return name;
        }
        static public string stripUnusedChars(string filename)
        {
            int lastdot = filename.LastIndexOf(".");
            int lastslash = filename.LastIndexOf("\\0");
            string name; 
            if (lastdot!=-1)
            {
                name = filename.Substring(0, lastdot);
            }
            else
            {
                name = filename.Substring(0, lastslash);
            }
            
            return name;
        }
        static public string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
    }
}
