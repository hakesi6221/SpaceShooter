using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using NaughtyAttributes;


public enum State
{
    Move,
    CantMove,
    Death,
    AllStop,
    Over,
}


public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField, Header("ゲームオーバーUI")]
    private GameObject _gameOverUI;

    [SerializeField, Header("ゲームオーバースコアテキスト")]
    private TMP_Text _resultScoreOver;

    [SerializeField, Header("ゲームクリアUI")]
    private GameObject _gameClearUI;

    [SerializeField, Header("ゲームクリアスコアテキスト")]
    private TMP_Text _resultScoreClear;

    [Foldout("Player"), SerializeField, Header("ライフ0時のヒットストップ：秒")]
    private float _hitStopOnFinalDeath = 1.0f;

    [Foldout("Player"), SerializeField, Header("ヒットストップ終了後の減速率"), Range(0f, 1f)]
    private float _decelerationOnDeath = 0.5f;

    [Foldout("Player"), SerializeField, Header("ゲームオーバーUIが出るまでの時間：秒")]
    private float _gameOverDuration = 1.0f;

    [Foldout("Boss"), SerializeField, Header("ボスのヒットストップ時間")]
    private float _hitStopOnFinalDeathBoss = 1.0f;

    [Foldout("Boss"), SerializeField, Header("ヒットストップ終了後の減速率"), Range(0f, 1f)]
    private float _decelerationOnDeathBoss = 0.5f;

    [Foldout("Boss"), SerializeField, Header("ヒットストップ後の減速の時間")]
    private float _decelerationDurationBoss = 1.0f;

    [Foldout("Boss"), SerializeField, Header("ゲームクリアUIが出るまでの時間：秒")]
    private float _gameClearDuration = 1.0f;

    [Foldout("Boss"), SerializeField, Header("ボスを倒したときのスコア")]
    private int _addScoreByBrokeBoss = 10000;

    [SerializeField, Header("スコア表示テキスト")]
    private TMP_Text _scoreText;

    [SerializeField, Header("ライフ表示UI")]
    private GameObject _lifeUI;

    [SerializeField, Header("スコア更新の所要時間:秒")]
    private float _requireTimeScore = 1.0f;

    [SerializeField, Header("プレイヤー")]
    private GameObject _player;


    private State _state = State.CantMove;
    public State State { get { return _state; } }
    public void ChangeMove() { _state = State.Move; }
    public void ChangeCantMove() { _state = State.CantMove; }
    public void ChangeDeath() { _state = State.Death; }
    public void ChangeStop() { _state = State.AllStop; }

    protected override bool dontDestroyOnLoad => false;

    // スコア
    private int _score = 0;
    public int Score {  get { return _score; } }
    public void AddScore(int add) { _score += add; }

    // テキストに表示するスコア
    private float _displayScore = 0;

    // Start is called before the first frame update
    async void Start()
    {
        SoundManager.Instance.PlayBGMWithFadeIn(1);
        await UniTask.WaitUntil(() => !FadeManager.Instance.IsFade);
        _state = State.Move;
    }

    // Update is called once per frame
    void Update()
    {
        DisplayScore();
    }

    private void DisplayScore()
    {
        if (_state == State.AllStop) return;
        _displayScore += (int)(_score - _displayScore) * Time.deltaTime / _requireTimeScore;
        _scoreText.text = "Score : " + ((int)_displayScore).ToString();
        if (Mathf.Abs(_score - _displayScore) <= 1) _displayScore = _score;
    }

    public async void GameOver()
    {
        _state = State.AllStop;
        await UniTask.Delay(TimeSpan.FromSeconds(_hitStopOnFinalDeath));
        _state = State.Over;
        Time.timeScale = _decelerationOnDeath;
        _player.SetActive(false);
        await UniTask.Delay(TimeSpan.FromSeconds(_gameOverDuration));
        Time.timeScale = 1;
        _scoreText.gameObject.SetActive(false);
        _lifeUI.SetActive(false);
        _resultScoreOver.text = _scoreText.text;
        _gameOverUI.SetActive(true);
    }

    public async void BossDeath(GameObject boss)
    {
        _state = State.AllStop;
        await UniTask.Delay(TimeSpan.FromSeconds(_hitStopOnFinalDeathBoss));
        _state = State.Over;
        Time.timeScale = _decelerationOnDeathBoss;
        await UniTask.Delay(TimeSpan.FromSeconds(_decelerationDurationBoss));
        _score += _addScoreByBrokeBoss;
        boss.SetActive(false);
        SoundManager.Instance.PlaySE(1);
        PopEffects.Instance.PopEffectsTargetPos(boss.transform.position, 2);
        Time.timeScale = 1;
        await UniTask.Delay(TimeSpan.FromSeconds(_gameClearDuration));
        _scoreText.gameObject.SetActive(false);
        _lifeUI.SetActive(false);
        _resultScoreClear.text = _scoreText.text;
        _gameClearUI.SetActive(true);
    }

    public void MoveScene(string sceneName)
    {
        SoundManager.Instance.StopBGMWithFadeOut(1);
        FadeManager.Instance.CallScene(sceneName);
    }
}
