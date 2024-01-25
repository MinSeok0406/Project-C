using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Main;

        Managers.UI.ShowPopupUI<UI_Button>("Main");
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.UI.CloseAllPopupUI();

            Managers.UI.ShowPopupUI<UI_Button>("Main");
        }
    }

    public void GameStart()
    {
        Managers.Scene.LoadScene(Define.Scene.Game);
    }

    public void GameLoad()
    {
        Managers.UI.CloseAllPopupUI();

        Managers.UI.ShowPopupUI<UI_Button>("LoadScene");
    }

    public void GameSettings()
    {
        Managers.UI.CloseAllPopupUI();

        Managers.UI.ShowPopupUI<UI_Button>("SettingScene");
    }

    public void GameExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void BackKey()
    {
        Managers.UI.CloseAllPopupUI();

        Managers.UI.ShowPopupUI<UI_Button>("Main");
    }

    public override void Clear()
    {
        // 씬 끝날 때 처리
    }
}
