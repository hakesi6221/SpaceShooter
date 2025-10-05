using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Cysharp.Threading.Tasks;
using System;

public class BossMovement : MonoBehaviour
{

    [Foldout("General"), SerializeField, Header("スプライトレンダラー")]
    private SpriteRenderer _sprite;

    [Foldout("General"), SerializeField, Header("BossAttack")]
    private BossAttack _bossAttack;

    [Foldout("Effects"), SerializeField, Header("破壊エフェクト")]
    private List<GameObject> _brokeEffecs = new List<GameObject>();

    [Foldout("Effects"), SerializeField, Header("破壊エフェクト")]
    private float _effectpopDuration = 0.2f;

    [Foldout("Paramater"), SerializeField, Header("HP")]
    private int _hp = 100;

    [Foldout("Paramater"), SerializeField, Header("無敵時間")]
    private float _invisibleTime = 0.1f;

    [Foldout("Paramater"), SerializeField, Header("無敵時間中の色")]
    private Color _invisibleColor = Color.white;

    public void DecreaceHP()
    {
        if (_hp <= 0)
        {
            _isDead = true;
            PopBrokenEffecs();
            GameManager.Instance.BossDeath(this.gameObject);
            _bossAttack.DestroyAllBullet();
            return;
        }
        _hp--;
    }

    [Foldout("Movement"), SerializeField, Header("上下移動の速度：秒速")]
    private float _movespeedVer = 1.0f;

    [Foldout("Movement"), SerializeField, Header("上下の最高移動距離：unit")]
    private float _maxDistanceVer = 3.0f;

    // 現在時間の物差し
    private float _timeFramePerSec = 0f;

    // 最初のy座標
    private float _firstPosVer = 0f;

    // 無敵時間中か
    private bool _isInvisible = false;

    // 倒されたか
    private bool _isDead = false;
    public bool IsDead { get { return _isDead; } }

    // Start is called before the first frame update
    void Start()
    {
        _firstPosVer = transform.position.y;
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveWaveVerticle();
    }

    private async void PopBrokenEffecs()
    {
        foreach (GameObject effect in _brokeEffecs)
        {
            effect.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(_effectpopDuration));
        }
    }

    private void MoveWaveVerticle()
    {
        if (GameManager.Instance.State == State.AllStop) return;
        if (_isDead) return;
        _timeFramePerSec += Time.deltaTime;
        float performedPos = Mathf.Sin(_timeFramePerSec * _movespeedVer) * _maxDistanceVer;
        transform.position = new Vector3(transform.position.x, _firstPosVer + performedPos, transform.position.z);
    }

    private async void FlashSprite()
    {
        _isInvisible = true;
        _sprite.color = _invisibleColor;
        await UniTask.Delay(TimeSpan.FromSeconds(_invisibleTime));
        _sprite.color = Color.white;
        _isInvisible = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (_isInvisible) return;
            SoundManager.Instance.PlaySE(2);
            FlashSprite();
            PopEffects.Instance.PopEffectsTargetPos(collision.transform.position, 0);
            Destroy(collision.gameObject);
            DecreaceHP();
        }
    }
}
