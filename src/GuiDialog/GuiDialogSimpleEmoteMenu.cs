using static SimpleEmoteMenu.Core;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using System.Collections.Generic;

namespace SimpleEmoteMenu;

public class GuiDialogSimpleEmoteMenu : GuiDialog
{
    protected string[] Emotes { get; private set; }

    public GuiDialogSimpleEmoteMenu(ICoreClientAPI capi) : base(capi) { }

    public override string ToggleKeyCombinationCode => Domain;

    public override bool CaptureAllInputs()
    {
        return IsOpened();
    }

    public override void OnKeyDown(KeyEvent args)
    {
        HandleAutoSelectionByKeyPress(args);
        base.OnKeyDown(args);
    }

    public static string[] GetTranslatedEmotes(string[] emotes)
    {
        return emotes
        .ToList()
        .ConvertAll(emote => $"[{GlKeyNames.ToString(IndexesAndGlKeys[emotes.ToList().IndexOf(emote)])}] " + Lang.Get($"{Core.Domain}:emote-{emote}"))
        .ToArray();
    }

    public static Dictionary<int, GlKeys> IndexesAndGlKeys { get; } = new()
    {
        [-1] = GlKeys.Unknown,
        [0] = GlKeys.Number1,
        [1] = GlKeys.Number2,
        [2] = GlKeys.Number3,
        [3] = GlKeys.Number4,
        [4] = GlKeys.Number5,
        [5] = GlKeys.Number6,
        [6] = GlKeys.Number7,
        [7] = GlKeys.Number8,
        [8] = GlKeys.Number9,
        [9] = GlKeys.Number0,
        [10] = GlKeys.A,
        [11] = GlKeys.B,
        [12] = GlKeys.C,
        [13] = GlKeys.D,
        [14] = GlKeys.E,
        [15] = GlKeys.F,
        [16] = GlKeys.G,
        [17] = GlKeys.H,
        [18] = GlKeys.I,
        [19] = GlKeys.J,
        [20] = GlKeys.K,
        [21] = GlKeys.L,
        [22] = GlKeys.M,
        [23] = GlKeys.N,
        [24] = GlKeys.O,
        [25] = GlKeys.P,
        [26] = GlKeys.Q,
        [27] = GlKeys.R,
        [28] = GlKeys.S,
        [29] = GlKeys.T,
        [30] = GlKeys.U,
        [31] = GlKeys.V,
        [32] = GlKeys.W,
        [33] = GlKeys.X,
        [34] = GlKeys.Y,
        [35] = GlKeys.Z
    };

    public void HandleAutoSelectionByKeyPress(KeyEvent args)
    {
        int index = IndexesAndGlKeys.FirstOrDefault(x => x.Value == (GlKeys)args.KeyCode, IndexesAndGlKeys.First()).Key;
        if (index != -1 && Emotes.Length > index)
        {
            SelectionChangedDelegate(Emotes[index], true);
        }
    }

    private void ComposeDialog()
    {
        ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.None).WithAlignment(EnumDialogArea.CenterFixed).WithFixedPosition(0, 140);
        ElementBounds leftColumn = ElementBounds.Fixed(0, 50, 240, 0);

        ElementBounds buttonBaseBounds = ElementBounds.Fixed(EnumDialogArea.CenterFixed, leftColumn.fixedX, leftColumn.fixedY - 10, leftColumn.fixedWidth, 40);
        ElementBounds dropdownBounds = ElementBounds.Fixed(buttonBaseBounds.fixedX, buttonBaseBounds.fixedY, leftColumn.fixedWidth, 40);

        dialogBounds.WithChildren(leftColumn);
        SingleComposer = capi.Gui.CreateCompo(Domain, dialogBounds)
        .AddDialogTitleBar(Lang.Get($"{Domain}:dialog-name"), () => TryClose())
        .AddDropDown(Emotes, GetTranslatedEmotes(Emotes), 0, SelectionChangedDelegate, dropdownBounds, "dropdown-emotes")
        .Compose();
    }

    private void SelectionChangedDelegate(string code, bool selected)
    {
        if (selected)
        {
            capi.SendChatMessage($"/emote {code}");
            TryClose();
        }
    }

    public override bool TryOpen()
    {
        if (!base.TryOpen())
        {
            return false;
        }

        Emotes ??= capi?.World?.Player?.Entity?.Properties?.Attributes?["emotes"]?.AsArray<string>();

        if (Emotes?.Length != 0)
        {
            ComposeDialog();
            SingleComposer.GetDropDown("dropdown-emotes").listMenu.Open();
        }

        return true;
    }
}