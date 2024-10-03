using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public GameObject weapon1;
    public GameObject weapon2;
    Animator animator;

    private Rigidbody rb;
    private bool isGrounded;
    private bool weaponsSheathed = false;

    public Transform playerCamera;
    public float lookSensitivity = 2f;
    private float xRotation = 0f;

    public Transform holdPosition;
    public float pickupRange = 3f;
    public LayerMask interactableLayer;
    private GameObject heldObject = null;
    private bool isHoldingObject = false;

    void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        HandleMovement();
        HandleJump();
        HandleSheath();
        HandleLook();
        HandleInteraction();
        Wave();
    }

    void HandleMovement() {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        bool isWalking = moveX != 0 || moveZ != 0;
        animator.SetBool("IsWalking", isWalking);

        rb.MovePosition(transform.position + move * Time.deltaTime);
    }

    void HandleLook() {
        // Get the mouse inputs
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        // Rotate the player horizontally based on the Mouse X input
        transform.Rotate(Vector3.up * mouseX);

        // Adjust the camera's vertical rotation (Mouse Y) while clamping it between -90 and 90 degrees to avoid flipping
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply the rotation to the camera's local rotation
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleJump() {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("IsJumping");
        }
    }

    void HandleSheath() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            weaponsSheathed = !weaponsSheathed;

            weapon1.SetActive(!weaponsSheathed);
            weapon2.SetActive(!weaponsSheathed);
        }
    }

    void Wave() {
        if (Input.GetKeyDown(KeyCode.T)) {
            animator.SetTrigger("Wave");
            GhostFollowerController ghostController = FindObjectOfType<GhostFollowerController>();
            if (ghostController != null) {
                if (ghostController != null) {
                    float distanceToGhost = Vector3.Distance(transform.position, ghostController.transform.position);

                    if (distanceToGhost <= 2f) {
                        animator.SetTrigger("Wave");
                        ghostController.StartFollowing();
                    }
                }
            }
        }
    }

            void HandleInteraction() {
        if (!weaponsSheathed) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHoldingObject) {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange, interactableLayer);
                if (hitColliders.Length > 0) {
                    PickUpObject(hitColliders[0].gameObject);
                }
            } else {
                DropObject();
            }
        }
    }

    void PickUpObject(GameObject obj) {
        heldObject = obj;
        isHoldingObject = true;

        Rigidbody objRb = heldObject.GetComponent<Rigidbody>();
        if (objRb != null) {
            objRb.isKinematic = true;
        }

        heldObject.transform.position = holdPosition.position;
        heldObject.transform.SetParent(holdPosition);
    }

    void DropObject() {
        if (heldObject != null) {
            Rigidbody objRb = heldObject.GetComponent<Rigidbody>();
            if (objRb != null) {
                objRb.isKinematic = false;
            }
            heldObject.transform.SetParent(null);
            heldObject = null;
            isHoldingObject = false;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}