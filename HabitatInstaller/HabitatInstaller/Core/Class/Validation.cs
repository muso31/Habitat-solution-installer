using System;
using System.IO;

namespace HabitatInstaller.Core.Class
{
    public static class Validation
    {
        private const string NO_TRAILING_CHAR = "invalid - field must end with ";
        private const string PATH_DOES_NOT_EXIST = "does not exist";
        private const string PATH_ALREADY_EXISTS = "already exists";
        private const string STRING_EMPTY = "Fields cannot be empty";

        public static bool DirectoryExists(string path, out string errorReason)
        {
            if (string.IsNullOrEmpty(path))
            {
                errorReason = string.Format("{0}", STRING_EMPTY);
                return false;
            }
            else if (!Directory.Exists(path))
            {
                errorReason = string.Format("{0} {1}", path, PATH_DOES_NOT_EXIST);
                return false;
            }
            else if (!path.EndsWith(@"\"))
            {
                errorReason = string.Format("{0} {1} {2} {3}", "Input", path, NO_TRAILING_CHAR, @"\");
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

        public static bool IsValidFieldInputWithTrailingChar(string inputText, string character, out string errorReason)
        {
            if (string.IsNullOrEmpty(inputText))
            {
                errorReason = string.Format("{0}", STRING_EMPTY);
                return false;
            }
            else if (!inputText.EndsWith(character))
            {
                errorReason = string.Format("{0} {1} {2} {3}", "Input", inputText, NO_TRAILING_CHAR, character);
                return false;
            }
            else if (Directory.Exists(inputText))
            {
                errorReason = string.Format("{0} {1}", inputText, PATH_ALREADY_EXISTS);
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
