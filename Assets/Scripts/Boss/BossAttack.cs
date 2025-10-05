using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{

    // 通常攻撃メソッド
    private List<Func<UniTask>> _normalAttacks = new List<Func<UniTask>>();

    // 特殊攻撃メソッド
    private List<Func<UniTask>> _specialAttacks = new List<Func<UniTask>>();

    [Foldout("General"), SerializeField, Header("攻撃の間隔：秒")]
    private float _attackDuration = 2.0f;

    [Foldout("General"), SerializeField, Header("BossMovement")]
    private BossMovement _bossMove = null;

    [Foldout("General"), SerializeField, Header("特殊攻撃の頻度")]
    private int _specialAttackSpan = 5;

    [Foldout("Skill01"), SerializeField, Header("通常技1のビーム光線")]
    private GameObject _beam01;

    [Foldout("Skill01"), SerializeField, Header("通常技1のビーム光線発射位置")]
    private List<Transform> _beam01GeneratePoss = new List<Transform>();

    [Foldout("Skill01"), SerializeField, Header("ビームを打つ回数")]
    private int _fireCount01 = 5;

    [Foldout("Skill01"), SerializeField, Header("ビームを打つ間隔：秒")]
    private float _fireDurationSec01 = 1.0f;

    [Foldout("Skill01"), SerializeField, Header("プレイヤーのプレハブ")]
    private GameObject _playerPre;

    [Foldout("Skill02"), SerializeField, Header("通常技2の小惑星")]
    private GameObject _bullet02;

    [Foldout("Skill02"), SerializeField, Header("通常技2の小惑星発射位置")]
    private Transform _generatePos02 = null;

    [Foldout("Skill02"), SerializeField, Header("小惑星の角度変更の所要時間：秒")]
    private float _fireRotSpeed02 = 5;

    [Foldout("Skill02"), SerializeField, Header("小惑星を打つ間隔：秒")]
    private float _fireDurationSec02 = 1.0f;

    [Foldout("Skill02"), SerializeField, Header("小惑星を打つ角度：最大")]
    private float _fireAngleMax02 = 60.0f;

    [Foldout("Skill02"), SerializeField, Header("小惑星を打つ角度：最小")]
    private float _fireAngleMin02 = -60.0f;

    [Foldout("Skill03"), SerializeField, Header("ビームを打つ際の範囲指定")]
    private GameObject _specialBeamRange03 = null;

    [Foldout("Skill03"), SerializeField, Header("ビームを打つ際の範囲指定のフィル")]
    private GameObject _specialBeamRangeFill03 = null;

    [Foldout("Skill03"), SerializeField, Header("特殊攻撃で打つビーム")]
    private GameObject _specialBeamObj03 = null;

    [Foldout("Skill03"), SerializeField, Header("特殊攻撃でビームを打つ場所")]
    private Transform _generatePos03;

    [Foldout("Skill03"), SerializeField, Header("特殊攻撃でビーム打つまでの時間")]
    private float _specialBeamShootSpan03 = 2f;

    // 攻撃中か
    private bool _isAttack = false;

    // 打った球
    private List<GameObject> _bullets = new List<GameObject>();

    // プレイヤーのオブジェクト
    private GameObject _player;

    // 攻撃のカウント
    private int _attackCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find(_playerPre.gameObject.name);
        _specialBeamRange03 = transform.Find(_specialBeamRange03.gameObject.name).gameObject;
        _specialBeamRangeFill03 = _specialBeamRange03.transform.GetChild(0).gameObject;
        _normalAttacks = new List<Func<UniTask>>()
        {
            NormalAttack01,
            NormalAttack02,
        };
        _specialAttacks = new List<Func<UniTask>>()
        {
            SpecialAttack01,
        };
    }

    // Update is called once per frame
    void Update()
    {
        OnAttack();
    }

    private Vector3 CalcSkewedUnitVector(Vector3 forward, float angle)
    {
        Vector3 targetDir = Quaternion.Euler(new Vector3(0, 0, (angle))) * forward;
        return targetDir;
    }

    private async UniTask NormalAttack01()
    {
        var token = this.GetCancellationTokenOnDestroy();

        for (int i = 0; i < _fireCount01; i++)
        {
            for (int j = 0; j < _beam01GeneratePoss.Count; j++)
            {
                if (_bossMove.IsDead) return;
                Transform generatePos = _beam01GeneratePoss[j];
                float angle = Vector3.Angle(generatePos.right, (generatePos.position - _player.transform.position).normalized);
                if (generatePos.position.y < _player.transform.position.y) angle *= -1f;
                Debug.Log(angle.ToString());
                SoundManager.Instance.PlaySE(0);
                GameObject beam = Instantiate(_beam01, generatePos.position, Quaternion.Euler(0, 0, angle));
                _bullets.Add(beam);
                beam.GetComponent<MoveonConstVelocity>().SetDirection(CalcSkewedUnitVector(generatePos.right, angle));
            }
            await UniTask.Delay(TimeSpan.FromSeconds(_fireDurationSec01));
            await UniTask.WaitUntil(() => GameManager.Instance.State != State.AllStop);

        }
    }

    private async UniTask NormalAttack02()
    {
        var token = this.GetCancellationTokenOnDestroy();

        float angle = _fireAngleMax02;
        float rotSpeed = Mathf.Abs(_fireAngleMin02 - _fireAngleMax02) / _fireRotSpeed02;
        while (angle >= _fireAngleMin02)
        {
            if (_bossMove.IsDead) return;
            Vector3 shotVector = CalcSkewedUnitVector(_generatePos02.right, angle);
            GameObject astro = Instantiate(_bullet02, _generatePos02.position, Quaternion.identity);
            _bullets.Add(astro);
            astro.GetComponent<MoveonConstVelocity>().SetDirection(shotVector);
            angle -= (Time.deltaTime + _fireDurationSec02) * rotSpeed;
            await UniTask.Delay(TimeSpan.FromSeconds(_fireDurationSec02));
            await UniTask.WaitUntil(() => GameManager.Instance.State != State.AllStop);
        }
    }

    private async UniTask SpecialAttack01()
    {
        var token = this.GetCancellationTokenOnDestroy();

        float timePerSec = 0f;
        _specialBeamRange03.SetActive(true);
        _specialBeamRangeFill03.transform.localScale = new Vector3(_specialBeamRangeFill03.transform.localScale.x, 0, _specialBeamRangeFill03.transform.localScale.z);

        while (timePerSec <= 1)
        {
            if (_bossMove.IsDead) return;
            await UniTask.WaitUntil(() => GameManager.Instance.State != State.AllStop);
            _specialBeamRangeFill03.transform.localScale = new Vector3(_specialBeamRangeFill03.transform.localScale.x, timePerSec, _specialBeamRangeFill03.transform.localScale.z);
            timePerSec += Time.deltaTime / _specialBeamShootSpan03;
            Debug.Log(timePerSec);
            await UniTask.Yield();
        }
        _specialBeamRange03.SetActive(false);
        GameObject beam = Instantiate(_specialBeamObj03, _generatePos03.position, _specialBeamObj03.transform.rotation);
        SoundManager.Instance.PlaySE(4);
        _bullets.Add(beam);
    }

    private async void OnAttack()
    {
        var token = this.GetCancellationTokenOnDestroy();

        if (_bossMove.IsDead) return;
        if (_isAttack) return;

        if (_attackCount % _specialAttackSpan != 0 || _attackCount == 0)
        {
            _isAttack = true;
            Func<UniTask> attack = _normalAttacks[UnityEngine.Random.Range(0, _normalAttacks.Count)];
            await attack.Invoke();
            await UniTask.Delay(TimeSpan.FromSeconds(_attackDuration));
            _isAttack = false;
        }
        else if (_attackCount % _specialAttackSpan == 0 && _attackCount != 0)
        {
            _isAttack = true;
            Func<UniTask> attack = _specialAttacks[UnityEngine.Random.Range(0, _specialAttacks.Count)];
            await attack.Invoke();
            await UniTask.Delay(TimeSpan.FromSeconds(_attackDuration));
            _isAttack = false;
        }

        _attackCount++;
    }

    public void DestroyAllBullet()
    {
        foreach (GameObject bullet in _bullets)
        {
            Destroy(bullet);
        }
    }
}
