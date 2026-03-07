using System.Collections.Generic;
using System.Linq;

namespace ASTRANET_Hidden_Sector.Entities
{
    public class Background
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public Dictionary<string, int> StatBonuses { get; set; } = new Dictionary<string, int>();
        public string AbilityName { get; set; } = "";
        public string AbilityEffect { get; set; } = "";

        // Добавляем поле Id для идентификации
        public string Id { get; set; } = "";

        public static List<Background> GetAll()
        {
            return new List<Background>
            {
                new Background
                {
                    Id = "soldier",
                    Name = "Солдат",
                    Description = "Прошедший жёсткую подготовку в десантных группах флота.",
                    StatBonuses = new Dictionary<string, int> { { "Strength", 2 }, { "Dexterity", 1 } },
                    AbilityName = "Интенсивный курс пехотного боя",
                    AbilityEffect = "-10% к шансу попадания по вам"
                },
                new Background
                {
                    Id = "tech",
                    Name = "Боевой техник",
                    Description = "Сапёр, эксперт по взрывчатке и минному делу.",
                    StatBonuses = new Dictionary<string, int> { { "Intelligence", 2 }, { "Dexterity", 1 } },
                    AbilityName = "Сертификация сапёра",
                    AbilityEffect = "+25% урона от взрывчатки; особые диалоги"
                },
                new Background
                {
                    Id = "diplomat",
                    Name = "Дипломат",
                    Description = "Бывший практикант консульства, умеет договариваться.",
                    StatBonuses = new Dictionary<string, int> { { "Charisma", 2 }, { "Luck", 1 } },
                    AbilityName = "Практика в консульстве",
                    AbilityEffect = "+20% убеждения; шанс 15% избежать боя"
                },
                new Background
                {
                    Id = "scout",
                    Name = "Разведчик",
                    Description = "Командир информационного центра, мастер сканирования.",
                    StatBonuses = new Dictionary<string, int> { { "Dexterity", 2 }, { "Intelligence", 1 } },
                    AbilityName = "Командир информационного центра",
                    AbilityEffect = "-1 к обнаружению; видит скрытые особенности систем"
                },
                new Background
                {
                    Id = "engineer",
                    Name = "Бортинженер",
                    Description = "Профессионал, способный собрать что угодно из подручных средств.",
                    StatBonuses = new Dictionary<string, int> { { "Intelligence", 2 }, { "Strength", 1 } },
                    AbilityName = "Золотые руки",
                    AbilityEffect = "-20% стоимость улучшений; -10% энергопотребление модулей"
                },
                new Background
                {
                    Id = "medic",
                    Name = "Полевой фельдшер",
                    Description = "Врач, спасающий даже безнадёжных.",
                    StatBonuses = new Dictionary<string, int> { { "Intelligence", 2 }, { "Charisma", 1 } },
                    AbilityName = "Расширенный курс полевой медицины",
                    AbilityEffect = "Лечение +50% эффективность"
                },
                new Background
                {
                    Id = "quartermaster",
                    Name = "Интендант",
                    Description = "Хитрый снабженец, умеющий торговаться.",
                    StatBonuses = new Dictionary<string, int> { { "Charisma", 2 }, { "Intelligence", 1 } },
                    AbilityName = "Прапорщик или торгаш?",
                    AbilityEffect = "-15% цены; +20% доход"
                },
                new Background
                {
                    Id = "cartographer",
                    Name = "Картограф",
                    Description = "Исследователь, знающий короткие пути.",
                    StatBonuses = new Dictionary<string, int> { { "Intelligence", 2 }, { "Luck", 1 } },
                    AbilityName = "Ad astra per aspera",
                    AbilityEffect = "-25% расход топлива"
                }
            };
        }

        // Метод для получения предыстории по ID
        public static Background GetById(string id)
        {
            return GetAll().FirstOrDefault(b => b.Id == id);
        }
    }
}