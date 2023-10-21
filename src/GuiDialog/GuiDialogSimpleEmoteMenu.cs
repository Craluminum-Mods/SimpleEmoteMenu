using Vintagestory.API.Client;
using Vintagestory.API.Config;
using static SimpleEmoteMenu.Core;

namespace SimpleEmoteMenu;

public class GuiDialogSimpleEmoteMenu : GuiDialog
{
    protected double ListHeight => (emotes?.Length * 50) ?? 500;
    protected string[] emotes;

    public GuiDialogSimpleEmoteMenu(ICoreClientAPI capi) : base(capi) { }

    public override string ToggleKeyCombinationCode => Domain;

    public override bool CaptureAllInputs()
    {
        return true;
    }

    public override void OnKeyPress(KeyEvent args)
    {
        int key = KeyConverter.NewKeysToGlKeys[args.KeyCode];

        switch ((GlKeys)key)
        {
            case GlKeys.Number1: OnClick(0); break;
            case GlKeys.Number2: OnClick(1); break;
            case GlKeys.Number3: OnClick(2); break;
            case GlKeys.Number4: OnClick(3); break;
            case GlKeys.Number5: OnClick(4); break;
            case GlKeys.Number6: OnClick(5); break;
            case GlKeys.Number7: OnClick(6); break;
            case GlKeys.Number8: OnClick(7); break;
            case GlKeys.Number9: OnClick(8); break;
            case GlKeys.Number0: OnClick(9); break;
        }

        base.OnKeyPress(args);
    }

    public void AddButton(ElementBounds buttonBounds, out ElementBounds refBounds, int buttonId)
    {
        ElementBounds newBounds = ElementBounds.FixedSize(buttonBounds.fixedWidth, buttonBounds.fixedHeight).FixedUnder(buttonBounds, 5).WithAlignment(EnumDialogArea.CenterFixed);
        int num = buttonId == 9 ? 0 : buttonId + 1;
        string keyNumber = (num >= 0 && 9 >= num) ? $"[{num}] " : "";
        SingleComposer = SingleComposer.AddSmallButton(keyNumber + Lang.Get($"{Domain}:emote-{emotes[buttonId]}"), () => OnClick(buttonId), newBounds, key: $"button-{buttonId}");
        refBounds = newBounds;
    }

    private bool OnClick(int buttonId)
    {
        capi.SendChatMessage($"/emote {emotes[buttonId]}");
        return TryClose();
    }

    private void ComposeDialog()
    {
        ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.None).WithAlignment(EnumDialogArea.CenterFixed).WithFixedPosition(0, 140);
        ElementBounds leftColumn = ElementBounds.Fixed(0, 50, 240, ListHeight);

        ElementBounds buttonBaseBounds = ElementBounds.Fixed(EnumDialogArea.CenterFixed, leftColumn.fixedX, leftColumn.fixedY - 50, 0, 0);
        ElementBounds buttonBounds = ElementBounds.Fixed(buttonBaseBounds.fixedX, buttonBaseBounds.fixedY, leftColumn.fixedWidth, 40);

        ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);

        bgBounds.BothSizing = ElementSizing.FitToChildren;
        bgBounds.WithChildren(leftColumn);
        SingleComposer = capi.Gui.CreateCompo(Domain, dialogBounds)
        .AddShadedDialogBG(bgBounds)
        .AddDialogTitleBar(Lang.Get($"{Domain}:dialog-name"), OnTitleBarCloseClicked);

        ElementBounds prevBounds = buttonBounds;
        for (int i = 0; i < emotes.Length; i++)
        {
            AddButton(prevBounds, out ElementBounds refBounds, i);
            prevBounds = refBounds;
        }

        SingleComposer.Compose();
    }

    private void OnTitleBarCloseClicked()
    {
        TryClose();
    }

    public override bool TryOpen()
    {
        if (!base.TryOpen())
        {
            return false;
        }

        emotes ??= capi?.World?.Player?.Entity?.Properties?.Attributes?["emotes"]?.AsArray<string>();

        if (emotes?.Length != 0)
        {
            ComposeDialog();
        }

        return true;
    }
}