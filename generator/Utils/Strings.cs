﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Utils
{
    public static class Strings
    {

        public static StringBuilder AppendLine(this StringBuilder sb, int pad, string text)
        {
            for (var i=0; i<pad; i++)
            {
                sb.Append("  ");
            }
            return sb.AppendLine(text);
        }

        private static string OrderSuffix = "篇章条";

        public static string GetOrderString1(int pad, int value, int sub)
        {
            var s = $"第{GetNumberString(value)}{OrderSuffix[pad]}";
            if (sub > 0)
            {
                s += $"之{GetNumberString(sub)}";
            }
            return s;
        }

        public static string GetOrderString2(int pad, int value, int sub)
        {
            if (pad == 0)
            {
                return "（" + GetNumberString(value) + "）";
            }
            else
            {
                return $"{value}.";
            }
        }

        private static string Numbers = "零一二三四五六七八九十";

        public static string GetNumberString(int value)
        {
            var s = "";
            var _1 = value % 10;
            if (_1 > 0)
            {
                s += Numbers[_1];
            }
            if (value < 10)
            {
                return s;
            }
            value /= 10;
            var _2 = value % 10;
            value /= 10;
            var _3 = value % 10;
            if (_2 > 0)
            {
                if (_3 != 0 || _2 > 1)
                {
                    s = Numbers[_2] + "十" + s;
                }
                else
                {
                    s = "十" + s;
                }
            }
            if (_3 > 0)
            {
                if (_2 == 0) {
                    s = Numbers[_3] + "百零" + s;
                }
                else
                {
                    s = Numbers[_3] + "百" + s;
                }
            }
            return s;
        }

    }
}
