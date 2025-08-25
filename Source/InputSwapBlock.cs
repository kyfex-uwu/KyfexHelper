

using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

[Tracked, CustomEntity("KyfexHelper/InputSwapBlock")]
public class InputSwapBlock : SwapBlock {
    private bool shouldSwap = false;
    public InputSwapBlock(EntityData data, Vector2 offset) : base(data, offset) {
        this.Add(new InputListener(data.Enum("input", InputListener.Input.DASH),
            () => {
                this.shouldSwap = true;
            }));
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