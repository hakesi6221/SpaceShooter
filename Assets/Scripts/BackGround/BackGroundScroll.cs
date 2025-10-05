using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScroll : MonoBehaviour
{
    [SerializeField, Header("スクロール速度 秒速")]
    private float _scrollSpeed = 2.0f;

    [SerializeField, Header("スクロールの終点")]
    private float _finishScrollLocalPosX = -17.5f;

    [SerializeField, Header("スクロールの始点")]
    private float _startScrollLocalPosX = -0.5f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.State == State.AllStop) return;
        Vector3 performedPosition = new Vector3(transform.position.x + (Time.deltaTime * _scrollSpeed), transform.position.y, transform.position.z);
        if (performedPosition.x <= _finishScrollLocalPosX)
        {
            performedPosition.x = _startScrollLocalPosX;
        }
        transform.position = performedPosition;
    }
}
