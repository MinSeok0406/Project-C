using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    // Component가 없으면 생성하고 있으면 가져오는 메서드
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
		if (component == null)
            component = go.AddComponent<T>();
        return component;
	}

    // GameObject을 반환하는 메서드
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;
        
        return transform.gameObject;
    }

    // GameObject에서 자식들을 찾는 메서드
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                // GetChild : 직속 자식만 가져오는 메서드
                Transform transform = go.transform.GetChild(i);
                // name이 비어있거나 name이 일치하면 반환
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
		}
        else  // 재귀적으로 자식을 찾는 방법
        {
            // T타입과 같이 동일한 타입이면 가져오는 반복문
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                // name이 비어있거나 name이 일치하면 반환
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }


}
