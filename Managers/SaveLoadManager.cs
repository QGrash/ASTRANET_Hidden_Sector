// Managers/SaveLoadManager.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASTRANET.Core;
using ASTRANET.Data;
using ASTRANET.Models.Entities;
using ASTRANET.Models.Instances;
using Newtonsoft.Json;

namespace ASTRANET.Managers;

public class SaveLoadManager
{
    private readonly string _saveFolder = Path.Combine(Environment.CurrentDirectory, "Saves");
    private readonly ItemManager _itemManager;
    private readonly WorldManager _worldManager;
    private readonly QuestManager _questManager;
    private readonly ReputationManager _reputationManager;
    private readonly Player _player;

    public SaveLoadManager()
    {
        try
        {
            _player = DI.Resolve<Player>();
        }
        catch
        {
            _player = new Player();
            DI.Register(_player);
        }

        _itemManager = DI.Resolve<ItemManager>();
        _worldManager = DI.Resolve<WorldManager>();
        _questManager = DI.Resolve<QuestManager>();
        _reputationManager = DI.Resolve<ReputationManager>();

        if (!Directory.Exists(_saveFolder))
            Directory.CreateDirectory(_saveFolder);
    }

    public List<string> GetSaveFiles()
    {
        if (!Directory.Exists(_saveFolder))
            return new List<string>();
        return Directory.GetFiles(_saveFolder, "*.json").Select(Path.GetFileName).ToList();
    }

    public void SaveGame(string fileName)
    {
        try
        {
            var saveData = new SaveData
            {
                Timestamp = DateTime.Now,
                PlayerName = _player.Name,
                GalaxySeed = _worldManager.Galaxy?.Seed ?? 12345,
                Player = SavePlayer(),
                World = SaveWorld(),
                Quests = SaveQuests(),
                Reputation = SaveReputation()
            };

            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(Path.Combine(_saveFolder, fileName), json);
            Console.WriteLine($"Игра сохранена в {fileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сохранения: {ex.Message}");
        }
    }

    public bool LoadGame(string fileName)
    {
        try
        {
            string path = Path.Combine(_saveFolder, fileName);
            if (!File.Exists(path))
                return false;

            string json = File.ReadAllText(path);
            var saveData = JsonConvert.DeserializeObject<SaveData>(json);
            if (saveData == null)
                return false;

            _worldManager.GenerateGalaxy(saveData.GalaxySeed);
            LoadWorld(saveData.World);
            LoadPlayer(saveData.Player);
            LoadQuests(saveData.Quests);
            LoadReputation(saveData.Reputation);

            Console.WriteLine($"Игра загружена из {fileName}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки: {ex.Message}");
            return false;
        }
    }

    private PlayerData SavePlayer()
    {
        var data = new PlayerData
        {
            Name = _player.Name,
            Strength = _player.Strength,
            Dexterity = _player.Dexterity,
            Intelligence = _player.Intelligence,
            Luck = _player.Luck,
            Charisma = _player.Charisma,
            Class = _player.Class,
            Health = _player.Health,
            MaxHealth = _player.MaxHealth,
            Hull = _player.Hull,
            MaxHull = _player.MaxHull,
            Shields = _player.Shields,
            MaxShields = _player.MaxShields,
            Energy = _player.Energy,
            MaxEnergy = _player.MaxEnergy,
            Fuel = _player.Fuel,
            MaxFuel = _player.MaxFuel,
            Level = _player.Level,
            Experience = _player.Experience,
            NextLevelExp = _player.NextLevelExp,
            Credits = _player.Credits,
            TechPoints = _player.TechPoints
        };

        foreach (var item in _player.Inventory.Items)
        {
            data.Inventory.Add(new ItemData
            {
                PrototypeId = item.PrototypeId,
                Quantity = item.Quantity,
                CurrentDurability = item.CurrentDurability
            });
        }

        if (_player.Head != null) data.Head = _player.Head.PrototypeId;
        if (_player.Torso != null) data.Torso = _player.Torso.PrototypeId;
        if (_player.Body != null) data.Body = _player.Body.PrototypeId;
        if (_player.Hands != null) data.Hands = _player.Hands.PrototypeId;
        if (_player.Legs != null) data.Legs = _player.Legs.PrototypeId;
        if (_player.Weapon != null) data.Weapon = _player.Weapon.PrototypeId;
        if (_player.Neck != null) data.Neck = _player.Neck.PrototypeId;
        foreach (var item in _player.Belt)
        {
            if (item != null)
                data.Belt.Add(item.PrototypeId);
        }

        foreach (var module in _player.InstalledModules)
        {
            data.InstalledModules.Add(new ModuleData
            {
                Item = new ItemData
                {
                    PrototypeId = module.Item.PrototypeId,
                    Quantity = module.Item.Quantity,
                    CurrentDurability = module.Item.CurrentDurability
                },
                Level = module.Level,
                TechPointsSpent = module.TechPointsSpent
            });
        }

        return data;
    }

