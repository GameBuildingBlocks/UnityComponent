using System.Collections.Generic;
using System.Diagnostics;
using CommonComponent;

namespace GameResource
{
    internal class RequestBase<Data> where Data : RefCountData
    {
        private readonly ulong m_id = 0;
        protected int m_priority = 0;
        protected Data m_data = null;
        private long m_costTime = 0;
        private long m_startTime = 0;

        public event System.Action<RequestBase<Data>> OnLoadDone;

        public ulong Id
        {
            get { return m_id; }
        }

        public Data asset
        {
            get { return m_data; }
        }

        // Reserved
        public long costTime
        {
            get { return m_costTime; }
        }

        public RequestBase(ulong id)
        {
            m_id = id;
        }

        virtual public bool TryLoad()
        {
            m_startTime = Stopwatch.GetTimestamp();
            return true;
        }

        virtual public bool IsDone()
        {
            return true;
        }

        virtual public bool LoadDone()
        {
            m_costTime = (Stopwatch.GetTimestamp() - m_startTime) * 1000 / Stopwatch.Frequency;
            if (OnLoadDone != null)
            {
                OnLoadDone(this);
            }
            return true;
        }
    }
}