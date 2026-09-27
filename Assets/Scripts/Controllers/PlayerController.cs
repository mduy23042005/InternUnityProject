using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IUpdatable
{
    [SerializeField] private float _moveSpeed = 6f;

    private Vector2 _movement;

    public void OnEnable()
    {
        GameManager.Instance.Register(this);
    }

    public void OnDisable()
    {
        GameManager.Instance.Unregister(this);
    }

    public void OnFixedUpdate() { }

    public void OnLateUpdate() { }

    public void OnUpdate()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            _movement = mouseWorldPosition;

            this.transform.position = Vector2.MoveTowards(transform.position, _movement, _moveSpeed * Time.deltaTime);
        }
    }
}
