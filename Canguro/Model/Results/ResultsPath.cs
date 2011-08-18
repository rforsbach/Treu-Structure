using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Model.Results
{
    class ResultsPath
    {
        public const char Separator = '/';
        public const char AlternateSeparator = '~';

        public static string Combine(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1))
                return path2.Trim(Separator);
            else if (string.IsNullOrEmpty(path2))
                return path1.Trim(Separator);
            else
                return path1.Trim(Separator) + Separator + path2.Trim(Separator);
        }

        public static string Name(string path)
        {
            int i = path.LastIndexOf(Separator);
            if (i == -1)
                return path;
            else
                return path.Substring(i).Trim(Separator);
        }

        public static string OneUp(string path)
        {
            int i = path.LastIndexOf(Separator);
            if (i == -1)
                return string.Empty;
            else
                return path.Substring(0, i).Trim(Separator);
        }

        public static string FirstPart(string path)
        {
            path = path.Trim(Separator);

            int i = path.IndexOf(Separator);
            if (i == -1)
                return path;
            else
                return path.Substring(0, i).Trim(Separator);
        }

        public static string Format(string caseName)
        {
            string[] pathParts = caseName.Split(new char[] {AlternateSeparator, Separator}, StringSplitOptions.RemoveEmptyEntries);
            string path = string.Empty;

            foreach (string part in pathParts)
                path += part + Separator;

            return path.Trim(Separator);
        }

        /// <summary>
        /// Checks whether a path is part of another
        /// </summary>
        /// <param name="path1">Path that may extend path2 (i.e. Modal/Mode/1)</param>
        /// <param name="path2">Container path (i.e. Modal/Mode)</param>
        /// <returns></returns>
        public static bool Contains(string path1, string path2)
        {
            path1 = path1.Trim(Separator);
            path2 = path2.Trim(Separator);

            if (path1.Length >= path2.Length && path1.Substring(0, path2.Length).Equals(path2))
                return true;

            return false;
        }
    }
}
