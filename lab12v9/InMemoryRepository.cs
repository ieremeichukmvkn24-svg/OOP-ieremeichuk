using System.Collections.Generic;
using System.Linq;

namespace lab12v9
{
    public class InMemoryRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly List<T> _items = new List<T>();

        public T? GetById(int id)
        {
            return _items.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            return _items;
        }

        public void Add(T item)
        {
            if (item != null)
            {
                _items.Add(item);
            }
        }

        public void Update(T item)
        {
            if (item == null) return;
            
            var existingItem = GetById(item.Id);
            if (existingItem != null)
            {
                var index = _items.IndexOf(existingItem);
                _items[index] = item;
            }
        }

        public void Delete(int id)
        {
            var item = GetById(id);
            if (item != null)
            {
                _items.Remove(item);
            }
        }
    }
}