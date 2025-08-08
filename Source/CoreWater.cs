using System.Collections.Generic;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

[CustomEntity("KyfexHelper/CoreWater"), TrackedAs(typeof(Water), true)]
public class CoreWater : Water {
    public readonly bool hotDangerous;
    public readonly bool coldDangerous;
    public readonly Color hotColor;
    public readonly Color coldColor;
    public readonly Color hotColorEdge;
    public readonly Color coldColorEdge;

    private bool isHot = false;
    
    public CoreWater(EntityData data, Vector2 offset)
        : base(data.Position + offset, true, data.Bool("hasBottom"), data.Width, data.Height) {
        this.hotDangerous = data.Bool("hotDangerous");
        this.coldDangerous = data.Bool("coldDangerous");
        this.hotColor = data.HexColor("hotColor") * data.Float("hotTransparency");
        this.coldColor = data.HexColor("coldColor") * data.Float("coldTransparency");
        this.hotColorEdge = data.HexColor("hotColorEdge");
        this.coldColorEdge = data.HexColor("coldColorEdge");
        
        this.Add(new CoreModeListener(this.OnCoreMode));
        this.Add(new PlayerCollider(this.OnCollide));
    }
    
    public override void Update() {
        base.Update();

        foreach (var surface in this.Surfaces) {
            int num1 = (int) (surface.Width / 4.0);
        
            for (int fillStartIndex = surface.fillStartIndex; fillStartIndex < surface.fillStartIndex + num1 * 6; ++fillStartIndex)
                surface.mesh[fillStartIndex].Color = (this.isHot ? this.hotColor : this.coldColor);
            for (int surfaceStartIndex = surface.surfaceStartIndex; surfaceStartIndex < surface.surfaceStartIndex + num1 * 6; ++surfaceStartIndex)
                surface.mesh[surfaceStartIndex].Color = (this.isHot ? this.hotColorEdge : this.coldColorEdge);
        
            var pos = surface.rayStartIndex;
            foreach (var ray in surface.Rays) {
                float num3 = 1f;
                if (ray.Percent < 0.1)
                    num3 = Calc.ClampedMap(ray.Percent, 0.0f, 0.1f);
                else if (ray.Percent > 0.9)
                    num3 = Calc.ClampedMap(ray.Percent, 0.9f, 1f, 1f, 0.0f);
                surface.mesh[pos].Color = (this.isHot ? this.hotColorEdge : this.coldColorEdge) * num3;
                surface.mesh[pos + 1].Color = surface.mesh[pos].Color;
                surface.mesh[pos + 3].Color = surface.mesh[pos].Color;
                pos += 6;
            }
        }
    }

    public override void Render() {
        Draw.Rect(this.X + this.fill.X, this.Y + this.fill.Y, this.fill.Width, this.fill.Height,
            this.isHot ? this.hotColor : this.coldColor);
        GameplayRenderer.End();
        foreach (Surface surface in this.Surfaces)
            surface.Render(this.SceneAs<Level>().Camera);
        GameplayRenderer.Begin();
    }

    public void OnCollide(Player player) {
        if (this.isHot && this.hotDangerous) {
            player.Die(Vector2.Zero);
        }
        if (!this.isHot && this.coldDangerous) {
            player.Die(Vector2.Zero);
        }
    }
    private void OnCoreMode(Session.CoreModes mode) {
        this.isHot = mode == Session.CoreModes.Hot;
        
        foreach(var surface in this.Surfaces) {
            int num1 = (int) (surface.Width / 4.0);
            
            for (int fillStartIndex = surface.fillStartIndex; fillStartIndex < surface.fillStartIndex + num1 * 6; ++fillStartIndex)
                surface.mesh[fillStartIndex].Color = mode == Session.CoreModes.Hot ? this.hotColor : this.coldColor;
            for (int surfaceStartIndex = surface.surfaceStartIndex; surfaceStartIndex < surface.surfaceStartIndex + num1 * 6; ++surfaceStartIndex)
                surface.mesh[surfaceStartIndex].Color = mode == Session.CoreModes.Hot ? this.hotColorEdge : this.coldColorEdge;
        }
    }
}