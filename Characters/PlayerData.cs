#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;
using SubstanceAbuse.Common;
using SubstanceAbuse.Items;

namespace SubstanceAbuse.Characters;

/// <summary>Данные персонажа игрока. Обычный C#-класс, пригодный для будущей сериализации.</summary>
public class PlayerData
{
    // ---- Константы баланса ----
    public const int StartXpToLevelUp = 10;
    public const int XpToLevelUpStep = 5;
    public const double DamageModPerDedication = 0.15;
    public const int MaxDedicationLevel = 10;

    private readonly List<ItemData> _inventory = new();
    private readonly Func<int, int, int> _roll;

    /// <param name="rollInclusive">Источник случайных чисел [min; max] включительно.
    /// По умолчанию — общий Rng проекта (см. DefaultRoll).</param>
    public PlayerData(Func<int, int, int>? rollInclusive = null)
        => _roll = rollInclusive ?? DefaultRoll;

    private static int DefaultRoll(int min, int max)
    {
        try
        {
            if (Core.Instance?.Random is { } rng) return rng.Range(min, max);
        }
        catch (Exception ex)
        {
            GameLog.Error(ex, nameof(DefaultRoll));
        }

        // Запасной вариант, если autoload Core ещё не создан (например, одиночный запуск сцены).
        return GD.RandRange(min, max);
    }

    // ---- Состояние ----
    public IReadOnlyList<ItemData> Inventory => _inventory;
    public ItemData? Weapon { get; private set; }
    public ItemData? Armor { get; private set; }

    public int XP { get; private set; } = 0;
    public int XPtoLVLup { get; private set; } = StartXpToLevelUp;
    public int Level { get; private set; } = 1;
    public int UpgradePoints { get; private set; } = 0;

    public int Dedication { get; private set; } = 0;
    public int Aspiration { get; private set; } = 0;
    public int Negotiation { get; private set; } = 0;
    public int Reservation { get; private set; } = 0;

    public double DamageMod { get; private set; } = 1.0;

    public int StaminaCurrent { get; private set; } = 5;
    public int StaminaMax { get; private set; } = 5;
    public int HealthPointsCurrent { get; private set; } = 10;
    public int HealthPointsMax { get; private set; } = 10;
    public int SanityPointsCurrent { get; private set; } = 5;
    public int SanityPointsMax { get; private set; } = 5;

    public NotebookData Notebook { get; } = new();

    // ---- События (C#) ----
    public event Action? StatsChanged;
    public event Action? InventoryChanged;
    public event Action? EquipmentChanged;

    // ---- Опыт и уровни ----
    /// <summary>Выдаёт опыт. После выдачи повышает уровень столько раз, сколько нужно.</summary>
    public void GiveXP(int amount)
    {
        Safe(() =>
        {
            if (amount <= 0)
            {
                GameLog.Warn($"Некорректное количество опыта: {amount}");
                return false;
            }

            XP += amount;
            int gained = 0;
            while (XP >= XPtoLVLup) // единственное место сравнения порога
            {
                XP -= XPtoLVLup;
                Level++;
                UpgradePoints++;
                XPtoLVLup += XpToLevelUpStep;
                gained++;
            }

            GameLog.Info($"Получено {amount} XP. Уровень {Level}, XP {XP}/{XPtoLVLup}, очков улучшений {UpgradePoints}"
                         + (gained > 0 ? $" (+{gained} ур.)" : ""));
            StatsChanged?.Invoke();
            return true;
        });
    }

    // ---- Прокачка ----
    public bool UpgradeDedication() => Safe(() =>
    {
        if (Dedication >= MaxDedicationLevel)
        {
            GameLog.Warn($"Dedication уже на максимуме ({MaxDedicationLevel})");
            return false;
        }

        if (!TrySpendUpgradePoint()) return false;

        Dedication++;
        DamageMod = Math.Round(DamageMod + DamageModPerDedication, 2);
        GameLog.Info($"Dedication -> {Dedication}, DamageMod = {DamageMod:0.00}");
        StatsChanged?.Invoke();
        return true;
    });

    public bool UpgradeAspiration() => Safe(() =>
    {
        if (!TrySpendUpgradePoint()) return false;

        Aspiration++;
        StaminaMax += 1;
        int bonus = _roll(1, 4);
        StaminaCurrent = Math.Min(StaminaCurrent + bonus, StaminaMax);
        GameLog.Info($"Aspiration -> {Aspiration}, стамина {StaminaCurrent}/{StaminaMax} (бонус {bonus})");
        StatsChanged?.Invoke();
        return true;
    });

    public bool UpgradeNegotiation() => Safe(() =>
    {
        if (!TrySpendUpgradePoint()) return false;

        Negotiation++;
        HealthPointsMax += 4;
        int bonus = _roll(4, 10);
        HealthPointsCurrent = Math.Min(HealthPointsCurrent + bonus, HealthPointsMax);
        GameLog.Info($"Negotiation -> {Negotiation}, здоровье {HealthPointsCurrent}/{HealthPointsMax} (бонус {bonus})");
        StatsChanged?.Invoke();
        return true;
    });

