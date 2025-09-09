using Celeste.Mod.auspicioushelper;
using Celeste.Mod.auspicioushelper.iop;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper.ausZip;

[CustomEntity("KyfexHelper/ZipperTrafficLight")]
public class ZipperTrafficLight : Entity{
    public readonly TemplateIop.TemplateChildComponent tcComponent;
    private readonly Decal decal1;
    private readonly Decal decal2;
    private readonly Decal decal3;
    public ZipperTrafficLight(EntityData data, Vector2 offs) : base(data.Position + offs) {
        this.Depth = -10001;
        
        this.Add(this.tcComponent = new TemplateIop.TemplateChildComponent(this)); 
        this.decal1 = new Decal(data.String("decal1", ""), this.Position, Vector2.One, -10001);
        this.decal2 = new Decal(data.String("decal2", ""), this.Position, Vector2.One, -10001);
        this.decal3 = new Decal(data.String("decal3", ""), this.Position, Vector2.One, -10001);
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        scene.Add(this.decal1);
        scene.Add(this.decal2);
        scene.Add(this.decal3);
    }

    public override void Update() {
        base.Update();

        if (!this.Visible) {
            this.enable(false, 1);
            this.enable(false, 2);
            this.enable(false, 3);
            return;
        }
        
        if (!(AuspiciousCompat.tcComponentParentField.GetValue(this.tcComponent) is TemplateZipmover template)) return;
        var spline = (SplineAccessor)AuspiciousCompat.sposField.GetValue(template);
        if (template.triggered) {//green
            this.enable(false,1);
            this.enable(false,2);
            this.enable(true,3);
        } else {
            if (spline.t == 0) {//red
                this.enable(true,1);
                this.enable(false,2);
                this.enable(false,3);
            } else {//yellow
                this.enable(false,1);
                this.enable(true,2);
                this.enable(false,3);
            }
        }
    }

    public override void Render() {
        this.decal1.Position = this.Position;
        this.decal2.Position = this.Position;
        this.decal3.Position = this.Position;
        base.Render();
    }

    private bool decal1Enabled = true;
    private bool decal2Enabled = true;
    private bool decal3Enabled = true;

    private void enable(bool enabled, int decal) {
        var decalObj = this.decal1;
        switch (decal) {
            case 1:
                if (this.decal1Enabled == enabled) return;
                this.decal1Enabled = enabled;
                decalObj = this.decal1;
                break;
            case 2:
                if (this.decal2Enabled == enabled) return;
                this.decal2Enabled = enabled;
                decalObj = this.decal2;
                break;
            case 3:
                if (this.decal3Enabled == enabled) return;
                this.decal3Enabled = enabled;
                decalObj = this.decal3;
                break;
        }
        if(!enabled) decalObj.RemoveSelf();
        else 
            this.Scene.Add(decalObj);
    }
}