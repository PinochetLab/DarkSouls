using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HeroMoveController : MonoBehaviour
{
    [SerializeField] private StaminaController staminaController;

    [SerializeField] private Transform headRigTransform;

    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private Transform heroBody;
    [SerializeField] private Transform heroHeadSocket;

    [SerializeField] private float lookSpeed = 500;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float walkSpeed = 5;
    [SerializeField] private float jumpSpeed = 5;


    [SerializeField] private float height = 1.8f;
    [SerializeField] private float crouchHeight = 1.1f;
    [SerializeField] private float distanceFromEyesToTop = 0.15f;

    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheck;

    [SerializeField] private float shiftSpeed = 15;
    [SerializeField] private float shiftDistance = 1f;

    [SerializeField] private float crouchSpeed = 10f;

    [SerializeField] private HitController hitController;

    private float yaw = 0;
    private float pitch = 0;
    private const float minPitch = -90;
    private const float maxPitch = 60;

    private bool crouch = false;

    private float hor = 0;
    private float ver = 0;

    private bool shift = false;
    private Vector3 shiftVelocity = Vector3.zero;

    private float currentHeight = 0;
    private float targetHeight = 0;

    private void Awake()
    {
        currentHeight = targetHeight = capsuleCollider.height;
    }

    private void Update()
    {
        LookLogic();
        MoveLogic();
        ShiftLogic();
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            hitController.Click();
        }
    }

    private bool IsGrounded()
    {
        return Physics.OverlapSphere(groundCheck.position, 0.05f, whatIsGround).Length > 0;
    }

    private void LookLogic()
    {
        yaw += lookSpeed * Input.GetAxisRaw("Mouse X");
        pitch -= lookSpeed * Input.GetAxisRaw("Mouse Y");
        pitch = Mathf.Clamp(pitch, -maxPitch, -minPitch);
        heroBody.localEulerAngles = Vector3.up * yaw;
        heroHeadSocket.localEulerAngles = Vector3.right * pitch;

        headRigTransform.eulerAngles = Vector3.up * yaw + Vector3.right * pitch;
    }

    private void MoveLogic()
    {
        WalkLogic();
        JumpLogic();
        CrouchLogic();
    }

    private void FixedUpdate()
    {
        WalkLogicFixed();
    }

    private void SetCrouch(bool crouch)
    {
        this.crouch = crouch;
        targetHeight = crouch ? crouchHeight : height;
    }

    private void SetHeight(float height)
    {
        capsuleCollider.height = height;
        capsuleCollider.center = Vector3.up * (height / 2);
        headTransform.localPosition = Vector3.up * (height - distanceFromEyesToTop);
    }

    private void CrouchLogic()
    {
        if (currentHeight == targetHeight) return;
        var distance = Mathf.Abs(targetHeight - currentHeight);
        currentHeight = Mathf.Lerp(currentHeight, targetHeight, crouchSpeed * Time.deltaTime / distance);
        SetHeight(currentHeight);
    }

    private void ShiftLogic()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !shift)
        {
            if (staminaController.TrySpendShiftCosts())
            {
                var direction = rb.velocity;
                direction.y = 0;
                direction.Normalize();
                if (direction == Vector3.zero)
                {
                    direction = -heroBody.forward;
                }
                StartCoroutine(ShiftCor(direction * shiftSpeed));
            }
        }
    }

    private IEnumerator ShiftCor(Vector3 velocity)
    {
        shift = true;
        shiftVelocity = velocity;
        yield return new WaitForSeconds(shiftDistance / shiftSpeed);
        shift = false;
    }

    private void JumpLogic()
    {
        var newCrouch = Input.GetKey(KeyCode.LeftControl) && IsGrounded();
        if (newCrouch != crouch)
        {
            if (staminaController.TrySpendCrouchCosts())
            {
                SetCrouch(newCrouch);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            if (staminaController.TrySpendJumpCosts())
            {
                var v = rb.velocity;
                v.y = jumpSpeed;
                rb.velocity = v;
            }
        }
    }

    private void WalkLogic()
    {
        hor = Input.GetAxisRaw("Horizontal");
        ver = Input.GetAxisRaw("Vertical");
    }

    private void WalkLogicFixed()
    {
        if (shift)
        {
            rb.velocity = shiftVelocity;
            return;
        }
        var dir = (heroBody.right * hor + heroBody.forward * ver).normalized;
        var velocity = walkSpeed * dir;
        var v = rb.velocity;
        v.x = velocity.x;
        v.z = velocity.z;
        rb.velocity = v;
    }
}
