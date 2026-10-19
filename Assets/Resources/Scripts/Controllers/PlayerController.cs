using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IUpdatable
{
    [SerializeField] private float moveSpeed = 6f;

    private Vector2 movement;

    public void OnEnable()
    {
        GameManager.Instance.Register(this);
    }

    public void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Unregister(this);
    }

    public void OnFixedUpdate() { }

    public void OnLateUpdate() { }

    public void OnUpdate()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            this.transform.position = Vector2.MoveTowards(transform.position, mouseWorldPosition, moveSpeed * Time.deltaTime);
        }
    }
}
