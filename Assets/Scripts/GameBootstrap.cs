using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    private const float MaxDeltaTime = 0.1f;

    [SerializeField] private TerritoryView territoryPrefab;
    [SerializeField] private LevelConfig level;
    [SerializeField] private FactionPalette palette;

    private GameWorld world;

    private void Start()
    {
        world = new GameWorld();

        foreach (TerritoryData data in level.Territories)
        {
            Territory territory = world.AddTerritory(new Territory(data));
            TerritoryView view = Instantiate(territoryPrefab, territory.Position, Quaternion.identity);
            view.Bind(territory, palette);
        }
    }

    private void Update()
    {
        world.Tick(Mathf.Min(Time.deltaTime, MaxDeltaTime));
    }
}