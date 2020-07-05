﻿using Discord.Commands;
using System.Threading.Tasks;
using Enums.PictureServices;
using HeadpatPictures.Contracts;
using TastyBot.Contracts;
using System;
using System.Linq;
using System.Globalization;

namespace TastyBot.Modules
{
    [Name("Picture Commands")]
    public class PictureModule : ModuleBase<SocketCommandContext>
    {
        private readonly ICatModule _catModule;
        private readonly INekoClientModule _nekoClientModule;

        public PictureModule(ICatModule catModule, INekoClientModule nekoClientModule)
        {
            _catModule = catModule;
            _nekoClientModule = nekoClientModule;
        }

        #region Cat meow meow region
        [Command("cat")]
        public async Task CatAsync(int textsize = 32, string Colour = "white", [Remainder] string text = " ")
        {
            var s = await _catModule.CatPictureAsync(textsize, Colour, text);
            await Context.Channel.SendFileAsync(s, "cat.png");
        }


        [Command("cat")]
        public async Task CatAsync(string Colour = "white", [Remainder] string text = " ")
        {
            if (Colour.ToLower() != "gif")
            {
                var stream = await _catModule.CatPictureAsync(32, Colour, text);
                await Context.Channel.SendFileAsync(stream, "cat.png");
            }
            else
            {
                var stream = await _catModule.CatGifAsync();
                await Context.Channel.SendFileAsync(stream, "cat.gif");
            }
        }

        [Command("cat")]
        public async Task CatAsync([Remainder] string text = " ")
        {
            var stream = await _catModule.CatPictureAsync(32, "white", text);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }
        #endregion 

        #region Nekos

        [Command("neko")]
		public async Task NekoAsync([Remainder]string text = "")
		{
			var s = await _nekoClientModule.SFWNekoClientPictureAsync(RegularNekos.Neko, text);
			await Context.Channel.SendFileAsync(s, "Neko.png");
		}

		[Command("nekoavatar")]
		public async Task NekoAvatarAsync([Remainder]string text = "")
		{
			var s = await _nekoClientModule.SFWNekoClientPictureAsync(RegularNekos.Avatar, text);
			await Context.Channel.SendFileAsync(s, "Avatar.png");
		}

		[Command("nekowallpaper")]
		public async Task NekoWallpaperAsync([Remainder]string text = "")
		{
			var s = await _nekoClientModule.SFWNekoClientPictureAsync(RegularNekos.Wallpaper, text);
			await Context.Channel.SendFileAsync(s, "Wallpaper.png");
		}

		[Command("fox")]
		public async Task FoxAsync([Remainder]string text = "")
		{
			var s = await _nekoClientModule.SFWNekoClientPictureAsync(RegularNekos.Fox, text);
			await Context.Channel.SendFileAsync(s, "Fox.png");
		}

        #endregion

        #region NSFW Nekos owo

        //TODO: ADD command descriptors to all commands in this module.
        [Command("NSFW")]
        [RequireNsfw]
        public async Task NSFWAhegaoAsync([Remainder]string text = "")
        {

            string E = text;
            NSFWNekos res;

            if (E == "")
                res = RandomEnumValue<NSFWNekos>();
            else
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                E = textInfo.ToTitleCase(text.Split(' ').FirstOrDefault());
                Console.WriteLine(E);
                Enum.TryParse(E, out res);
            }
            text = string.Join(' ', text.Split(' ').Skip(1));

            var s = await _serv.GetNSFWNekoClientPictureAsync(res, text);
            await Context.Channel.SendFileAsync(s, "OwO.png");
        }
        #endregion
  
        static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(_R.Next(v.Length));
        }

    }

}