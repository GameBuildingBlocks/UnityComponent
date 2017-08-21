using EditorCommon;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
        public int AndroidSize;
        public int IosSize;
        public int Width;
        public int Height;

        public static TextureInfo CreateTextureInfo(string assetPath)
        {
            if (!EditorPath.IsTexture(assetPath))
            {
                return null;
            }

            TextureInfo tInfo = null;
            if (!m_dictTexInfo.TryGetValue(assetPath, out tInfo))
            {
                tInfo = new TextureInfo();
                m_dictTexInfo.Add(assetPath, tInfo);
            }
            TextureImporter tImport = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
            if (tImport == null || texture == null)
                return null;

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
            tInfo.Width = texture.width;
            tInfo.Height = texture.height;
            tInfo.AndroidSize = EditorTool.CalculateTextureSizeBytes(texture, tInfo.AndroidFormat);
            tInfo.IosSize = EditorTool.CalculateTextureSizeBytes(texture, tInfo.IosFormat);
            tInfo.MemSize = Mathf.Max(tInfo.AndroidSize, tInfo.IosSize);

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

        public static List<TextureInfo> GetTextureInfoByDirectory(string dir)
        {
            List<TextureInfo> texInfoList = new List<TextureInfo>();
            List<string> list = new List<string>();
            EditorPath.ScanDirectoryFile(dir, true, list);
            for (int i = 0; i < list.Count; ++i)
            {
                string assetPath = EditorPath.FormatAssetPath(list[i]);
                TextureInfo texInfo = CreateTextureInfo(assetPath);
                if (texInfo != null)
                {
                    texInfoList.Add(texInfo);
                }
            }

            return texInfoList;
        }

        private static int m_loadCount = 0;
        private static Dictionary<string, TextureInfo> m_dictTexInfo = new Dictionary<string, TextureInfo>();
    }
}
