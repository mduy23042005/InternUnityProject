using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingController : MonoBehaviour
{
    [SerializeField] private GameObject frontWall;
    [SerializeField] private GameObject roof;

    private float fadedAlpha = 0.1f;

    private Tilemap[] frontWallTilemaps;
    private Tilemap roofTilemap;

    private Color[] frontWallOriginalColors;
    private Color roofOriginalColor;

    private void Awake()
    {
        frontWallTilemaps = frontWall.GetComponentsInChildren<Tilemap>(true);
        frontWallOriginalColors = new Color[frontWallTilemaps.Length];

        for (int i = 0; i < frontWallTilemaps.Length; i++)
        {
            frontWallOriginalColors[i] = frontWallTilemaps[i].color;
        }

        roofTilemap = roof.GetComponent<Tilemap>();
        roofOriginalColor = roofTilemap.color;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        for (int i = 0; i < frontWallTilemaps.Length; i++)
        {
            Color color = frontWallOriginalColors[i];
            color.a = fadedAlpha;
            frontWallTilemaps[i].color = color;
        }

        Color roofColor = roofOriginalColor;
        roofColor.a = fadedAlpha;
        roofTilemap.color = roofColor;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        for (int i = 0; i < frontWallTilemaps.Length; i++)
        {
            frontWallTilemaps[i].color = frontWallOriginalColors[i];
        }

        roofTilemap.color = roofOriginalColor;
    }
}