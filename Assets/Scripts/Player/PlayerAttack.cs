using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField, Header("’ÊíƒVƒ‡ƒbƒg‚Åo‚·’e")]
    private GameObject _normalShotBullet;

    [SerializeField, Header("’e‚Ì”­¶ˆÊ’u")]
    private Transform _shotPos;

    [SerializeField, Header("’e‚ð‘Å‚Ä‚éŠÔŠu •b")]
    private float _timeSpanShootSec = 1.0f;

    // ’e‚ð‘Å‚ÂŠÔŠu‚ð}‚é•b”
    private float _timeCountSpanShoot = 0f;

    // Update is called once per frame
    void Update()
    {
        if (_timeCountSpanShoot > 0)
        {
            _timeCountSpanShoot -= Time.deltaTime;
        }
    }

    public void OnShootNormalBullet(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.State != State.Move) return;
        if (context.started) return;
        if (context.canceled) return;
        if (_timeCountSpanShoot > 0) return;
        _timeCountSpanShoot = _timeSpanShootSec;

        SoundManager.Instance.PlaySE(0);
        Instantiate(_normalShotBullet, _shotPos.position, Quaternion.identity);
    }
}
