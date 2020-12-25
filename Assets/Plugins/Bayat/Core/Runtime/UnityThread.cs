using System.Threading;
using UnityEngine;

namespace Bayat
{

    public static class UnityThread
    {

        public static Thread thread = Thread.CurrentThread;

        public static bool allowsAPI => Thread.CurrentThread == thread;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInitialize()
        {
            thread = Thread.CurrentThread;
        }

    }

}