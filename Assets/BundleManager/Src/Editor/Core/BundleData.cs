using System.Collections.Generic;

namespace BundleManager
{
    public class BundleData
    {
        public string name = string.Empty;
        public string parent = string.Empty;
        public BundleType type = BundleType.None;
        public BundleLoadState loadState = BundleLoadState.UnLoadOnUnloadAsset;
        public long size = 0;
        public List<string> includs = new List<string>();
        public List<string> children = new List<string>();
    }

    public class BundleImportData
    {
        public string rootPath = string.Empty;
        public string regexName = string.Empty;
        public BundleType type = BundleType.None;
        public BundleLoadState loadState = BundleLoadState.UnLoadOnUnloadAsset;
        public int limitCount = -1;
        public int limitKBSize = -1;
        public int index = -1;
        public bool pushDependice = false;
    }
}