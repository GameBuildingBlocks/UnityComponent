using UnityEngine;
using EditorCommon;

namespace ResourceFormat
{
    public class BaseViewer
    {
        public virtual void OnLeave() { }
        public virtual void OnEnter() { }
        public virtual void Draw(Rect view) { }

        protected TableView m_dataTable;
        protected TableView m_showTable;
    }
}