    private void LoadPlayer(PlayerData data)
    {
        _player.Name = data.Name;
        _player.Strength = data.Strength;
        _player.Dexterity = data.Dexterity;
        _player.Intelligence = data.Intelligence;
        _player.Luck = data.Luck;
        _player.Charisma = data.Charisma;
        _player.Class = data.Class;
        _player.Health = data.Health;
        _player.MaxHealth = data.MaxHealth;
        _player.Hull = data.Hull;
        _player.MaxHull = data.MaxHull;
        _player.Shields = data.Shields;
        _player.MaxShields = data.MaxShields;
        _player.Energy = data.Energy;
        _player.MaxEnergy = data.MaxEnergy;
        _player.Fuel = data.Fuel;
        _player.MaxFuel = data.MaxFuel;
        _player.Level = data.Level;
        _player.Experience = data.Experience;
        _player.NextLevelExp = data.NextLevelExp;
        _player.Credits = data.Credits;
        _player.TechPoints = data.TechPoints;

        _player.Inventory = new Inventory();

        foreach (var itemData in data.Inventory)
        {
            var proto = _itemManager.GetPrototype(itemData.PrototypeId);
            if (proto != null)
            {
                var item = new ItemInstance(itemData.PrototypeId, itemData.Quantity)
                {
                    CurrentDurability = itemData.CurrentDurability,
                    Prototype = proto
                };
                _player.Inventory.AddItem(item);
            }
        }

        _player.Head = GetItemInstanceById(data.Head);
        _player.Torso = GetItemInstanceById(data.Torso);
        _player.Body = GetItemInstanceById(data.Body);
        _player.Hands = GetItemInstanceById(data.Hands);
        _player.Legs = GetItemInstanceById(data.Legs);
        _player.Weapon = GetItemInstanceById(data.Weapon);
        _player.Neck = GetItemInstanceById(data.Neck);
        _player.Belt.Clear();
        foreach (var id in data.Belt)
        {
            var item = GetItemInstanceById(id);
            if (item != null)
                _player.Belt.Add(item);
        }

        _player.InstalledModules.Clear();
        foreach (var moduleData in data.InstalledModules)
        {
            var proto = _itemManager.GetPrototype(moduleData.Item.PrototypeId);
            if (proto != null)
            {
                var item = new ItemInstance(moduleData.Item.PrototypeId, moduleData.Item.Quantity)
                {
                    CurrentDurability = moduleData.Item.CurrentDurability,
                    Prototype = proto
                };
                var module = new ShipModule { Item = item, Level = moduleData.Level, TechPointsSpent = moduleData.TechPointsSpent };
                _player.InstalledModules.Add(module);
            }
        }

        _player.RecalculateShipStats();
    }

    private ItemInstance GetItemInstanceById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        return _player.Inventory.Items.FirstOrDefault(i => i.PrototypeId == id);
    }

    private WorldData SaveWorld()
    {
        var data = new WorldData();
        if (_worldManager.Galaxy == null) return data;

        foreach (var system in _worldManager.Galaxy.Systems)
        {
            if (system.Visited)
                data.VisitedSystems.Add(system.Id);
            if (!system.Hidden && system.Type == SystemType.Hidden)
                data.RevealedHiddenSystems.Add(system.Id);
        }
        return data;
    }

    private void LoadWorld(WorldData data)
    {
        if (_worldManager.Galaxy == null) return;

        foreach (var system in _worldManager.Galaxy.Systems)
        {
            system.Visited = data.VisitedSystems.Contains(system.Id);
            if (data.RevealedHiddenSystems.Contains(system.Id))
                system.Hidden = false;
        }
    }

    private QuestsData SaveQuests()
    {
        var data = new QuestsData();
        foreach (var quest in _questManager.ActiveQuests)
        {
            data.ActiveQuests.Add(new QuestInstanceData
            {
                PrototypeId = quest.PrototypeId,
                State = quest.State,
                ObjectiveProgress = new List<int>(quest.ObjectiveProgress)
            });
        }
        return data;
    }

    private void LoadQuests(QuestsData data)
    {
        _questManager.ClearQuests();
        foreach (var qd in data.ActiveQuests)
        {
            _questManager.LoadQuest(qd);
        }
    }

    private ReputationData SaveReputation()
    {
        var data = new ReputationData();
        foreach (FactionId faction in Enum.GetValues(typeof(FactionId)))
        {
            data.Values[faction] = _reputationManager.GetReputation(faction);
        }
        return data;
    }

    private void LoadReputation(ReputationData data)
    {
        foreach (var kv in data.Values)
        {
            _reputationManager.SetReputation(kv.Key, kv.Value);
        }
    }
}