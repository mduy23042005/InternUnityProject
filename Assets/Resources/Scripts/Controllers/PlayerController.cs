using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IUpdatable
{
    [SerializeField] private float moveSpeed = 2f;

    [SerializeField] private LayerMask obstacleMask = ~0; // layer được coi là vật cản
    [SerializeField] private float skinWidth = 0.02f;     // khoảng hở nhỏ để không dính sát vật cản

    private Rigidbody2D rb;
    private Camera cam;
    private ContactFilter2D filter;
    private readonly RaycastHit2D[] hits = new RaycastHit2D[8];

    private Vector2 targetPosition;
    private bool isMoving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.SetLayerMask(obstacleMask);
    }

    public void OnEnable()
    {
        GameManager.Instance.Register(this);
    }
    public void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Unregister(this);
    }

    public void OnUpdate()
    {
        // Update chỉ đọc input, không di chuyển
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector3 world = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            targetPosition = world;
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    public void OnFixedUpdate()
    {
        if (!isMoving) return;

        Vector2 current = rb.position;
        Vector2 next = Vector2.MoveTowards(current, targetPosition, moveSpeed * Time.fixedDeltaTime);

        Vector2 delta = next - current;
        float distance = delta.magnitude;

        if (distance > 0.0001f)
        {
            Vector2 dir = delta / distance;

            // Cast toàn bộ collider của player theo hướng đi, đúng bằng quãng đường sắp đi
            int count = rb.Cast(dir, filter, hits, distance + skinWidth);

            if (count > 0)
            {
                // Lấy vật cản gần nhất
                float nearest = float.MaxValue;
                for (int i = 0; i < count; i++)
                    if (hits[i].distance < nearest) nearest = hits[i].distance;

                // Dừng sát vật cản thay vì đi xuyên qua
                float allowed = Mathf.Max(0f, nearest - skinWidth);
                next = current + dir * allowed;
            }
        }

        rb.MovePosition(next);
    }

    public void OnLateUpdate() { }
}