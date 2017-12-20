using System;
using System.Text;

namespace GameResource
{
    public static class ResourceHash
    {
        static ulong FNV_offset_basis = 0xcbf29ce484222325;
        static ulong FNV_prime = 1099511628211; //240 + 28 + 0xb3

        public static ulong FNVHashForPath(string str)
        {
            if (str == null) return 0;

            ulong hashCode = FNV_offset_basis;
            for (int i = 0; i < str.Length; ++i)
            {
                char ch = Char.ToLowerInvariant(str[i]);

                if (str[i] == '\\')
                {
                    ch = '/';
                }

                hashCode = hashCode * FNV_prime;
                hashCode = hashCode ^ ch;
            }

            return hashCode;
        }

        public static ulong FNVHashForPath(char[] str)
        {
            if (str == null) return 0;

            ulong hashCode = FNV_offset_basis;
            for (int i = 0; i < str.Length; ++i)
            {
                char ch = Char.ToLowerInvariant(str[i]);

                if (str[i] == '\\')
                {
                    ch = '/';
                }

                hashCode = hashCode * FNV_prime;
                hashCode = hashCode ^ ch;
            }

            return hashCode;
        }

        public static string GetUniquePath(string str)
        {
            ms_strBuilder.Length = 0;

            for (int i = 0; i < str.Length; ++i)
            {
                char ch = Char.ToLowerInvariant(str[i]);

                if (str[i] == '\\')
                {
                    ch = '/';
                }

                ms_strBuilder.Append(ch);
            }

            return string.Intern(ms_strBuilder.ToString());
        }

        private static StringBuilder ms_strBuilder = new StringBuilder();
    }
}