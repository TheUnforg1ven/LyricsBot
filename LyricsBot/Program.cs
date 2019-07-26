using System;
using LyricTestBot.Services;
using Telegram.Bot;

namespace LyricTestBot
{
    class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("<your_token>");
        
        public static void Main(string[] args)
        {
            Bot.OnMessage += Bot_OnMessage;
            Bot.OnMessageEdited += Bot_OnMessage;

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }
        
        private static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                if (e.Message.Text.StartsWith("/lyrics") && e.Message.Text.Length > 7)
                {
                    var songTitle = GetSong(e.Message.Text);
                    var artist = GetArtist(e.Message.Text);
                    var parser = new LyricParser(artist, songTitle);

                    var result = parser.GetLyricsAsync().GetAwaiter().GetResult();

                    Bot.SendTextMessageAsync(e.Message.Chat.Id, result ?? "No such artist or song 😔");
                }
                else switch (e.Message.Text)
                {
                    case "/start":
                        Bot.SendTextMessageAsync(e.Message.Chat.Id, "Hi, I'm LyricsBot, here you can find your fav song lyrics 😊");
                        break;
                    case "/help":
                        Bot.SendTextMessageAsync(e.Message.Chat.Id, "To find song lyrics, use command 💻: /lyrics artist songname\n" +
                                                                    "Write artist and song name without spaces and separators 😏\n" +
                                                                    "Simple example 📗: /lyrics metallica theunforgiven");
                        break;
                    default:
                        Bot.SendTextMessageAsync(e.Message.Chat.Id, "Unknown command 😐");
                        break;
                }
            }
        }

        private static string GetSong(string message)
        {
            var songName = message.Substring(message.LastIndexOf(' '))
                                    .Trim().ToLower();
            return songName;
        }
        
        private static string GetArtist(string message)
        {
            var artist = message.Split(" ".ToCharArray())[1].ToLower();
            return artist;
        }
    }
}