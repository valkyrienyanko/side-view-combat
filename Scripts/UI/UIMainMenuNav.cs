namespace Template;

public partial class UIMainMenuNav : Node
{
    SceneManager sceneManager;
    AudioManager audioManager;

    public override void _Ready()
    {
        sceneManager = Global.Services.Get<SceneManager>();
        audioManager = Global.Services.Get<AudioManager>();

        GetNode<Button>("Play").GrabFocus();
    }

    void _on_play_pressed()
    {
        //audioManager.PlayMusic(Music.Level1, false);
        sceneManager.SwitchScene("level_3D", SceneManager.TransType.Fade);
    }

    void _on_mods_pressed()
    {
        //audioManager.PlayMusic(Music.Level4);
        sceneManager.SwitchScene("UI/mod_loader");
    }

    void _on_options_pressed()
    {
        //audioManager.PlayMusic(Music.Level4);
        sceneManager.SwitchScene("Prefabs/UI/options");
    }

    void _on_credits_pressed()
    {
        //audioManager.PlayMusic(Music.Level4);
        sceneManager.SwitchScene("UI/credits");
    }

    void _on_quit_pressed() => GetNode<Global>("/root/Global").Quit();

    void _on_discord_pressed() => OS.ShellOpen("https://discord.gg/j8HQZZ76r8");
    void _on_github_pressed() => OS.ShellOpen("https://github.com/ValksGodotTools/Template");
}
