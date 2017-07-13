namespace EditorCommon
{
    public static class EditorConst
    {
        public static string[] TextureExts = { ".tga", ".png", ".jpg", ".tif", ".psd", ".exr" };
        public static string[] MaterialExts = { ".mat" };
        public static string[] ModelExts = { ".fbx", ".asset", ".obj" };
        public static string[] AnimationExts = { ".anim" };
        public static string[] MetaExts = { ".meta" };
        public static string[] ShaderExts = { ".shader" };
        public static string[] ScriptExts = { ".cs" };
        public static string[] PrefabExts = { ".prefab" };

        public static string PlatformAndroid = "Android";
        public static string PlatformIos = "iPhone";
        public static string PlatformStandalones = "Standalones";

        public static string EDITOR_ANICLIP_NAME = "__preview__Take 001";
        public static string[] EDITOR_CONTROL_NAMES = {"AnimatorStateMachine",
            "AnimatorStateTransition", "AnimatorState", "AnimatorTransition", "BlendTree" };
    }
}