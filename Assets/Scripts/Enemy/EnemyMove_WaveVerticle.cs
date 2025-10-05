using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove_WaveVerticle : MonoBehaviour
{
    [SerializeField, Header("最初のカウント開始のオフセット")]
    private float _countStartOffsetSec = 0f;

    [SerializeField, Header("開始から何秒たったら、上下に動き始めるか")]
    private float _startVerMoveSec = 4.0f;

    [SerializeField, Header("上下の最高移動距離：unit")]
    private float _maxDistanceVer = 3.0f;

    [SerializeField, Header("波の速度")]
    private float _waveSpeedVer = 2.0f;

    // 上下に動き始めるまでのカウントを始めるかどうかのフラグ
    private bool _isStartCount = false;
    // 上下に動き始めるかのフラグ
    private bool _isStartMoveVer = false;
    // 現在の秒数を測る物差し
    private float _timeFramePerSec = 0f;
    // 最初のy座標
    private float _firstPosVer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _firstPosVer = transform.position.y;
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
        if (_isStartMoveVer)
        {
            float performedPos = Mathf.Sin(_timeFramePerSec * _waveSpeedVer) * _maxDistanceVer;
            transform.position = new Vector3(transform.position.x, _firstPosVer + performedPos, transform.position.z);
        }

    }
}
