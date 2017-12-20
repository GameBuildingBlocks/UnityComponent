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
    }
}