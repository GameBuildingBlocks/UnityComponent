using System.Collections.Generic;

public class GraphLinkList<TKey, TValue>
{
    private Dictionary<TKey, int> m_dict = null;
    private List<int> m_head = null;
    private List<int> m_nxt = null;
    private List<TValue> m_value = null;
    private int m_nodeCount = 0;

    public GraphLinkList()
        : this(0, 0)
    {
    }

    public GraphLinkList(int V, int E)
    {
        m_dict = new Dictionary<TKey, int>();
        m_head = new List<int>(V);
        m_nxt = new List<int>(E);
        m_value = new List<TValue>(E);
        m_nodeCount = 0;
        for (int i = 0; i < V; ++i)
        {
            m_head.Add(-1);
        }
    }

    public void Add(TKey key, TValue value)
    {
        int id = -1;
        if (!m_dict.TryGetValue(key, out id))
        {
            id = m_dict.Count;
            m_dict.Add(key, id);
        }

        if (id >= m_head.Count)
        {
            m_head.Add(-1);
        }

        if (m_nodeCount < m_nxt.Count)
        {
            m_nxt[m_nodeCount] = m_head[id];
            m_value[m_nodeCount] = value;
        }
        else
        {
            m_nxt.Add(m_head[id]);
            m_value.Add(value);
        }

        m_head[id] = m_nodeCount;
        m_nodeCount = m_nodeCount + 1;
    }

    public int GetFirstID(TKey key)
    {
        int ret = -1;
        if (m_dict.TryGetValue(key, out ret))
        {
            if (ret >= 0 && ret < m_head.Count)
            {
                return m_head[ret];
            }
        }
        return -1;
    }

    public int GetNextID(int id)
    {
        if (id >= 0 && id < m_nxt.Count)
        {
            return m_nxt[id];
        }
        return -1;
    }

    public bool GetValueByID(int id, out TValue retValue)
    {
        if (id >= 0 && id < m_value.Count)
        {
            retValue = m_value[id];
            return true;
        }
        retValue = default(TValue);
        return false;
    }
}