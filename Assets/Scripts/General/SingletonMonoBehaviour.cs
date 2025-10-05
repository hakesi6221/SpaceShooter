using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// このオブジェクトをDontDestroyOnRoadにするかどうか
    /// </summary>
    protected abstract bool dontDestroyOnLoad { get; }

    private static T instance;

    /// <summary>
    /// インスタンスを取得
    /// インスタンスがない場合エラーを出力
    /// </summary>
    public static T Instance
    {
        get
        {
            if (!instance)
            {
                Type t = typeof(T);
                instance = (T)FindObjectOfType(t);
                if (!instance)
                {
                    Debug.LogError(t + " is nothing.");
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// すでにインスタンスがある場合、これを削除、dontDestroyOnLoadがtrueの場合設定
    /// </summary>
    protected virtual void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
