using System;
using System.IO;
using System.Collections.Generic;

namespace BundleManager
{
    public class BundleNexus
    {
        public void AddPathToBundle(string path, string bundleName)
        {
            if (!m_pathToBundle.ContainsKey(path))
            {
                m_pathToBundle.Add(path, bundleName);
            }
            else
            {
                m_pathToBundle[path] = bundleName;
            }
        }

        public void AddBundleRely(string bundleName, string relyBundle)
        {
            List<string> list = null;
            if (!m_bundleRely.TryGetValue(bundleName, out list))
            {
                list = new List<string>();
                m_bundleRely.Add(bundleName, list);
            }

            if (!list.Contains(relyBundle))
            {
                list.Add(relyBundle);
            }
        }

        public void Clear()
        {
            m_bundleRely.Clear();
            m_pathToBundle.Clear();
        }

        public bool SaveBytes(string path)
        {
            FileStream file = File.Open(path, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(file);

            binaryWriter.Write(m_pathToBundle.Count);

            using (var iterator = m_pathToBundle.GetEnumerator())
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

            binaryWriter.Write(m_bundleRely.Count);
            using (var iterator = m_bundleRely.GetEnumerator())
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

        private Dictionary<string, string> m_pathToBundle = new Dictionary<string, string>();
        private Dictionary<string, List<string>> m_bundleRely = new Dictionary<string, List<string>>();
    }
}