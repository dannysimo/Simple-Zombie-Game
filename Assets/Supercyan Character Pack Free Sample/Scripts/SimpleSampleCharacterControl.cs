using System.Collections.Generic;
using UnityEngine;

namespace Supercyan.FreeSample
{
    public class SimpleTankCharacterControl : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float turnSpeed = 200f;
        [SerializeField] private float jumpForce = 4f;
        [SerializeField] private Animator animator = null;
        [SerializeField] private Rigidbody rigidBody = null;
        [SerializeField] private Transform cameraTransform;

        private float currentV = 0;
        private float currentH = 0;
        private readonly float interpolation = 10;
        private bool wasGrounded;
        private bool isGrounded;
        private List<Collider> collisions = new List<Collider>();
        private float jumpTimeStamp = 0;
        private float minJumpInterval = 0.25f;
        private bool jumpInput = false;

        private void Awake()
        {
            if (!animator) { animator = gameObject.GetComponent<Animator>(); }
            if (!rigidBody) { rigidBody = gameObject.GetComponent<Rigidbody>(); }
        }

        private void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
                {
                    if (!collisions.Contains(collision.collider))
                    {
                        collisions.Add(collision.collider);
                    }
                    isGrounded = true;
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            bool validSurfaceNormal = false;
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
                {
                    validSurfaceNormal = true;
                    break;
                }
            }

            if (validSurfaceNormal)
            {
                isGrounded = true;
                if (!collisions.Contains(collision.collider))
                {
                    collisions.Add(collision.collider);
                }
            }
            else
            {
                if (collisions.Contains(collision.collider))
                {
                    collisions.Remove(collision.collider);
                }
                if (collisions.Count == 0) { isGrounded = false; }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collisions.Contains(collision.collider))
            {
                collisions.Remove(collision.collider);
            }
            if (collisions.Count == 0) { isGrounded = false; }
        }

        private void Update()
        {
            if (!jumpInput && Input.GetKey(KeyCode.Space))
            {
                jumpInput = true;
            }
        }

        private void FixedUpdate()
        {
            animator.SetBool("Grounded", isGrounded);
            Move();
            StepUp();
            wasGrounded = isGrounded;
            jumpInput = false;
        }

        private void Move()
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            currentV = Mathf.Lerp(currentV, v, Time.deltaTime * interpolation);
            currentH = Mathf.Lerp(currentH, h, Time.deltaTime * interpolation);

            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = forward * currentV + right * currentH;

            rigidBody.MovePosition(rigidBody.position + desiredMoveDirection * moveSpeed * Time.deltaTime);

            if (desiredMoveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredMoveDirection);
                rigidBody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed));
            }

            animator.SetFloat("MoveSpeed", Mathf.Abs(currentV) + Mathf.Abs(currentH));

            JumpingAndLanding();
        }

        private void StepUp()
        {
            float stepHeight = 0.2f;
            float stepDistance = 0.5f;
            RaycastHit hitLower;
            RaycastHit hitUpper;

            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            if (Physics.Raycast(rayOrigin, transform.forward, out hitLower, stepDistance))
            {
                Vector3 upperRayOrigin = rayOrigin + Vector3.up * stepHeight;
                if (!Physics.Raycast(upperRayOrigin, transform.forward, out hitUpper, stepDistance))
                {
                    rigidBody.position += new Vector3(0, stepHeight, 0);
                }
            }
        }

        private void JumpingAndLanding()
        {
            bool jumpCooldownOver = (Time.time - jumpTimeStamp) >= minJumpInterval;

            if (jumpCooldownOver && isGrounded && jumpInput)
            {
                jumpTimeStamp = Time.time;
                rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                animator.SetTrigger("Jump");
            }
        }
    }
}