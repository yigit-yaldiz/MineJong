using UnityEngine;

namespace BaseAssets
{

    /// <summary>
    /// Abstract class for making reload-proof singletons out of ScriptableObjects
    /// Returns the asset created on the editor, or null if there is none
    /// Based on https://www.youtube.com/watch?v=VBA1QCoEAX4
    /// </summary>
    /// <typeparam name="T">Singleton type</typeparam>

    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        static T _instance = null;
        public static T Instance
        {
            get
            {
                //if (!_instance)
                //    _instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();

                if (!_instance)
                {
                    T[] objects = Resources.LoadAll<T>("");

                    if (objects.Length > 0 && objects[0] != null)
                    {
                        _instance = objects[0];
                    }

                    if (objects.Length > 1)
                        Debug.LogError("You have more than 1 object of type - " + typeof(T).FullName + ". Choosing first.");
                    else if (objects.Length == 0)
                        Debug.LogError("Something is wrong. Can't find the object - " + typeof(T).FullName + ". Make sure it's on the Resources folder.");
                }


                return _instance;
            }
        }
    }
}