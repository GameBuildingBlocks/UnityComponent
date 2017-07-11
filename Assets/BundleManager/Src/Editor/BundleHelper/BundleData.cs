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
        // includs contain asset path or some buid-in resource with name.
        public List<string> includs = new List<string>();
        public List<string> children = new List<string>();
    }
}