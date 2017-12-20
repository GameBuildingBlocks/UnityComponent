using UnityEngine;

namespace GameResource
{
    internal class ResourceData : RefCountData
    {
        private readonly Object m_asset = null;
        private readonly System.Type m_type = default(System.Type);

        public Object asset { get { return m_asset; } }
        public System.Type type { get { return m_type; } }

        public ResourceData(ulong hash, System.Type type, Object asset)
            : base(hash)
        {
            m_type = type;
            m_asset = asset;
        }
    }
}