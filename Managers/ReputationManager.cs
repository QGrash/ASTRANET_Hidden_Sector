// Managers/ReputationManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using ASTRANET.Core;
using ASTRANET.Models.Combat;
using ASTRANET.Models.Entities;

namespace ASTRANET.Managers;

public class ReputationManager
{
    private readonly Dictionary<FactionId, int> _reputation = new();

    public ReputationManager()
    {
        _reputation[FactionId.SolarFederation] = 0;
        _reputation[FactionId.PyroThetaEmpire] = -20;
        _reputation[FactionId.AquaTarrissRepublic] = 10;
        _reputation[FactionId.BrogNomads] = -30;
        _reputation[FactionId.GreenCrossFoundation] = 20;
        _reputation[FactionId.MercArmory] = 0;
        _reputation[FactionId.CultOfVoid] = -50;
        _reputation[FactionId.TarrshakAqua] = 5;
        _reputation[FactionId.MegaTransen] = 0;
        _reputation[FactionId.CrimsonVoyagers] = -40;
        _reputation[FactionId.VoidDragons] = -40;

        EventBus.Subscribe<SOSBeaconActivatedEvent>(OnSOSActivated);
        EventBus.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
    }

    public int GetReputation(FactionId faction)
    {
        return _reputation.TryGetValue(faction, out int value) ? value : 0;
    }

    public ReputationLevel GetReputationLevel(FactionId faction)
    {
        int value = GetReputation(faction);
        if (value <= -51) return ReputationLevel.Hostile;
        if (value <= -1) return ReputationLevel.Negative;
        if (value <= 20) return ReputationLevel.Neutral;
        if (value <= 50) return ReputationLevel.Friendly;
        return ReputationLevel.Ally;
    }

    public void AddReputation(FactionId faction, int amount)
    {
        if (!_reputation.ContainsKey(faction)) return;
        int oldValue = _reputation[faction];
        _reputation[faction] = Math.Clamp(oldValue + amount, -100, 100);
        int newValue = _reputation[faction];
        if (oldValue != newValue)
        {
            EventBus.Publish(new ReputationChangedEvent
            {
                Faction = faction,
                OldValue = oldValue,
                NewValue = newValue
            });
        }
    }

    // Прямая установка (для загрузки)
    public void SetReputation(FactionId faction, int value)
    {
        if (!_reputation.ContainsKey(faction)) return;
        int oldValue = _reputation[faction];
        _reputation[faction] = Math.Clamp(value, -100, 100);
        int newValue = _reputation[faction];
        if (oldValue != newValue)
        {
            EventBus.Publish(new ReputationChangedEvent
            {
                Faction = faction,
                OldValue = oldValue,
                NewValue = newValue
            });
        }
    }

    public double GetPriceModifier(FactionId? faction)
    {
        double repMod = 1.0;
        if (faction.HasValue)
        {
            repMod = GetReputationLevel(faction.Value) switch
            {
                ReputationLevel.Hostile => 2.0,
                ReputationLevel.Negative => 1.3,
                ReputationLevel.Neutral => 1.0,
                ReputationLevel.Friendly => 0.9,
                ReputationLevel.Ally => 0.8,
                _ => 1.0
            };
        }

        try
        {
            var player = DI.Resolve<Player>();
            double charismaMod = 1.0 + (player.Charisma - 10) * 0.02;
            if (charismaMod < 0.7) charismaMod = 0.7;
            if (charismaMod > 1.3) charismaMod = 1.3;
            return repMod * charismaMod;
        }
        catch
        {
            return repMod;
        }
    }

    private void OnSOSActivated(SOSBeaconActivatedEvent evt)
    {
        AddReputation(FactionId.GreenCrossFoundation, 5);
        AddReputation(FactionId.CrimsonVoyagers, -5);
    }

    private void OnEnemyKilled(EnemyKilledEvent evt)
    {
        AddReputation(evt.EnemyFaction, -5);
        if (evt.EnemyFaction == FactionId.CrimsonVoyagers || evt.EnemyFaction == FactionId.VoidDragons)
        {
            AddReputation(FactionId.SolarFederation, 2);
            AddReputation(FactionId.MercArmory, 1);
        }

        if (evt.EnemyId == "ancient_dreadnought")
        {
            AddReputation(FactionId.SolarFederation, 50);
            AddReputation(FactionId.PyroThetaEmpire, 30);
            AddReputation(FactionId.AquaTarrissRepublic, 30);
            AddReputation(FactionId.GreenCrossFoundation, 40);
        }
    }
}