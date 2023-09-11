using System.Text.RegularExpressions;

namespace Branas.Services
{
    public class CpfValidator
    {
        public bool Validate(string cpf)
        {
            if (string.IsNullOrEmpty(cpf)) return false;

            cpf = clean(cpf);
            if (isInvalidLength(cpf)) return false;
            if (allDigitsTheSame(cpf))
            {
                int dg1 = this.calculateDigit(cpf, 10);
                int dg2 = this.calculateDigit(cpf, 11);
                var checkDigit = this.extractDigit(cpf);
                string calculatedDigit = $"{dg1}{dg2}";
                return checkDigit == calculatedDigit;
            }
            else
            {
                return false;
            }

        }

        private string clean(string cpf)
        {
            return Regex.Replace(cpf, "\\D", string.Empty);
        }

        private bool isInvalidLength(string cpf)
        {
            return cpf.Length != 11;
        }

        private bool allDigitsTheSame(string cpf)
        {
            return !cpf.Split("").All(c => c == cpf[0].ToString());
        }

        private int calculateDigit(string cpf, int factor)
        {
            var total = 0;

            foreach (var digit in cpf)
            {
                if (factor > 1) total += int.Parse(digit.ToString()) * factor--;
            }
            var rest = total % 11;
            return (rest < 2) ? 0 : 11 - rest;
        }

        private string extractDigit(string cpf)
        {
            return cpf.Substring(cpf.Length - 2, 2);
        }
    }
}
