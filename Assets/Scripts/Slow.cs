using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : MonoBehaviour
{
    private TowerWeapon towerWeapon;

    private void Awake()
    {
        towerWeapon = GetComponentInParent<TowerWeapon>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
        {
            return;
        }

        Movement2D movement2D = collision.GetComponent<Movement2D>();
        // 이동속도 = 이동속도 - 이동속도 * 감속률;
        // 4 = 5 - 5 * 0.2;
        movement2D.MoveSpeed -= movement2D.MoveSpeed * towerWeapon.Slow;
    }

    private void OnTriggerEixt2D(Collider2D collision)
    {
        if (!collision.CompareTag("Eenmy"))
        {
            return;
        }

        collision.GetComponent<Movement2D>().ResetMoveSpeed();
    }
}
