using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField, Header("エネミーのパターンのプレハブ")]
    private List<GameObject> _enemies = new List<GameObject>();

    [SerializeField, Header("スポーンの間隔")]
    private float _spawnDuration = 5f;

    [SerializeField, Header("スポーンのx座標")]
    private float _spawnPosX;

    [SerializeField, Header("移動制限：上")]
    private float _spawnPosLimitUp = 5f;

    [SerializeField, Header("移動制限：下")]
    private float _spawnPosLimitDown = -5f;

    private float _timePerSec = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.State == State.Over) return;
        _timePerSec += Time.deltaTime;

        if ( _timePerSec > _spawnDuration)
        {
            _timePerSec = 0;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemy = _enemies[Random.Range(0, _enemies.Count)];
        Vector3 spawnPos = new Vector3(_spawnPosX, Random.Range(_spawnPosLimitDown, _spawnPosLimitUp), 0);

        Instantiate(enemy, spawnPos, Quaternion.identity);
    }
}
