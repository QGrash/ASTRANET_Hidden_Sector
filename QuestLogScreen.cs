using System;
using System.Collections.Generic;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Entities.Quest;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class QuestLogScreen : GameScreen
    {
        private int selectedQuestIndex = 0;
        private List<Quest> activeQuests;
        private List<Quest> completedQuests;
        private bool showingCompleted = false;

        public QuestLogScreen(GameStateManager stateManager, UIManager uiManager)
            : base(stateManager, uiManager)
        {
            activeQuests = QuestManager.GetActiveQuests();
            completedQuests = QuestManager.GetCompletedQuests();
        }

        public override void Render()
        {
            uiManager.Clear();

            int leftX = 5;
            int topY = 3;

            string header = "=== ЖУРНАЛ КВЕСТОВ ===";
            for (int i = 0; i < header.Length; i++)
                uiManager.SetPixel(leftX + i, topY - 1, header[i], ConsoleColor.Yellow);

            string tabActive = showingCompleted ? " Активные   " : "[Активные]  ";
            string tabCompleted = showingCompleted ? "[Выполненные]" : " Выполненные ";

            for (int i = 0; i < tabActive.Length; i++)
                uiManager.SetPixel(leftX + i, topY, tabActive[i],
                    showingCompleted ? ConsoleColor.DarkGray : ConsoleColor.Cyan);
            for (int i = 0; i < tabCompleted.Length; i++)
                uiManager.SetPixel(leftX + 12 + i, topY, tabCompleted[i],
                    showingCompleted ? ConsoleColor.Cyan : ConsoleColor.DarkGray);

            var currentList = showingCompleted ? completedQuests : activeQuests;
            int listStartY = topY + 2;

            if (currentList.Count == 0)
            {
                string empty = "Нет квестов";
                for (int i = 0; i < empty.Length; i++)
                    uiManager.SetPixel(leftX + i, listStartY, empty[i], ConsoleColor.DarkGray);
            }
            else
            {
                for (int i = 0; i < currentList.Count; i++)
                {
                    int y = listStartY + i;
                    if (i == selectedQuestIndex)
                        uiManager.SetPixel(leftX, y, '>', ConsoleColor.Yellow);
                    else
                        uiManager.SetPixel(leftX, y, ' ', ConsoleColor.Black);

                    string name = currentList[i].Name;
                    for (int j = 0; j < name.Length; j++)
                        uiManager.SetPixel(leftX + 2 + j, y, name[j], ConsoleColor.White);
                }

                if (selectedQuestIndex < currentList.Count)
                {
                    var quest = currentList[selectedQuestIndex];
                    int detailsX = 40;
                    int detailsY = listStartY;

                    for (int i = 0; i < quest.Description.Length; i++)
                        uiManager.SetPixel(detailsX + i, detailsY, quest.Description[i], ConsoleColor.Gray);
                    detailsY++;

                    foreach (var obj in quest.Objectives)
                    {
                        string status = obj.IsCompleted ? "[X]" : "[ ]";
                        string text = $"{status} {obj.Description}";
                        if (!obj.IsCompleted)
                            text += $" ({obj.CurrentAmount}/{obj.RequiredAmount})";

                        for (int i = 0; i < text.Length; i++)
                            uiManager.SetPixel(detailsX + i, detailsY, text[i],
                                obj.IsCompleted ? ConsoleColor.Green : ConsoleColor.White);
                        detailsY++;
                    }
                }
            }

            string hint = "Tab - переключить вкладку, Backspace - назад, ESC - меню";
            for (int i = 0; i < hint.Length; i++)
                uiManager.SetPixel(2 + i, Console.WindowHeight - 2, hint[i], ConsoleColor.DarkGray);

            uiManager.Render();
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            var currentList = showingCompleted ? completedQuests : activeQuests;

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (currentList.Count > 0)
                        selectedQuestIndex = (selectedQuestIndex - 1 + currentList.Count) % currentList.Count;
                    break;
                case ConsoleKey.DownArrow:
                    if (currentList.Count > 0)
                        selectedQuestIndex = (selectedQuestIndex + 1) % currentList.Count;
                    break;
                case ConsoleKey.Tab:
                    showingCompleted = !showingCompleted;
                    selectedQuestIndex = 0;
                    break;
                case ConsoleKey.Backspace:
                    stateManager.PopScreen();
                    break;
            }
        }
    }
}