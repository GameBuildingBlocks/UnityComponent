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
            EditorRegex regex = EditorRegex.Create(data.FileNameMatch);
            if (regex == null) return false;
            return regex.IsMatch(packagePath);
        }
    }

}