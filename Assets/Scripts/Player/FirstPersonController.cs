using UnityEngine;

namespace Unobserved
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonController : MonoBehaviour
    {
        public Camera eye;
        public float sensitivity = 1.8f;
        public float speed = 4.3f;
        CharacterController controller;
        Vector3 planar;
        float pitch, vertical, coyote, jumpBuffer, stepTimer;
        public Vector3 Checkpoint { get; set; }
        public float CheckpointYaw { get; set; }
        void Awake() => controller = GetComponent<CharacterController>();
        void Update()
        {
            if (!GameFlow.Playing) return;
            transform.Rotate(0, Input.GetAxisRaw("Mouse X") * sensitivity, 0);
            pitch = Mathf.Clamp(pitch - Input.GetAxisRaw("Mouse Y") * sensitivity, -82, 82);
            eye.transform.localRotation = Quaternion.Euler(pitch,0,0);
            if (controller.isGrounded) { coyote = .12f; if (vertical < 0) vertical = -2; }
            else coyote -= Time.deltaTime;
            if (Input.GetButtonDown("Jump")) jumpBuffer = .12f; else jumpBuffer -= Time.deltaTime;
            if (jumpBuffer > 0 && coyote > 0) { vertical = 6.6f; coyote = 0; jumpBuffer = 0; }
            Vector3 wish = Vector3.ClampMagnitude(transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical"),1) * speed;
            planar = Vector3.MoveTowards(planar,wish,(controller.isGrounded ? 30 : 11)*Time.deltaTime);
            vertical -= 19 * Time.deltaTime;
            // Separate ground-parallel travel from gravity so downward pressure cannot
            // cancel forward travel on a shallow collision ramp at very high frame rates.
            controller.Move(planar*Time.deltaTime);
            controller.Move(Vector3.up*vertical*Time.deltaTime);
            if (controller.isGrounded && planar.sqrMagnitude > 1)
            {
                stepTimer -= Time.deltaTime;
                if (stepTimer <= 0) { stepTimer = .48f; Soundscape.Instance.Footstep(transform.position); }
            }
            if (transform.position.y < -7) GameFlow.Instance.Respawn();
        }
        public void Teleport(Vector3 position)
        {
            controller.enabled = false; transform.position = position; controller.enabled = true;
            vertical = 0; planar = Vector3.zero;
        }
    }
}
