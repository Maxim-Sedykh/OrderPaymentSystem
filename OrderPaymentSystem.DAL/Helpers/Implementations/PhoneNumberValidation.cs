using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrderPaymentSystem.DAL.Helpers.Implementations
{
    public class PhoneNumberValidation
    {
        public static string FormatPhoneNumber(string phoneNumber)
        {
            return Regex.Replace(phoneNumber, @"(\d{1})(\d{3})(\d{3})(\d{2})(\d{2})", "+7 ($2) $3-$4-$5");
        }
    }
}
