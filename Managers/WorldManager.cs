// Managers/WorldManager.cs
using ASTRANET.Generators;
using ASTRANET.Models.World;

namespace ASTRANET.Managers;

public class WorldManager
{
    private Galaxy _galaxy;
    private StarSystem _currentSystem;
    private LocalMap _currentLocalMap;

    public Galaxy Galaxy => _galaxy;
    public StarSystem CurrentSystem => _currentSystem;
    public LocalMap CurrentLocalMap => _currentLocalMap;

    public void GenerateGalaxy(int seed)
    {
        _galaxy = GalaxyGenerator.Generate(seed);
    }

    public bool TryEnterSystem(int systemIndex, int fuelCost = 10)
    {
        if (_galaxy == null || systemIndex < 0 || systemIndex >= _galaxy.Systems.Count)
            return false;

        var targetSystem = _galaxy.Systems[systemIndex];

        if (_currentSystem != null && !_currentSystem.ConnectedSystems.Contains(targetSystem.Id))
            return false;

        _currentSystem = targetSystem;
        _currentLocalMap = LocalMapGenerator.Generate(_currentSystem);
        _currentSystem.Visited = true;
        return true;
    }

    public void ExitToGalaxy()
    {
        _currentLocalMap = null;
        _currentSystem = null;
    }
}