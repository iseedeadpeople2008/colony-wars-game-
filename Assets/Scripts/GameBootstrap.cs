using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    private const float MaxDeltaTime = 0.1f;

    [SerializeField] private TerritoryView territoryPrefab;
    [SerializeField] private LevelConfig level;
    [SerializeField] private SaveData save;
    [SerializeField] private FactionPalette palette;
    [SerializeField] private DragLineView dragLine;
    [SerializeField] private BattleSettings battleSettings;
    [SerializeField] private AIProfile aiProfile;
    [SerializeField] private int aiSeed = 1;
    [SerializeField] private SoldierLayerView soldierLayer;
    [SerializeField] private DragInputHandler input;
    [SerializeField] private float hitRadius = 0.5f;

    private GameWorld world;
    private PlayerController playerController;
    private AIController[] bots;

    private void Start()
    {
        battleSettings = BattleRulesBuilder.BuildSettings(battleSettings, level, save);
        world = new GameWorld(battleSettings);
        palette.Build();
        soldierLayer.Bind(world, palette);

        for (int i = 0; i < level.Territories.Count; i++)
        {
            Territory territory = world.AddTerritory(new Territory(i, level.Territories[i], battleSettings.GetRule(level.Territories[i].OwnerId).StartCount));
            TerritoryView view = Instantiate(territoryPrefab, territory.Position, Quaternion.identity);
            view.Bind(territory, palette);
        }

        playerController = new PlayerController(world, input, hitRadius);
        playerController.TargetSelected += (from, to) =>
        {
            var command = new AttackCommand(Faction.Player, from.Id, to.Id);
            bool accepted = world.Execute(command);
        };

        playerController.GestureStarted += source =>
            dragLine.Show(source.Position, palette.GetDotColor(Faction.Player));

        playerController.GestureEnded += dragLine.Hide;
        input.DragMoved += dragLine.MoveEnd;

        bots = new AIController[level.Bots.Count];
        for (int i = 0; i < level.Bots.Count; i++)
        {
            var botData = level.Bots[i];
            bots[i] = new AIController(world, botData.FactionId, botData.Profile, aiSeed + i);
        }
    }

    private void Update()
    {
        float dt = Mathf.Min(Time.deltaTime, MaxDeltaTime);
        for (int i = 0; i < bots.Length; i++)
            bots[i].Tick(dt);
        world.Tick(dt);
    }

    private void OnDestroy()
    {
        input.DragMoved -= dragLine.MoveEnd;
        playerController?.Dispose();
    }
}