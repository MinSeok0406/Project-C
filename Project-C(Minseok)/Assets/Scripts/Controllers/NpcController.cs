using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcController : MonoBehaviour
{
    private GameObject player = Managers.Game.GetPlayer();
    public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;

    public void init()
    {
        WorldObjectType = Define.WorldObject.Npc;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.GetComponentInChildren<UI_NPC>() == null)
                Managers.UI.MakeWorldSpaceUI<UI_NPC>(player.transform, "UI_NPC_Text");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject go = Util.FindChild(player, "UI_NPC_Text", true);
        Destroy(go);
    }
}
