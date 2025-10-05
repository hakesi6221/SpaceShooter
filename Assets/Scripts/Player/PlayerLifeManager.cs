using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerLifeManager : SingletonMonoBehaviour<PlayerLifeManager>
{
    protected override bool dontDestroyOnLoad => false;

    [SerializeField, Header("プレイヤーのライフ")]
    private int _playerLife = 3;

    [SerializeField, Header("基準となるプレイヤーのライフUI")]
    private GameObject _lifeUIBase;

    [SerializeField, Header("ライフUIのXpivot間隔")]
    private float _lifeUIDuration = 1.5f;

    [SerializeField, Header("プレイヤーのプレハブ")]
    private GameObject _playerPrefab;

    [SerializeField, Header("リスポーン地点")]
    private Transform _respawnPos;

    [SerializeField, Header("行動開始地点")]
    private Transform _startMovePos;

    [SerializeField, Header("リスポーンまでの時間")]
    private float _respwanWaitTime = 0f;

    [SerializeField, Header("リスポーンしてから行動開始地点までの移動する時間")]
    private float _moveToStartMovePos = 1f;

    [SerializeField, Header("行動開始地点まで移動した後、行動開始になるまでの時間")]
    private float _startMoveWaitTime = 0f;

    [SerializeField, Header("行動開始になってから無敵時間の時間")]
    private float _invisibleTime = 1.0f;

    // ライフUIの出ているもののリスト
    private List<GameObject> _lifeUIs = new List<GameObject>();

    public int PlayerLife { get { return _playerLife; } }

    private bool _isInvisible = false;
    public bool IsInvisible { get { return _isInvisible; } }

    public void DecreaceLife()
    {
        if (_playerLife == 0) return;
        _playerLife--;
        UpdateLifeUINumber(_playerLife);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeLifeUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeLifeUI()
    {
        _lifeUIs.Add(_lifeUIBase);

        for (int i = 1; i < _playerLife; i++)
        {
            GameObject lifeInstance = Instantiate(_lifeUIBase, _lifeUIBase.transform.position, Quaternion.identity, _lifeUIBase.transform.parent);
            // 生成したUIの中心を補正
            lifeInstance.GetComponent<RectTransform>().pivot = new Vector2(_lifeUIBase.GetComponent<RectTransform>().pivot.x - (_lifeUIDuration * i), _lifeUIBase.GetComponent<RectTransform>().pivot.y);
            lifeInstance.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            _lifeUIs.Add(lifeInstance);
        }
        UpdateLifeUINumber(_playerLife);
    }

    private void UpdateLifeUINumber(int number)
    {
        foreach (GameObject lifeInstance in _lifeUIs)
        {
            lifeInstance.SetActive(false);
        }
        for (int i = 0; i < number; i++)
        {
            _lifeUIs[i].SetActive(true);
        }
    }

    public void DeathPlayer()
    {
        if (_playerLife == 0)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            RespwanPlayer();
        }
    }

    public async void RespwanPlayer()
    {
        var token = this.GetCancellationTokenOnDestroy();

        _isInvisible = true;
        DecreaceLife();
        _playerPrefab.transform.position = _respawnPos.position;
        await UniTask.Delay(TimeSpan.FromSeconds(_respwanWaitTime));
        _playerPrefab.SetActive(true);

        GameManager.Instance.ChangeCantMove();
        float timeFramePerSec = 0f;
        while (timeFramePerSec <= _moveToStartMovePos)
        {
            _playerPrefab.transform.position = Vector3.Lerp(_respawnPos.position, _startMovePos.position, timeFramePerSec / _moveToStartMovePos);
            timeFramePerSec += Time.deltaTime;
            await UniTask.Yield();
        }

        await UniTask.Delay(TimeSpan.FromSeconds(_startMoveWaitTime));
        GameManager.Instance.ChangeMove();
        await UniTask.Delay(TimeSpan.FromSeconds(_invisibleTime));
        _isInvisible = false;
    }
}
