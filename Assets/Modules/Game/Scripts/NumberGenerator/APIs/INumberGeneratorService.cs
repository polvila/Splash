namespace Modules.Game
{
    public interface INumberGeneratorService
    {
        int GetMaxRange { get; }
        int GetMinRange { get; }
        CardGeneratorMode GeneratorMode { get; set; }
        int GetNumber();
        void SetDefaultGeneratorMode();
    }

    public enum CardGeneratorMode
    {
        Random,
        RandomExcluding,
        FTUE,
    }
}