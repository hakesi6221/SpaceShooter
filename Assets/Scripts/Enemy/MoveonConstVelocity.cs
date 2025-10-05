using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveonConstVelocity : MonoBehaviour
{
    [SerializeField, Header("移動速度：秒速")]
    private float _moveSpeed = -3.0f;

    [SerializeField, Header("消滅するx座標")]
    private float _destroyPosX = -10f;

    [SerializeField, Header("進む方向")]
    private Vector3 _direction = Vector3.left;
    public void SetDirection(Vector3 direction) {  _direction = direction; }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.State == State.AllStop) return;
        Vector3 performedPosition = transform.position + _direction * _moveSpeed * Time.deltaTime;
        if (performedPosition.x < _destroyPosX) Destroy(this.gameObject);

        transform.position = performedPosition;
    }
}
