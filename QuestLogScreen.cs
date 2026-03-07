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

            uiManager.SetCursorPosition(leftX, topY - 1);
            uiManager.Write("=== ЖУРНАЛ КВЕСТОВ ===", ConsoleColor.Yellow);

            uiManager.SetCursorPosition(leftX, topY);
            if (!showingCompleted)
                uiManager.Write("[Активные]  ", ConsoleColor.Cyan);
            else
                uiManager.Write(" Активные   ", ConsoleColor.DarkGray);

            uiManager.SetCursorPosition(leftX + 12, topY);
            if (showingCompleted)
                uiManager.Write("[Выполненные]", ConsoleColor.Cyan);
            else
                uiManager.Write(" Выполненные ", ConsoleColor.DarkGray);

            var currentList = showingCompleted ? completedQuests : activeQuests;
            int listStartY = topY + 2;

            if (currentList.Count == 0)
            {
                uiManager.SetCursorPosition(leftX, listStartY);
                uiManager.Write("Нет квестов", ConsoleColor.DarkGray);
            }
            else
            {
                for (int i = 0; i < currentList.Count; i++)
                {
                    uiManager.SetCursorPosition(leftX, listStartY + i);
                    if (i == selectedQuestIndex)
                        uiManager.Write("> ", ConsoleColor.Yellow);
                    else
                        uiManager.Write("  ", ConsoleColor.Gray);
                    uiManager.Write(currentList[i].Name, ConsoleColor.White);
                }

                if (selectedQuestIndex < currentList.Count)
                {
                    var quest = currentList[selectedQuestIndex];
                    int detailsX = 40;
                    int detailsY = listStartY;

                    uiManager.SetCursorPosition(detailsX, detailsY++);
                    uiManager.Write(quest.Description, ConsoleColor.Gray);

                    foreach (var obj in quest.Objectives)
                    {
                        uiManager.SetCursorPosition(detailsX, detailsY++);
                        string status = obj.IsCompleted ? "[X]" : "[ ]";
                        string text = $"{status} {obj.Description}";
                        if (!obj.IsCompleted)
                            text += $" ({obj.CurrentAmount}/{obj.RequiredAmount})";
                        uiManager.Write(text, obj.IsCompleted ? ConsoleColor.Green : ConsoleColor.White);
                    }
                }
            }

            uiManager.SetCursorPosition(2, Console.WindowHeight - 2);
            uiManager.Write("Tab - переключить вкладку, Backspace - назад, ESC - меню", ConsoleColor.DarkGray);
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