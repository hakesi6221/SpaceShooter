using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove_VerticleInRange : MonoBehaviour
{
    [SerializeField, Header("最初のカウント開始のオフセット")]
    private float _countStartOffsetSec = 0f;

    [SerializeField, Header("開始から何秒たったら、上下に動き始めるか")]
    private float _startVerMoveSec = 4.0f;

    [SerializeField, Header("何unit動きたいか")]
    private float _moveDistanceVer = 3.0f;

    [SerializeField, Header("何秒動きたいか")]
    private float _moveDurationVer = 1.0f;

    // 上下に動き始めるまでのカウントを始めるかどうかのフラグ
    private bool _isStartCount = false;
    // 上下に動き始めるかのフラグ
    private bool _isStartMoveVer = false;
    // 上下の移動が終了したかのフラグ
    private bool _isFinshMoveVer = false;
    // 現在の秒数を測る物差し
    private float _timeFramePerSec = 0f;
    // 上下の移動速度
    private float _velocityVer = 0f;
    // 上下の方向
    private int _velocityDirection = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.position.y >= 0) _velocityDirection = -1;
        else _velocityDirection = 1;
        _velocityVer = _moveDistanceVer * _velocityDirection / _moveDurationVer;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.State == State.AllStop) return;
        _timeFramePerSec += Time.deltaTime;

        if (!_isStartCount && _timeFramePerSec > _countStartOffsetSec)
        {
            
            _timeFramePerSec = 0f;
            _isStartCount = true;
        }
        if (!_isStartMoveVer && _isStartCount && _timeFramePerSec > _startVerMoveSec) 
        {
            _timeFramePerSec = 0f;
            _isStartMoveVer = true; 
        }
        if (_isStartMoveVer && !_isFinshMoveVer)
        {
            Vector3 performedPos = new Vector3(transform.position.x, transform.position.y + (_velocityVer * Time.deltaTime), transform.position.z);
            transform.position = performedPos;
            if (_timeFramePerSec > _moveDurationVer) 
            {
                _isFinshMoveVer = true; 
            }
        }
    }
}
