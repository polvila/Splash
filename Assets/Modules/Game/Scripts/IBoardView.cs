using System;

namespace Modules.Game
{
    public interface IBoardView
    {
        event Action SplashZoneSelected;
        event Action<int> CardSelected;
        event Action StartGameEvent;

        Mode GameMode { get; }

        void MoveCard(int from, int to, Action onComplete = null);
        void MissCardMove(int from, Action onComplete = null);
        void DestroyCard(int position, float delay = 0);
        void AddNewCardTo(int cardPosition, int number, Action onComplete = null);
        void FinishGame(Action onComplete);
        void StartCountdown(Action onComplete);
        void ShowSplash(bool fromHumanPlayer, int totalPoints);
        void UnblockMiddleCards(int newLeftNumber, int newRightNumber);
    }
}