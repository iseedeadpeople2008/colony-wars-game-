using System.Collections.Generic;

public class GameWorld
{
    private readonly List<Territory> territories = new List<Territory>();

    public IReadOnlyList<Territory> Territories => territories;

    public Territory AddTerritory(Territory territory)
    {
        territories.Add(territory);
        return territory;
    }

    public void Tick(float dt)
    {
        for (int i = 0; i < territories.Count; i++)
            territories[i].Tick(dt);
    }
}