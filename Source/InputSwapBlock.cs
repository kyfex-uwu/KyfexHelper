

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
        foreach (var listener in self.Tracker.GetComponents<InputListener>()) {
            switch ((listener as InputListener).input) {
                case InputListener.Input.DASH: 
                    if(Input.Dash.Pressed) (listener as InputListener).OnInput();
                    break;
                case InputListener.Input.JUMP: 
                    if(Input.Jump.Pressed) (listener as InputListener).OnInput();
                    break;
                case InputListener.Input.DEMO: 
                    if(Input.CrouchDash.Pressed) (listener as InputListener).OnInput();
                    break;
                case InputListener.Input.GRAB: 
                    if(Input.Grab.Pressed) (listener as InputListener).OnInput();
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