/*
    Reference: http://wiki.unity3d.com/index.php/Singleton
*/

namespace UToolbox
{
    using UnityEngine;

    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T mInstance;
        private static object mLock = new object();
        private static bool mApplicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (mApplicationIsQuitting)
                {
                    Debug.LogWarning("[UToolbox - Singleton] Instance '" + typeof(T) +
                                     "' already destroyed on application quit. Won't create again - returning null.");
                    return null;
                }

                if (mInstance == null)
                {
                    lock (mLock)
                    {
                        var objs = FindObjectsOfType(typeof(T));
                        if (objs.Length > 1)
                        {
                            Debug.LogError("[UToolbox - Singleton] There should never be more than 1 singleton! Reopening the scene might fix it.");
                            return objs[0] as T;
                        }

                        if (objs.Length == 0)
                        {
                            GameObject singleton = new GameObject();
                            mInstance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();

                            DontDestroyOnLoad(singleton);

                            Debug.Log("[UToolbox - Singleton] An instance of " + typeof(T) +
                                      " is needed in the scene, so '" + singleton + "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            mInstance = objs[0] as T;
                            Debug.Log("[UToolbox - Singleton] Using instance already created: " + mInstance.gameObject.name);
                        }
                    }
                }
                return mInstance;
            }
        }

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        private void OnDestroy()
        {
            mApplicationIsQuitting = true;
        }
    }
}
