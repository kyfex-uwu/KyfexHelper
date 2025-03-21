using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using On.Monocle;

namespace Celeste.Mod.KyfexHelper;

public abstract class TransparentDreamBlock{
    private static void setForDream(Draw.orig_Rect_float_float_float_float_Color method,
        float x, float y, float w, float h, Color color) {
        
        if (color == DreamBlock.activeBackColor) color = new Color(DreamBlock.activeBackColor, 0.5f);
        if (color == DreamBlock.disabledBackColor) color = new Color(DreamBlock.disabledBackColor, 0.5f);
        
        method(x, y, w, h, color);
    }
    public static void Load() {
        On.Monocle.Draw.Rect_float_float_float_float_Color += setForDream;
    }

    public static void Unload() {
        On.Monocle.Draw.Rect_float_float_float_float_Color -= setForDream;
    }
}