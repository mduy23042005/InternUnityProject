using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class BuildingController : MonoBehaviour
{
    [SerializeField] private GameObject frontWall;
    [SerializeField] private GameObject roof;

    private float fadedAlpha = 0.1f;

    private Tilemap[] frontWallTilemaps;
    private Tilemap[] roofTilemaps;

    private Color[] frontWallOriginalColors;
    private Color[] roofOriginalColors;

    private void Awake()
    {
        frontWallTilemaps = frontWall.GetComponentsInChildren<Tilemap>(true);
        frontWallOriginalColors = new Color[frontWallTilemaps.Length];

        for (int i = 0; i < frontWallTilemaps.Length; i++)
        {
            frontWallOriginalColors[i] = frontWallTilemaps[i].color;
        }

        roofTilemaps = roof.GetComponentsInChildren<Tilemap>(true);
        roofOriginalColors = new Color[roofTilemaps.Length];

        for (int i = 0; i < roofTilemaps.Length; i ++)
        {
            roofOriginalColors[i] = roofTilemaps[i].color;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (other.transform.parent != transform)
            other.transform.SetParent(transform, true);

        SetAlpha(frontWallTilemaps, frontWallOriginalColors, fadedAlpha);
        SetAlpha(roofTilemaps, roofOriginalColors, fadedAlpha);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (other.transform.parent == transform)
            other.transform.SetParent(null, true);

        Restore(frontWallTilemaps, frontWallOriginalColors);
        Restore(roofTilemaps, roofOriginalColors);
    }

    private void SetAlpha(Tilemap[] maps, Color[] originals, float a)
    {
        for (int i = 0; i < maps.Length; i++)
        {
            Color c = originals[i];
            c.a = a;
            maps[i].color = c;
        }
    }

    private void Restore(Tilemap[] maps, Color[] originals)
    {
        for (int i = 0; i < maps.Length; i++)
            maps[i].color = originals[i];
    }
}