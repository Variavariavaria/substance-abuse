#if DEBUG
#nullable enable
using System;
using System.Linq;
using SubstanceAbuse.Characters;
using SubstanceAbuse.Common;
using SubstanceAbuse.Items;

namespace SubstanceAbuse.Dev;

/// <summary>Самопроверка логики PlayerData. Запускается по F9, ничего не ломает при провале.</summary>
public static class PlayerDataSelfTest
{
    private static int _passed;
    private static int _total;

    public static void Run()
    {
        _passed = 0;
        _total = 0;
        int logCount = 0;
        int warningCount = 0;

        void OnMessage(LogLevel level, string text)
        {
            logCount++;
            if (level == LogLevel.Warning) warningCount++;
        }

        GameLog.MessageLogged += OnMessage;
        try
        {
            Scenario01();
            Scenario02And03();
            Scenario04();
            Scenario05();
            Scenario06();
            Scenario07();
            Scenario08();
            Scenario09();
            Scenario10And11();
            Scenario12And13();
            Scenario14And15And16();
            Scenario17And18();
            Scenario19To22();
            Scenario23And24();
            Check("25. Все вызовы пишут в консоль, false дают WARNING",
                logCount > 0 && warningCount > 0);
        }
        catch (Exception ex)
        {
            GameLog.Error(ex, nameof(Run));
        }
        finally
        {
            GameLog.MessageLogged -= OnMessage;
        }

        GameLog.Info($"Самопроверка PlayerData: {_passed}/{_total} пройдено");
    }

    private static void Scenario01()
    {
        PlayerData player = NewPlayer();
        Check("1. Стартовое состояние",
            player.XP == 0 && player.XPtoLVLup == 10 && player.Level == 1 &&
            player.UpgradePoints == 0 &&
            player.Dedication == 0 && player.Aspiration == 0 &&
            player.Negotiation == 0 && player.Reservation == 0 &&
            Almost(player.DamageMod, 1.0) &&
            player.StaminaCurrent == 5 && player.StaminaMax == 5 &&
            player.HealthPointsCurrent == 10 && player.HealthPointsMax == 10 &&
            player.SanityPointsCurrent == 5 && player.SanityPointsMax == 5 &&
            player.Weapon == null && player.Armor == null && player.Inventory.Count == 0);
    }

    private static void Scenario02And03()
    {
        PlayerData player = NewPlayer();
        player.GiveXP(9);
        Check("2. GiveXP(9): без левел-апа",
            player.Level == 1 && player.XP == 9 && player.UpgradePoints == 0);

        player.GiveXP(1);
        Check("3. GiveXP(1): уровень 2, порог 15",
            player.Level == 2 && player.XP == 0 && player.XPtoLVLup == 15 &&
            player.UpgradePoints == 1);
    }

    private static void Scenario04()
    {
        PlayerData player = NewPlayer();
        player.GiveXP(30);
        Check("4. GiveXP(30): двойной левел-ап",
            player.Level == 3 && player.XP == 5 && player.XPtoLVLup == 20 &&
            player.UpgradePoints == 2);
    }

    private static void Scenario05()
    {
        PlayerData player = NewPlayer();
        player.GiveXP(0);
        player.GiveXP(-5);
        Check("5. GiveXP(0) и GiveXP(-5): ничего не меняется",
            player.Level == 1 && player.XP == 0 && player.UpgradePoints == 0);
    }

    private static void Scenario06()
    {
        PlayerData player = NewPlayer();
        bool upgraded = player.UpgradeDedication();
        Check("6. Прокачка без очков возвращает false",
            !upgraded && player.Dedication == 0 && Almost(player.DamageMod, 1.0));
    }

    private static void Scenario07()
    {
        PlayerData player = NewPlayer();
        player.GiveXP(10);
        bool upgraded = player.UpgradeDedication();
        Check("7. Dedication 1, DamageMod 1.15",
            upgraded && player.Dedication == 1 && Almost(player.DamageMod, 1.15) &&
            player.UpgradePoints == 0);
    }

    private static void Scenario08()
    {
        PlayerData player = NewPlayer();
        player.GiveXP(10);
        player.UpgradeDedication();
        player.GiveXP(35);
        player.UpgradeDedication();
        player.UpgradeDedication();
        Check("8. Dedication 3, DamageMod 1.45 без хвоста",
            player.Dedication == 3 && Almost(player.DamageMod, 1.45) &&
            player.UpgradePoints == 0);
    }

    private static void Scenario09()
    {
        PlayerData player = NewPlayer();
        player.GiveXP(325);

        bool allUpgraded = true;
        for (int i = 0; i < PlayerData.MaxDedicationLevel; i++)
        {
            allUpgraded &= player.UpgradeDedication();
        }

        player.GiveXP(60);
        int pointsBefore = player.UpgradePoints;
        bool eleventh = player.UpgradeDedication();

        Check("9. Лимит Dedication 10, очко не тратится",
            allUpgraded && !eleventh && pointsBefore == 1 &&
            player.UpgradePoints == 1 && player.Dedication == 10 &&
            Almost(player.DamageMod, 2.50));
    }

