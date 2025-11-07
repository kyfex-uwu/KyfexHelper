using System;
using System.Collections.Generic;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/TempleGateLoooongWay")]
public class TempleGateLoooongWay : TempleGate {
    private readonly Sprite baseSprite;
    private readonly Sprite topSprite;
    private readonly List<Sprite> middleSprites = new ();
    public TempleGateLoooongWay(EntityData data, Vector2 offset, string levelID)
        : base(data, offset, levelID) {
        // this.Remove(this.sprite);
        // var name = data.Attr(nameof(sprite), "default");
        //
        // this.Add(this.baseSprite = GFX.SpriteBank.Create("KyfexHelper_temple_gate_loooongway_" + name + "_base"));
        // this.baseSprite.X = this.Width / 2f;
        // this.baseSprite.Y = this.Height - 34;
        // this.baseSprite.Play("idle");
        //
        // for (int i = 33; i < this.Height; i += 24) {
        //     var toAdd = GFX.SpriteBank.Create("KyfexHelper_temple_gate_loooongway_" + name + "_middle");
        //     toAdd.X = this.Width / 2f;
        //     toAdd.Y = this.Height - i-24;
        //     toAdd.Play("idle");
        //     if (i % 16 != 0) toAdd.Scale.X = -1;
        //     this.middleSprites.Add(toAdd);
        //     this.Add(toAdd);
        // }
        //
        // this.Add(this.topSprite = GFX.SpriteBank.Create("KyfexHelper_temple_gate_loooongway_" + name + "_top"));
        // this.topSprite.X = this.Width / 2f;
        // this.topSprite.Play("idle");
    }

    // public override void Render() {
    //     Vector2 vector2 = new Vector2(Math.Sign(this.shaker.Value.X), 0.0f);
    //     Draw.Rect(this.X - 2f, this.Y - 8f, 14f, 10f, Color.Black);
    //     var rect = new Rectangle(0, (int)(this.sprite.Height - this.drawHeight), (int)this.sprite.Width,
    //         (int)this.drawHeight);
    //     this.baseSprite.DrawSubrect(vector2+new Vector2(0,this.Height-34), rect);
    //     for(int i=0;i<this.middleSprites.Count;i++)
    //         this.middleSprites[i].DrawSubrect(vector2+new Vector2(0,this.Height-34-i*24), rect);
    //     this.topSprite.DrawSubrect(vector2, rect);
    // }

    //--

    private static void SwitchOpenHook(On.Celeste.TempleGate.orig_SwitchOpen orig, TempleGate self) {
        if (self is TempleGateLoooongWay loooongWay) {
            loooongWay.topSprite.Play("open");
            loooongWay.baseSprite.Play("open");
            foreach (var sprite in loooongWay.middleSprites) sprite.Play("open");
        }
        orig(self);
    }
    private static void OpenHook(On.Celeste.TempleGate.orig_Open orig, TempleGate self) {
        if (self is TempleGateLoooongWay loooongWay) {
            loooongWay.topSprite.Play("open");
            loooongWay.baseSprite.Play("open");
            foreach (var sprite in loooongWay.middleSprites) sprite.Play("open");
        }
        orig(self);
    }
    private static void CloseHook(On.Celeste.TempleGate.orig_Close orig, TempleGate self) {
        if (self is TempleGateLoooongWay loooongWay) {
            loooongWay.topSprite.Play("hit");
            loooongWay.baseSprite.Play("hit");
            foreach (var sprite in loooongWay.middleSprites) sprite.Play("hit");
        }
        orig(self);
    }

    public static void Load() {
        // On.Celeste.TempleGate.SwitchOpen += SwitchOpenHook;
        // On.Celeste.TempleGate.Open += OpenHook;
        // On.Celeste.TempleGate.Close += CloseHook;
    }

    public static void Unload() {
        // On.Celeste.TempleGate.SwitchOpen -= SwitchOpenHook;
        // On.Celeste.TempleGate.Open -= OpenHook;
        // On.Celeste.TempleGate.Close -= CloseHook;
    }
}