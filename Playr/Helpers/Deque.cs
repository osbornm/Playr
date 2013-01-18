using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playr.Helpers
{
    public class Deque<T> : LinkedList<T>
        where T : class
    {
        public T PopFirst()
        {
            var value = First;
            if (value == null)
                return null;

            RemoveFirst();
            return value.Value;
        }

        public T PopLast()
        {
            var value = Last;
            if (value == null)
                return null;

            RemoveLast();
            return value.Value;
        }
    }
}
