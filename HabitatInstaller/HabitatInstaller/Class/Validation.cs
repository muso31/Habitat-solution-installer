using System;
using System.IO;

namespace HabitatInstaller.Class
{
    public static class Validation
    {
        private const string NO_TRAILINGSLASH = @"invalid - field must end with \";
        private const string PATH_DOES_NOT_EXIST = "does not exist";
        private const string STRING_EMPTY = "Fields cannot be empty";
        private const string NOT_VALID_URL = "please insert a valid Url";

        public static bool DirectoryExistsWithTrailingSlash(string path, out string errorReason)
        {
            if (!path.EndsWith(@"\"))
            {
                errorReason = string.Format("{0} {1} {2}", "Path", path, NO_TRAILINGSLASH);
                return false;
            }
            else if (!Directory.Exists(path))
            {
                errorReason = string.Format("{0} {1}", path, PATH_DOES_NOT_EXIST);
                return false;
            }
            else
            {
                errorReason = String.Empty;
                return true;
            }
        }

        public static bool IsValidFieldInput(string inputText, out string errorReason)
        {
            if (string.IsNullOrEmpty(inputText))
            {
                errorReason = string.Format("{0}", STRING_EMPTY);
                return false;
            }
            else
            {
                errorReason = String.Empty;
                return true;
            }
        }
    }
}
