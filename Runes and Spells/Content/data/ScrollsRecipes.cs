using System.Collections.Generic;

namespace Runes_and_Spells.Content.data;

public class ScrollsRecipes
{
    public record ScrollInfo(string id, string runeElementAndVariant1, string runeElementAndVariant2, ScrollType Type, string rus);

    public static Dictionary<(string runeElementAndVariant1, string runeElementAndVariant2), ScrollInfo> Recipes = new ()
    {
        {("water_1", "water_2"), new ScrollInfo("scroll_water_water_1", "water_1", "water_2", ScrollType.Ocean, "Водного потока")},
        {("water_1", "grass_1"), new ScrollInfo("scroll_water_grass_1", "water_1", "grass_1", ScrollType.Nature, "Лечения")},
        {("water_1", "fire_1"), new ScrollInfo("scroll_water_fire_1", "water_1", "fire_1", ScrollType.Ocean, "Кипячения")},
        {("water_1", "air_1"), new ScrollInfo("scroll_water_air_1", "water_1", "air_1", ScrollType.Ocean, "Хождения по воде")},
        {("water_1", "ice_1"), new ScrollInfo("scroll_water_ice_1", "water_1", "ice_1", ScrollType.Ocean, "Морозной воды")},
        {("water_1", "moon_1"), new ScrollInfo("scroll_water_moon_1", "water_1", "moon_1", ScrollType.Ocean, "Удачной рыбалки")},
        {("water_1", "blood_1"), new ScrollInfo("scroll_water_blood_1", "water_1", "blood_1", ScrollType.Faerie, "Омоложения")},
        {("water_1", "distorted_1"), new ScrollInfo("scroll_water_distorted_1", "water_1", "distorted_1", ScrollType.Ocean, "Кислоты")},
        {("water_1", "black_1"), new ScrollInfo("scroll_water_black_1", "water_1", "black_1", ScrollType.Toxic, "Защиты от кислоты")},
        {("water_2", "grass_2"), new ScrollInfo("scroll_water_grass_2", "water_2", "grass_2", ScrollType.Nature, "Регенерации")},
        {("water_2", "fire_2"), new ScrollInfo("scroll_water_fire_2", "water_2", "fire_2", ScrollType.Wind, "Тумана")},
        {("water_2", "air_2"), new ScrollInfo("scroll_water_air_2", "water_2", "air_2", ScrollType.Ocean, "Прилива")},
        {("water_2", "ice_2"), new ScrollInfo("scroll_water_ice_2", "water_2", "ice_2", ScrollType.Ice, "Снега")},
        {("water_2", "moon_2"), new ScrollInfo("scroll_water_moon_2", "water_2", "moon_2", ScrollType.Faerie, "Вдохновения")},
        {("water_2", "blood_2"), new ScrollInfo("scroll_water_blood_2", "water_2", "blood_2", ScrollType.Ocean, "Склейки")},
        {("water_2", "distorted_2"), new ScrollInfo("scroll_water_distorted_2", "water_2", "distorted_2", ScrollType.Toxic, "Опьянения")},
        {("water_2", "black_2"), new ScrollInfo("scroll_water_black_2", "water_2", "black_2", ScrollType.Life, "Лени")},
        
        {("grass_1", "grass_2"), new ScrollInfo("scroll_grass_grass_1", "grass_1", "grass_2", ScrollType.Nature, "Буйного роста")},
        {("grass_1", "fire_1"), new ScrollInfo("scroll_grass_fire_1", "grass_1", "fire_1", ScrollType.Sun, "Лавы")},
        {("grass_1", "air_1"), new ScrollInfo("scroll_grass_air_1", "grass_1", "air_1", ScrollType.Wind, "Ловкости")},
        {("grass_1", "ice_1"), new ScrollInfo("scroll_grass_ice_1", "grass_1", "ice_1", ScrollType.Ice, "Скольжения")},
        {("grass_1", "moon_1"), new ScrollInfo("scroll_grass_moon_1", "grass_1", "moon_1", ScrollType.Nature, "Землекопа")},
        {("grass_1", "blood_1"), new ScrollInfo("scroll_grass_blood_1", "grass_1", "blood_1", ScrollType.Nature, "Каменной кожи")},
        {("grass_1", "distorted_1"), new ScrollInfo("scroll_grass_distorted_1", "grass_1", "distorted_1", ScrollType.Nature, "Шипов")},
        {("grass_1", "black_1"), new ScrollInfo("scroll_grass_black_1", "grass_1", "black_1", ScrollType.Life, "Стальной кожи")},
        {("grass_2", "fire_2"), new ScrollInfo("scroll_grass_fire_2", "grass_2", "fire_2", ScrollType.Nature, "Быстрой ковки")},
        {("grass_2", "air_2"), new ScrollInfo("scroll_grass_air_2", "grass_2", "air_2", ScrollType.Nature, "Земного дыхания")},
        {("grass_2", "ice_2"), new ScrollInfo("scroll_grass_ice_2", "grass_2", "ice_2", ScrollType.Ice, "Замедления")},
        {("grass_2", "moon_2"), new ScrollInfo("scroll_grass_moon_2", "grass_2", "moon_2", ScrollType.Faerie, "Удачных событий")},
        {("grass_2", "blood_2"), new ScrollInfo("scroll_grass_blood_2", "grass_2", "blood_2", ScrollType.Nature, "Силы")},
        {("grass_2", "distorted_2"), new ScrollInfo("scroll_grass_distorted_2", "grass_2", "distorted_2", ScrollType.Toxic, "Неплодородия")},
        {("grass_2", "black_2"), new ScrollInfo("scroll_grass_black_2", "grass_2", "black_2", ScrollType.Corrupted, "Окаменения")},
        
        {("fire_1", "fire_2"), new ScrollInfo("scroll_fire_fire_1", "fire_1", "fire_2", ScrollType.Sun, "Испепеления")},
        {("fire_1", "air_1"), new ScrollInfo("scroll_fire_air_1", "fire_1", "air_1", ScrollType.Sun, "Огненного смерча")},
        {("fire_1", "ice_1"), new ScrollInfo("scroll_fire_ice_1", "fire_1", "ice_1", ScrollType.Wind, "Ледяной ауры")},
        {("fire_1", "moon_1"), new ScrollInfo("scroll_fire_moon_1", "fire_1", "moon_1", ScrollType.Sun, "Теплого пламени")},
        {("fire_1", "blood_1"), new ScrollInfo("scroll_fire_blood_1", "fire_1", "blood_1", ScrollType.Sun, "Призыва огня")},
        {("fire_1", "distorted_1"), new ScrollInfo("scroll_fire_distorted_1", "fire_1", "distorted_1", ScrollType.Toxic, "Злобного огня")},
        {("fire_1", "black_1"), new ScrollInfo("scroll_fire_black_1", "fire_1", "black_1", ScrollType.Sun, "Защиты от огня")},
        {("fire_2", "air_2"), new ScrollInfo("scroll_fire_air_2", "fire_2", "air_2", ScrollType.Sun, "Света")},
        {("fire_2", "ice_2"), new ScrollInfo("scroll_fire_ice_2", "fire_2", "ice_2", ScrollType.Ocean, "Оттепели")},
        {("fire_2", "moon_2"), new ScrollInfo("scroll_fire_moon_2", "fire_2", "moon_2", ScrollType.Faerie, "Магической силы")},
        {("fire_2", "blood_2"), new ScrollInfo("scroll_fire_blood_2", "fire_2", "blood_2", ScrollType.Life, "Решительности")},
        {("fire_2", "distorted_2"), new ScrollInfo("scroll_fire_distorted_2", "fire_2", "distorted_2", ScrollType.Sun, "Адского пламени")},
        {("fire_2", "black_2"), new ScrollInfo("scroll_fire_black_2", "fire_2", "black_2", ScrollType.Sun, "Взрыва")},

        {("air_1", "air_2"), new ScrollInfo("scroll_air_air_1", "air_1", "air_2", ScrollType.Wind, "Левитации")},
        {("air_1", "ice_1"), new ScrollInfo("scroll_air_ice_1", "air_1", "ice_1", ScrollType.Wind, "Ледяного дыхания")},
        {("air_1", "moon_1"), new ScrollInfo("scroll_air_moon_1", "air_1", "moon_1", ScrollType.Wind, "Невидимости")},
        {("air_1", "blood_1"), new ScrollInfo("scroll_air_blood_1", "air_1", "blood_1", ScrollType.Life, "Легкомыслия")},
        {("air_1", "distorted_1"), new ScrollInfo("scroll_air_distorted_1", "air_1", "distorted_1", ScrollType.Wind, "Молнии")},
        {("air_1", "black_1"), new ScrollInfo("scroll_air_black_1", "air_1", "black_1", ScrollType.Wind, "Защиты от бури")},
        {("air_2", "ice_2"), new ScrollInfo("scroll_air_ice_2", "air_2", "ice_2", ScrollType.Ice, "Сосульки")},
        {("air_2", "moon_2"), new ScrollInfo("scroll_air_moon_2", "air_2", "moon_2", ScrollType.Wind, "Перышка")},
        {("air_2", "blood_2"), new ScrollInfo("scroll_air_blood_2", "air_2", "blood_2", ScrollType.Life, "Либидо")},
        {("air_2", "distorted_2"), new ScrollInfo("scroll_air_distorted_2", "air_2", "distorted_2", ScrollType.Toxic, "Зловония")},
        {("air_2", "black_2"), new ScrollInfo("scroll_air_black_2", "air_2", "black_2", ScrollType.Corrupted, "Слепоты")},
        
        {("ice_1", "ice_2"), new ScrollInfo("scroll_ice_ice_1", "ice_1", "ice_2", ScrollType.Ice, "Снежного бурана")},
        {("ice_1", "moon_1"), new ScrollInfo("scroll_ice_moon_1", "ice_1", "moon_1", ScrollType.Ice, "Магического льда")},
        {("ice_1", "blood_1"), new ScrollInfo("scroll_ice_blood_1", "ice_1", "blood_1", ScrollType.Life, "Хладнокровия")},
        {("ice_1", "distorted_1"), new ScrollInfo("scroll_ice_distorted_1", "ice_1", "distorted_1", ScrollType.Ice, "Ядовитого льда")},
        {("ice_1", "black_1"), new ScrollInfo("scroll_ice_black_1", "ice_1", "black_1", ScrollType.Ice, "Защиты от мороза")},
        {("ice_2", "moon_2"), new ScrollInfo("scroll_ice_moon_2", "ice_2", "moon_2", ScrollType.Faerie, "Ледяной феи")},
        {("ice_2", "blood_2"), new ScrollInfo("scroll_ice_blood_2", "ice_2", "blood_2", ScrollType.Life, "Обездвиживания")},
        {("ice_2", "distorted_2"), new ScrollInfo("scroll_ice_distorted_2", "ice_2", "distorted_2", ScrollType.Ice, "Ледяного хаоса")},
        {("ice_2", "black_2"), new ScrollInfo("scroll_ice_black_2", "ice_2", "black_2", ScrollType.Corrupted, "Осколка злобы")},
        
        {("moon_1", "moon_2"), new ScrollInfo("scroll_moon_moon_1", "moon_1", "moon_2", ScrollType.Faerie, "Сна")},
        {("moon_1", "blood_1"), new ScrollInfo("scroll_moon_blood_1", "moon_1", "blood_1", ScrollType.Faerie, "Галлюцинаций")},
        {("moon_1", "distorted_1"), new ScrollInfo("scroll_moon_distorted_1", "moon_1", "distorted_1", ScrollType.Faerie, "Хождения во сне")},
        {("moon_1", "black_1"), new ScrollInfo("scroll_moon_black_1", "moon_1", "black_1", ScrollType.Faerie, "Защиты от магии")},
        {("moon_2", "blood_2"), new ScrollInfo("scroll_moon_blood_2", "moon_2", "blood_2", ScrollType.Life, "Очарования")},
        {("moon_2", "distorted_2"), new ScrollInfo("scroll_moon_distorted_2", "moon_2", "distorted_2", ScrollType.Corrupted, "Кошмаров")},
        {("moon_2", "black_2"), new ScrollInfo("scroll_moon_black_2", "moon_2", "black_2", ScrollType.Corrupted, "Страха")},
        
        {("blood_1", "blood_2"), new ScrollInfo("scroll_blood_blood_1", "blood_1", "blood_2", ScrollType.Life, "Управл. кровью")},
        {("blood_1", "distorted_1"), new ScrollInfo("scroll_blood_distorted_1", "blood_1", "distorted_1", ScrollType.Toxic, "Старения")},
        {("blood_1", "black_1"), new ScrollInfo("scroll_blood_black_1", "blood_1", "black_1", ScrollType.Corrupted, "Гниения")},
        {("blood_2", "distorted_2"), new ScrollInfo("scroll_blood_distorted_2", "blood_2", "distorted_2", ScrollType.Toxic, "Отравления")},
        {("blood_2", "black_2"), new ScrollInfo("scroll_blood_black_2", "blood_2", "black_2", ScrollType.Corrupted, "Некромантии")},
        
        {("distorted_1", "distorted_2"), new ScrollInfo("scroll_distorted_distorted_1", "distorted_1", "distorted_2", ScrollType.Toxic, "Разрыва материи")},
        {("distorted_1", "black_1"), new ScrollInfo("scroll_distorted_black_1", "distorted_1", "black_1", ScrollType.Toxic, "Снятия порчи")},
        {("distorted_2", "black_2"), new ScrollInfo("scroll_distorted_black_2", "distorted_2", "black_2", ScrollType.Corrupted, "Ярости")},
        
        {("black_1", "black_2"), new ScrollInfo("scroll_black_black_1", "black_1", "black_2", ScrollType.Corrupted, "Проклятия")},
    };
}