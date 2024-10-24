using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{

    [Header("Player")]
    [Tooltip("ĳ������ �̵� �ӵ� (m/s)")]
    public float MoveSpeed = 2.0f;

    [Tooltip("ĳ���Ͱ� �����̴� �������� ȸ���ϴ� �ӵ�")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("���� �� ���� ����")]
    public float SpeedChangeRate = 10.0f;

    [Tooltip("�ұ�Ģ�� ���鿡�� ������ ������")]
    public float GroundedOffset = -0.14f;

    [Tooltip("���� üũ�� ����� �ݰ�. CharacterController �ݰ�� ��ġ�ؾ� ��")]
    public float GroundedRadius = 0.28f;

    [Tooltip("ĳ���Ͱ� �������� ����� ���̾�")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("Cinemachine ���� ī�޶� ���� ��ǥ")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("ī�޶� ���� �̵��� �� �ִ� �ִ� ����")]
    public float TopClamp = 70.0f;

    [Tooltip("ī�޶� �Ʒ��� �̵��� �� �ִ� �ִ� ����")]
    public float BottomClamp = -30.0f;

    //player
    private float speed;
    private float animationBlend;
    private float targetRotation = 0.0f;
    private float rotationVelocity;
    private float verticalVelocity;

    private CharacterController controller;
    private Vector3? targetPosition = null; // ��ġ�� ������ ��ǥ ��ġ
    private bool isMovingToTarget = false;  // ��ġ�� ������ ��ġ�� �̵� ������ ����
    private bool isMovementBlocked = false; // ������ ���� ����

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    public void OnTouchPosition(InputValue value)
    {
        // ��ġ�� ��ǥ�� �޾ƿ�
        Vector2 touchPosition = value.Get<Vector2>();
        Debug.Log("Touch Position: " + touchPosition);
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        // �����̳� �̵� ������ ǥ�鿡 �¾��� ��
        if (Physics.Raycast(ray, out hit))
        {
            targetPosition = hit.point + new Vector3(0, transform.position.y, 0); // Ŭ���� ��ġ�� ��ǥ�� ����
            isMovingToTarget = true;    // ��ǥ �������� �̵� ����
            Debug.Log("hit Position: " + targetPosition);
        }
    }
    public void OnTouchPress(InputValue value)
    {
        Debug.Log("Touch Press: " + value.isPressed);
    }
    private void Update()
    {
        // ĳ���� �̵�
        if (!isMovementBlocked && isMovingToTarget)
        {
            Move();
        }
    }

    private void Move()
    {
        float targetSpeed = MoveSpeed;

        // ���� ���� �ӵ� ���
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;

        // ī�޶��� ������ ������� ĳ������ �̵� ���� ����
        Vector3 targetDirection;

        targetDirection = (targetPosition.Value - transform.position).normalized; // ��ǥ ���������� ����
        targetDirection.y = 0; // ���� �̵��� ó��

        // ��ǥ ���������� �Ÿ�
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition.Value);

        // ��ǥ ������ ���� ������ ��� ����
        if (distanceToTarget < 0.1f)
        {
            isMovingToTarget = false; // �̵� �Ϸ�
            targetSpeed = 0.0f;
        }
        else
        {
            // ��ǥ ������ ���� ȸ��
            targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        // ��ǥ �ӵ��� ���� �Ǵ� ����
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // �������� ��� ��� ��� ����� �� �������� �ӵ� ��ȭ ����
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                Time.deltaTime * SpeedChangeRate);

            // �ӵ��� �Ҽ��� �� �ڸ��� �ݿø�
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }


        // �÷��̾� �̵�
        controller.Move(targetDirection * (targetSpeed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
    }
}
