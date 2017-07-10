namespace BundleManager
{
    public enum BundleLoadState
    {
        PreLoad,                // load at start and nerver unload
        NerverUnLoad,           // when loaded nerver unload
        UnLoadOnUnloadAsset,    // a depend asset can only unload at Resources.UnloadUnusedAssets 
        UnLoadImmediately,      // a no depend asset can unload immediately, for a fast load we cache limit count of this.
    }

    public enum BundleStorePos
    {
        StreamingAssets,
        PersistentDataPath,

        Building,
    }

    public class BundleState
    {
        public string bundleID = string.Empty;
        public uint crc = 0;
        public uint compressCrc = 0;
        public int version = -1;
        public long size = -1;
        public BundleLoadState loadState = BundleLoadState.UnLoadOnUnloadAsset;
        public BundleStorePos storePos = BundleStorePos.Building;
    }
}