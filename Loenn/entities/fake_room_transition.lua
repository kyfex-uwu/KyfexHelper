local drawableLine = require("structs.drawable_line")
local rectangle = require("structs.rectangle")

local entity = {
    name = "KyfexHelper/FakeRoomTransition",
    placements = {{
        
        name = "main",
        data = {
            canGoBack = true,
            size = 8,
            isHorizontal = false,
        },
    }},
    canResize = {true, true},
    ignoredFields = {
        "size", "isHorizontal",
    },
}
entity.resize = function(room, entity, offsetX, offsetY, directionX, directionY)
    if math.abs(offsetX) > math.abs(offsetY) ~= entity.isHorizontal then
        entity.isHorizontal = math.abs(offsetX) > math.abs(offsetY)
        entity.size = math.abs(entity.isHorizontal and offsetX or offsetY)
        if directionX == -1 or directionY == -1 then--todo
            if entity.isHorizontal then entity.x -= entity.size 
            else entity.y-= entity.size end
        end
    
        return true
    end

    local newSize = entity.size + (entity.isHorizontal and offsetX or offsetY) 
    if math.abs(newSize) < 8 then return false end
    if newSize < 0 then 
        entity.size = -newSize
        if entity.isHorizontal then entity.x -= entity.size
        else entity.y-= entity.size end
    else
        entity.size = newSize
    end
    if (directionX == 0 and directionY or directionX) == -1 then
        if entity.isHorizontal then entity.x -= offsetX
        else entity.y -= offsetY end
    end

    return true
end
entity.sprite = function(room, entity)
    return drawableLine.fromPoints({entity.x, entity.y, 
        entity.x+(entity.isHorizontal and entity.size or 0), entity.y+(entity.isHorizontal and 0 or entity.size)}, {255,255,255})
end
entity.selection = function(room, entity)
    if entity.isHorizontal then return rectangle.create(entity.x, entity.y-4, entity.size, 8) end
    return rectangle.create(entity.x-4, entity.y, 8, entity.size)
end

return entity