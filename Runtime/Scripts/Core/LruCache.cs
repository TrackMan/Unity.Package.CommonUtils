using System.Collections.Generic;

namespace Trackman
{
    public class LruCache<T>
    {
        #region Containers
        struct CacheItem
        {
            public int Key { get; set; }
            public T Value { get; set; }
        }
        #endregion

        #region Fields
        readonly int capacity;
        readonly Dictionary<int, LinkedListNode<CacheItem>> cacheMap;
        readonly LinkedList<CacheItem> cacheList;
        #endregion

        #region Constructors
        public LruCache(int capacity)
        {
            this.capacity = capacity;
            cacheMap = new Dictionary<int, LinkedListNode<CacheItem>>(capacity);
            cacheList = new LinkedList<CacheItem>();
        }
        #endregion

        #region Methods
        public bool TryGet(int key, out T value)
        {
            if (!cacheMap.TryGetValue(key, out LinkedListNode<CacheItem> node))
            {
                value = default;
                return false;
            }

            cacheList.Remove(node);
            cacheList.AddFirst(node);
            value = node.Value.Value;
            return true;
        }
        public void AddToCache(int key, T value)
        {
            if (cacheMap.Count >= capacity)
            {
                LinkedListNode<CacheItem> lastNode = cacheList.Last;
                cacheMap.Remove(lastNode.Value.Key);
                cacheList.RemoveLast();
            }

            CacheItem newItem = new() { Key = key, Value = value };
            LinkedListNode<CacheItem> newNode = new(newItem);

            cacheList.AddFirst(newNode);
            cacheMap[key] = newNode;
        }
        #endregion
    }
}