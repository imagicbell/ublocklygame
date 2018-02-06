namespace UToolbox
{
    using System;
    using System.Reflection;
    using UnityEngine;
    
    public class Singleton<T> where T : Singleton<T>
    {
        private static T mInstance;
        private static object mLock = new object();

        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (mLock)
                    {
                        // first get the non-public constructor
                        ConstructorInfo[] ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                        // get the parameterless constructor
                        ConstructorInfo ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

                        if (ctor == null)
                        {
                            Debug.LogWarning("[UToolbox - Singleton] It's strongly recommended to define a non-public constructor for singleton class!");
                            ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.Public);
                            ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
                        }

                        mInstance = ctor.Invoke(null) as T;
                    }
                }
                return mInstance;
            }
        }
    }
}