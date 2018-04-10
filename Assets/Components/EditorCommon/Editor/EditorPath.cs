using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using LitJson;

namespace EditorCommon
{
    public static class EditorPath
    {
        public static string FormatAssetPath(string path)
        {
            int index = path.IndexOf("Assets");
            if (index != -1)
            {
                path = path.Substring(index);
            }
            return NormalizePathSplash(path);
        }

        public static bool IsTexture(string path)
        {
            return PathEndWithExt(path, EditorConst.TextureExts);
        }

        public static bool IsMaterial(string path)
        {
            return PathEndWithExt(path, EditorConst.MaterialExts);
        }

        public static bool IsModel(string path)
        {
            return PathEndWithExt(path, EditorConst.ModelExts);
        }

        public static bool IsMeta(string path)
        {
            return PathEndWithExt(path, EditorConst.MetaExts);
        }

        public static bool IsShader(string path)
        {
            return PathEndWithExt(path, EditorConst.ShaderExts);
        }

        public static bool IsScript(string path)
        {
            return PathEndWithExt(path, EditorConst.ScriptExts);
        }

        public static bool IsAnimation(string path)
        {
            if (PathEndWithExt(path, EditorConst.ModelExts))
            {
                string assetPath = FormatAssetPath(path);
                ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                if (modelImporter != null && modelImporter.importAnimation)
                {
                    return true;
                }
                return false;
            }
            return PathEndWithExt(path, EditorConst.AnimationExts);
        }

        public static void ScanDirectoryFile(string root, bool deep, List<string> list)
        {
            if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
            {
                Debug.LogWarning("scan directory file failed! " + root);
                return;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(root);
            FileInfo[] files = dirInfo.GetFiles("*.*");
            for (int i = 0; i < files.Length; ++i)
            {
                list.Add(files[i].FullName);
            }

            if (deep)
            {
                DirectoryInfo[] dirs = dirInfo.GetDirectories("*.*");
                for (int i = 0; i < dirs.Length; ++i)
                {
                    ScanDirectoryFile(dirs[i].FullName, deep, list);
                }
            }
        }

        public static List<string> GetAssetPathList(string rootPath)
        {
            List<string> ret = new List<string>();
            ScanDirectoryFile(rootPath, true, ret);

            for (int i = 0; i < ret.Count; ++i)
            {
                ret[i] = FormatAssetPath(ret[i]);
            }

            return ret;
        }

        public static string NormalizePathSplash(string path)
        {
            return path.Replace('\\', '/');
        }

        public static bool PathEndWithExt(string path, string[] ext)
        {
            for (int i = 0; i < ext.Length; ++i)
            {
                if (path.EndsWith(ext[i], System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}