using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStop : MonoBehaviour
{
    [SerializeField, Header("アニメーター")]
    private Animator _anim;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.State == State.AllStop)
        {
            _anim.SetFloat("speed", 0f);
        }
        else
        {
            _anim.SetFloat("speed", 1f);
        }
    }
}
