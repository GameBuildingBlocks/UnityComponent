using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace ResourceFormat
{
    public class ModelOverviewReport
    {
        [MenuItem(OverviewConfig.ModelReportMenu)]
        public static void GenerateRportByConfig()
        {
            List<ModelInfo> modelInfoList = ModelInfo.GetModelInfoByDirectory(OverviewConfig.RootPath);
            GenerateReport(OverviewConfig.ModelReportPath, modelInfoList);
        }

        public static void GenerateReport(string filePath, List<ModelInfo> modelInfoList)
        {
            UnityEngine.Debug.Log("Begin ModelOverviewReport Generate.");

            EditorCommon.EditorTool.CreateDirectory(filePath);

            FileStream fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

            sw.WriteLine("##Model Total Info Report");
            sw.WriteLine(GenerateReadWriteData(modelInfoList));
            sw.WriteLine(GenerateImportMaterialData(modelInfoList));
            sw.WriteLine(GenerateOptimizeMeshData(modelInfoList));
            sw.WriteLine(GenerateMeshVerticesData(modelInfoList));
            sw.WriteLine(GenerateMeshCompressData(modelInfoList));
            sw.WriteLine(GenerateMeshVertexCountData(modelInfoList));
            sw.WriteLine(GenerateMeshVertexCountData(modelInfoList));

            sw.Flush();
            sw.Close();

            UnityEngine.Debug.Log("End ModelOverviewReport Generate.");
        }

        private static string GenerateReadWriteData(List<ModelInfo> modelInfoList)
        {
            Dictionary<bool, KeyValuePair<int, long>> dict
                = new Dictionary<bool, KeyValuePair<int, long>>();

            for (int i = 0; i < modelInfoList.Count; ++i)
            {
                var key = modelInfoList[i].ReadWriteEnable;
                var value = modelInfoList[i].MemSize;

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

        private static string GenerateImportMaterialData(List<ModelInfo> modelInfoList)
        {
            Dictionary<bool, KeyValuePair<int, long>> dict
                = new Dictionary<bool, KeyValuePair<int, long>>();

            for (int i = 0; i < modelInfoList.Count; ++i)
            {
                var key = modelInfoList[i].ImportMaterials;
                var value = modelInfoList[i].MemSize;

                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new KeyValuePair<int, long>());
                }

                KeyValuePair<int, long> rwData = dict[key];
                dict[key] = new KeyValuePair<int, long>(rwData.Key + 1, rwData.Value + value);
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("####Import Material");
            sb.AppendLine("|Importer|Count|Size|");
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

        private static string GenerateOptimizeMeshData(List<ModelInfo> modelInfoList)
        {
            Dictionary<bool, KeyValuePair<int, long>> dict
                = new Dictionary<bool, KeyValuePair<int, long>>();

            for (int i = 0; i < modelInfoList.Count; ++i)
            {
                var key = modelInfoList[i].OptimizeMesh;
                var value = modelInfoList[i].MemSize;

                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new KeyValuePair<int, long>());
                }

                KeyValuePair<int, long> rwData = dict[key];
                dict[key] = new KeyValuePair<int, long>(rwData.Key + 1, rwData.Value + value);
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("####Optimize Mesh");
            sb.AppendLine("|optimizeMesh|Count|Size|");
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

        private static string GenerateMeshVerticesData(List<ModelInfo> modelInfoList)
        {
            Dictionary<int, KeyValuePair<int, long>> dict
                = new Dictionary<int, KeyValuePair<int, long>>();

            for (int i = 0; i < modelInfoList.Count; ++i)
            {
                ModelInfo mInfo = modelInfoList[i];
                int meshDataIndex = _WorkData(0, mInfo.bHasUV);
                meshDataIndex = _WorkData(0, mInfo.bHasUV2);
                meshDataIndex = _WorkData(meshDataIndex, mInfo.bHasUV3);
                meshDataIndex = _WorkData(meshDataIndex, mInfo.bHasUV4);
                meshDataIndex = _WorkData(meshDataIndex, mInfo.bHasColor);
                meshDataIndex = _WorkData(meshDataIndex, mInfo.bHasNormal);
                meshDataIndex = _WorkData(meshDataIndex, mInfo.bHasTangent);

                var key = meshDataIndex;
                var value = modelInfoList[i].MemSize;

                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new KeyValuePair<int, long>());
                }

                KeyValuePair<int, long> rwData = dict[key];
                dict[key] = new KeyValuePair<int, long>(rwData.Key + 1, rwData.Value + value);
            }

            List<KeyValuePair<int, KeyValuePair<int, long>>> list =
                new List<KeyValuePair<int, KeyValuePair<int, long>>>(dict);
            list.Sort((x, y) =>
            {
                return y.Value.Key.CompareTo(x.Value.Key);
            });

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("####Mesh Data");
            sb.AppendLine("|Data|Count|Size|");
            sb.AppendLine("|-|-|-|");
            bool[] bData = new bool[7];
            string[] strData = { "tangent", "normal", "color", "uv4", "uv3", "uv2", "uv" };
            foreach (var itor in list)
            {
                int key = itor.Key;
                for (int i = 0; i < 7; ++i) {
                    bData[i] = ((key >> i) & 1) > 0;
                }

                sb.Append("|vertices");
                for (int i = 6; i >= 0; --i)
                {
                    if (bData[i])
                    {
                        sb.Append("," + strData[i]);
                    }
                }
                sb.Append("|");

                sb.AppendFormat("{0}|{1}|", itor.Value.Key, EditorUtility.FormatBytes(itor.Value.Value));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string GenerateMeshCompressData(List<ModelInfo> modelInfoList)
        {
            Dictionary<ModelImporterMeshCompression, KeyValuePair<int, long>> dict
                = new Dictionary<ModelImporterMeshCompression, KeyValuePair<int, long>>();

            for (int i = 0; i < modelInfoList.Count; ++i)
            {
                var key = modelInfoList[i].MeshCompression;
                var value = modelInfoList[i].MemSize;

                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new KeyValuePair<int, long>());
                }

                KeyValuePair<int, long> rwData = dict[key];
                dict[key] = new KeyValuePair<int, long>(rwData.Key + 1, rwData.Value + value);
            }

            List<KeyValuePair<ModelImporterMeshCompression, KeyValuePair<int, long>>> list =
                new List<KeyValuePair<ModelImporterMeshCompression, KeyValuePair<int, long>>>(dict);
            list.Sort((x, y) =>
            {
                return y.Value.Value.CompareTo(x.Value.Value);
            });

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("####Mesh Compress");
            sb.AppendLine("|Compression|Count|Size|");
            sb.AppendLine("|-|-|-|");
            foreach (var itor in list)
            {
                sb.AppendFormat("|{0}|", itor.Key);
                sb.AppendFormat("{0}|{1}|", itor.Value.Key, EditorUtility.FormatBytes(itor.Value.Value));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string GenerateMeshVertexCountData(List<ModelInfo> modelInfoList)
        {
            int verTexMod = OverviewConfig.VertexCountMod;
            Dictionary<int, KeyValuePair<int, long>> dict
                = new Dictionary<int, KeyValuePair<int, long>>();

            for (int i = 0; i < modelInfoList.Count; ++i)
            {
                var key = modelInfoList[i].vertexCount / verTexMod;
                var value = modelInfoList[i].MemSize;

                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new KeyValuePair<int, long>());
                }

                KeyValuePair<int, long> rwData = dict[key];
                dict[key] = new KeyValuePair<int, long>(rwData.Key + 1, rwData.Value + value);
            }

            List<KeyValuePair<int, KeyValuePair<int, long>>> list =
                new List<KeyValuePair<int, KeyValuePair<int, long>>>(dict);
            list.Sort((x, y) =>
            {
                return y.Value.Value.CompareTo(x.Value.Value);
            });

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("####Vertex Count");
            sb.AppendLine("|VertexCount|Count|Size|");
            sb.AppendLine("|-|-|-|");
            foreach (var itor in list)
            {
                sb.AppendFormat("|{0}-{1}|", itor.Key * verTexMod, (itor.Key + 1) * verTexMod - 1);
                sb.AppendFormat("{0}|{1}|", itor.Value.Key, EditorUtility.FormatBytes(itor.Value.Value));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string GenerateMeshTriangleCountData(List<ModelInfo> modelInfoList)
        {
            int triTexMod = OverviewConfig.TriangleCountMod;
            Dictionary<int, KeyValuePair<int, long>> dict
                = new Dictionary<int, KeyValuePair<int, long>>();

            for (int i = 0; i < modelInfoList.Count; ++i)
            {
                var key = modelInfoList[i].triangleCount / triTexMod;
                var value = modelInfoList[i].MemSize;

                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, new KeyValuePair<int, long>());
                }

                KeyValuePair<int, long> rwData = dict[key];
                dict[key] = new KeyValuePair<int, long>(rwData.Key + 1, rwData.Value + value);
            }

            List<KeyValuePair<int, KeyValuePair<int, long>>> list =
                new List<KeyValuePair<int, KeyValuePair<int, long>>>(dict);
            list.Sort((x, y) =>
            {
                return y.Value.Value.CompareTo(x.Value.Value);
            });

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("####Triangle Count");
            sb.AppendLine("|TriangleCount|Count|Size|");
            sb.AppendLine("|-|-|-|");
            foreach (var itor in list)
            {
                sb.AppendFormat("|{0}-{1}|", itor.Key * triTexMod, (itor.Key + 1) * triTexMod - 1);
                sb.AppendFormat("{0}|{1}|", itor.Value.Key, EditorUtility.FormatBytes(itor.Value.Value));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static int _WorkData(int data, bool flag)
        {
            if (flag)
                return (data << 1) | 1;
            else
                return data << 1;
        }
    }
}