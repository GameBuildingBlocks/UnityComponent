using System.Collections.Generic;
using UnityEditor;

namespace ResourceFormat {
    public class TextureImportData : ImportData {
        public int TextureTypeIndex {
            get { return ArrayUtility.IndexOf<TextureImporterType>(TableConst.ImporterType, TexType); }
            set { TexType = TableConst.ImporterType[value]; }
        }
        public int TextureShapeIndex {
            get { return ArrayUtility.IndexOf<TextureImporterShape>(TableConst.ImporterShape, ShapeType); }
            set { ShapeType = TableConst.ImporterShape[value]; }
        }
        public int AndroidFormatIndex {
            get { return ArrayUtility.IndexOf<TextureImporterFormat>(TableConst.AndroidImporterFormat, AndroidFormat); }
            set { AndroidFormat = TableConst.AndroidImporterFormat[value];}
        }
        public int IosFormatIndex {
            get { return ArrayUtility.IndexOf<TextureImporterFormat>(TableConst.IosImporterFormat, IosFormat); }
            set { IosFormat = TableConst.IosImporterFormat[value]; }
        }

        public TextureAlphaMode AlphaMode = TextureAlphaMode.FormTexture;
        public TextureImporterType TexType = TextureImporterType.Default;
        public TextureImporterShape ShapeType = TextureImporterShape.Texture2D;
        public TextureImporterFormat AndroidFormat = TextureImporterFormat.ETC2_RGB4;
        public TextureImporterFormat IosFormat = TextureImporterFormat.PVRTC_RGB4;
        public bool ReadWriteEnable = false;
        public bool MipmapEnable = false;
        public int MaxSize = -1;

        public override bool IsMatch(string path) {
            bool pathMatch = PathConfig.IsTexture(path) && base.IsMatch(path);
            if (!pathMatch) return false;
            TextureImporter texureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
#pragma warning disable 0618
            if (TexType == TextureImporterType.Cubemap) {
#pragma warning restore 0618
                if (texureImporter.textureShape != TextureImporterShape.TextureCube) return false;
                else return true;
            } else {
                if (texureImporter.textureShape == TextureImporterShape.TextureCube) return false;
            }
            return texureImporter.textureType == TexType;
        }
        public override void CopyData(ImportData data) {
            TextureImportData tData = data as TextureImportData;
            if (tData == null) return;

            base.CopyData(data);
            AlphaMode = tData.AlphaMode;
            TexType = tData.TexType;
            AndroidFormat = tData.AndroidFormat;
            IosFormat = tData.IosFormat;
            ReadWriteEnable = tData.ReadWriteEnable;
            MipmapEnable = tData.MipmapEnable;
            MaxSize = tData.MaxSize;
        }
        public override void ClearObject() {
            m_initUnFormatList = false;
            base.ClearObject();
        }
        public void AddObject(TextureInfo textureInfo) {
            TotalCount = TotalCount + 1;
            TotalMemuse = TotalMemuse + textureInfo.MemSize;
            m_objects.Add(textureInfo);
            if (m_initUnFormatList && IsFormatTexture(textureInfo)) {
                m_unFortmatObjects.Add(textureInfo);
            }
        }
        public List<object> GetObjects(bool unformat) {
            if (!unformat) {
                return m_objects;
            } else {
                if (!m_initUnFormatList) {
                    m_initUnFormatList = true;
                    _InitUnFormatList();
                }
                return m_unFortmatObjects;
            }
        }
        public TextureImporterFormat GetFormatByAlphaMode(TextureImporterFormat format, TextureImporter tImporter) {
            if (AlphaMode == TextureAlphaMode.None || tImporter.alphaSource == TextureImporterAlphaSource.None
                || !tImporter.DoesSourceTextureHaveAlpha()) {
                return format;
            } else {
                if (format == TextureImporterFormat.ETC_RGB4 || format == TextureImporterFormat.ETC2_RGB4)
                    return TextureImporterFormat.ETC2_RGBA8;
                if (format == TextureImporterFormat.PVRTC_RGB4)
                    return TextureImporterFormat.PVRTC_RGBA4;
                if (format == TextureImporterFormat.RGB24)
                    return TextureImporterFormat.RGBA32;
                return format;
            }
        }
        public bool IsFormatTexture(TextureInfo tInfo) {
            TextureImporter tImporter = AssetImporter.GetAtPath(tInfo.Path) as TextureImporter;
            if (tImporter == null) return false;
            if (tImporter.isReadable != ReadWriteEnable) return false;
            if (tImporter.mipmapEnabled != MipmapEnable) return false;
            if (tImporter.textureType != TexType) return false;
            TextureImporterPlatformSettings settingAndroid = tImporter.GetPlatformTextureSettings(EditorConst.PlatformAndroid);
            if (!settingAndroid.overridden || settingAndroid.format != GetFormatByAlphaMode(AndroidFormat, tImporter)) return false;
            TextureImporterPlatformSettings settingIos = tImporter.GetPlatformTextureSettings(EditorConst.PlatformIos);
            if (!settingIos.overridden || settingIos.format != GetFormatByAlphaMode(IosFormat, tImporter)) return false;
            if (MaxSize != -1 && tImporter.maxTextureSize != MaxSize) return false;
            if (tImporter.maxTextureSize != settingAndroid.maxTextureSize) return false;
            if (tImporter.maxTextureSize != settingIos.maxTextureSize) return false;
            return true;
        }
        private void _InitUnFormatList() {
            for (int i = 0; i < m_objects.Count; ++i) {
                TextureInfo texInfo = m_objects[i] as TextureInfo;
                if (texInfo == null) continue;
                string name = System.IO.Path.GetFileName(texInfo.Path);
                EditorUtility.DisplayProgressBar("更新非法贴图数据", name, (i * 1.0f) / m_objects.Count);
                if (!IsFormatTexture(texInfo)) {
                    m_unFortmatObjects.Add(texInfo);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private bool m_initUnFormatList = false;
    }
}