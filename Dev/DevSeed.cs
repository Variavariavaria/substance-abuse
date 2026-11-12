#if DEBUG
#nullable enable
using SubstanceAbuse.Characters;
using SubstanceAbuse.Items;

namespace SubstanceAbuse.Dev;

/// <summary>Тестовые предметы, записки и игрок для debug-сборки.</summary>
public static class DevSeed
{
    private const int StartingItemCount = 6;

    private static ItemData[]? _items;
    private static NoteData[]? _notes;

    /// <summary>Все тестовые предметы. Первые шесть попадают в стартовый инвентарь, остальные крутятся по F2.</summary>
    public static ItemData[] TestItems => _items ??= CreateItems();

    public static NoteData[] TestNotes => _notes ??= CreateNotes();

    public static PlayerData CreateTestPlayer()
    {
        var player = new PlayerData();
        for (int i = 0; i < StartingItemCount; i++)
        {
            player.AddItem(TestItems[i]);
        }

        return player;
    }

    private static ItemData[] CreateItems() => new ItemData[]
    {
        new WeaponData { ItemID = "weapon_rusty_knife", ItemName = "Ржавый нож", Rarity = 0, MinDamage = 1, MaxDamage = 3 },
        new ArmorData { ItemID = "armor_padded_jacket", ItemName = "Ватник", Rarity = 0, DefenceMod = 1 },
        new ItemData { ItemID = "junk_tin_can", ItemName = "Консервная банка", Rarity = 0 },
        new ItemData { ItemID = "med_syringe", ItemName = "Шприц", Rarity = 1 },
        new WeaponData { ItemID = "weapon_crowbar", ItemName = "Монтировка", Rarity = 1, MinDamage = 2, MaxDamage = 5 },
        new ItemData { ItemID = "jewel_gold_ring", ItemName = "Золотое кольцо", Rarity = 3 },
        new ArmorData { ItemID = "armor_riot_vest", ItemName = "Бронежилет", Rarity = 2, DefenceMod = 3 },
    };

    private static NoteData[] CreateNotes() => new NoteData[]
    {
        new NoteData
        {
            NoteId = "note_test_1",
            Title = "Обрывок с 97-го",
            Text = "Они спускаются ночью. Не по лестнице — по стенам. Я слышал, как шуршит бетон, будто по нему ползут тысячи ногтей.\n\nНе зажигай свет после полуночи.",
        },
        new NoteData
        {
            NoteId = "note_test_2",
            Title = "Дневник смотрителя",
            Text = "Воду меняем раз в три дня. Фильтр забился, но чинить нечем. Если кто найдёт этот блокнот — на нижних этажах есть склад. Код от двери знает только Кувалда.\n\nКувалда больше не отвечает.",
        },
        new NoteData
        {
            NoteId = "note_test_3",
            Title = "Детский рисунок",
            Text = "На обратной стороне листа нарисованы пять фигур без лиц и длинная лестница, уходящая вниз. Под рисунком кривым почерком выведено: «не смотрите на пятого».",
        },
        new NoteData
        {
            NoteId = "note_test_4",
            Title = "Инструкция по выживанию",
            Text = "Первое: не входи в комнату, где пахнет мокрой шерстью.\nВторое: если слышишь своё имя из вентиляции — не отвечай.\nТретье: оставляй записки. Тот, кто идёт следом, должен знать, где ты ошибся.\n\nЯ ошибся трижды.",
        },
        new NoteData
        {
            NoteId = "note_test_5",
            Title = "Последняя запись",
            Text = "Я спускался с сотого этажа двадцать три дня. Сначала считал этажи, потом ступени, потом удары сердца. На шестьдесят четвёртом они закончились все сразу.\n\nЗдесь внизу нет темноты. Здесь есть только вещество, из которого сделана темнота. Оно смотрит на меня из каждой щели и ждёт, когда я закрою глаза. Я не закрываю. Я пишу, потому что пока я пишу — я ещё я. Карандаш почти стёрся. Бумага мягкая, как кожа. Если ты читаешь это — не верь тишине. Тишина здесь не пустая, она плотная, она давит на барабанные перепонки, и в этой плотности что-то плавает: медленно, без спешки, будто ему принадлежит всё время мира. Оно не нападает. Оно ждёт, пока ты сам решишь, что снаружи безопаснее, чем внутри. Не решай. Останься у лампы. Считай до тысячи. Потом начинай заново. Я дошёл до семисот тридцати одного и услышал, как за спиной открылась дверь. Двери здесь не открываются. Я не обернулся. Наверное, это была моя самая большая ошибка.",
        },
    };
}
#endif
