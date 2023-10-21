using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

[assembly: ModInfo(name: "Simple Emote Menu", modID: "simpleemotemenu", Side = "Client")]

namespace SimpleEmoteMenu;

public class Core : ModSystem
{
    public const string Domain = "simpleemotemenu";

    private ICoreClientAPI _capi;
    private GuiDialog _dialog;

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        _capi = api;

        api.Input.RegisterHotKey(
          Domain,
          Lang.Get($"{Domain}:dialog-name", $"{Domain}:dialog-name"),
          GlKeys.K,
          HotkeyType.GUIOrOtherControls);

        api.Input.SetHotKeyHandler(Domain, ToggleGui);

        api.World.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }

    private bool ToggleGui(KeyCombination keyCombination)
    {
        _dialog ??= new GuiDialogSimpleEmoteMenu(_capi);

        if (!_dialog.IsOpened())
        {
            return _dialog.TryOpen();
        }

        if (!_dialog.TryClose())
        {
            return true;
        }

        _dialog.Dispose();
        _dialog = null;
        return true;
    }
}