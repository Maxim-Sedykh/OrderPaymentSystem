using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrderPaymentSystem.DAL.Helpers.Implementations
{
    public class EmailValidation
    {
        public static string FormatEmail(string email)
        {
            // Простейшая проверка формата email с использованием регулярного выражения
            string pattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            Regex regex = new Regex(pattern);
            if (regex.IsMatch(email))
            {
                return email;
            }
            return "Некорректное значение";
        }
    }
}
