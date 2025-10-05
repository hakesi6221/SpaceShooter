using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField, Header("上下の移動速度 秒速")]
    private float _movespeedVer = 1.0f;

    [SerializeField, Header("前の移動速度 秒速")]
    private float _movespeedFront = 1.0f;

    [SerializeField, Header("後ろの移動速度 秒速")]
    private float _movespeedBack = 1.0f;

    [SerializeField, Header("移動制限：上")]
    private float _moveLimitUp = 5f;

    [SerializeField, Header("移動制限：下")]
    private float _moveLimitDown = -5f;

    [SerializeField, Header("移動制限：右")]
    private float _moveLimitRight = 9f;

    [SerializeField, Header("移動制限：左")]
    private float _moveLimitLeft = -9f;

    private Vector2 _velocity = Vector2.zero;
    private Vector2 _moveDirection = Vector2.zero;

    // アニメーター
    private Animator _anim;

    // 死亡中かどうか
    private bool _isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead && !PlayerLifeManager.Instance.IsInvisible)
        {
            _isDead = false;
            _anim.SetTrigger("Recovery");
        }
        MoveCharacter();
    }

    public void OnMoveCharacter(InputAction.CallbackContext context)
    {
        Vector2 moveDirection = context.ReadValue<Vector2>();
        if (GameManager.Instance.State != State.Move)
        {
            return;
        }

        _moveDirection = moveDirection;
    }

    private void MoveCharacter()
    {
        // 上下のアニメーション制御
        if (_anim != null)
        {
            _anim.SetFloat("y", _moveDirection.y);
        }
        if (GameManager.Instance.State != State.Move)
        {
            _velocity = Vector2.zero;
            return;
        }
        // 移動入力がないなら速度を0にしてリターン
        if (_moveDirection == Vector2.zero)
        {
            _velocity = Vector2.zero;
            return;
        }

        float movespeedHor = 0f;
        if (_moveDirection.x > 0f)
        {
            movespeedHor = _movespeedFront;
        }
        else if (_moveDirection.x < 0f)
        {
            movespeedHor = _movespeedBack;
        }

        _velocity = new Vector2(_moveDirection.x * movespeedHor, _moveDirection.y * _movespeedVer);

        transform.position = UpdatePosition(transform.position, _velocity);

    }

    private Vector3 UpdatePosition(Vector3 currentPosition, Vector3 velocity)
    {
        Vector3 fixedPosition = currentPosition + (velocity * Time.deltaTime);
        fixedPosition = new Vector3(Mathf.Clamp(fixedPosition.x, _moveLimitLeft, _moveLimitRight), Mathf.Clamp(fixedPosition.y, _moveLimitDown, _moveLimitUp), fixedPosition.z);
        return fixedPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PlayerLifeManager.Instance.IsInvisible) return;
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss"))
        {
            SoundManager.Instance.PlaySE(1);
            _anim.SetTrigger("Invisible");
            GameManager.Instance.ChangeDeath();
            _moveDirection = Vector3.zero;
            PopEffects.Instance.PopEffectsTargetPos(transform.position, 1);
            //this.gameObject.SetActive(false);
            _isDead = true;
            PlayerLifeManager.Instance.DeathPlayer();
        }
    }
}
