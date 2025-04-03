local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")
local drawableNinePatch = require("structs.drawable_nine_patch")
local drawableRectangle = require("structs.drawable_rectangle")
local utils = require("utils")

local zipMover = {
    name="KyfexHelper/KevinZipper",
    depth=-9000,
    nodeVisibility = "never",
    nodeLimits = {1, 1},
    warnBelowSize = {24, 24},
    placements={{
        name="",
        data={
            width=24,
            height=24,
            axes="both"
        },
    }},
    fieldInformation={
        axes = {
            options = {
                Both = "both",
                Vertical = "vertical",
                Horizontal = "horizontal"
            },
            editable = false
        }
    },
}
local axesToSprite={
    none = "objects/crushblock/block00",
    horizontal = "objects/crushblock/block01",
    vertical = "objects/crushblock/block02",
    both = "objects/crushblock/block03"
}

local ropeColor = {102 / 255, 57 / 255, 49 / 255}
local function addNodeSprites(sprites, entity, centerX, centerY, centerNodeX, centerNodeY)
    local nodeCogSprite = drawableSprite.fromTexture("objects/zipmover/cog", entity)

    nodeCogSprite:setPosition(centerNodeX, centerNodeY)
    nodeCogSprite:setJustification(0.5, 0.5)

    local points = {centerX, centerY, centerNodeX, centerNodeY}
    local leftLine = drawableLine.fromPoints(points, ropeColor, 1)
    local rightLine = drawableLine.fromPoints(points, ropeColor, 1)

    leftLine:setOffset(0, 4.5)
    rightLine:setOffset(0, -4.5)

    leftLine.depth = 5000
    rightLine.depth = 5000

    for _, sprite in ipairs(leftLine:getDrawableSprite()) do
        table.insert(sprites, sprite)
    end

    for _, sprite in ipairs(rightLine:getDrawableSprite()) do
        table.insert(sprites, sprite)
    end

    table.insert(sprites, nodeCogSprite)
end

local function addBlockSprites(sprites, entity, blockTexture, x, y, width, height)
    local rectangle = drawableRectangle.fromRectangle("fill", x + 2, y + 2, width - 4, height - 4, {98 / 255, 34 / 255, 43 / 255})

    local frameNinePatch = drawableNinePatch.fromTexture(blockTexture, {
            mode = "border",
            borderMode = "repeat"
        }, x, y, width, height)
    local frameSprites = frameNinePatch:getDrawableSprite()

    table.insert(sprites, rectangle:getDrawableSprite())
    
    local width, height = entity.width or 24, entity.height or 24
    local faceSprite = drawableSprite.fromTexture("objects/crushblock/idle_face", entity)
    faceSprite:addPosition(math.floor(width / 2), math.floor(height / 2))
    table.insert(sprites, faceSprite)

    for _, sprite in ipairs(frameSprites) do
        table.insert(sprites, sprite)
    end
end

function zipMover.sprite(room, entity)
    local sprites = {}

    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 16, entity.height or 16
    local halfWidth, halfHeight = math.floor(entity.width / 2), math.floor(entity.height / 2)

    local nodes = entity.nodes or {{x = 0, y = 0}}
    local nodeX, nodeY = nodes[1].x, nodes[1].y

    local centerX, centerY = x + halfWidth, y + halfHeight
    local centerNodeX, centerNodeY = nodeX + halfWidth, nodeY + halfHeight

    addNodeSprites(sprites, entity, centerX, centerY, centerNodeX, centerNodeY)
    addBlockSprites(sprites, entity, axesToSprite[entity.axes], x, y, width, height)

    return sprites
end

function zipMover.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 8, entity.height or 8

    local nodes = entity.nodes or {{x = 0, y = 0}}
    local nodeX, nodeY = nodes[1].x, nodes[1].y

    local mainRectangle = utils.rectangle(x, y, width, height)
    local nodeRectangle = utils.rectangle(nodeX+width/2 - 5, nodeY+height/2 - 5, 10, 10)

    return mainRectangle, {nodeRectangle}
end

return zipMover