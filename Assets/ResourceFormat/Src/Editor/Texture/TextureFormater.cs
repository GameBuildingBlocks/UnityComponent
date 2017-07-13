using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorCommon;

namespace ResourceFormat
{
    public static class TextureFormater
    {
        public static void ApplyFormatToObject(TextureImportData data)
        {
            List<object> unFortmatObject = data.GetObjects(true);

            for (int i = 0; i < unFortmatObject.Count; ++i)
            {
                TextureInfo texInfo = unFortmatObject[i] as TextureInfo;
                string name = System.IO.Path.GetFileName(texInfo.Path);
                if (EditorUtility.DisplayCancelableProgressBar("设置贴图格式", name, (i * 1.0f) / unFortmatObject.Count))
                {
                    Debug.LogWarning("ApplyFormatTextureObject Stop.");
                    break;
                }
                if (texInfo == null) continue;
                TextureImporter tImporter = AssetImporter.GetAtPath(texInfo.Path) as TextureImporter;
                if (tImporter == null) continue;
                if (tImporter.textureType != data.TexType)
                {
                    tImporter.textureType = data.TexType;
                }
                if (tImporter.textureShape != data.ShapeType)
                {
                    tImporter.textureShape = data.ShapeType;
                }
                tImporter.isReadable = data.ReadWriteEnable;
                tImporter.mipmapEnabled = data.MipmapEnable;

                if (data.MaxSize > 0)
                {
                    tImporter.maxTextureSize = data.MaxSize;
                }

                TextureImporterPlatformSettings settingAndroid = tImporter.GetPlatformTextureSettings(EditorConst.PlatformAndroid);
                settingAndroid.overridden = true;
                settingAndroid.format = data.GetFormatByAlphaMode(data.AndroidFormat, tImporter);
                settingAndroid.maxTextureSize = tImporter.maxTextureSize;
                tImporter.SetPlatformTextureSettings(settingAndroid);

                TextureImporterPlatformSettings settingIos = tImporter.GetPlatformTextureSettings(EditorConst.PlatformIos);
                settingIos.overridden = true;
                settingIos.format = data.GetFormatByAlphaMode(data.IosFormat, tImporter);
                settingIos.maxTextureSize = tImporter.maxTextureSize;
                tImporter.SetPlatformTextureSettings(settingIos);

                tImporter.SaveAndReimport();
            }
            EditorUtility.ClearProgressBar();

            for (int i = 0; i < unFortmatObject.Count; ++i)
            {
                TextureInfo texInfo = unFortmatObject[i] as TextureInfo;
                string name = System.IO.Path.GetFileName(texInfo.Path);
                EditorUtility.DisplayProgressBar("更新贴图数据", name, (i * 1.0f) / unFortmatObject.Count);
                TextureInfo.CreateTextureInfo(texInfo.Path);
            }
            EditorUtility.ClearProgressBar();
        }
    }
}