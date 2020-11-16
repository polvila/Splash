using CloudOnce;

namespace Core.CloudOnce
{
    public class CloudOnceService : ICloudOnceService
    {
        public void SubmitScoreToLeaderboard(int score)
        {
            Leaderboards.highScore.SubmitScore(score);
        }
    }
}