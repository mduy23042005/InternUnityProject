using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject playerPrefab;

    public static int idPlayer = 0;
    public static GameObject player;

    public void OnDisable()
    {
        GameManager.Instance.Unregister(this);

        ObserverManager.Unregister<PlayClickEvent>(eventData => InitPlayer());
    }

    public void OnEnable()
    {
        GameManager.Instance.Register(this);

        ObserverManager.Register<PlayClickEvent>(eventData => InitPlayer());
    }

    public void OnFixedUpdate() { }

    public void OnLateUpdate() { }

    public void OnUpdate() 
    {
        if (player == null && SceneManager.GetActiveScene().name == "Map1")
        {
            player = PoolManager.Instance.Get(playerPrefab);
            player.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Sprites/Appearance/{CacheManager.characters[idPlayer].appearance}");
        }
    }

    private void InitPlayer()
    {
        if (idPlayer != 0)       
            SceneManager.LoadScene("Map1");
    }
}