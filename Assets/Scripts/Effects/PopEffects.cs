using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopEffects : SingletonMonoBehaviour<PopEffects>
{
    protected override bool dontDestroyOnLoad => false;

    [SerializeField, Header("エフェクトのプレハブ")]
    private List<GameObject> _effectObjs = new List<GameObject>();

    /// <summary>
    /// エフェクトを生成
    /// </summary>
    /// <param name="position">生成する場所</param>
    /// <param name="effectIndex">生成するエフェクト</param>
    public void PopEffectsTargetPos(Vector3 position, int effectIndex)
    {
        GameObject popEffect = _effectObjs[effectIndex];

        Instantiate(popEffect, position, Quaternion.identity);
    }


}
