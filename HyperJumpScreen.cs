using System;
using System.Threading;
using ASTRANET_Hidden_Sector.Core;

namespace ASTRANET_Hidden_Sector.Screens
{
    public class HyperJumpScreen : GameScreen
    {
        private string fromName;
        private string toName;
        private Action onComplete;
        private DateTime startTime;
        private const float JUMP_DURATION = 3f;
        private bool isComplete = false;
        private Timer timer;
        private bool timerElapsed = false;

        public HyperJumpScreen(GameStateManager stateManager, UIManager uiManager, string from, string to, Action onComplete)
            : base(stateManager, uiManager)
        {
            fromName = from;
            toName = to;
            this.onComplete = onComplete;
            startTime = DateTime.Now;

            // Запасной таймер на случай проблем с Update
            timer = new Timer(_ => timerElapsed = true, null, TimeSpan.FromSeconds(JUMP_DURATION), Timeout.InfiniteTimeSpan);
        }

        public override void Update(float deltaTime)
        {
            if (isComplete) return;

            // Проверка по реальному времени или по флагу таймера
            if (timerElapsed || (DateTime.Now - startTime).TotalSeconds >= JUMP_DURATION)
                Complete();
        }

        public override void Render()
        {
            uiManager.Clear();

            string message = $"ГИПЕРПРЫЖОК: {fromName} → {toName}";
            for (int i = 0; i < message.Length; i++)
                uiManager.SetPixel(2 + i, 1, message[i], ConsoleColor.Cyan);

            float elapsed = (float)(DateTime.Now - startTime).TotalSeconds;
            int barWidth = 40;
            int filled = (int)((elapsed / JUMP_DURATION) * barWidth);
            if (filled > barWidth) filled = barWidth;
            string bar = new string('█', filled) + new string('░', barWidth - filled);
            for (int i = 0; i < bar.Length; i++)
                uiManager.SetPixel(2 + i, Console.WindowHeight - 3, bar[i], ConsoleColor.Blue);

            string hint = "Нажмите любую клавишу для пропуска...";
            for (int i = 0; i < hint.Length; i++)
                uiManager.SetPixel(2 + i, Console.WindowHeight - 2, hint[i], ConsoleColor.DarkGray);

            uiManager.Render();
        }

        public override void HandleInput(ConsoleKeyInfo key)
        {
            if (!isComplete)
                Complete();
        }

        private void Complete()
        {
            if (isComplete) return;
            isComplete = true;
            timer?.Dispose();
            timer = null;
            onComplete?.Invoke();
        }
    }
}