using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nawlian.Lib.Utils
{
    /// <summary>
    /// Defines a Singleton. <br/>
    /// This Singleton will NOT create a new object if it does not exist.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ManagerSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<T>(includeInactive: false);
                return _instance;
            }
        }
    }

    public abstract class Singleton<T>
    {
        private static T _instance;

        public static T Instance => _instance ??= (T)Activator.CreateInstance(typeof(T));

        public Singleton() {}
    }
}