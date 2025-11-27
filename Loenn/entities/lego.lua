local drawableNinePatch = require("structs.drawable_nine_patch")

return {
    name="KyfexHelper/Lego",
--     depth=-10001,
    placements={
        name="main",
        data={
            width=8,
            height=8,
            safe=true,
        },
    },
    sprite = function(room, entity)
        return drawableNinePatch.fromTexture("objects/KyfexHelper/lego/lego", {
                mode = "fill",
                borderMode = "repeat",
                fillMode = "repeat",
            }, entity.x, entity.y, entity.width, entity.height):getDrawableSprite()
    end,
}