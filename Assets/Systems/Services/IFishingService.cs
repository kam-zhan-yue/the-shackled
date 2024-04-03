using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Kuroneko.UtilityDelivery;

public interface IFishingService : IGameService
{
    public void RegisterGameStart(Action<FishingGame> gameStart);
    public void RegisterGameEnd(Action<FishingGame> gameEnd);
    public void UnregisterGameStart(Action<FishingGame> gameStart);
    public void UnregisterGameEnd(Action<FishingGame> gameEnd);
    public void StartGame(FishingPole pole, int slots, CancellationToken token);
}