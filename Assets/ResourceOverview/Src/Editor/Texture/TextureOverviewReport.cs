using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace ResourceFormat
{
    public class TextureOverviewReport
    {
        [MenuItem(OverviewConfig.TextureReportMenu)]
        public static void GenerateRportByConfig()
        {
            List<TextureInfo> texInfoList = TextureInfo.GetTextureInfoByDirectory(OverviewConfig.RootPath);
            GenerateReport(OverviewConfig.TextureReportPath, texInfoList);
        }

        public static void GenerateReport(string filePath, List<TextureInfo> texInfoList)
        {
            UnityEngine.Debug.Log("Begin TextureOverviewReport Generate.");

            EditorCommon.EditorTool.CreateDirectory(filePath);

            FileStream fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

            sw.WriteLine("##Texture Total Info Report");
            sw.WriteLine(GenerateReadWirteData(texInfoList));
            sw.WriteLine(GenerateMipMapData(texInfoList));
            sw.WriteLine(GenerateTextureTypeData(texInfoList));
            sw.WriteLine(GenerateTextureSizeData(texInfoList));
            sw.WriteLine(GenerateTextureDiffSizeData(texInfoList));
            sw.WriteLine(GenerateTextureAndroidFormatData(texInfoList));
            sw.WriteLine(GenerateTextureiOSFormatData(texInfoList));

            sw.Flush();
            sw.Close();

            UnityEngine.Debug.Log("End TextureOverviewReport Generate.");
        }

        private static string GenerateReadWirteData(List<TextureInfo> texInfoList)
        {
            Dictionary<bool, KeyValuePair<int, long>> dict
                = new Dictionary<bool, KeyValuePair<int, long>>();

            for (int i = 0; i < texInfoList.Count; ++i)
            {
                var key = texInfoList[i].ReadWriteEnable;
                var value = texInfoList[i].MemSize;

                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new KeyValuePair<int, long>());
                }

                KeyValuePair<int, long> rwData = dict[key];
                dict[key] = new KeyValuePair<int, long>(rwData.Key + 1, rwData.Value + value);
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("####Read/Write Enable");
            sb.AppendLine("|Read/Write Enable|Count|Size|");
            sb.AppendLine("|-|-|-|");
            for (int i = 0; i < 2; ++i)
            {
                KeyValuePair<int, long> itor;
                if (!dict.TryGetValue(i == 1, out itor))
                    continue;
                sb.Append(i == 1 ? "|True|" : "|False|");
                sb.AppendFormat("{0}|{1}|", itor.Key, EditorUtility.FormatBytes(itor.Value));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string GenerateMipMapData(List<TextureInfo> texInfoList)
        {
            Dictionary<bool, KeyValuePair<int, long>> dict
                = new Dictionary<bool, KeyValuePair<int, long>>();

            for (int i = 0; i < texInfoList.Count; ++i)
            {
                var key = texInfoList[i].MipmapEnable;
                var value = texInfoList[i].MemSize;

                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new KeyValuePair<int, long>());
                }

                KeyValuePair<int, long> rwData = dict[key];
                dict[key] = new KeyValuePair<int, long>(rwData.Key + 1, rwData.Value + value);
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("####MipMap Enable");
            sb.AppendLine("|MipMap Enable|Count|Size|");
            sb.AppendLine("|-|-|-|");
            for (int i = 0; i < 2; ++i)
            {
                KeyValuePair<int, long> itor;
                if (!dict.TryGetValue(i == 1, out itor))
                    continue;
                sb.Append(i == 1 ? "|True|" : "|False|");
                sb.AppendFormat("{0}|{1}|", itor.Key, EditorUtility.FormatBytes(itor.Value));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string GenerateTextureTypeData(List<TextureInfo> texInfoList)
        {
            Dictionary<TextureImporterType, KeyValuePair<int, long>> dict
                = new Dictionary<TextureImporterType, KeyValuePair<int, long>>();

            for (int i = 0; i < texInfoList.Count; ++i)
            {
                var key = texInfoList[i].ImportType;
                var value = texInfoList[i].MemSize;

                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new KeyValuePair<int, long>());
                }

                KeyValuePair<int, long> rwData = dict[key];
                dict[key] = new KeyValuePair<int, long>(rwData.Key + 1, rwData.Value + value);
            }

            StringBuilder sb = new StringBuilder();

            List<KeyValuePair<TextureImporterType, KeyValuePair<int, long>>> list
                = new List<KeyValuePair<TextureImporterType, KeyValuePair<int, long>>>(dict);
            list.Sort((x, y) =>
            {
                return y.Value.Value.CompareTo(x.Value.Value);
            });

            sb.AppendLine("####Texture Type");
            sb.AppendLine("|Type|Count|Size|");
            sb.AppendLine("|-|-|-|");
            foreach (var itor in list)
            {
                sb.AppendFormat("|{0}|", OverviewTableConst.TextureTypeStr[(int)itor.Key]);
                sb.AppendFormat("{0}|{1}|", itor.Value.Key, EditorUtility.FormatBytes(itor.Value.Value));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string GenerateTextureSizeData(List<TextureInfo> texInfoList)
        {
            Dictionary<int, KeyValuePair<int, long>> dict
                = new Dictionary<int, KeyValuePair<int, long>>();

            for (int i = 0; i < texInfoList.Count; ++i)
            {
                TextureInfo texInfo = texInfoList[i];
                var key = OverviewTableConst.GetTextureSizeIndex(texInfo.Width, texInfo.Height);
                var value = texInfoList[i].MemSize;

                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new KeyValuePair<int, long>());
                }

                KeyValuePair<int, long> rwData = dict[key];
                dict[key] = new KeyValuePair<int, long>(rwData.Key + 1, rwData.Value + value);
            }

            StringBuilder sb = new StringBuilder();

            List<KeyValuePair<int, KeyValuePair<int, long>>> list
                = new List<KeyValuePair<int, KeyValuePair<int, long>>>(dict);
            list.Sort((x, y) =>
            {
                return x.Key.CompareTo(y.Key);
            });

            sb.AppendLine("####Texture Size");
            sb.AppendLine("|Range|Count|Size|");
            sb.AppendLine("|-|-|-|");
            foreach (var itor in list)
            {
                sb.AppendFormat("|{0}|", OverviewTableConst.TextureSizeStr[(int)itor.Key]);
                sb.AppendFormat("{0}|{1}|", itor.Value.Key, EditorUtility.FormatBytes(itor.Value.Value));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string GenerateTextureDiffSizeData(List<TextureInfo> texInfoList)
        {
            Dictionary<bool, KeyValuePair<int, long>> dict
                = new Dictionary<bool, KeyValuePair<int, long>>();

            for (int i = 0; i < texInfoList.Count; ++i)
            {
                var key = texInfoList[i].Width == texInfoList[i].Height;
                var value = texInfoList[i].MemSize;

                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new KeyValuePair<int, long>());
                }

                KeyValuePair<int, long> rwData = dict[key];
                dict[key] = new KeyValuePair<int, long>(rwData.Key + 1, rwData.Value + value);
            }

            StringBuilder sb = new StringBuilder();

            List<KeyValuePair<bool, KeyValuePair<int, long>>> list
                = new List<KeyValuePair<bool, KeyValuePair<int, long>>>(dict);
            list.Sort((x, y) =>
            {
                return y.Value.Value.CompareTo(x.Value.Value);
            });

            sb.AppendLine("####Width VS Height");
            sb.AppendLine("|Width VS Height|Count|Size|");
            sb.AppendLine("|-|-|-|");
            foreach (var itor in list)
            {
                sb.AppendFormat("|{0}|", itor.Key ? "Width == Height" : "Width != Height");
                sb.AppendFormat("{0}|{1}|", itor.Value.Key, EditorUtility.FormatBytes(itor.Value.Value));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string GenerateTextureAndroidFormatData(List<TextureInfo> texInfoList)
        {
            Dictionary<TextureImporterFormat, KeyValuePair<int, long>> dict
                = new Dictionary<TextureImporterFormat, KeyValuePair<int, long>>();

            for (int i = 0; i < texInfoList.Count; ++i)
            {
                var key = texInfoList[i].AndroidFormat;
                var value = texInfoList[i].MemSize;

                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new KeyValuePair<int, long>());
                }

                KeyValuePair<int, long> rwData = dict[key];
                dict[key] = new KeyValuePair<int, long>(rwData.Key + 1, rwData.Value + value);
            }

            StringBuilder sb = new StringBuilder();

            List<KeyValuePair<TextureImporterFormat, KeyValuePair<int, long>>> list
                = new List<KeyValuePair<TextureImporterFormat, KeyValuePair<int, long>>>(dict);
            list.Sort((x, y) =>
            {
                return y.Value.Value.CompareTo(x.Value.Value);
            });

            sb.AppendLine("####AndroidFormat");
            sb.AppendLine("|Format|Count|Size|");
            sb.AppendLine("|-|-|-|");
            foreach (var itor in list)
            {
                sb.AppendFormat("|{0}|", itor.Key);
                sb.AppendFormat("{0}|{1}|", itor.Value.Key, EditorUtility.FormatBytes(itor.Value.Value));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string GenerateTextureiOSFormatData(List<TextureInfo> texInfoList)
        {
            Dictionary<TextureImporterFormat, KeyValuePair<int, long>> dict
                = new Dictionary<TextureImporterFormat, KeyValuePair<int, long>>();

            for (int i = 0; i < texInfoList.Count; ++i)
            {
                var key = texInfoList[i].IosFormat;
                var value = texInfoList[i].MemSize;

                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new KeyValuePair<int, long>());
                }

                KeyValuePair<int, long> rwData = dict[key];
                dict[key] = new KeyValuePair<int, long>(rwData.Key + 1, rwData.Value + value);
            }

            StringBuilder sb = new StringBuilder();

            List<KeyValuePair<TextureImporterFormat, KeyValuePair<int, long>>> list
                = new List<KeyValuePair<TextureImporterFormat, KeyValuePair<int, long>>>(dict);
            list.Sort((x, y) =>
            {
                return y.Value.Value.CompareTo(x.Value.Value);
            });

            sb.AppendLine("####iOSFormat");
            sb.AppendLine("|Format|Count|Size|");
            sb.AppendLine("|-|-|-|");
            foreach (var itor in list)
            {
                sb.AppendFormat("|{0}|", itor.Key);
                sb.AppendFormat("{0}|{1}|", itor.Value.Key, EditorUtility.FormatBytes(itor.Value.Value));
                sb.AppendLine();
            }

            return sb.ToString();
        }

    }

}
