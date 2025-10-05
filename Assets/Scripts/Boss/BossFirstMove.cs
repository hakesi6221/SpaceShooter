using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFirstMove : MonoBehaviour
{
    [SerializeField, Header("ボスが攻撃開始するx座標")]
    private float _startX;

    [SerializeField, Header("BossMovement")]
    private BossMovement _movement;

    [SerializeField, Header("BossAttack")]
    private BossAttack _attack;

    [SerializeField, Header("MoveConstVelocity")]
    private MoveonConstVelocity _firstMove;

    [SerializeField, Header("コライダー")]
    private Collider2D _collider;

    private bool _isStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        _collider.enabled = false;
        _movement.enabled = false;
        _attack.enabled = false;
        _isStarted = false;
        _firstMove.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <= _startX && !_isStarted)
        {
            _isStarted = true;
            _firstMove.enabled = false;
            _movement.enabled = true;
            _attack.enabled = true;
            _collider.enabled = true;
        }

        if (_isStarted && _movement.IsDead)
        {
            _collider.enabled = false;
        }
    }
}
