using UnityEngine;

namespace Lucky4u.Common
{
    public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    T[] assets = Resources.LoadAll<T>("");
                    if (assets != null && assets.Length > 0)
                    {
                        instance = assets[0];
                    }
                    else
                    {
                        Debug.LogError("No config found");
                    }
                }
                return instance;
            }
        }
    }
}