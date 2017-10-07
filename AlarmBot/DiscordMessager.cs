using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AlarmBot
{
    internal class DiscordMessager
    {
        internal static Action s;
        internal static DiscordSocketClient _client;

        public static bool IsWriteToChatMode { get; internal set; }
        public static bool IsInviterMode { get; internal set; }
        public static bool IsLaunchPilotMode { get; internal set; }

        internal static List<ISocketMessageChannel> channels = new List<ISocketMessageChannel>();


        internal static void SendToAll(string v)
        {
            for (int i = 0; i < channels.Count; i++)
            {
                channels[i].SendMessageAsync(v).GetAwaiter();
            }
        }

        internal static void SendPhotoAsync()
        {
            for (int i = 0; i < channels.Count; i++)
            {
                try
                {
                    Thread.Sleep(1000);
                    Screen.getScreen(41, 0, 982, 768).Save("image.png");
                    channels[i].SendFileAsync("image.png").GetAwaiter();
                }
                catch
                {
                    channels[i].SendMessageAsync("Ошибка отправки скриншота (инфа 99%, одновременный доступ)");
                }
                
            }
        }


        public static async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;
            _client.MessageReceived += MessageReceived;

            string token = "MzY1MDM5NjQ0MDk1Njc2NDE2.DLZD8A.vUn3vUct1zz38tjz0BQ5vgZN8q0";
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private static async Task MessageReceived(SocketMessage message)
        {
            if (message.Content == "!start")
            {
                DiscordMessager.IsInviterMode = true;
                bool isAlreadyAdded = false;
                for (int i = 0; i<channels.Count; i++)
                {
                    if (channels[i].Id == message.Channel.Id)
                    {
                        isAlreadyAdded = true;
                    }
                }
                if (!isAlreadyAdded)
                {
                    channels.Add(message.Channel);
                }
                await message.Channel.SendMessageAsync("Вроде запущен, а там фиг его знает!");
            }
            if (message.Content == "!stop")
            {
                DiscordMessager.s();
                await message.Channel.SendMessageAsync("Остановлено если было что останавливать!");
            }
            if (message.Content == "!scan")
            {
                Screen.getScreen(41, 0, 982, 768).Save("imageScan.png");
                for (int i = 0; i < channels.Count; i++)
                {
                    channels[i].SendFileAsync("imageScan.png").GetAwaiter();
                }
            }
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}