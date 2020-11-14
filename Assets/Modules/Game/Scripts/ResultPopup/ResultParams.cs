namespace Modules.Game.Scripts.Result
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