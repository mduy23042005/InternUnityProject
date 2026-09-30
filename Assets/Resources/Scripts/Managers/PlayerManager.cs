using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Cinemachine.CinemachinePathBase;

public class PlayerManager : MonoBehaviour, IUpdatable
{
    public static Dictionary<int, (string, string, int)> characters = new Dictionary<int, (string, string, int)>();

    [SerializeField] private GameObject playerPrefab;

    public static int idPlayer = -1;
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
            player.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Sprites/Appearance/{PlayerManager.characters[idPlayer].Item2}");
        }
    }

    private void InitPlayer()
    {
        if (idPlayer != -1)       
            SceneManager.LoadScene("Map1");
    }
}