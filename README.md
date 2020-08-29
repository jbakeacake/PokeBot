# PokeBot
A Discord Pokemon bot that sporadically displays Pokemon throughout the day. Any user can use a command to display their collection or catch a new pokemon!

## Features
### Collect them All! (Gen I Only)

Every hour, you have the opportunity to catch some of your favorite Pokemon from the first original 151 pokemon.

### Duel Each Other and Become the Very Best (Still Testing)

_Note: This feature is not entirely complete, though the full combat cycle is ready_
Challenge your fellow discord members with PokeBot's dueling system. Train and level up your Pokemon, increasing their power and stats along the way.

# Quick Start
> **Pre-Setup**

Before running the bot, there's a couple of things that need to be set up in order to get your own bot up and running. If you don't have your own
Discord Bot set up, take a look at this article on [how to make a Discord Bot](https://www.digitaltrends.com/gaming/how-to-make-a-discord-bot/) -- follow this guide's steps
from **step 2** to **step 4**. Make sure you know where to find your bot's `Token` before moving on. Additionally,to protect your bot 
from being abused on your server make sure you keep your `Token` secret, and hidden from the public.


> **Configuring our App Settings**

With you Discord Bot's `Token` in hand, navigate to `./path/to/my/PokeBot/` and create a new `.json` file called `appsettings.json`.
Inside this file, copy and paste the following:

```
{
    "ConnectionStrings": {
        "DefaultConnection": "Data Source=pokebot.db"
    },
    "AppSettings": {
        "Token": "[Discord_Bot_Token]",
        "ChannelId": [textChannelId : ulong]
    }
}
```

where `[Discord_Bot_Token]` is the actual token you acquired from **Pre-Setup**, and `[textChannelId : ulong]` is the id of type `ulong` for 
the text channel you want the bot to output Pokemon to (you can find this by entering 'developer mode' in discord, then right clicking and copying the id of the
text channel on your server).

> **Running the Application**

Once your bot is in your server and you've completed the `appsettings.json` file, you're free to run the command `dotnet run` to launch the application!

> **Commands**

Here's a list of commands you can use:
- !catch : Captures the current wandering Pokemon. Is succesful if you are the first user to send the command.
- !inventory : Displays a list of all the Pokemon you've caught.

## Dependencies
- [Discord .Net v2.2.0](https://github.com/discord-net/Discord.Net)

## Requirements
- [.Net SDK](https://dotnet.microsoft.com/download)

