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

        UNKnown,
    }

    public class BundleState
    {
        public const int INVALID_BUNDLE_ID = -1;
        // detail see BundleTypeTool
        public int bundleID = INVALID_BUNDLE_ID;
        public uint crc = 0;
        public int version = -1;
        public long size = -1;
        public BundleLoadState loadState = BundleLoadState.UnLoadOnUnloadAsset;
        public BundleStorePos storePos = BundleStorePos.UNKnown;
    }
}