using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Define.CameraMode _mode = Define.CameraMode.QuarterView;

    [SerializeField]
    Vector3 _delta = new Vector3(0.0f, 6.0f, -5.0f);

    [SerializeField]
    GameObject _player = null;

    public void SetPlayer(GameObject player) { _player = player; }

    void Start()
    {
        
    }

    void LateUpdate()
    { 
        if (_mode == Define.CameraMode.QuarterView)
        {
            if (_player.IsValid() == false)
            {
                return;
            }

            transform.position = _player.transform.position + _delta;
            transform.LookAt(_player.transform);

            // 건물 뒤에 서 있으면 투명화
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, Mathf.Infinity,
                        1 << LayerMask.NameToLayer("Block"));

            for (int i = 0; i < hits.Length; i++)
            {
                TransparentObject[] obj = hits[i].transform.GetComponentsInChildren<TransparentObject>();

                for (int j = 0; j < obj.Length; j++)
                {
                    obj[j]?.BecomeTransparent();
                }
            }
        }
    }

    public void SetQuarterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuarterView;
        _delta = delta;
    }
}
