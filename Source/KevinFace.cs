using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Celeste.Mod.auspicioushelper;
using Celeste.Mod.auspicioushelper.iop;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/KevinFace")]
public class KevinFace : Entity{
    public readonly TemplateIop.TemplateChildComponent tcComponent;
    private readonly Sprite sprite;
    
    public KevinFace(EntityData data, Vector2 offset) : base(offset + data.Position) {
        this.Depth = -10000;
        this.Add(this.tcComponent = new TemplateIop.TemplateChildComponent(this));
        
        this.Add(this.sprite = GFX.SpriteBank.Create(data.String("facePath", "crushblock_face")));
        this.sprite.Play("idle");
        this.sprite.OnLastFrame = f => {
            if (f != "hit") return;
            var parent = AuspiciousCompat.tcComponentParentField.GetValue(this.tcComponent);
            if (!(parent is TemplateKevin kevin)) return;
            if(nextFaces.TryGetValue(kevin, out var nextFace))
                this.sprite.Play(nextFace);
        };
    }

    private bool inited = false;
    public override void Update() {
        base.Update();
        if (!this.inited) {
            this.inited = true;
            var tempalte = (Template)AuspiciousCompat.tcComponentParentField.GetValue(this.tcComponent);
            if (!(tempalte is TemplateKevin kevin)) return;
            if (!facesFromTemplate.TryGetValue(kevin, out var faceList)) {
                facesFromTemplate[kevin] = new List<KevinFace>();
                faceList = facesFromTemplate[kevin];
            }
            faceList.Add(this);
        }
    }

    public override void Render() {
        base.Render();
        if (!(AuspiciousCompat.tcComponentParentField.GetValue(this.tcComponent) is TemplateKevin template)) return;
    }

    private static IEnumerator OnSuccessfulHit(Func<TemplateKevin, Vector2, IEnumerator> orig, TemplateKevin self,
        Vector2 dir) {
        if (dir.Angle() > Math.PI * 3f / 4f) nextFaces[self] = "left";
        else if (dir.Angle() > Math.PI / 4f) nextFaces[self] = "up";
        else if (dir.Angle() > Math.PI / -4f) nextFaces[self] = "right";
        else if (dir.Angle() > Math.PI * 3f / -4f) nextFaces[self] = "down";
        else nextFaces[self] = "left";
        if (facesFromTemplate.TryGetValue(self, out var faces)) {
            foreach (var face in faces) face.sprite.Play("hit");
        }
        return new SwapImmediately(orig(self, dir));
    }

    private static Dictionary<TemplateKevin, string> nextFaces = new();
    private static Dictionary<TemplateKevin, List<KevinFace>> facesFromTemplate = new();
    private static List<Hook> hooks = new();
    public static void LoadHooks() {
        hooks.Add(new Hook(typeof(TemplateKevin).GetMethod("GoSequence", BindingFlags.Instance|BindingFlags.NonPublic),
            OnSuccessfulHit));
    }
    public static void UnloadHooks() {
        foreach (var hook in hooks) hook.Dispose();
    }
}