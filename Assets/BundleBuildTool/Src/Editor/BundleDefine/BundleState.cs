namespace BundleManager
{
    public enum BundleLoadState
    {
        Preload,                // load at start and nerver unload
        NerverUnload,           // when loaded nerver unload
        UnloadOnUnloadAsset,    // a depend asset can only unload at Resources.UnloadUnusedAssets 
        UnloadImmediately,      // a no depend asset can unload immediately, for a fast load we cache limit count of this.
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
        public BundleLoadState loadState = BundleLoadState.UnloadOnUnloadAsset;
        public BundleStorePos storePos = BundleStorePos.Building;
    }
}