    private static void Scenario10And11()
    {
        PlayerData player = NewPlayer();
        player.GiveXP(10);
        bool first = player.UpgradeAspiration();
        Check("10. Aspiration при 5/5: максимум 6, текущее 6",
            first && player.Aspiration == 1 && player.StaminaMax == 6 &&
            player.StaminaCurrent == 6);

        player.GiveXP(15);
        player.ModifyStamina(-5);
        bool second = player.UpgradeAspiration();
        Check("11. Aspiration из 1/6 с броском 1: 2/7",
            second && player.StaminaMax == 7 && player.StaminaCurrent == 2);
    }

    private static void Scenario12And13()
    {
        PlayerData player = NewPlayer();
        player.GiveXP(10);
        bool first = player.UpgradeNegotiation();
        Check("12. Negotiation при 10/10: максимум 14, текущее 14",
            first && player.Negotiation == 1 && player.HealthPointsMax == 14 &&
            player.HealthPointsCurrent == 14);

        // В ТЗ ожидается старт 1/14: доводим здоровье до 10/14, затем наносим 9 урона.
        player.ModifyHealth(-4);
        player.ModifyHealth(-9);
        player.GiveXP(15);
        bool second = player.UpgradeNegotiation();
        Check("13. Negotiation с 1/14 и броском 4: 5/18",
            second && player.HealthPointsMax == 18 && player.HealthPointsCurrent == 5);
    }

    private static void Scenario14And15And16()
    {
        PlayerData player = NewPlayer();
        player.GiveXP(10);
        bool first = player.UpgradeReservation();
        Check("14. Reservation при 5/5: максимум 6, текущее 6",
            first && player.Reservation == 1 && player.SanityPointsMax == 6 &&
            player.SanityPointsCurrent == 6);

        // В ТЗ ожидается старт 1/6: доводим менталку до 5/6, затем снимаем 4.
        player.ModifySanity(-1);
        player.ModifySanity(-4);
        player.GiveXP(15);
        bool second = player.UpgradeReservation();
        Check("15. Reservation с 1/6 и броском 1: 2/7",
            second && player.SanityPointsMax == 7 && player.SanityPointsCurrent == 2);

        player.ModifyHealth(-100);
        bool clampedLow = player.HealthPointsCurrent == 0;
        player.ModifyHealth(100);
        bool clampedHigh = player.HealthPointsCurrent == player.HealthPointsMax;
        Check("16. ModifyHealth зажимает в [0; Max]", clampedLow && clampedHigh);
    }

    private static void Scenario17And18()
    {
        PlayerData player = NewPlayer();
        bool addedNull = player.AddItem(null);
        bool equippedPlain = player.EquipWeapon(new ItemData
        {
            ItemID = "test_plain",
            ItemName = "Обычный предмет",
        });
        Check("17-18. null-предмет и не-оружие не принимаются",
            !addedNull && !equippedPlain && player.Inventory.Count == 0 &&
            player.Weapon == null);
    }

    private static void Scenario19To22()
    {
        PlayerData player = NewPlayer();
        var knife = new WeaponData
        {
            ItemID = "test_knife",
            ItemName = "Ржавый нож",
            Rarity = 0,
            MinDamage = 1,
            MaxDamage = 3,
        };
        var crowbar = new WeaponData
        {
            ItemID = "test_crowbar",
            ItemName = "Монтировка",
            Rarity = 1,
            MinDamage = 2,
            MaxDamage = 5,
        };

        player.AddItem(knife);
        bool knifeEquipped = player.EquipWeapon(knife);
        Check("19. Экипировка ножа убирает его из инвентаря",
            knifeEquipped && ReferenceEquals(player.Weapon, knife) &&
            !player.Inventory.Contains(knife));

        player.AddItem(crowbar);
        bool crowbarEquipped = player.EquipWeapon(crowbar);
        Check("20. Замена оружия возвращает нож в инвентарь",
            crowbarEquipped && ReferenceEquals(player.Weapon, crowbar) &&
            player.Inventory.Contains(knife));

        bool unequipped = player.UnequipWeapon();
        Check("21. Снятие оружия возвращает монтировку",
            unequipped && player.Weapon == null && player.Inventory.Contains(crowbar));

        player.RemoveItem(knife);
        bool missing = player.EquipWeapon(knife);
        Check("22. Экипировка отсутствующего в инвентаре невозможна",
            !missing && player.Weapon == null);
    }

    private static void Scenario23And24()
    {
        var notebook = new NotebookData();
        bool notesChanged = false;
        notebook.NotesChanged += () => notesChanged = true;

        var note = new NoteData
        {
            NoteId = "test_note",
            Title = "Тестовая записка",
            Text = "Текст",
        };

        bool first = notebook.AddNote(note);
        bool second = notebook.AddNote(note);
        Check("23. Дубликат записки не добавляется",
            first && !second && notebook.PageCount == 2);

        notesChanged = false;
        notebook.SetPlayerText("abc");
        Check("24. SetPlayerText не вызывает NotesChanged",
            notebook.PlayerText == "abc" && !notesChanged);
    }

    private static PlayerData NewPlayer() => new((min, max) => min);

    private static bool Almost(double value, double expected) =>
        Math.Abs(value - expected) < 1e-9;

    private static void Check(string name, bool ok)
    {
        _total++;
        if (ok)
        {
            _passed++;
            GameLog.Info($"PASS {name}");
        }
        else
        {
            GameLog.Error($"FAIL {name}");
        }
    }
}
#endif
