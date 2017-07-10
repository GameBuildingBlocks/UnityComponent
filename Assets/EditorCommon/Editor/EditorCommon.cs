using UnityEditor;
using UnityEngine;
using System.IO;
using LitJson;
using System.Collections.Generic;

public static class EditorCommon {
    public static List<object> ToObjectList<T>(List<T> data) {
        if (data == null) return null;
        List<object> ret = new List<object>();
        for (int i = 0; i < data.Count; ++i) {
            ret.Add(data[i]);
        }
        return ret;
    }

    public static string GetCurrentBuildPlatform() {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) {
            return EditorConst.PlatformAndroid;
        } else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS) {
            return EditorConst.PlatformIos;
        } else {
            return EditorConst.PlatformAndroid;
        }
    }

    public static int GetBitsPerPixel(TextureImporterFormat format) {
        switch (format) {
        case TextureImporterFormat.Alpha8: //	 Alpha-only texture format.
            return 8;
        case TextureImporterFormat.RGB24: // A color texture format.
            return 24;
        case TextureImporterFormat.RGBA32: //Color with an alpha channel texture format.
            return 32;
        case TextureImporterFormat.ARGB32: //Color with an alpha channel texture format.
            return 32;
        case TextureImporterFormat.DXT1: // Compressed color texture format.
            return 4;
        case TextureImporterFormat.DXT5: // Compressed color with alpha channel texture format.
            return 8;
        case TextureImporterFormat.PVRTC_RGB2: //	 PowerVR (iOS) 2 bits/pixel compressed color texture format.
            return 2;
        case TextureImporterFormat.PVRTC_RGBA2: //	 PowerVR (iOS) 2 bits/pixel compressed with alpha channel texture format
            return 2;
        case TextureImporterFormat.PVRTC_RGB4: //	 PowerVR (iOS) 4 bits/pixel compressed color texture format.
            return 4;
        case TextureImporterFormat.PVRTC_RGBA4: //	 PowerVR (iOS) 4 bits/pixel compressed with alpha channel texture format
            return 4;
        case TextureImporterFormat.ETC_RGB4: //	 ETC (GLES2.0) 4 bits/pixel compressed RGB texture format.
            return 4;
        case TextureImporterFormat.ETC2_RGB4:
            return 4;
        case TextureImporterFormat.ETC2_RGBA8:
            return 8;
        case TextureImporterFormat.ATC_RGB4: //	 ATC (ATITC) 4 bits/pixel compressed RGB texture format.
            return 4;
        case TextureImporterFormat.ATC_RGBA8: //	 ATC (ATITC) 8 bits/pixel compressed RGB texture format.
            return 8;
#pragma warning disable 0618
        case TextureImporterFormat.AutomaticCompressed:
            return 4;
        case TextureImporterFormat.AutomaticTruecolor:
            return 32;
        default:
            return 32;
#pragma warning restore 0618
        }
    }

    public static int CalculateTextureSizeBytes(Texture tTexture, TextureImporterFormat format) {
        var tWidth = tTexture.width;
        var tHeight = tTexture.height;
        if (tTexture is Texture2D) {
            var tTex2D = tTexture as Texture2D;
            var bitsPerPixel = GetBitsPerPixel(format);
            var mipMapCount = tTex2D.mipmapCount;
            var mipLevel = 1;
            var tSize = 0;
            while (mipLevel <= mipMapCount) {
                tSize += tWidth * tHeight * bitsPerPixel / 8;
                tWidth = tWidth / 2;
                tHeight = tHeight / 2;
                mipLevel++;
            }
            return tSize;
        }

        if (tTexture is Cubemap) {
            var bitsPerPixel = GetBitsPerPixel(format);
            return tWidth * tHeight * 6 * bitsPerPixel / 8;
        }
        return 0;
    }

    public static int CalculateTextureSizeBytes(string path) {
        TextureImporter tImport = AssetImporter.GetAtPath(path) as TextureImporter;
        Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(path);
        if (tImport == null || texture == null) return 0;

        TextureImporterPlatformSettings setting = tImport.GetPlatformTextureSettings(GetCurrentBuildPlatform());

        int retSize = 0;
        if (!setting.overridden) {
#pragma warning disable 0618
            retSize = CalculateTextureSizeBytes(texture, tImport.textureFormat);
#pragma warning restore 0618
        } else {
            retSize = CalculateTextureSizeBytes(texture, setting.format);
        }

        Resources.UnloadAsset(texture);

        return retSize;
    }

    public static int CalculateModelSizeBytes(string path) {
        int size = 0;
        Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
        for (int i = 0; i < objs.Length; ++i) {
            if (objs[i] is Mesh) {
#pragma warning disable 0618
                size += UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(objs[i]);
#pragma warning restore 0618
            }
            if ((!(objs[i] is GameObject)) && (!(objs[i] is Component))) {
                Resources.UnloadAsset(objs[i]);
            }
        }
        return size;
    }

    public static int CalculateAnimationSizeBytes(string path) {
        int size = 0;
        Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
        for (int i = 0; i < objs.Length; ++i) {
            if ((objs[i] is AnimationClip) && objs[i].name != EditorConst.EDITOR_ANICLIP_NAME) {
#pragma warning disable 0618
                size += UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(objs[i]);
#pragma warning restore 0618
            }
            if ((!(objs[i] is GameObject)) && (!(objs[i] is Component))) {
                Resources.UnloadAsset(objs[i]);
            }
        }
        return size;
    }

    public static void CreateDirectory(string path) {
        if (string.IsNullOrEmpty(path)) return;
        string dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
    }

    public static T LoadJsonData<T>(string path) {
        try {
            if (!File.Exists(path)) {
                return default(T);
            }
            string str = File.ReadAllText(path);
            if (string.IsNullOrEmpty(str)) {
                return default(T);
            }
            T data = JsonMapper.ToObject<T>(str);
            if (data == null) {
                Debug.LogError("Cannot read json data from " + path);
            }

            return data;
        } catch (System.Exception e) {
            Debug.LogException(e);
            return default(T);
        }
    }

    public static void SaveJsonData<T>(T data, string path) {
        CreateDirectory(path);

        string jsonStr = JsonFormatter.PrettyPrint(JsonMapper.ToJson(data));
        File.WriteAllText(path, jsonStr);
    }

}