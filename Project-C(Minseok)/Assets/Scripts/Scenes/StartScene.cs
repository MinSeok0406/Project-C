using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Start;

        Managers.UI.ShowPopupUI<UI_Button>("Start");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Managers.Scene.LoadScene(Define.Scene.Main);
        }
    }

    public override void Clear()
    {
        // 씬 끝날 때 처리
    }

}
