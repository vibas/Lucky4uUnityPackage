using UnityEngine;
namespace Lucky4u
{
    public class SingletonMonobehaviour<T> : MonoBehaviour where T : SingletonMonobehaviour<T>
    {
        private static T _instance;

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    DontDestroyOnLoad(_instance.gameObject);
                }

                if (_instance == null)
                {
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = "(Singleton) " + typeof(T).ToString();
                    DontDestroyOnLoad(singleton);
                }

                return _instance;
            }
        }

        void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            _instance = (T)this;
        }
    }
}