﻿using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TastyBot.Services
{
    public class CatBotService
    {
        private readonly DiscordSocketClient _discord;
        private readonly PictureService _p;

        public CatBotService(DiscordSocketClient discord, PictureService P)
        {
            _discord = discord;
            _p = P;
            //we'll just change the role colors when we get a message on the discord, instead of doing it over time ;)
            _discord.MessageReceived += MessageReceivedAsync;
            Console.WriteLine("Botcat service ready!");
        }

        
        private async Task MessageReceivedAsync(SocketMessage arg)
        {
            // Ignore system messages, or messages from other bots
            if (!(arg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            //Catbot channel
            if (arg.Channel.Name.ToLower() == "botcat") //More generic
            {
                if (message.Content.Length >= 15)
                    Console.WriteLine($"catbot:{message.Author} - {message.Content.Substring(15).ToLower()} - cattified :3"); //just to keep a basic console log
                else
                    Console.WriteLine($"catbot:{message.Author} - {message.Content.ToLower()} - cattified :3"); //just to keep a basic console log

                await arg.DeleteAsync();

                if (arg.Content.Length > 0 && arg.Content != null)
                {
                    string content = arg.Content;
                    // Replace invalid characters with empty strings.
                    try
                    {
                        content = Regex.Replace(content, @"[^a-zA-Z0-9 ]+", "", RegexOptions.None, TimeSpan.FromSeconds(5));
                        Console.WriteLine("Regexed into: " + content);
                    }
                    catch (TimeoutException) { }
                    // Get a stream containing an image of a cat
                    if (content == "" || content.Length == 0)
                        content = "error :)";
                    var stream = await _p.GetCatPictureAsync(content);
                    // Streams must be seeked to their beginning before being uploaded!
                    stream.Seek(0, SeekOrigin.Begin);
                    await arg.Channel.SendFileAsync(stream, "cat.png");
                }
            }
            return;
        }
        
    }
}
