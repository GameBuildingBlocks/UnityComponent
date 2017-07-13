using System;
using System.IO;
using System.Collections.Generic;

namespace BundleManager
{
    public class BundleMainfest
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

        public void AddBundleDepend(string bundleName, string relyBundle)
        {
            List<string> list = null;
            if (!m_bundleDepend.TryGetValue(bundleName, out list))
            {
                list = new List<string>();
                m_bundleDepend.Add(bundleName, list);
            }

            if (!list.Contains(relyBundle))
            {
                list.Add(relyBundle);
            }
        }

        public void AddBundleState(List<BundleState> bundleStates)
        {
            if (bundleStates == null)
                return;
            for (int i = 0; i < bundleStates.Count; ++i)
            {
                BundleState states = bundleStates[i];
                if (!m_bundleStates.ContainsKey(states.bundleID))
                {
                    m_bundleStates.Add(states.bundleID, states);
                }
            }
        }

        public void Clear()
        {
            m_bundleStates.Clear();
            m_bundleDepend.Clear();
            m_pathToBundle.Clear();
        }

        public bool SaveBytes(string path)
        {
            FileStream file = File.Open(path, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(file);

            binaryWriter.Write(m_bundleStates.Count);
            using (var iterator = m_bundleStates.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    binaryWriter.Write(iterator.Current.Value.bundleID.Length);
                    binaryWriter.Write(iterator.Current.Value.bundleID.ToCharArray());
                    binaryWriter.Write(iterator.Current.Value.crc);
                    binaryWriter.Write(iterator.Current.Value.compressCrc);
                    binaryWriter.Write(iterator.Current.Value.version);
                    binaryWriter.Write(iterator.Current.Value.size);
                    binaryWriter.Write((int)iterator.Current.Value.loadState);
                    binaryWriter.Write((int)iterator.Current.Value.storePos);
                }
            }

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

            binaryWriter.Write(m_bundleDepend.Count);
            using (var iterator = m_bundleDepend.GetEnumerator())
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

        private Dictionary<string, BundleState> m_bundleStates = new Dictionary<string, BundleState>();
        private Dictionary<string, string> m_pathToBundle = new Dictionary<string, string>();
        private Dictionary<string, List<string>> m_bundleDepend = new Dictionary<string, List<string>>();
    }
}