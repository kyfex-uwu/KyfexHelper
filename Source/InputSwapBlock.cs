

using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

[Tracked, CustomEntity("KyfexHelper/InputSwapBlock")]
public class InputSwapBlock : SwapBlock {
    private bool shouldSwap = false;
    private readonly string bgMaybe;
    public InputSwapBlock(EntityData data, Vector2 offset) : base(data, offset) {
        this.Add(new InputListener(data.Enum("input", InputListener.Input.DASH),
            () => {
                this.shouldSwap = true;
            }));

        var inactiveTex = data.String("inactive");
        var activeTex = data.String("active");
        var bgTex = data.String("bgTex");
        for (int index1 = 0; index1 < 3; ++index1) {
            for (int index2 = 0; index2 < 3; ++index2) {
                if(activeTex != null)
                    this.nineSliceGreen[index1, index2] = GFX.Game[activeTex].GetSubtexture(new Rectangle(index1 * 8, index2 * 8, 8, 8));
                if(inactiveTex != null)
                    this.nineSliceRed[index1, index2] = GFX.Game[inactiveTex].GetSubtexture(new Rectangle(index1 * 8, index2 * 8, 8, 8));
                if(bgTex != null)
                    this.nineSliceTarget[index1, index2] = GFX.Game[bgTex].GetSubtexture(new Rectangle(index1 * 8, index2 * 8, 8, 8));
            }
        }

        var activeTexCenter = data.String("activeCenter");
        if (activeTexCenter != null) {
            this.Remove(this.middleGreen);
            this.Add(this.middleGreen = GFX.SpriteBank.Create(activeTexCenter));
        }
        var inactiveTexCenter = data.String("inactiveCenter");
        if (inactiveTexCenter != null) {
            this.Remove(this.middleRed);
            this.Add(this.middleRed = GFX.SpriteBank.Create(inactiveTexCenter));
        }

        this.bgMaybe = data.String("underBg");
        if (this.bgMaybe != null) this.Theme = Themes.Normal;
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if(this.bgMaybe != null)
            this.path.pathTexture = GFX.Game[this.bgMaybe + (this.start.X == this.end.X ? "V" : "H")];
    }

    public override void Update() {
        if(this.shouldSwap) this.OnDash(Vector2.UnitX);
        base.Update();
    }

    private static void hookDash(On.Celeste.SwapBlock.orig_OnDash orig, SwapBlock self, Vector2 dir) {
        if (self is InputSwapBlock inputSwapBlock) {
            if (inputSwapBlock.shouldSwap) orig(self, dir);
            inputSwapBlock.shouldSwap = false;
            return;
        }
        orig(self, dir);
    }
    private static void inputListeners(On.Monocle.Scene.orig_Update orig, Scene self) {
        orig(self);
        foreach (var e in self.Tracker.GetComponents<InputListener>()) {
            var listener = e as InputListener;
            switch (listener.input) {
                case InputListener.Input.DASH: 
                    if(Input.Dash.Pressed) listener.OnInput();
                    break;
                case InputListener.Input.DEMO: 
                    if(Input.CrouchDash.Pressed) listener.OnInput();
                    break;
                case InputListener.Input.DASHORDEMO: 
                    if(Input.CrouchDash.Pressed || Input.Dash.Pressed) listener.OnInput();
                    break;
                case InputListener.Input.JUMP: 
                    if(Input.Jump.Pressed) listener.OnInput();
                    break;
                case InputListener.Input.GRAB: 
                    if(Input.Grab.Pressed) listener.OnInput();
                    break;
            }
        }
    }
    
    public static void Load() {
        On.Celeste.SwapBlock.OnDash += hookDash;
        On.Monocle.Scene.Update += inputListeners;
    }
    public static void Unload() {
        On.Celeste.SwapBlock.OnDash -= hookDash;
        On.Monocle.Scene.Update -= inputListeners;
    }
}