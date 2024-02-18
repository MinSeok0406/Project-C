using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bullet : MonoBehaviour
{
    public int damage;

    GameObject _player = Managers.Game.GetPlayer();

    private void Update()
    {
        transform.position += _player.transform.forward * 100.0f * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Monster")
        {
            Destroy(gameObject);
        }
    }
}
