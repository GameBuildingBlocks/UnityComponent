using UnityEngine;

namespace ResourceFormat
{
    public class BaseViewer
    {
        public virtual void OnLeave() { }
        public virtual void OnEnter() { }
        public virtual void Draw(Rect view) { }

        protected EditorCommon.TableView m_dataTable;
        protected EditorCommon.TableView m_showTable;
    }
}