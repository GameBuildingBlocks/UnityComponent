using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EditorCommon;

namespace ResourceFormat
{
    public enum TextureType
    {
        Default,
        NormalMap,
        Lightmap
    }

    public enum TextureAlphaMode
    {
        FormTexture,
        None,
    }

    public class TextureInfo : BaseInfo
    {
        public bool ReadWriteEnable = false;
        public bool MipmapEnable = false;
        public TextureImporterFormat AndroidFormat;
        public TextureImporterFormat IosFormat;
        public TextureImporterType ImportType;
        public TextureWrapMode WrapMode;
        public FilterMode FilterMode;
        public TextureImporterShape ImportShape;

        public static TextureInfo CreateTextureInfo(string assetPath)
        {
            TextureInfo tInfo = null;
            if (!m_dictTexInfo.TryGetValue(assetPath, out tInfo))
            {
                tInfo = new TextureInfo();
                m_dictTexInfo.Add(assetPath, tInfo);
            }
            TextureImporter tImport = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
            if (tImport == null || texture == null) return null;

            tInfo.Path = tImport.assetPath;
            tInfo.ImportType = tImport.textureType;
            tInfo.ImportShape = tImport.textureShape;
            tInfo.ReadWriteEnable = tImport.isReadable;
            tInfo.MipmapEnable = tImport.mipmapEnabled;
            tInfo.WrapMode = tImport.wrapMode;
            tInfo.FilterMode = tImport.filterMode;
            TextureImporterPlatformSettings settingAndroid = tImport.GetPlatformTextureSettings(EditorConst.PlatformAndroid);
            tInfo.AndroidFormat = settingAndroid.format;
            TextureImporterPlatformSettings settingIos = tImport.GetPlatformTextureSettings(EditorConst.PlatformIos);
            tInfo.IosFormat = settingIos.format;
            tInfo.MemSize = Mathf.Max(
                EditorTool.CalculateTextureSizeBytes(texture, tInfo.AndroidFormat),
                EditorTool.CalculateTextureSizeBytes(texture, tInfo.IosFormat));

            if (Selection.activeObject != texture)
            {
                Resources.UnloadAsset(texture);
            }

            if (++m_loadCount % 256 == 0)
            {
                Resources.UnloadUnusedAssets();
            }

            return tInfo;
        }

        private static int m_loadCount = 0;
        private static Dictionary<string, TextureInfo> m_dictTexInfo = new Dictionary<string, TextureInfo>();
    }
}