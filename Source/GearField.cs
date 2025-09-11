using System;
using System.Collections.Generic;
using System.Globalization;
using Celeste.Mod.auspicioushelper;
using Celeste.Mod.auspicioushelper.iop;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper.ausZip;

[CustomEntity("KyfexHelper/GearField"), Tracked]
public class GearField : Entity {
    public readonly List<MTexture> cogs;
    public readonly Color bgColor;
    public readonly TemplateIop.TemplateChildComponent tcComponent;
    private readonly MTexture temp = new();
    
    public GearField(EntityData data, Vector2 offset) : base(offset + data.Position) {
        this.Depth = -10000;
        this.Collider = new Hitbox(data.Width, data.Height);
        this.cogs = GFX.Game.GetAtlasSubtextures(data.String("gearPath", "objects/zipmover/innercog"));
        this.bgColor = Calc.HexToColor(data.String("bgColor", "000000").PadRight(8,'f').Substring(0, 6)) *
                       (int.Parse(data.String("bgColor", "000000ff").PadRight(8, 'f').Substring(6, 2),
                           NumberStyles.HexNumber) / 255f);
        
        this.Add(this.tcComponent = new TemplateIop.TemplateChildComponent(this));
    }
    
    private static float mod(float x, float m) => (x % m + m) % m;
    public override void Render() {
        base.Render();
        if (!(AuspiciousCompat.tcComponentParentField.GetValue(this.tcComponent) is TemplateZipmover template)) return;
        
        Draw.Rect(this.X, this.Y, this.Width, this.Height, this.bgColor);
        int yPolarity = 1;
        float rotation = 0.0f;
        var spline = (SplineAccessor)AuspiciousCompat.sposField.GetValue(template);
        var percent = spline.t % spline.numsegs;
        
        for (int y = 4; y <= this.Height - 4.0; y += 8) {
            int xPolarity = yPolarity;
            for (int x = 4; x <= this.Width - 4.0; x += 8) {
                MTexture innerCog = this.cogs[(int) ((double) mod((float) ((rotation + yPolarity * percent * Math.PI * 4.0) / (Math.PI/2)), 1f) * this.cogs.Count)];
                Rectangle rectangle = new Rectangle(0, 0, innerCog.Width, innerCog.Height);
                Vector2 zero = Vector2.Zero;
                if (x <= 4) {
                    zero.X = 2f;
                    rectangle.X = 2;
                    rectangle.Width -= 2;
                }
                else if (x >= this.Width - 4.0) {
                    zero.X = -2f;
                    rectangle.Width -= 2;
                }
                if (y <= 4) {
                    zero.Y = 2f;
                    rectangle.Y = 2;
                    rectangle.Height -= 2;
                }
                else if (y >= this.Height - 4.0) {
                    zero.Y = -2f;
                    rectangle.Height -= 2;
                }
                innerCog.GetSubtexture(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, this.temp)
                    .DrawCentered(this.Position + new Vector2(x, y) + zero, Color.White * (yPolarity < 0 ? 0.5f : 1f));
                yPolarity = -yPolarity;
                rotation += (float) Math.PI/3;
            }
            if (xPolarity == yPolarity) yPolarity = -yPolarity;
        }
    }
}