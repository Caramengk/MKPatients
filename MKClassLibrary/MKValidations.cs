using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MKClassLibrary
{
    public static class MKValidations
    {
        public static string MKCapitalize(string input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            string LowerCase= input.Trim().ToLower();

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string titleCase = textInfo.ToTitleCase(LowerCase);

            return titleCase;
        }
        public static string MKExtractDigits(string input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            string digitsString = new string(input.Where(char.IsDigit).ToArray());
            

            return digitsString;
        }
        public static bool MKPostalCodeValidation(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return true;
            }
                

            input = input.ToUpper();
            var regex = new Regex("^[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ]?[0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]$");
            return regex.IsMatch(input);
        }

        public static string MKPostalCodeFormat(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
                

            input = input.ToUpper();
            if (input.Length == 6)
            {
                input = input.Insert(3, " ");
            }
            return input;
        }

        public static bool MKZipCodeValidation(ref string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                input = string.Empty;
                return true;
            }

            var digits = MKExtractDigits(input);

            switch (digits.Length)
            {
                case 5:
                    input = digits;
                    return true;
                case 9:
                    input = digits.Insert(5, "-");
                    return true;
                default:
                    return false;
            }
        }
    }
}
