using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBulletMove : MonoBehaviour
{
    [SerializeField, Header("’e‘¬ •b‘¬")]
    private float _shootSpeed = 6.0f;

    [SerializeField, Header("ŽË’ö‹——£")]
    private float _range = 5.0f;

    private float _firstPosX;

    // Start is called before the first frame update
    void Start()
    {
        _firstPosX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.State == State.AllStop) return;
        Vector3 performedPosition = new Vector3(transform.position.x + (Time.deltaTime * _shootSpeed), transform.position.y, transform.position.z);
        if (performedPosition.x - _firstPosX >= _range) Destroy(this.gameObject);

        transform.position = performedPosition;
    }
}
