using UnityEngine;

public class PlayerModel : IPlayerModel
{
    private static readonly string HumanRecordKey = "HumanRecord";
    private static readonly string FTUECompletedKey = "FTUECompleted";
    private static readonly string MissFTUECompletedKey = "MissFTUECompleted";
    
    private int _humanRecord = -1;
    public int HumanRecord
    {
        get
        {
            if (_humanRecord == -1)
            {
                _humanRecord = PlayerPrefs.GetInt(HumanRecordKey);
            }

            return _humanRecord;
        }
        set
        {
            if (value <= _humanRecord) return;
            _humanRecord = value;
            PlayerPrefs.SetInt(HumanRecordKey, _humanRecord);
            PlayerPrefs.Save();
        }
    }

    private bool _ftueCompleted;
    public bool FTUECompleted
    {
        get
        {
            if (_ftueCompleted == false)
            {
                _ftueCompleted = PlayerPrefs.GetInt(FTUECompletedKey) != 0;
            }

            return _ftueCompleted;
        }
        set
        {
            if (value == false) return;
            _ftueCompleted = true;
            PlayerPrefs.SetInt(FTUECompletedKey, 1);
            PlayerPrefs.Save();
        }
    }
}
