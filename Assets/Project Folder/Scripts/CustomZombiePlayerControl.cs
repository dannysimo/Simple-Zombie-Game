using UnityEngine;
using System.Collections;

namespace CustomZombieNamespace
{
    public class CustomZombiePlayerControl : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float turnSpeed = 200f;
        [SerializeField] private Animator m_animator = null;
        [SerializeField] private Rigidbody rigidBody = null;
        [SerializeField] private Transform cameraTransform;

        private float originalMoveSpeed;

        private float currentV = 0;
        private float currentH = 0;

        private readonly float interpolation = 10;
        private Coroutine speedBoostCoroutine;

        private void Awake()
        {
            if (!m_animator) { m_animator = GetComponent<Animator>(); }
            if (!rigidBody) { rigidBody = GetComponent<Rigidbody>(); }
            originalMoveSpeed = moveSpeed;
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            currentV = Mathf.Lerp(currentV, v, Time.deltaTime * interpolation);
            currentH = Mathf.Lerp(currentH, h, Time.deltaTime * interpolation);

            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = forward * currentV + right * currentH;

            rigidBody.MovePosition(rigidBody.position + desiredMoveDirection * moveSpeed * Time.deltaTime);

            if (desiredMoveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredMoveDirection);
                rigidBody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed));
            }

            m_animator.SetFloat("MoveSpeed", Mathf.Abs(currentV) + Mathf.Abs(currentH));
        }

        public void ApplySpeedBoost(float multiplier, float duration)
        {
            if (speedBoostCoroutine != null)
            {
                StopCoroutine(speedBoostCoroutine);
                moveSpeed = originalMoveSpeed;
            }

            speedBoostCoroutine = StartCoroutine(SpeedBoost(multiplier, duration));
        }

        private IEnumerator SpeedBoost(float multiplier, float duration)
        {
            moveSpeed = originalMoveSpeed * multiplier;
            yield return new WaitForSeconds(duration);
            moveSpeed = originalMoveSpeed;
            speedBoostCoroutine = null;
        }
    }
}