using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

[Tracked(false)]
public class InputListener : Component {
    public enum Input {
        JUMP, DASH, DEMO, GRAB, DASHORDEMO
    }
    
    public readonly Input input;
    public readonly Action OnInput;

    public InputListener(Input input, Action onInput) : base(true, false){
        this.input = input;
        this.OnInput = onInput;
    }
}