local drawableRectangle = require("structs.drawable_rectangle")

local water = {}

water.name = "KyfexHelper/CoreWater"
water.placements = {
    name = "main",
    data = {
        hasBottom = false,
        width = 8,
        height = 8,
        
        hotDangerous=true,
        coldDangerous=false,
        hotColor="f25e29",
        coldColor="6cd6eb",
        hotColorEdge="ff8933",
        coldColorEdge="a6fff4",
        hotTransparency=0.3,
        coldTransparency=0.3
    }
}
water.fieldInformation = {
    hotColor = {
        fieldType="color",
        options={"f25e29"},
    },
    coldColor = {
        fieldType="color",
        options={"6cd6eb"},
    },
    hotColorEdge = {
        fieldType="color",
        options={"ff8933"},
    },
    coldColorEdge = {
        fieldType="color",
        options={"a6fff4"},
    },
}
water.fieldOrder = {
        "x","y",
        "width",
        "height",
        
        "hotColor",
        "coldColor",
        "hotColorEdge",
        "coldColorEdge",
        "hotTransparency",
        "coldTransparency",
        
        "hotDangerous",
        "coldDangerous",
        "hasBottom",
}
function water.sprite(room, entity)
    return {
        drawableRectangle.fromRectangle("fill", entity.x, entity.y, entity.width/2,entity.height, entity.hotColor),
        drawableRectangle.fromRectangle("fill", entity.x+entity.width/2, entity.y, entity.width/2,entity.height, entity.coldColor),
    }
end

water.depth = 0

return water