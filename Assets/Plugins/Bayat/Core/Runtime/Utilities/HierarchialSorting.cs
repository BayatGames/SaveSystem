using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Bayat.Core.Utilities
{

    public static class HierarchicalSorting
    {

        private static int Compare(Component x, Component y)
        {
            return Compare(x != null ? x.transform : null, y != null ? y.transform : null);
        }

        private static int Compare(GameObject x, GameObject y)
        {
            return Compare(x != null ? x.transform : null, y != null ? y.transform : null);
        }

        private static int Compare(Transform x, Transform y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return +1;

            var hierarchy1 = GetHierarchy(x);
            var hierarchy2 = GetHierarchy(y);

            while (true)
            {
                if (!hierarchy1.Any())
                    return -1;

                var pop1 = hierarchy1.Pop();

                if (!hierarchy2.Any())
                    return +1;

                var pop2 = hierarchy2.Pop();

                var compare = pop1.CompareTo(pop2);

                if (compare == 0)
                    continue;

                return compare;
            }
        }

        private static Stack<int> GetHierarchy(Transform transform)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform));

            var stack = new Stack<int>();

            var current = transform;

            while (current != null)
            {
                var index = current.GetSiblingIndex();

                stack.Push(index);

                current = current.parent;
            }

            return stack;
        }

        public static List<T> Sort<T>(List<T> components) where T : Component
        {
            if (components == null)
                throw new ArgumentNullException(nameof(components));

            components.Sort(new RelayComparer<T>(Compare));

            return components;
        }

        public static T[] Sort<T>(T[] components) where T : Component
        {
            if (components == null)
                throw new ArgumentNullException(nameof(components));

            Array.Sort(components, new RelayComparer<T>(Compare));

            return components;
        }

        public static GameObject[] Sort(GameObject[] gameObjects)
        {
            if (gameObjects == null)
                throw new ArgumentNullException(nameof(gameObjects));

            Array.Sort(gameObjects, new RelayComparer<GameObject>(Compare));

            return gameObjects;
        }

        public static Transform[] Sort(Transform[] transforms)
        {
            if (transforms == null)
                throw new ArgumentNullException(nameof(transforms));

            Array.Sort(transforms, new RelayComparer<Transform>(Compare));

            return transforms;
        }

        private sealed class RelayComparer<T> : Comparer<T>
        {
            public RelayComparer(Func<T, T, int> func)
            {
                Func = func ?? throw new ArgumentNullException(nameof(func));
            }

            private Func<T, T, int> Func { get; }

            public override int Compare(T x, T y)
            {
                return Func(x, y);
            }
        }

    }

}