using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : BaseController
{
    [SerializeField] 
    Transform groundCheck;

    [SerializeField] 
    Transform raycastOrigin;

    [SerializeField] 
    float maxSlopeAngle;

    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    private const float RAY_DISTANCE = 2f;
    private RaycastHit slopeHit;
    private int groundLayer = 1 << (int)Define.Layer.Ground;

    public GameObject Sword;
    public GameObject HandGun;
    public GameObject ShotGun;

    PlayerStat _stat;
    Rigidbody _rb;
    CapsuleCollider _cc;
    Transform _aimPoint;

	bool _stopSkill = false;
    public float _attackRate = 3.0f;
    public float _range = 100.0f;
    private float _attackTimer;
    private bool _canDash = true;
    private bool _isDashing = true;
    private bool _canMove = true;

    public Define.Weapons Weapon { get; protected set; } = Define.Weapons.Unknown;

    public bool IsOnSlope()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out slopeHit, RAY_DISTANCE, groundLayer))
        {
            var angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle != 0f && angle < maxSlopeAngle;
        }
        return false;
    }

    protected Vector3 AdjustDirectionToSlope(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public bool IsGrounded()
    {
        Vector3 boxSize = new Vector3(transform.lossyScale.x, 0.4f, transform.lossyScale.z);
        return Physics.CheckBox(groundCheck.position, boxSize, Quaternion.identity, groundLayer);
    }
    // Quaternion.identity는 회전값이 없다는 의미입니다.

    private float CalculateNextFrameGroundAngle(float moveSpeed)
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(hAxis, 0, vAxis);

        // 다음 프레임 캐릭터 앞 부분 위치
        var nextFramePlayerPosition = raycastOrigin.position + dir * moveSpeed * Time.fixedDeltaTime;

        if (Physics.Raycast(nextFramePlayerPosition, Vector3.down, out RaycastHit hitInfo,
                            RAY_DISTANCE, groundLayer))
            return Vector3.Angle(Vector3.up, hitInfo.normal);
        return 0f;
    }

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

    /*protected override void UpdateMoving()
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
    }*/

    protected void Update()
    {
        if (IsOnSlope() || _canDash)
            _rb.useGravity = true;

        UpdateCamera();

        if (_canMove)
            UpdateMoving();

        if (Input.GetMouseButtonDown(1))
        {
            WeaponSwap();
            WeaponEquip();
        }

        if (Input.GetKeyDown(KeyCode.Space) && _canDash)
            StartCoroutine(Dash());

        NpcScript("UI_NPC_Text");

        if (Input.GetMouseButtonDown(0))
            Attack();
            

        if (_attackTimer < _attackRate)
            _attackTimer += Time.deltaTime;
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

    private void Attack()
    {
        RaycastHit _hit;
        Ray _cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 _mousePos = _cameraRay.GetPoint(5.0f);
        Vector3 _dir = (_mousePos - transform.position).normalized;

        if (Weapon == Define.Weapons.Unknown)
        {
            return;
        }

        if (Weapon == Define.Weapons.Sword)
        {

        }
        else if (Weapon == Define.Weapons.HandGun)
        {
            if (Physics.Raycast(transform.position, transform.forward, out _hit, _range))
            {
                GameObject _bullet = Managers.Resource.Instantiate("Weapon/Gun/Bullet HandGun");
                _bullet.transform.position = transform.position + transform.forward;
                Destroy(_bullet, 1.0f);
            }
        }
        else if (Weapon == Define.Weapons.ShotGun)
        {
            if (Physics.Raycast(transform.position, transform.forward, out _hit, _range))
            {
                GameObject _bullet = Managers.Resource.Instantiate("Weapon/Gun/Bullet ShotGun");
                _bullet.transform.position = transform.position + transform.forward;
                Destroy(_bullet, 1.0f);
            }
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
            Managers.UI.ShowPopupUI<UI_Popup>("Dialogue");
        }
    }

    protected override void UpdateMoving()
    {
        bool isOnSlope = IsOnSlope();
        bool isGrounded = IsGrounded();
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(hAxis, 0, vAxis);
        Vector3 velocity = CalculateNextFrameGroundAngle(_stat.MoveSpeed) < maxSlopeAngle ? dir : Vector3.zero;
        Vector3 gravity = Vector3.down * Mathf.Abs(_rb.velocity.y);

        if (isGrounded && isOnSlope)         // 경사로에 있을 때
        {
            velocity = AdjustDirectionToSlope(dir);
            gravity = Vector3.zero;
            _rb.useGravity = false;
        }
        else
        {
            _rb.useGravity = true;
        }

        if (dir.magnitude < 0.1f)
        {
            _rb.velocity = Vector3.zero;
            State = Define.State.Idle;
        }
        else
        {
            if (hAxis == 0 && vAxis == 0)
            {
                _rb.velocity = Vector3.zero;
                State = Define.State.Idle;
            }
            _rb.velocity = velocity * _stat.MoveSpeed + gravity;
            //State = Define.State.Moving;
            //float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
            //transform.position += dir.normalized * moveDist;
        }
    }

    protected void UpdateCamera()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane GroupPlane = new Plane(Vector3.up, transform.position);

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
        Plane GroupPlane = new Plane(Vector3.up, transform.position);

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
            _isDashing = false;
            yield return new WaitForSeconds(_stat.DashingCooldown);

            _canDash = true;
            _canMove = true;
            _rb.useGravity = originalGravity;
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
