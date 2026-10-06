using System;
using System.Collections.Generic;

namespace Unobserved
{
    // Exhaust every alternative before refilling; never repeat the current state.
    public sealed class StateDeck
    {
        readonly List<int> bag = new List<int>();
        readonly Random random;
        public StateDeck(int seed) => random = new Random(seed);
        public int Next(int count, int current)
        {
            if (count <= 1) return 0;
            bag.Remove(current);
            if (bag.Count == 0)
            {
                for (int i = 0; i < count; i++) if (i != current) bag.Add(i);
                for (int i = bag.Count - 1; i > 0; i--)
                { int j = random.Next(i + 1); int v = bag[i]; bag[i] = bag[j]; bag[j] = v; }
            }
            int result = bag[bag.Count - 1]; bag.RemoveAt(bag.Count - 1); return result;
        }
    }
}
