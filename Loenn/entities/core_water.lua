local xnaColors = require("consts.xna_colors")
local lightBlue = xnaColors.LightBlue

local water = {}

water.name = "KyfexHelper/CoreWater"
water.placements = {
    name = "water",
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
        coldTransparency=0.3,
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
water.fillColor = {lightBlue[1] * 0.3, lightBlue[2] * 0.3, lightBlue[3] * 0.3, 0.6}
water.borderColor = {lightBlue[1] * 0.8, lightBlue[2] * 0.8, lightBlue[3] * 0.8, 0.8}

water.depth = 0

return water