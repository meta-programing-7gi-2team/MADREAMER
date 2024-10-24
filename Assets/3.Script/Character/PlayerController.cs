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
    [Tooltip("캐릭터의 이동 속도 (m/s)")]
    public float MoveSpeed = 2.0f;

    [Tooltip("캐릭터가 움직이는 방향으로 회전하는 속도")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("가속 및 감속 비율")]
    public float SpeedChangeRate = 10.0f;

    [Tooltip("불규칙한 지면에서 유용한 오프셋")]
    public float GroundedOffset = -0.14f;

    [Tooltip("지면 체크에 사용할 반경. CharacterController 반경과 일치해야 함")]
    public float GroundedRadius = 0.28f;

    [Tooltip("캐릭터가 지면으로 사용할 레이어")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("Cinemachine 가상 카메라가 따라갈 목표")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("카메라가 위로 이동할 수 있는 최대 각도")]
    public float TopClamp = 70.0f;

    [Tooltip("카메라가 아래로 이동할 수 있는 최대 각도")]
    public float BottomClamp = -30.0f;

    //player
    private float speed;
    private float animationBlend;
    private float targetRotation = 0.0f;
    private float rotationVelocity;
    private float verticalVelocity;

    private CharacterController controller;
    private Vector3? targetPosition = null; // 터치로 설정된 목표 위치
    private bool isMovingToTarget = false;  // 터치로 설정된 위치로 이동 중인지 여부
    private bool isMovementBlocked = false; // 움직임 차단 여부

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    public void OnTouchPosition(InputValue value)
    {
        // 터치된 좌표를 받아옴
        Vector2 touchPosition = value.Get<Vector2>();
        Debug.Log("Touch Position: " + touchPosition);
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        // 지형이나 이동 가능한 표면에 맞았을 때
        if (Physics.Raycast(ray, out hit))
        {
            targetPosition = hit.point + new Vector3(0, transform.position.y, 0); // 클릭한 위치를 목표로 설정
            isMovingToTarget = true;    // 목표 지점으로 이동 시작
            Debug.Log("hit Position: " + targetPosition);
        }
    }
    public void OnTouchPress(InputValue value)
    {
        Debug.Log("Touch Press: " + value.isPressed);
    }
    private void Update()
    {
        // 캐릭터 이동
        if (!isMovementBlocked && isMovingToTarget)
        {
            Move();
        }
    }

    private void Move()
    {
        float targetSpeed = MoveSpeed;

        // 현재 수평 속도 계산
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;

        // 카메라의 방향을 기반으로 캐릭터의 이동 방향 설정
        Vector3 targetDirection;

        targetDirection = (targetPosition.Value - transform.position).normalized; // 목표 지점까지의 방향
        targetDirection.y = 0; // 수평 이동만 처리

        // 목표 지점까지의 거리
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition.Value);

        // 목표 지점에 거의 도달한 경우 멈춤
        if (distanceToTarget < 0.1f)
        {
            isMovingToTarget = false; // 이동 완료
            targetSpeed = 0.0f;
        }
        else
        {
            // 목표 지점을 향해 회전
            targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        // 목표 속도로 가속 또는 감속
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // 선형적인 결과 대신 곡선을 만들어 더 유기적인 속도 변화 제공
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                Time.deltaTime * SpeedChangeRate);

            // 속도를 소수점 세 자리로 반올림
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }


        // 플레이어 이동
        controller.Move(targetDirection * (targetSpeed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
    }
}
