using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : BaseController
{
	int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    public GameObject Sword;
    public GameObject HandGun;
    public GameObject ShotGun;

    PlayerStat _stat;
    Rigidbody _rb;
    CapsuleCollider _cc;
	bool _stopSkill = false;
    private bool _canDash = true;
    private bool _isDashing = true;
    private bool _canMove = true;

    public Define.Weapons Weapon { get; protected set; } = Define.Weapons.Unknown;

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Player;
        Weapon = Define.Weapons.Sword;
        _stat = gameObject.GetComponent<PlayerStat>();
        _rb = gameObject.GetComponent<Rigidbody>();
        _cc = gameObject.GetComponent<CapsuleCollider>();
        _rb.useGravity = true;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _cc.isTrigger = false;

        // 플레이어 OnMouseEvent의 중복을 피하기 위해서 (-)로 함수를 제거해주고 (+)로 다시 실행
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
    }

    protected override void UpdateMoving()
    {
        // 몬스터가 내 사정거리보다 가까우면 공격
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= 1)
            {
                State = Define.State.Skill;
                return;
            }
        }
    }

    protected void Update()
    {
        UpdateCamera();

        if (_canMove)
            UpdateMove();

        if (Input.GetMouseButtonDown(1))
        {
            WeaponSwap();
            WeaponEquip();
        }

        if (Input.GetKeyDown(KeyCode.Space) && _canDash)
            StartCoroutine(Dash());

        NpcScript("UI_NPC_Text");
    }

    private void WeaponEquip()
    {
        if (Weapon == Define.Weapons.Sword)
        {
            Sword.SetActive(true);
            ShotGun.SetActive(false);
            Debug.Log("칼");
        }
        else if (Weapon == Define.Weapons.HandGun)
        {
            HandGun.SetActive(true);
            Sword.SetActive(false);
            Debug.Log("총");
        }
        else if (Weapon == Define.Weapons.ShotGun)
        {
            ShotGun.SetActive(true);
            HandGun.SetActive(false);
            Debug.Log("샷건");
        }
    }

    private void WeaponSwap()
    {
        switch(Weapon)
        {
            case Define.Weapons.Sword:
                Weapon = Define.Weapons.HandGun;
                break;
            case Define.Weapons.HandGun:
                Weapon = Define.Weapons.ShotGun;
                break;
            case Define.Weapons.ShotGun:
                Weapon = Define.Weapons.Sword;
                break;
        }
    }

    private void NpcScript(string prefab = null)
    {
        GameObject root = Managers.UI.Root.gameObject;

        if (Util.FindChild(gameObject, prefab, true) == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F) && Util.FindChild(root, "Dialogue", true) == null)
        {
            Managers.UI.ShowPopupUI<UI_Button>("Dialogue");
        }
    }

    private void UpdateMove()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(hAxis, 0, vAxis);

        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            if (hAxis == 0 && vAxis == 0)
            {
                State = Define.State.Idle;
            }
            //State = Define.State.Moving;
            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;
        }
    }

    protected void UpdateCamera()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane GroupPlane = new Plane(Vector3.up, Vector3.zero);

        float rayLength;

        if (GroupPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointTolook = cameraRay.GetPoint(rayLength);
            transform.LookAt(new Vector3(pointTolook.x, transform.position.y, pointTolook.z));
        }
    }

    private IEnumerator Dash()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane GroupPlane = new Plane(Vector3.up, Vector3.zero);

        float rayLength;

        if (GroupPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 _mouse = cameraRay.GetPoint(rayLength);
            Vector3 dashDirection = (_mouse - transform.position).normalized;

            _canDash = false;
            _canMove = false;
            _isDashing = true;
            bool originalGravity = true;
            _rb.useGravity = false;

            _rb.velocity = dashDirection.normalized * _stat.DashingSpeed;

            yield return new WaitForSeconds(_stat.DashingTime);
            _rb.useGravity = originalGravity;
            _isDashing = false;
            yield return new WaitForSeconds(_stat.DashingCooldown);
            _canDash = true;
            _canMove = true;
            _rb.velocity = Vector3.zero;
        }

    }

    protected override void UpdateSkill()
    {
        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }
    void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            targetStat.OnAttacked(_stat);
        }

        if (_stopSkill)
        {
            State = Define.State.Idle;
        }
        else
        {
            State = Define.State.Skill;
        }
    }

    void OnMouseEvent(Define.MouseEvent evt)
    {
        switch (State)
        {
            case Define.State.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Skill:
                {
                    if (evt == Define.MouseEvent.PointerUp)
                        _stopSkill = true;
                }
                break;
        }
    }

    void OnMouseEvent_IdleRun(Define.MouseEvent evt)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);
        //Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                {
                    State = Define.State.Moving;
                    if (raycastHit)
                    {
                        _destPos = hit.point;
                        _stopSkill = false;

                        if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                            _lockTarget = hit.collider.gameObject;
                        else
                            _lockTarget = null;
                    }
                }
                break;
            case Define.MouseEvent.Press:
                {
                    if (_lockTarget == null && raycastHit)
                        _destPos = hit.point;
                }
                break;
            case Define.MouseEvent.PointerUp:
                _stopSkill = true;
                break;
        }
    }
}
