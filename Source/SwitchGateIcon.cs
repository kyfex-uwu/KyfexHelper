using System;
using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/SwitchGateIcon")]
public class SwitchGateIcon : Entity {
    private readonly Color inactiveColor;
    private readonly Color activeColor;
    private readonly Color finishColor;
    
    private readonly Sprite icon;
    private readonly string flag;
    private readonly Wiggler wiggler;

    private readonly float moveTime;
    
    private readonly Color particleColor;
    private readonly bool particles;

    private readonly bool shouldInheritInfo;
    
    public SwitchGateIcon(EntityData data, Vector2 offset) : base(data.Position + offset) {
        this.shouldInheritInfo = data.Bool("takeFromRiding", false);
        
        this.flag = data.String("flag", "");
        this.moveTime = data.Float("time", 1.8f);
        
        this.Depth = data.Int("depth",-9000);
        this.Add(this.icon = new Sprite(GFX.Game, data.String("icon","objects/switchgate/icon")));
        this.icon.Add("spin", "", 0.1f, "spin");
        this.icon.Play("spin");
        this.icon.Rate = 0.0f;
        
        this.icon.Color = this.inactiveColor = data.HexColor("inactive", Calc.HexToColor("5fcde4"));
        this.activeColor = data.HexColor("active", Color.White);
        this.finishColor = data.HexColor("finish", Calc.HexToColor("f141df"));

        this.particleColor = data.HexColor("color", TouchSwitch.P_Fire.Color);
        this.particles = data.Bool("particles", true);

        this.icon.CenterOrigin();

        this.Add(this.wiggler = Wiggler.Create(
            data.Float("shake", 0.5f), 
            data.Float("shakeStrength", 1f)*4f,  
            f => this.icon.Scale = Vector2.One * (1f + f)));
        
        if (data.Bool("attached", true)) {
            this.Add(new StaticMover() {
                SolidChecker = s => s.CollideRect(new Rectangle((int)this.X - 4, (int)this.Y - 4, 8, 8)),
                OnDestroy = this.RemoveSelf,
                OnMove = v => this.Position += v,
                OnShake = v => this.Position += v,
            });
        }
    }
    
    public override void Awake(Scene scene) {
        base.Awake(scene);
        if (this.touchSwitchActive()) {
            this.icon.Rate = 0.0f;
            this.icon.SetAnimationFrame(0);
            this.icon.Color = this.finishColor;
        }
        else {
            this.Add(new Coroutine(this.Sequence()));
        }
    }
    
    public override void Render() {
        this.icon.DrawOutline();
        base.Render();
    }
    
    public IEnumerator Sequence() {
        while (!this.touchSwitchActive()) yield return null;
        yield return 0.1f;
      
        while ( this.icon.Rate < 1.0) {
            this.icon.Color = Color.Lerp(this.inactiveColor, this.activeColor, this.icon.Rate);
            this.icon.Rate += Engine.DeltaTime * 2f;
            yield return null;
        }
       
        yield return 0.3f+this.moveTime;
        while (this.icon.Rate > 0.0) { 
            this.icon.Color = Color.Lerp(this.activeColor, this.finishColor, 1f - this.icon.Rate);
            this.icon.Rate -= Engine.DeltaTime * 4f;
            yield return null;
        }
        this.icon.Rate = 0.0f;
        this.icon.SetAnimationFrame(0);
        this.wiggler.Start();
        for (int index = 0; index < 32; ++index) {
            float num = Calc.Random.NextFloat(6.2831855f);
            this.SceneAs<Level>().ParticlesFG.Emit(TouchSwitch.P_FireWhite, 
                this.Center + Calc.AngleToVector(num, 4f), this.particleColor, num);
        }
    }

    private bool touchSwitchActive() {
        if (this.flag == "") {
            return Switch.Check(this.SceneAs<Level>());
        } else {
            return this.SceneAs<Level>().Session.GetFlag(this.flag);
        }
    }
}