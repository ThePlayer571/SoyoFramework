/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace SoyoFramework.ToolKits.Runtime.SingletonKit
{
    public abstract class PersistentMonoSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T mInstance;
        protected bool mEnabled;

        public static T Instance
        {
            get
            {
                if (mInstance) return mInstance;
                mInstance = FindObjectOfType<T>();
                if (mInstance) return mInstance;
                var obj = new GameObject();
                mInstance = obj.AddComponent<T>();
                return mInstance;
            }
        }

        protected virtual void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            
            if (!mInstance)
            {
                mInstance = this as T;
                mEnabled = true;
            }
            else
            {
                if (this != mInstance)
                {
                    Destroy(gameObject);
                    return;
                }
            }
            
            DontDestroyOnLoad(gameObject);
        }
    }
}