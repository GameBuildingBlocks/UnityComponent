using System.Collections.Generic;

namespace ResourceFormat {
    public class ImportData {
        public string RootPath   = "";
        public string FileNameMatch     = "*.*";
        public int  Index = -1;
        public int  TotalCount = 0;
        public int  TotalMemuse = 0;
        public bool PreBuild = true;

        public virtual bool IsMatch(string path) {
            return ImportRegex.IsMatch(this, path);
        }

        public virtual void CopyData(ImportData data) {
            RootPath = data.RootPath;
            FileNameMatch = data.FileNameMatch;
            PreBuild = data.PreBuild;
        }

        public virtual void ClearObject() {
            m_objects.Clear();
            m_unFortmatObjects.Clear();
            TotalCount = 0;
            TotalMemuse = 0;
        }

        public virtual List<object> GetObjects() {
            return m_objects;
        }

        protected List<object> m_objects = new List<object>();
        protected List<object> m_unFortmatObjects = new List<object>();
    }
}