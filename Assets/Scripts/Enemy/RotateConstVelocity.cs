using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateConstVelocity : MonoBehaviour
{
    [SerializeField, Header("âÒì]é≤")]
    private Vector3 _rotAxis = Vector3.zero;

    [SerializeField, Header("âÒì]ÇÃë¨ìxÅFïbë¨")]
    private float _rotSpeedSec = 1f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.State == State.AllStop) return;
        transform.rotation *= Quaternion.AngleAxis(_rotSpeedSec * Time.deltaTime, _rotAxis);
    }
}
