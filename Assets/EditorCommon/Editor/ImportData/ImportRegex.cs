using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ResourceFormat {
    public static class ImportRegex {
        public static bool IsMatch(ImportData data, string path) {
            int index = path.IndexOf(RFConfig.ResourceRootPath);
            if (index != -1) {
                path = path.Substring(index + RFConfig.ResourceRootPath.Length);
            }
            string packagePath = PathConfig.NormalizePathSplash(path);
            if (packagePath.StartsWith("/")) {
                packagePath = packagePath.Substring(1);
            }
            string formatPath = PathConfig.NormalizePathSplash(data.RootPath);
            if (!string.IsNullOrEmpty(formatPath) &&
                !packagePath.StartsWith(formatPath, System.StringComparison.OrdinalIgnoreCase))
                return false;
            MyRegex regex = MyRegex.Create(data.FileNameMatch);
            if (regex == null) return false;
            return regex.IsMatch(packagePath);
        }
    }
    
    public class MyRegex {
        public bool IsMatch(string path) {
            for (int i = 0; i < m_regexs.Count; ++i) {
                if (m_regexs[i].IsMatch(path)) return true;
            }
            return false;
        }

        private List<Regex> m_regexs = new List<Regex>();

        public static MyRegex Create(string str) {
            MyRegex ret = null;
            if (ms_dict.TryGetValue(str, out ret)) {
                return ret;
            }
            ret = new MyRegex();

            string pattern = BuildPatternString(str);
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            ret.m_regexs.Add(regex);

            ms_dict.Add(str, ret);
            return ret;
        }
        private static Dictionary<string, MyRegex> ms_dict = new Dictionary<string, MyRegex>();

        public static string BuildPatternString(string match) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < match.Length; ++i) {
                if (match[i] == '*') {
                    sb.Append('.');
                } else if (IsMatchRegexChar(match[i])) {
                    sb.Append('\\');
                }
                sb.Append(match[i]);
            }
            return sb.ToString();
        }

        public static bool IsMatchRegexChar(char ch) {
            return ch == '\\' || ch == '^' || ch == '$' || ch == '*' ||
                ch == '+' || ch == '?' || ch == '.';
        }
    }
}