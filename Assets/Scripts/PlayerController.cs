using System;
using UnityEngine;

public class PlayerController : IDisposable
{
    public event Action<Territory> GestureStarted;
    public event Action GestureEnded;
    public event Action<Territory, Territory> TargetSelected;

    private readonly GameWorld world;
    private readonly DragInputHandler input;
    private readonly int factionId;
    private readonly float hitRadius;

    private Territory source;

    public PlayerController(GameWorld world, DragInputHandler input, int factionId, float hitRadius)
    {
        this.world = world;
        this.input = input;
        this.factionId = factionId;
        this.hitRadius = hitRadius;

        input.DragStarted += OnDragStarted;
        input.DragEnded += OnDragEnded;
    }

    public void Dispose()
    {
        input.DragStarted -= OnDragStarted;
        input.DragEnded -= OnDragEnded;
    }

    private void OnDragStarted(Vector2 point)
    {
        Territory selected = world.FindTerritoryAt(point, hitRadius);
        if (selected != null && selected.OwnerId == Faction.Player)
        {
            source = selected;
            GestureStarted?.Invoke(source);
        }
    }

    private void OnDragEnded(Vector2 point)
    {
        // 1. Если source == null, жест не шёл: выйти.
        // 2. Сохранить source в локальную переменную и сразу сбросить поле source в null.
        // 3. Поднять GestureEnded (линия скроется при любом исходе).
        // 4. Найти территорию под пальцем через world.FindTerritoryAt(point, hitRadius).
        // 5. Если она null или совпадает с исходной: выйти (отмена).
        // 6. Если исходная территория уже не принадлежит factionId: выйти.
        // 7. Поднять TargetSelected(исходная, цель).
        if (source == null)
            return;

        Territory from = source;
        source = null;
        GestureEnded?.Invoke();

        Territory to = world.FindTerritoryAt(point, hitRadius);
        if (to == null || to == from)
            return;

        if (from.OwnerId != Faction.Player)
            return;

        TargetSelected?.Invoke(from, to);
    }
}