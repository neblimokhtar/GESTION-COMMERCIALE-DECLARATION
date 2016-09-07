using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace GestionCommerciale.Controllers
{
    public class ConvertisseurChiffresLettres
    {
        public  string NumberToCurrencyText(string number)
        {
            // Round the value just in case the decimal value is longer than two digits
            //number = decimal.Round(number, 2, MidpointRounding.ToEven);

            string wordNumber = string.Empty;

            // Divide the number into the whole and fractional part strings
            string[] arrNumber = number.Split(',');

            // Get the whole number text
            long wholePart = long.Parse(arrNumber[0]);
            string strWholePart = NumberToText(wholePart);

            // For amounts of zero dollars show 'No Dollars...' instead of 'Zero Dollars...'
            wordNumber = (wholePart == 0 ? "Zero" : strWholePart) + (wholePart == 1 ? " Dinar et " : " Dinars et ");

            // If the array has more than one element then there is a fractional part otherwise there isn't
            // just add 'No Cents' to the end
            if (arrNumber.Length > 1)
            {
                // If the length of the fractional element is only 1, add a 0 so that the text returned isn't,
                // 'One', 'Two', etc but 'Ten', 'Twenty', etc.
                long fractionPart = long.Parse((arrNumber[1].Length == 1 ? arrNumber[1] + "0" : arrNumber[1]));
                string strFarctionPart = NumberToText(fractionPart);

                wordNumber += (fractionPart == 0 ? " Zero" : strFarctionPart) + (fractionPart == 1 ? " Millime" : " Millimes");
            }
            else
                wordNumber += "Zero Millimes";

            return wordNumber;
        }
        public  string NumberToText(long number)
        {
            StringBuilder wordNumber = new StringBuilder();

            string[] powers = new string[] { "Mille ", "Million ", "Milliard " };
            string[] tens = new string[] { "Vingt", "Trente", "Quarante", "Cinquante", "Soixante", "Soixante dix", "Quatres Vingt", "Quatres Vingt dix" };
            string[] ones = new string[] { "Un", "Deux", "Troi", "Quatre", "Cinq", "Six", "Sept", "Huit", "Neuf", "Dix", 
                                       "Onze", "Douze", "Treize", "Quatorze", "Quinze", "Seize", "Dix Sept", "Dix Huit", "Dix Neuf" };

            if (number == 0) { return "Zero"; }
            if (number < 0)
            {
                wordNumber.Append("Negative ");
                number = -number;
            }

            long[] groupedNumber = new long[] { 0, 0, 0, 0 };
            int groupIndex = 0;

            while (number > 0)
            {
                groupedNumber[groupIndex++] = number % 1000;
                number /= 1000;
            }

            for (int i = 3; i >= 0; i--)
            {
                long group = groupedNumber[i];

                if (group >= 100)
                {
                    wordNumber.Append(ones[group / 100 - 1] + " Cent ");
                    group %= 100;

                    if (group == 0 && i > 0)
                        wordNumber.Append(powers[i - 1]);
                }

                if (group >= 20)
                {
                    if ((group % 10) != 0)
                        wordNumber.Append(tens[group / 10 - 2] + " " + ones[group % 10 - 1] + " ");
                    else
                        wordNumber.Append(tens[group / 10 - 2] + " ");
                }
                else if (group > 0)
                    wordNumber.Append(ones[group - 1] + " ");

                if (group != 0 && i > 0)
                    wordNumber.Append(powers[i - 1]);
            }

            return wordNumber.ToString().Trim();
        }
    }
}