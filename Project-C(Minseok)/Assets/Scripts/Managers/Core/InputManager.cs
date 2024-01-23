using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager   // 상태 체크
{
    public Action KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;
    public Action<Define.KeyEvent> Key = null;

    bool _pressed = false;
    float _pressedTime = 0;

    public void OnUpdate()
    {
        // UI버튼을 클릭했을 때 플레이어 이동 제한
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();

        if (Key != null)
        {
            if (Input.anyKey)
            {
                if (!_pressed)
                {
                    //MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    Key.Invoke(Define.KeyEvent.MoveDown);
                    _pressedTime = Time.time;
                }
                //MouseAction.Invoke(Define.MouseEvent.Press);
                Key.Invoke(Define.KeyEvent.MovePress);
                _pressed = true;
            }
            else
            {
                if (_pressed)
                {
                    /*if (Time.time < _pressedTime + 0.2f)
                        MouseAction.Invoke(Define.MouseEvent.Click);*/
                    //MouseAction.Invoke(Define.MouseEvent.PointerUp);
                    Key.Invoke(Define.KeyEvent.MoveUp);
                }
                _pressed = false;
                _pressedTime = 0;
            }
        }
    }

    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
        Key = null;
    }
}
