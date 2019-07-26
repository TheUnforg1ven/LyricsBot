using System.Threading.Tasks;

namespace LyricTestBot.Services
{
    public interface ILyricParser
    {
        string Artist { get; set; }
        
        string SongTitle { get; set; }

        Task<string> GetLyricsAsync();
    }
}