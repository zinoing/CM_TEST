using ColorMemory.DTO;

namespace ColorMemory.Repository
{
    public interface IRedisDb
    {
        public Task SaveScoreAsync(ScoreDTO scoreInfo);
        public Task<long?> GetRankAsyncById(string userId);
        public Task<double?> GetScoreAsyncById(string userId);
        public Task<bool> DeleteScoreAsyncById(string userId);
    }
}
