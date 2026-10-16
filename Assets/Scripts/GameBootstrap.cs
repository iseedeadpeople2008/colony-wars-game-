using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    private const float MaxDeltaTime = 0.1f;

    [SerializeField] private TerritoryView territoryPrefab;
    [SerializeField] private LevelConfig level;
    [SerializeField] private FactionPalette palette;
    [SerializeField] private DragLineView dragLine;
    [SerializeField] private BattleSettings battleSettings;
    [SerializeField] private SoldierLayerView soldierLayer;
    [SerializeField] private DragInputHandler input;
    [SerializeField] private float hitRadius = 0.5f;

    private GameWorld world;
    private PlayerController playerController;

    private void Start()
    {
        world = new GameWorld(battleSettings);
        soldierLayer.Bind(world, palette);

        for (int i = 0; i < level.Territories.Count; i++)
        {
            Territory territory = world.AddTerritory(new Territory(i, level.Territories[i]));
            TerritoryView view = Instantiate(territoryPrefab, territory.Position, Quaternion.identity);
            view.Bind(territory, palette);
        }

        playerController = new PlayerController(world, input, Faction.Player, hitRadius);
        playerController.TargetSelected += (from, to) =>
        {
            var command = new AttackCommand(Faction.Player, from.Id, to.Id);
            bool accepted = world.Execute(command);
            Debug.Log($"Attack {from.Id} → {to.Id}: {(accepted ? "принято" : "отклонено")}");
        };

        playerController.GestureStarted += source =>
            dragLine.Show(source.Position, palette.GetDotColor(Faction.Player));

        playerController.GestureEnded += dragLine.Hide;
        input.DragMoved += dragLine.MoveEnd;
    }

    private void Update()
    {
        world.Tick(Mathf.Min(Time.deltaTime, MaxDeltaTime));
    }

    private void OnDestroy()
    {
        input.DragMoved -= dragLine.MoveEnd;
        playerController?.Dispose();
    }
}