using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerControler : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 255f;
    [SerializeField] XRNode controllerNode = XRNode.RightHand;
    [SerializeField] private bool checkForJump = true;

    [Header("Capsule Collider Options")]
    [SerializeField] private Vector3 capsuleCenter = new Vector3(0, 1f, 0);
    [SerializeField] private float capsuleRadius = 0.3f;
    [SerializeField] private float capsuleHeight = 1.6f;
    [SerializeField] private CapsuleDirection capsuleDirection = CapsuleDirection.YAxis;

    private InputDevice controller;
    private bool isGrounded = true;
    private bool isButtonPressed = false;
    private Rigidbody playerRigidbody;
    private CapsuleCollider capsuleCollider;
    private List<InputDevice> devices = new List<InputDevice>();

    public enum CapsuleDirection
    {
        XAxis,
        YAxis,
        ZAxis
    }

    private void OnEnable()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.center = capsuleCenter;
        capsuleCollider.radius = capsuleRadius;
        capsuleCollider.height = capsuleHeight;
        capsuleCollider.direction = (int)capsuleDirection;
    }

    private void Start()
    {
        GetDevice();
    }

    private void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(controllerNode, devices);
        controller = devices[0];
    }

    private void Update()
    {
        if (controller == null)
        {
            GetDevice();
        }

        UpdateMovement();
    }

    private void UpdateMovement()
    {
        Vector2 primary2dValue;
        InputFeatureUsage<Vector2> primary2dAxis = CommonUsages.primary2DAxis;

        if (controller.TryGetFeatureValue(primary2dAxis, out primary2dValue))
        {
            Vector3 movement = new Vector3(primary2dValue.x, 0, primary2dValue.y);
            playerRigidbody.velocity = movement * speed;
        }
    }

}

