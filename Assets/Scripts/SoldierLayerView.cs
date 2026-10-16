using System.Collections.Generic;
using UnityEngine;

public class SoldierLayerView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer soldierPrefab;

    private readonly List<SpriteRenderer> views = new List<SpriteRenderer>();
    private GameWorld world;
    private FactionPalette palette;

    public void Bind(GameWorld world, FactionPalette palette)
    {
        this.world = world;
        this.palette = palette;
    }

    private void LateUpdate()
    {
        if (world == null)
            return;

        IReadOnlyList<Soldier> soldiers = world.Soldiers;

        while (views.Count < soldiers.Count)
            views.Add(Instantiate(soldierPrefab, transform));

        for (int i = 0; i < views.Count; i++)
        {
            if (i >= soldiers.Count)
            {
                views[i].gameObject.SetActive(false);
                continue;
            }

            views[i].gameObject.SetActive(true);
            views[i].transform.position = soldiers[i].Position;
            views[i].color = palette.GetDotColor(soldiers[i].FactionId);
        }
    }
}