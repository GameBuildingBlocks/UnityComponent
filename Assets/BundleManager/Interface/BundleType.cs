namespace BundleManager 
{
    public enum BundleType
    {
        None = 0,
        Script,         // .cs
        Shader,         // .shader or build-in shader with name
        Font,           // .ttf
        Texture,        // .tga, .png, .jpg, .tif, .psd, .exr
        Material,       // .mat
        Animation,      // .anim
        Controller,     // .controller
        FBX,            // .fbx
        TextAsset,      // .txt, .bytes
        Prefab,         // .prefab
        UnityMap,       // .unity

        Audio,          // .bnk, .wem for wwise
        Video,          // .mp4
    }

    public static class BundleTypeTool
    {
        public static int BUNDLE_ID_OFFSET = 16;

        public static int GetBundleID(BundleType bundleType, int count)
        {
            return (((int)bundleType) << BUNDLE_ID_OFFSET) + count;
        }

        public static BundleType GetBundleType(int bundleID)
        {
            return (BundleType)(bundleID >> BUNDLE_ID_OFFSET);
        }
    }
}