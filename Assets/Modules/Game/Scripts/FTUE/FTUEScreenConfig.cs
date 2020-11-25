namespace Modules.Game
{
    [System.Serializable]
    public class FTUEScreenConfig
    {
        public HighlightedPosition HighlightedPosition;
        public string TopText;
        public string BottomText;
        public string ContinueText;
        public FTUETrigger AwakeTrigger;
        public FTUETrigger HideTrigger;
        public bool InputBlocked;
    }
}