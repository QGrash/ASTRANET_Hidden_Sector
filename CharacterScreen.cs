using System;
using ASTRANET_Hidden_Sector.Core;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class CharacterScreen : GameScreen
    {
        public CharacterScreen(GameStateManager stateManager, UIManager uiManager)
            : base(stateManager, uiManager)
        {
        }

        public override void Render()
        {
            uiManager.Clear();

            int leftX = 5;
            int topY = 3;

            string title = "=== ХАРАКТЕРИСТИКИ ===";
            for (int i = 0; i < title.Length; i++)
                uiManager.SetPixel(leftX + i, topY, title[i], ConsoleColor.Green);
            topY += 2;

            if (Game.CurrentPlayer != null)
            {
                var p = Game.CurrentPlayer;

                string name = $"Имя: {p.Name}";
                for (int i = 0; i < name.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, name[i], ConsoleColor.White);
                topY++;

                string level = $"Уровень: {p.Level} (опыт: {p.Experience})";
                for (int i = 0; i < level.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, level[i], ConsoleColor.White);
                topY++;

                string health = $"Здоровье: {p.Health}/{p.MaxHealth}";
                for (int i = 0; i < health.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, health[i], ConsoleColor.Green);
                topY++;

                string credits = $"Кредиты: {p.Credits}";
                for (int i = 0; i < credits.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, credits[i], ConsoleColor.Yellow);
                topY++;

                string background = $"Предыстория: {p.Background?.Name ?? "Неизвестно"}";
                for (int i = 0; i < background.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, background[i], ConsoleColor.Cyan);
                topY += 2;

                string statsTitle = "Характеристики:";
                for (int i = 0; i < statsTitle.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, statsTitle[i], ConsoleColor.Yellow);
                topY++;

                string str = $"Сила: {p.Strength}";
                for (int i = 0; i < str.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, str[i], ConsoleColor.White);
                topY++;

                string dex = $"Ловкость: {p.Dexterity}";
                for (int i = 0; i < dex.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, dex[i], ConsoleColor.White);
                topY++;

                string intel = $"Интеллект: {p.Intelligence}";
                for (int i = 0; i < intel.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, intel[i], ConsoleColor.White);
                topY++;

                string luck = $"Удача: {p.Luck}";
                for (int i = 0; i < luck.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, luck[i], ConsoleColor.White);
                topY++;

                string cha = $"Харизма: {p.Charisma}";
                for (int i = 0; i < cha.Length; i++)
                    uiManager.SetPixel(leftX + i, topY, cha[i], ConsoleColor.White);
            }

            string hint = "Backspace - назад, ESC - меню";
            for (int i = 0; i < hint.Length; i++)
                uiManager.SetPixel(2 + i, Console.WindowHeight - 2, hint[i], ConsoleColor.DarkGray);

            uiManager.Render();
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Backspace)
            {
                stateManager.PopScreen();
            }
        }
    }
}