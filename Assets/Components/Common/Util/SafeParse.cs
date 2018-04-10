using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace CommonComponent
{
    public class SafeParse
    {
        public static bool ParseToInt(string s, int defaultValue, out int result)
        {
            bool retCode = int.TryParse(s, out result);
            if (retCode == false)
            {
                result = defaultValue;
                Log.ErrorFormat("Failed to ParseToInt: {0}", s);
            }

            return retCode;
        }

        public static bool ParseToUint(string s, uint defaultValue, out uint result)
        {
            bool retCode = uint.TryParse(s, out result);
            if (retCode == false)
            {
                result = defaultValue;
                Log.ErrorFormat("Failed to ParseToUint: {0}", s);
            }

            return retCode;
        }

        public static bool ParseToFloat(string s, float defaultValue, out float result)
        {
            bool retCode = float.TryParse(s, out result);
            if (retCode == false)
            {
                result = defaultValue;
                Log.ErrorFormat("Failed to ParseToFloat: {0}", s);
            }

            return retCode;
        }

        public static bool ParseToBool(string s, bool defaultValue, out bool result)
        {
            bool retCode = bool.TryParse(s, out result);
            if (retCode == false)
            {
                result = defaultValue;
                Log.ErrorFormat("Failed to ParseToBool: {0}", s);
            }

            return retCode;
        }

        public static bool ParseToEnum<T>(string value, bool ingoreCase, T defaultValue, out T result)
        {
            bool retCode = true;
            try
            {
                result = (T)Enum.Parse(typeof(T), value, ingoreCase);
            }
            catch
            {
                retCode = false;
                result = defaultValue;
                Log.ErrorFormat("Failed to ParseToEnum: {0}", value);
            }

            return retCode;
        }

        public static bool ParseToInt64(string s, Int64 defaultValue, out Int64 result)
        {
            bool retCode = Int64.TryParse(s, out result);
            if (retCode == false)
            {
                result = defaultValue;
                Log.ErrorFormat("Failed to ParseToInt64: {0}", s);
            }

            return retCode;
        }

        public static bool ParseToUint64(string s, UInt64 defaultValue, out UInt64 result)
        {
            bool retCode = UInt64.TryParse(s, out result);
            if (retCode == false)
            {
                result = defaultValue;
                Log.ErrorFormat("Failed to ParseToUint64: {0}", s);
            }

            return retCode;
        }
    }
}