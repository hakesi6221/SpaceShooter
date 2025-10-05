using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAddScore : MonoBehaviour
{
    [SerializeField, Header("ÉXÉRÉAÇÃëùâ¡ó ")]
    private int _addScore = 1000;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            SoundManager.Instance.PlaySE(2);
            GameManager.Instance.AddScore(_addScore);
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
            PopEffects.Instance.PopEffectsTargetPos(transform.position, 0);
        }
    }
}
