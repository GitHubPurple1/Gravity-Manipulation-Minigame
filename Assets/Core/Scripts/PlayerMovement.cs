using UnityEngine;

//Handles relative player movement and rotation
public class PlayerMovement : MonoBehaviour
{
    #region Serialized Fields

    [Header("Transform References")]
    [SerializeField] private Transform bodyOrientation;
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private Transform cameraTransform;

    [Header("Physics")]
    [SerializeField] private LayerMask walkableMask;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float leapForce = 5f;
    [SerializeField] private float feetRadius = 0.2f;
    [SerializeField] private float gravityForce = 9.81f;
    [SerializeField] private float turnSmoothing = 10f;

    #endregion

    private Rigidbody body;
    private float inputV;
    private float inputH;

    public bool isGrounded;
    public bool isMoving;

    private Vector3 GravityAxis => transform.up.normalized;

    //Freeze Rb rotation and lock cursor
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable() => body.linearVelocity = Vector3.zero;

    //Check if player is grounded, read input and rotate body every frame
    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheckPos.position, feetRadius, walkableMask);
        ReadInput();
        RotateBody();
    }

    //Apply locomotion and downward force every fixed update
    private void FixedUpdate()
    {
        ApplyLocomotion();
        ApplyDownwardForce();
    }

    //read input from arrow keys and spacebar for movement and jumping
    private void ReadInput()
    {
        inputV = Input.GetAxisRaw("Vertical");
        inputH = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
            Leap();
    }

    //compute current movement direction based on current relative Gravity axis.
    private Vector3 ComputeHeading()
    {
        isMoving = inputV != 0 || inputH != 0;

        var fwd = Vector3.ProjectOnPlane(cameraTransform.forward, GravityAxis);
        var rgt = Vector3.ProjectOnPlane(cameraTransform.right, GravityAxis);

        var raw = fwd * inputV + rgt * inputH;

        return Vector3.ProjectOnPlane(raw.normalized, GravityAxis);
    }

    //Aply Locomotion to player and keep vertical velocity
    private void ApplyLocomotion()
    {
        var heading = ComputeHeading();
        var verticalComponent = Vector3.Project(body.linearVelocity, GravityAxis);
        body.linearVelocity = heading * speed + verticalComponent;
    }

    //Rotate body to match movement direction
    private void RotateBody()
    {
        var heading = ComputeHeading();
        if (heading.magnitude <= 0.1f) return;

        var desired = Quaternion.LookRotation(heading, GravityAxis);
        bodyOrientation.rotation = Quaternion.Slerp(
            bodyOrientation.rotation, desired, Time.deltaTime * turnSmoothing
        );
    }

    //Apply downward force to player
    private void ApplyDownwardForce() =>
        body.AddForce(-GravityAxis * gravityForce, ForceMode.Acceleration);

    //Apply leap force to player
    private void Leap() =>
        body.AddForce(GravityAxis * leapForce, ForceMode.Impulse);

    #region Debug Visuals

    private void OnDrawGizmos()
    {
        if (groundCheckPos == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckPos.position, feetRadius);
    }

    #endregion
}
