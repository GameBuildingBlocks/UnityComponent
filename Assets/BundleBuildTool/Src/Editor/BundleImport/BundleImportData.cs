using System.Collections.Generic;
using UnityEditor;

namespace BundleManager
{
    public class BundleImportData
    {
        public string RootPath = "";
        public string FileNameMatch = "*.*";
        public int Index = -1;
        public int TotalCount = 0;
        public BundleType Type = BundleType.None;
        public BundleLoadState LoadState = BundleLoadState.UnloadOnUnloadAsset;
        public bool Publish = false;
        public int LimitCount = -1;
        public int LimitKBSize = -1;
        public bool PushDependice = false;
        public bool SkipData = false;

        public void CopyData(BundleImportData data)
        {
            RootPath = data.RootPath;
            FileNameMatch = data.FileNameMatch;
            Index = data.Index;
            Type = data.Type;
            LoadState = data.LoadState;
            Publish = data.Publish;
            LimitCount = data.LimitCount;
            LimitKBSize = data.LimitKBSize;
            PushDependice = data.PushDependice;
        }

        public bool IsMatch(string path)
        {
            int index = path.IndexOf(BuildConfig.ResourceRootPath);
            if (index != -1)
            {
                path = path.Substring(index + BuildConfig.ResourceRootPath.Length);
            }
            string packagePath = PathConfig.NormalizePathSplash(path);
            if (packagePath.StartsWith("/"))
            {
                packagePath = packagePath.Substring(1);
            }

            string formatPath = PathConfig.NormalizePathSplash(RootPath);
            if (!string.IsNullOrEmpty(formatPath) &&
                !packagePath.StartsWith(formatPath, System.StringComparison.OrdinalIgnoreCase))
                return false;

            EditorRegex regex = EditorRegex.Create(FileNameMatch);
            return regex == null ? false : regex.IsMatch(packagePath);
        }

        public void AddObject(AssetPathInfo pInfo)
        {
            if (!SkipData)
            {
                pInfo.Index = Index;
            }
            else
            {
                pInfo.Index = -1;
            }
            m_objects.Add(pInfo);
            TotalCount = m_objects.Count;
        }

        public List<object> GetObjects()
        {
            return m_objects;
        }

        public void ClearObject()
        {
            m_objects.Clear();
            TotalCount = 0;
        }

        protected List<object> m_objects = new List<object>();
    }
}