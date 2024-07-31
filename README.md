# Godot 4 C# Template
Never again spend 5 minutes setting up a new project, `ValksGodotTools/Template` has got your back. ❤️

Want to get right into it? Start off by reading the [setup guide](#setup-guide).

1. [Highlighted Features](#highlighted-features)
2. [Setup Guide](#setup-guide)
3. [Features](#features)
    - [Services](#services)
    - [Console Commands](#console-commands)
    - [Prefabs](#prefabs)
    - [Audio Manager](#audiomanager)
    - [Scene Manager](#scenemanager)
    - [State Manager](#state-manager)
    - [Experimental Event Manager](#experimental-eventmanager)
4. [Contributing](#contributing)
5. [Roadmap](#roadmap)
6. [Credits](#credits)

![main-menu](https://github.com/ValksGodotTools/Template/assets/6277739/e8abf19d-0ac7-4ae3-9942-e1b406edf7cf)  
![options](https://github.com/ValksGodotTools/Template/assets/6277739/c5a9e011-f433-4887-8947-36130dd83426)  
![keybindings](https://user-images.githubusercontent.com/6277739/236582745-8d69b91f-497f-4188-b669-66daaa43691d.png)  

## Highlighted Features
### ⭐ Automated Setup
Project settings are set for you and assets you do not need are deleted. See the start of the setup guide [here](#setup-guide).

### ⭐ Mod Loader
Mods can replace game assets and execute C# scripts, although there are some limitations. You can find the example mod repository [here](https://github.com/ValksGodotTools/ExampleMod).

### ⭐ In-Game Console
Press `F12` to bring up the [console](#console-commands) in game. New commands are very easy to code in.

### ⭐ Key Bindings
Full configuration of [key bindings](#keybindings) in-game.

### ⭐ Useful Functions
[Godot Utils](https://github.com/ValksGodotTools/GodotUtils) contains several useful extensions such as `.PrintFull()`, `.ForEach()` and `.QueueFreeChildren()`.

There are also useful classes like [Scene Manager](#scenemanager) and [Services](#services) that make your life easier.

### ⭐ Localisation
Add in your own [languages](https://github.com/ValksGodotTools/Template/blob/main/Localisation/text.csv).

## Setup Guide

### :one: Download the repo
1. Download and install the [latest Godot 4 C# release](https://godotengine.org/)
2. Clone with `git clone --recursive https://github.com/ValksGodotTools/Template`

If the GodotUtils folder is still empty for whatever reason, run `git submodule update --init --recursive`

### :two: Run the game with `F5`
> ℹ️ Steps 2 to 4 are optional but doing them with change arguably tedious to setup settings and delete unneeded assets.

You should see something like this

![setup-scene](https://github.com/user-attachments/assets/00262157-26e1-4909-9a71-7a3357a8c126)  

Enter a name for your game, this could be something like `muffin blaster 3`, this will be auto formatted to `MuffinBlaster3`. All namespaces in all scripts will be replaced with this name. The `.csproj`, `.sln` and `project.godot` files will also be modified with this new name.

Select the genre for your game. Currently there are only 3 types, "3D FPS", "2D Platformer" and "2D Top Down". Lets say you select "3D FPS", this means the "2D Platformer" and "2D Top Down" assets will be deleted and the 3D FPS assets will be moved to more accessible locations.

In all cases you will no longer see the following directories in your project.  

![asset-folders](https://github.com/user-attachments/assets/b04cd7bc-662f-4dc6-b89c-cb46f10799e8)  

### :three: Click "Apply Changes"

The following popup will appear, click "Reload"

![popup1](https://github.com/user-attachments/assets/8e037a68-235f-4df7-b3d1-94e0c9431808)

Close the Godot project and click "Don't Save" and close any text editors you may have opened

![popup2](https://github.com/user-attachments/assets/0d51ca08-adbc-4df3-9e5a-27cf1e5717d2)

### :four: Open the Godot Project

Click "Fix Dependencies" and then click "Fix Broken". Then and only after clicking "Fix Broken", click on "Open Anyway"

![popup3](https://github.com/user-attachments/assets/b0cae7be-21fa-46cd-8ee1-ee1f4b30cfad)
![popup4](https://github.com/user-attachments/assets/a4ab7d67-da32-46ef-9aa4-d5472eb18ec5)

If you selected "3D FPS" as an example then the 3D FPS scene should run when you press `F5`.

> ℹ️ Avoid deleting `res://Template` and `res://GodotUtils`, doing so will cause certain features to stop working. I have tried my best to move all assets you would need to modify for your game outside of `res://Template` into `res://`. If you want to modify the contents of `res://GodotUtils`, please consider creating a pull request on the [repo](https://github.com/ValksGodotTools/GodotUtils) first.

> ℹ️ A internet connection is required when running the game with `F5` for the first time. This is because the `.csproj` needs to retrieve the NuGet packages from the NuGet website.

## Features
### Services
**Stop using static everywhere!** Static exists for the lifetime of the application wasting valuable game memory. Instead lets make use of `Global.Services`.

In the `_Ready()` of any node add `Global.Services.Add(this)`. (if the script does not extend from node, you can use `Global.Services.Add<Type>`)
```cs
public partial class UIVignette : ColorRect
{
    public override void _Ready()
    {
        // Set persistent to true if this is an autoload script
        // (scripts that do not extend from Node are persistent by default)

        // Non persistent services will get removed just before the scene is changed
        // Example of persistent service: AudioManager; a node like this should exist
        // for the entire duration of the game

        // However this UIVignette exists within the scene so it should not be persistent
        Global.Services.Add(this, persistent: false); 
    }

    public void LightPulse() { ... }
}
```

Now you can get the instance of UIVignette from anywhere! No static or long GetNode\<T\> paths involved. It's magic.
```cs
UIVignette vignette = Global.Services.Get<UIVignette>();
vignette.LightPulse();
```

### Console Commands
```cs
// Simply add the "ConsoleCommand" attribute to any function
// it will be registered as a new console command

// Note to bring up the console in-game press F12

[ConsoleCommand("help")]
void Help()
{
    IEnumerable<string> cmds =
        Global.Services.Get<UIConsole>().Commands.Select(x => x.Name);

    Global.Services.Get<Logger>().Log(cmds.Print());
}

// Console commands can have aliases, this command has a
// alias called "exit"

[ConsoleCommand("quit", "exit")]
void Quit()
{
    GetTree().Root.GetNode<Global>("/root/Global").Quit();
}

// Method parameters are supported

[ConsoleCommand("debug")]
void Debug(int x, string y)
{
    Global.Services.Get<Logger>().Log($"Debug {x}, {y}");
}
```

### Prefabs
```cs
// Load all your scene prefabs here. This script can be found in
// "res://Scripts/Static/Prefabs.cs". Note that music and sounds are
// loaded in very similarily and these scripts can be found in the
// static folder as well.
public static class Prefabs
{
    public static PackedScene Options { get; } = Load("UI/options");

    static PackedScene Load(string path) =>
        GD.Load<PackedScene>($"res://Scenes/Prefabs/{path}.tscn");
}

// Prefabs are instantiated like this
UIOptions options = Prefabs.Options.Instantiate<UIOptions>();
```

### AudioManager
```cs
AudioManager audioManager = Global.Services.Get<AudioManager>();

// Play a soundtrack
audioManager.PlayMusic(Music.Menu);

// Play a sound
audioManager.PlaySFX(Sounds.GameOver);

// Set the music volume
audioManager.SetMusicVolume(75);

// Set the sound volume
audioManager.SetSFXVolume(100);

// Gradually fade out all sounds
audioManager.FadeOutSFX();
```

### SceneManager
```cs
// Switch to a scene instantly
Global.Services.Get<SceneManager>().SwitchScene("main_menu");

// Switch to a scene with a fade transition
Global.Services.Get<SceneManager>().SwitchScene("level_2D_top_down", 
    SceneManager.TransType.Fade);
```

### State Manager
This state manager uses **functions as states** as suppose to using classes for states. The [`State`](https://github.com/ValksGodotTools/GodotUtils/blob/ccd37342ab8d758a664d2abd3375a21b608d2198/State.cs) class is provided in the GodotUtils submodule. Below an example is given.

Create a new file named `Player.cs` and add the following script to it.
```cs
public partial class Player : Entity // This script extends from Entity but it may extend from CharacterBody3D for you
{
    State curState;

    public override void _Ready()
    {
        curState = Idle();
        curState.Enter();
    }

    public override void _PhysicsProcess(double delta)
    {
        curState.Update(delta);
    }

    public void SwitchState(State newState)
    {
        GD.Print($"Switched from {curState} to {newState}"); // Useful for debugging. May be more appealing to just say "Switched to {newState}" instead.

        curState.Exit();
        newState.Enter();
        curState = newState;
    }
}
```
Create another file named `PlayerIdle.cs` and add the following.
```cs
public partial class Player
{
    State Idle()
    {
        var state = new State(this, nameof(Idle));

        state.Enter = () =>
        {
            // What happens on entering the idle state?
        };

        state.Update = delta =>
        {
            // What happens on every frame in the idle state?
        };

        state.Exit = () =>
        {
            // What happens on exiting the idle state?
        }

        return state;
    }
}
```
Do a similar process when adding new states.

### Experimental EventManager
If you like the idea of having a universal static event manager that handles everything then try out the code below in your own project.

#### Event Enums
```cs
public enum EventGeneric
{
    OnKeyboardInput
}

public enum EventPlayer
{
    OnPlayerSpawn
}
```

#### Event Dictionaries
```cs
public static class Events
{
    public static EventManager<EventGeneric> Generic { get; } = new();
    public static EventManager<EventPlayer> Player { get; } = new();
}
```

#### Example #1
```cs
Events.Generic.AddListener(EventGeneric.OnKeyboardInput, (args) => 
{
    GD.Print(args[0]);
    GD.Print(args[1]);
    GD.Print(args[2]);
}, "someId");

Events.Generic.RemoveListeners(EventGeneric.OnKeyboardInput, "someId");

// Listener is never called because it was removed
Events.Generic.Notify(EventGeneric.OnKeyboardInput, 1, 2, 3);
```

#### Example #2
```cs
Events.Player.AddListener<PlayerSpawnArgs>(EventPlayer.OnPlayerSpawn, (args) => 
{
    GD.Print(args.Name);
    GD.Print(args.Location);
    GD.Print(args.Player);
});

Events.Player.Notify(EventPlayer.OnPlayerSpawn, new PlayerSpawnArgs(name, location, player));
```

## Contributing
Please have a quick look at the [Projects Coding Style](https://github.com/Valks-Games/sankari/wiki/Code-Style) and contact me over Discord before contributing. My Discord username is `valky5`.

## Roadmap
### 3D FPS
- Add an animated weapon model from Blender and add logic for it in game
- Add a test environment

### 2D Platformer
- Add example states

### 2D Top Down
- Add a sword swing system
- Add example states

### Mod Loader
- Beautify the mod loader scene and add the appropriate logic that follows it
- Figure out how to ignore scripts in mods. For example I want to add a script in each mod that helps modders export the C# dll mods but this script can't be included because it will conflict with the same script from other mods.
- Figure out how to allow duplicate scripts from different mods

## Msc
- Add a dialogue system that translates dialogue and choices in a text file to game logic
- Add a inventory system
- Add the ability to scroll in the credits scene
- Implement a dedicated server authorative multiplayer model

## Credits
For all credit to in-game assets used, see [credits.txt](https://github.com/ValksGodotTools/Template/blob/main/credits.txt).  
