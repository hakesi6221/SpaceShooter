using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BossStartManager : MonoBehaviour
{
    [SerializeField, Header("ボスの出現場所")]
    private Transform _bossGeneratePos = null;

    [SerializeField, Header("ボス")]
    private GameObject _bossObj = null;

    [SerializeField, Header("エネミーのスポーンが止まるまでの時間")]
    private float _stopEnemySpawnDuration = 60.0f;

    [SerializeField, Header("スポーンが止まってからボスが出現するまでの時間")]
    private float _startBossDuration = 2.0f;

    [SerializeField, Header("EnemySpawner")]
    private EnemySpawner _spawner;

    // 現在時間の物差し
    private float _timeFramePerSec = 0f;

    // スポーン終了済みか
    private bool _isSpawnFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isSpawnFinished) return;
        _timeFramePerSec += Time.deltaTime;
        if (_timeFramePerSec > _stopEnemySpawnDuration)
        {
            _isSpawnFinished = true;
            StopSpawner();
        }
    }

    private async void StopSpawner()
    {
        _spawner.enabled = false;
        await UniTask.Delay(TimeSpan.FromSeconds(_startBossDuration));
        Instantiate(_bossObj, _bossGeneratePos.position, Quaternion.identity);
    }
}
