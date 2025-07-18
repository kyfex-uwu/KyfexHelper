local swapBlock = require("entities.swap_block")

local newPlacements = {}
local inputs = {"jump", "dashOrDemo", "grab"}
for _,placement in ipairs(swapBlock.placements) do
    for _,input in ipairs(inputs) do
        table.insert(newPlacements, {
            name = placement.name .. input,
            data = {
                width = placement.data.width,
                height = placement.data.height,   
                theme = placement.data.theme,    
                input = input,
            }
        })
    end
end

local newFieldInformation = {}
for k,v in pairs(swapBlock.fieldInformation) do
    newFieldInformation[k]=v
end
newFieldInformation.input = {
    options = {"dashOrDemo", "jump", "grab", "dash", "demo"},
    editable = false
}

return {
    name="KyfexHelper/InputSwapBlock",
    nodeLimits = swapBlock.nodeLimits,
    fieldInformation = newFieldInformation,
    placements = newPlacements,
    warnBelowSize = swapBlock.warnBelowSize,
    sprite = swapBlock.sprite,
    nodeSprite = swapBlock.nodeSprite,
    selection = swapBlock.selection
}