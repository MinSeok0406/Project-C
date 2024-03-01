using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.World;
        //Managers.UI.ShowSceneUI<UI_Inven>();
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        gameObject.GetOrAddComponent<CursorController>();

        GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(player);

        //Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");
        GameObject go = new GameObject { name = "SpawningPool" };
        SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        pool.SetKeepMonsterCount(0);
    }

    void Update()
    {
        
    }

    public override void Clear()
    {

    }

}
