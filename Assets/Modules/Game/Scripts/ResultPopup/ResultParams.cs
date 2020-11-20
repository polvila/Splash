namespace Modules.Game
{
    public class ResultParams
    {
        public readonly int Result;
        public readonly bool NewRecord;

        public ResultParams(int result, bool newRecord)
        {
            Result = result;
            NewRecord = newRecord;
        }
    }
}