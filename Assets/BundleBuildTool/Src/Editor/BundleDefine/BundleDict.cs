using System;
using System.IO;
using System.Collections.Generic;

namespace BundleManager
{
    public class BundleDict
    {
        public void AddPathWithBundleName(string path, string bundleName)
        {
            if (!m_pathToBundleName.ContainsKey(path))
            {
                m_pathToBundleName.Add(path, bundleName);
            }
            else
            {
                m_pathToBundleName[path] = bundleName;
            }
        }

        public void AddBundleDepend(string bundleName, string depBundleName)
        {
            List<string> list = null;
            if (!m_bundleDependDict.TryGetValue(bundleName, out list))
            {
                list = new List<string>();
                m_bundleDependDict.Add(bundleName, list);
            }

            if (!list.Contains(depBundleName))
            {
                list.Add(depBundleName);
            }
        }

        public void Clear()
        {
            m_bundleDependDict.Clear();
            m_pathToBundleName.Clear();
        }

        public bool SaveBytes(string path)
        {
            FileStream file = File.Open(path, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(file);

            binaryWriter.Write(m_pathToBundleName.Count);

            using (var iterator = m_pathToBundleName.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    string publishPath = BuildConfig.FormatPublishPath(iterator.Current.Key);
                    binaryWriter.Write(publishPath.Length);
                    binaryWriter.Write(publishPath.ToCharArray());
                    binaryWriter.Write(iterator.Current.Value.Length);
                    binaryWriter.Write(iterator.Current.Value.ToCharArray());
                }
            }

            binaryWriter.Write(m_bundleDependDict.Count);
            using (var iterator = m_bundleDependDict.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    binaryWriter.Write(iterator.Current.Key.Length);
                    binaryWriter.Write(iterator.Current.Key.ToCharArray());
                    binaryWriter.Write(iterator.Current.Value.Count);

                    for (int i = 0; i < iterator.Current.Value.Count; ++i)
                    {
                        binaryWriter.Write(iterator.Current.Value[i].Length);
                        binaryWriter.Write(iterator.Current.Value[i].ToCharArray());
                    }
                }
            }

            binaryWriter.Flush();
            binaryWriter.Close();

            return true;
        }

        Dictionary<string, string> m_pathToBundleName = new Dictionary<string, string>();
        Dictionary<string, List<string>> m_bundleDependDict = new Dictionary<string, List<string>>();
    }
}