local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")
local drawableNinePatch = require("structs.drawable_nine_patch")
local drawableRectangle = require("structs.drawable_rectangle")
local utils = require("utils")

local entity = {
    name="KyfexHelper/TemplateKevinTrigger",
    associatedMods={"KyfexHelper", "auspicioushelper"},
    depth=-9000,
    placements={{
        name="main",
        data={
            width=8,
            height=8,
            direction="left"
        },
    }},
    fieldInformation={
        direction = {
            options = {
                "left",
                "right",
                "up",
                "down"
            },
            editable = false
        }
    },
}
local axesToSprite={
    both = "objects/crushblock/block03"
}


-- local function addBlockSprites(sprites, entity, blockTexture, x, y, width, height)
--     local rectangle = drawableRectangle.fromRectangle("fill", x + 2, y + 2, width - 4, height - 4, {98 / 255, 34 / 255, 43 / 255})
-- 
--     local frameNinePatch = drawableNinePatch.fromTexture(blockTexture, {
--             mode = "border",
--             borderMode = "repeat"
--         }, x, y, width, height)
--     local frameSprites = frameNinePatch:getDrawableSprite()
-- 
--     table.insert(sprites, rectangle:getDrawableSprite())
--     
--     local width, height = entity.width or 24, entity.height or 24
--     local faceSprite = drawableSprite.fromTexture("objects/crushblock/idle_face", entity)
--     faceSprite:addPosition(math.floor(width / 2), math.floor(height / 2))
--     table.insert(sprites, faceSprite)
-- 
--     for _, sprite in ipairs(frameSprites) do
--         table.insert(sprites, sprite)
--     end
-- end

function entity.resize(room, entity, offsetX, offsetY, directionX, directionY)
    if offsetX ~= 0 then
        entity.width = entity.width + offsetX
        if directionX == -1 then entity.x -= offsetX end
        entity.height=8
    elseif offsetY ~= 0 then
        entity.height = entity.height + offsetY
        if directionY == -1 then entity.y -= offsetY end
        entity.width=8
    end

    if entity.width < 8 then entity.width = 8 return false end
    if entity.height < 8 then entity.height = 8 return false end

    return true
end

function entity.sprite(room, entity)
    local sprites = {}
    
    table.insert(sprites, drawableRectangle.fromRectangle("fill", entity.x,entity.y,entity.width,entity.height))

    return sprites
end


return entity