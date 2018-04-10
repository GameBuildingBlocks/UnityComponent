using System.Collections.Generic;
using System.Diagnostics;
using CommonComponent;

namespace GameResource
{
    internal class RequestLoader<Request, Data>
        where Request : RequestBase<Data>
        where Data : RefCountData
    {

        public Request TryGetRequest(ulong hash)
        {
            Request request = null;
            if (m_dict.TryGetValue(hash, out request))
            {
                return request;
            }
            return null;
        }

        public bool PushRequst(Request request)
        {
            if (m_dict.ContainsKey(request.Id))
            {
                return false;
            }

            m_dict.Add(request.Id, request);
            m_undload.AddLast(request.Id);

            return true;
        }

        public void Update()
        {
            _ProcessTryLoad();
            if (ResourceTimeControl.TimeOut()) return;
            _PorcessLoading();
            if (ResourceTimeControl.TimeOut()) return;
            _ProcessLoadDone();
        }

        private void _ProcessTryLoad()
        {
            if (m_undload.Count == 0)
            {
                return;
            }

            ulong hash = m_undload.First.Value;
            m_undload.RemoveFirst();
            Request request = null;
            m_dict.TryGetValue(hash, out request);
            if (request != null)
            {
                if (!request.TryLoad())
                {
                    m_loaded.AddLast(hash);
                    Log.ErrorFormat("[RequestLoader] TryLoad Failed pathHash={0}.", hash);
                }
                else
                {
                    m_loading.AddLast(hash);
                }
            }
        }

        private void _PorcessLoading()
        {
            if (m_loading.Count == 0)
            {
                return;
            }

            var itor = m_loading.First;
            while (itor != null)
            {
                ulong hash = itor.Value;
                var cur = itor;
                itor = itor.Next;

                Request request = null;
                m_dict.TryGetValue(hash, out request);

                if (request == null)
                {
                    Log.WarningFormat("[RequestLoader] loading list container null request pathHash={0}.", hash);
                    m_loading.Remove(cur);
                }
                else if (request.IsDone())
                {
                    m_loaded.AddLast(hash);
                    m_loading.Remove(cur);
                }

                if (ResourceTimeControl.TimeOut()) break;
            }
        }

        private void _ProcessLoadDone()
        {
            if (m_loaded.Count == 0)
            {
                return;
            }

            var itor = m_loaded.First;
            while (itor != null)
            {
                ulong hash = itor.Value;
                var cur = itor;
                itor = itor.Next;

                Request request = null;
                m_dict.TryGetValue(hash, out request);

                if (request == null)
                {
                    Log.WarningFormat("[RequestLoader] load done list container null request pathHash={0}.", hash);
                    m_loaded.Remove(cur);
                }
                else if (request.LoadDone())
                {
                    m_dict.Remove(hash);
                    m_loaded.Remove(cur);
                }

                if (ResourceTimeControl.TimeOut()) break;
            }
        }

        private LinkedList<ulong> m_loaded = new LinkedList<ulong>();
        private LinkedList<ulong> m_loading = new LinkedList<ulong>();
        private LinkedList<ulong> m_undload = new LinkedList<ulong>();
        private Dictionary<ulong, Request> m_dict = new Dictionary<ulong, Request>();
    }
}