    public bool UpgradeReservation() => Safe(() =>
    {
        if (!TrySpendUpgradePoint()) return false;

        Reservation++;
        SanityPointsMax += 1;
        int bonus = _roll(1, 2);
        SanityPointsCurrent = Math.Min(SanityPointsCurrent + bonus, SanityPointsMax);
        GameLog.Info($"Reservation -> {Reservation}, менталка {SanityPointsCurrent}/{SanityPointsMax} (бонус {bonus})");
        StatsChanged?.Invoke();
        return true;
    });

    private bool TrySpendUpgradePoint()
    {
        if (UpgradePoints <= 0)
        {
            GameLog.Warn("Нет очков улучшений");
            return false;
        }

        UpgradePoints--;
        return true;
    }

    // ---- Вспомогательные изменения параметров ----
    // Значение всегда зажимается в [0; Max]. Логику смерти не реализуем.
    public void ModifyHealth(int delta) => Safe(() =>
    {
        HealthPointsCurrent = Math.Clamp(HealthPointsCurrent + delta, 0, HealthPointsMax);
        GameLog.Info($"Здоровье изменено на {delta}: {HealthPointsCurrent}/{HealthPointsMax}");
        StatsChanged?.Invoke();
        return true;
    });

    public void ModifyStamina(int delta) => Safe(() =>
    {
        StaminaCurrent = Math.Clamp(StaminaCurrent + delta, 0, StaminaMax);
        GameLog.Info($"Стамина изменена на {delta}: {StaminaCurrent}/{StaminaMax}");
        StatsChanged?.Invoke();
        return true;
    });

    public void ModifySanity(int delta) => Safe(() =>
    {
        SanityPointsCurrent = Math.Clamp(SanityPointsCurrent + delta, 0, SanityPointsMax);
        GameLog.Info($"Менталка изменена на {delta}: {SanityPointsCurrent}/{SanityPointsMax}");
        StatsChanged?.Invoke();
        return true;
    });

    // ---- Инвентарь ----
    public bool AddItem(ItemData? item) => Safe(() =>
    {
        if (item == null)
        {
            GameLog.Warn("Попытка добавить null-предмет");
            return false;
        }

        _inventory.Add(item);
        GameLog.Info($"В инвентарь добавлен: {item.ItemName} [{item.ItemID}]");
        InventoryChanged?.Invoke();
        return true;
    });

    public bool RemoveItem(ItemData? item) => Safe(() =>
    {
        if (item == null || !_inventory.Remove(item))
        {
            GameLog.Warn("Предмета нет в инвентаре");
            return false;
        }

        GameLog.Info($"Из инвентаря удалён: {item.ItemName}");
        InventoryChanged?.Invoke();
        return true;
    });

    // ---- Экипировка (экипированный предмет в Inventory не хранится) ----
    public bool EquipWeapon(ItemData? item) => Safe(() =>
    {
        if (item is not WeaponData)
        {
            GameLog.Warn($"'{item?.ItemName}' не оружие");
            return false;
        }

        if (!_inventory.Remove(item))
        {
            GameLog.Warn($"Оружия '{item.ItemName}' нет в инвентаре");
            return false;
        }

        if (Weapon != null) _inventory.Add(Weapon);
        Weapon = item;
        GameLog.Info($"Экипировано оружие: {item.ItemName}");
        InventoryChanged?.Invoke();
        EquipmentChanged?.Invoke();
        return true;
    });

    public bool UnequipWeapon() => Safe(() =>
    {
        if (Weapon == null)
        {
            GameLog.Warn("Оружие не экипировано");
            return false;
        }

        _inventory.Add(Weapon);
        GameLog.Info($"Оружие снято: {Weapon.ItemName}");
        Weapon = null;
        InventoryChanged?.Invoke();
        EquipmentChanged?.Invoke();
        return true;
    });

    public bool EquipArmor(ItemData? item) => Safe(() =>
    {
        if (item is not ArmorData)
        {
            GameLog.Warn($"'{item?.ItemName}' не броня");
            return false;
        }

        if (!_inventory.Remove(item))
        {
            GameLog.Warn($"Брони '{item.ItemName}' нет в инвентаре");
            return false;
        }

        if (Armor != null) _inventory.Add(Armor);
        Armor = item;
        GameLog.Info($"Экипирована броня: {item.ItemName}");
        InventoryChanged?.Invoke();
        EquipmentChanged?.Invoke();
        return true;
    });

    public bool UnequipArmor() => Safe(() =>
    {
        if (Armor == null)
        {
            GameLog.Warn("Броня не экипирована");
            return false;
        }

        _inventory.Add(Armor);
        GameLog.Info($"Броня снята: {Armor.ItemName}");
        Armor = null;
        InventoryChanged?.Invoke();
        EquipmentChanged?.Invoke();
        return true;
    });

    // ---- Защита от исключений ----
    private bool Safe(Func<bool> body, [CallerMemberName] string member = "")
    {
        try
        {
            return body();
        }
        catch (Exception ex)
        {
            GameLog.Error(ex, member);
            return false;
        }
    }
}
