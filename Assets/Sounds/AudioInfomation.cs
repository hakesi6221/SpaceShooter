using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum AudioType
{
    BGM,
    SE,
    VOICE,
    OTHERS,
}

[Serializable]
public class AudioInfomation
{
    [SerializeField, Header("サウンドの種類")]
    private AudioType _type = AudioType.BGM;
    /// <summary>
    /// サウンドの種類
    /// </summary>
    public AudioType Type { get { return _type; } }

    [SerializeField, Header("サウンドのクリップ")]
    private AudioClip _clip = null;
    /// <summary>
    /// サウンドのクリップ
    /// </summary>
    public AudioClip Clip { get { return _clip; } }

    [SerializeField, Header("サウンドのボリューム"), Range(0.0f, 1.0f)]
    private float _volume = 1.0f;
    /// <summary>
    /// サウンドのボリューム
    /// </summary>
    public float Volume { get { return _volume; } }

    [SerializeField, Header("ループするか")]
    private bool _loop = false;
    /// <summary>
    /// ループするか
    /// </summary>
    public bool Loop { get { return _loop; } }

    [SerializeField, Header("再生のオフセット"), Range(0.0f, 1.0f)]
    private float _ofset = 0f;
    /// <summary>
    /// 再生のオフセット
    /// </summary>
    public float Ofset { get { return _ofset; } }
}
