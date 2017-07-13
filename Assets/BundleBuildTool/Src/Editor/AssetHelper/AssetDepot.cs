using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace BundleManager
{
    public static class AssetDepot
    {
        public static string[] GetDependenciesCache(string assetPath)
        {
            string[] files = { assetPath };
            return GetDependenciesCache(files);
        }

        public static string[] GetDependenciesCache(string[] files)
        {
            List<string> ret = new List<string>();
            for (int i = 0; i < files.Length; ++i)
            {
                DependData data = null;
                m_dict.TryGetValue(files[i], out data);

                if (data == null)
                {
                    string[] deps = GetDependencies(files[i]);
                    data = new DependData();
                    data.assetPath = files[i];
                    data.dependPath = deps;
                    data.InterString();
                    m_dict.Add(data.assetPath, data);
                }
                ret.AddRange(data.dependPath);
            }
            return ret.ToArray();
        }

        public static string[] GetDependencies(string file)
        {
            string[] files = { file };
            return GetDependencies(files);
        }

        public static string[] GetDependencies(string[] files)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < files.Length; ++i)
            {
                if (!dict.ContainsKey(files[i]))
                {
                    dict.Add(files[i], files[i]);
                }
                getDependencies(files[i], dict);
            }
            return new List<string>(dict.Keys).ToArray();
        }

        private static void getDependencies(string file, Dictionary<string, string> dict)
        {
            string main_ext = System.IO.Path.GetExtension(file).ToLower();
            string[] dep = AssetDatabase.GetDependencies(file, false);

            // dirty material has dirty texture dependencies
            if (main_ext == ".mat")
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(file);
                if (mat == null)
                {
                    return;
                }
                MaterialProperty[] proTes = MaterialEditor.GetMaterialProperties(new Object[] {mat});
                for (int i = 0; i < proTes.Length; ++i)
                {
                    if (proTes[i].type == MaterialProperty.PropType.Texture)
                    {
                        Texture tex = mat.GetTexture(proTes[i].name);
                        string path = AssetDatabase.GetAssetPath(tex);
                        if (!dict.ContainsKey(path))
                        {
                            dict.Add(path, path);
                        }
                    }
                }
                for (int i = 0; i < dep.Length; ++i)
                {
                    // assume material only depencies Texture & Shader
                    if (dep[i].EndsWith(".shader") && !dict.ContainsKey(dep[i]))
                    {
                        dict.Add(dep[i], dep[i]);
                    }
                }
                Resources.UnloadAsset(mat);
            }
            else
            {
                for (int i = 0; i < dep.Length; ++i)
                {
                    if (!dict.ContainsKey(dep[i]))
                    {
                        dict.Add(dep[i], dep[i]);
                        getDependencies(dep[i], dict);
                    }
                }
            }
        }

        private class DependData
        {
            public string assetPath;
            public string[] dependPath;

            public void InterString()
            {
                assetPath = string.Intern(assetPath);
                for (int i = 0; dependPath != null && i < dependPath.Length; ++i)
                {
                    dependPath[i] = string.Intern(dependPath[i]);
                }
            }
        }

        public static void Clear()
        {
            m_dict.Clear();
        }

        private static Dictionary<string, DependData> m_dict = new Dictionary<string, DependData>();
    }
}