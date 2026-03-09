using System;
using System.Collections.Generic;
using ASTRANET_Hidden_Sector.Core;
using ASTRANET_Hidden_Sector.Entities.Dialogue;
using ASTRANET_Hidden_Sector.Entities.Interior;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class DialogueScreen : GameScreen
    {
        private Dialogue dialogue;
        private DialogueNode currentNode;
        private List<DialogueChoice> availableChoices;
        private int selectedChoiceIndex = 0;
        private NpcEntity? npc;

        public DialogueScreen(GameStateManager stateManager, UIManager uiManager, string dialogueId, NpcEntity? npc = null)
            : base(stateManager, uiManager)
        {
            dialogue = DialogueManager.GetDialogue(dialogueId);
            if (dialogue == null)
            {
                stateManager.PopScreen();
                return;
            }
            this.npc = npc;
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
            int startX = 2;
            int startY = 3;

            // Рамка
            for (int x = startX; x <= startX + boxWidth; x++)
            {
                uiManager.SetPixel(x, startY, '─', ConsoleColor.Gray);
                uiManager.SetPixel(x, startY + boxHeight, '─', ConsoleColor.Gray);
            }
            for (int y = startY; y <= startY + boxHeight; y++)
            {
                uiManager.SetPixel(startX, y, '│', ConsoleColor.Gray);
                uiManager.SetPixel(startX + boxWidth, y, '│', ConsoleColor.Gray);
            }
            uiManager.SetPixel(startX, startY, '┌', ConsoleColor.Gray);
            uiManager.SetPixel(startX + boxWidth, startY, '┐', ConsoleColor.Gray);
            uiManager.SetPixel(startX, startY + boxHeight, '└', ConsoleColor.Gray);
            uiManager.SetPixel(startX + boxWidth, startY + boxHeight, '┘', ConsoleColor.Gray);

            // Имя NPC
            string npcName = dialogue.NpcName;
            for (int i = 0; i < npcName.Length; i++)
                uiManager.SetPixel(startX + 2 + i, startY + 1, npcName[i], ConsoleColor.Yellow);

            // Разделитель
            for (int i = 0; i < boxWidth - 4; i++)
                uiManager.SetPixel(startX + 2 + i, startY + 2, '─', ConsoleColor.DarkGray);

            // Текст NPC (с переносом)
            var wrappedText = WrapText(currentNode.NpcText, boxWidth - 4);
            int textY = startY + 3;
            foreach (var line in wrappedText)
            {
                for (int i = 0; i < line.Length; i++)
                    uiManager.SetPixel(startX + 2 + i, textY, line[i], ConsoleColor.White);
                textY++;
            }

            // Варианты ответов
            int choicesStartY = startY + boxHeight - 3 - availableChoices.Count;
            for (int i = 0; i < availableChoices.Count; i++)
            {
                string text = availableChoices[i].ChoiceText;
                if (i == selectedChoiceIndex)
                {
                    uiManager.SetPixel(startX + 2, choicesStartY + i, '>', ConsoleColor.Yellow);
                    for (int j = 0; j < text.Length; j++)
                        uiManager.SetPixel(startX + 4 + j, choicesStartY + i, text[j], ConsoleColor.Cyan);
                }
                else
                {
                    uiManager.SetPixel(startX + 2, choicesStartY + i, ' ', ConsoleColor.Black);
                    for (int j = 0; j < text.Length; j++)
                        uiManager.SetPixel(startX + 4 + j, choicesStartY + i, text[j], ConsoleColor.Cyan);
                }
            }

            // Подсказка
            string hint = "Стрелки для выбора, Enter для подтверждения";
            for (int i = 0; i < hint.Length; i++)
                uiManager.SetPixel(startX + 2 + i, startY + boxHeight - 1, hint[i], ConsoleColor.DarkGray);

            uiManager.Render();
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
                action.Execute(stateManager, uiManager, npc);
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