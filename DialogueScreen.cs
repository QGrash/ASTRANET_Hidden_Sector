using System;
using System.Collections.Generic;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Entities.Dialogue;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class DialogueScreen : GameScreen
    {
        private Dialogue dialogue;
        private DialogueNode currentNode;
        private List<DialogueChoice> availableChoices;
        private int selectedChoiceIndex = 0;

        public DialogueScreen(GameStateManager stateManager, UIManager uiManager, string dialogueId)
            : base(stateManager, uiManager)
        {
            dialogue = DialogueManager.GetDialogue(dialogueId);
            if (dialogue == null)
            {
                stateManager.PopScreen();
                return;
            }
            currentNode = dialogue.Nodes[dialogue.StartNodeId];
            UpdateAvailableChoices();
        }

        private void UpdateAvailableChoices()
        {
            availableChoices = new List<DialogueChoice>();
            foreach (var choice in currentNode.Choices)
            {
                bool conditionsMet = true;
                foreach (var condition in choice.Conditions)
                {
                    if (!condition.IsMet())
                    {
                        conditionsMet = false;
                        break;
                    }
                }
                if (conditionsMet)
                {
                    availableChoices.Add(choice);
                }
            }
            selectedChoiceIndex = 0;
        }

        public override void Render()
        {
            uiManager.Clear();

            int boxWidth = 70;
            int boxHeight = 15;
            int startX = (Console.WindowWidth - boxWidth) / 2;
            int startY = (Console.WindowHeight - boxHeight) / 2;

            DrawBox(startX, startY, boxWidth, boxHeight);

            uiManager.SetCursorPosition(startX + 2, startY + 1);
            uiManager.Write(dialogue.NpcName, ConsoleColor.Yellow);

            uiManager.SetCursorPosition(startX + 2, startY + 2);
            uiManager.Write(new string('─', boxWidth - 4), ConsoleColor.DarkGray);

            var wrappedText = WrapText(currentNode.NpcText, boxWidth - 4);
            int textY = startY + 3;
            foreach (var line in wrappedText)
            {
                uiManager.SetCursorPosition(startX + 2, textY++);
                uiManager.Write(line, ConsoleColor.White);
            }

            int choicesStartY = startY + boxHeight - 3 - availableChoices.Count;
            for (int i = 0; i < availableChoices.Count; i++)
            {
                uiManager.SetCursorPosition(startX + 2, choicesStartY + i);
                if (i == selectedChoiceIndex)
                    uiManager.Write("> ", ConsoleColor.Yellow);
                else
                    uiManager.Write("  ", ConsoleColor.Gray);
                uiManager.Write(availableChoices[i].ChoiceText, ConsoleColor.Cyan);
            }

            uiManager.SetCursorPosition(startX + 2, startY + boxHeight - 1);
            uiManager.Write("Стрелки для выбора, Enter для подтверждения", ConsoleColor.DarkGray);
        }

        private void DrawBox(int x, int y, int width, int height)
        {
            uiManager.SetCursorPosition(x, y);
            uiManager.Write("┌" + new string('─', width - 2) + "┐", ConsoleColor.Gray);
            for (int i = 1; i < height - 1; i++)
            {
                uiManager.SetCursorPosition(x, y + i);
                uiManager.Write("│", ConsoleColor.Gray);
                uiManager.SetCursorPosition(x + width - 1, y + i);
                uiManager.Write("│", ConsoleColor.Gray);
            }
            uiManager.SetCursorPosition(x, y + height - 1);
            uiManager.Write("└" + new string('─', width - 2) + "┘", ConsoleColor.Gray);
        }

        private string[] WrapText(string text, int maxWidth)
        {
            var words = text.Split(' ');
            var lines = new List<string>();
            var currentLine = "";

            foreach (var word in words)
            {
                if ((currentLine + " " + word).Trim().Length <= maxWidth)
                {
                    if (currentLine.Length > 0)
                        currentLine += " ";
                    currentLine += word;
                }
                else
                {
                    if (currentLine.Length > 0)
                        lines.Add(currentLine);
                    currentLine = word;
                }
            }
            if (currentLine.Length > 0)
                lines.Add(currentLine);
            return lines.ToArray();
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            if (availableChoices.Count == 0)
            {
                stateManager.PopScreen();
                return;
            }

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedChoiceIndex = (selectedChoiceIndex - 1 + availableChoices.Count) % availableChoices.Count;
                    break;
                case ConsoleKey.DownArrow:
                    selectedChoiceIndex = (selectedChoiceIndex + 1) % availableChoices.Count;
                    break;
                case ConsoleKey.Enter:
                    ExecuteChoice(availableChoices[selectedChoiceIndex]);
                    break;
                case ConsoleKey.Escape:
                    stateManager.PopScreen();
                    break;
            }
        }

        private void ExecuteChoice(DialogueChoice choice)
        {
            foreach (var action in choice.Actions)
            {
                action.Execute(stateManager, uiManager); // передаём параметры
            }

            if (string.IsNullOrEmpty(choice.NextNodeId))
            {
                stateManager.PopScreen();
            }
            else if (dialogue.Nodes.ContainsKey(choice.NextNodeId))
            {
                currentNode = dialogue.Nodes[choice.NextNodeId];
                UpdateAvailableChoices();
                if (currentNode.Choices.Count == 0)
                {
                    stateManager.PopScreen();
                }
            }
            else
            {
                stateManager.PopScreen();
            }
        }
    }
}