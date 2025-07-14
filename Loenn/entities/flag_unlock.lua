local drawableLine = require("structs.drawable_line")
local rectangle = require("structs.rectangle")

local flag_unlock = {
    name="KyfexHelper/FlagUnlock",
    placementType="point",
    
    placements={
        {
            name = "main",
            data = {
                unlockSfx="event:/game/03_resort/key_unlock",
                unlockables="Celeste.Key",
                requireUnobstructed=true,
                flag="flag",
                temporary=false,
                shake=true,
                radius=60,
                sprite="lockdoor_wood",
                time=1,
            },
        }
    },
    associatedMods = function(entity)
        if entity.unlockSfx == "kyfexuwu_hider" and entity.hider == nil then
            entity.hider = ""
        end
        return {"KyfexHelper"}
    end,
    fieldInformation = {
        unlockSfx = {
            options = {"event:/game/03_resort/key_unlock"},
        },
        unlockables = {
            fieldType="list",
            elementDefault = "Celeste.Key",
        },
        sprite = {
            options = {
                "lockdoor_wood",
                "lockdoor_temple_a",
                "lockdoor_temple_b",
                "lockdoor_moon",
            }
        }
    }
}

local ids={}
local killswitch = true
if killswitch then
    local entities = require("entities")
    local oldGED = entities.getEntityDrawable
    local oldGND = entities.getNodeDrawable
    local oldGS = entities.getSelection
    entities.getEntityDrawable = function(name, handler, room, entity, viewport)
        if ids[tostring(entity._id)] then 
            return {} 
        end
        return oldGED(name, handler, room, entity, viewport)
    end
    entities.getNodeDrawable = function(name, handler, room, entity, node, nodeIndex, viewport)
        if ids[tostring(entity._id)] then 
            return {} 
        end
        return oldGND(name, handler, room, entity, node, nodeIndex, viewport)
    end
    entities.getSelection = function(room, entity, viewport, handlerOverride)
        if ids[tostring(entity._id)] then 
            return nil, {} 
        end
        return oldGS(room, entity, viewport, handlerOverride)
    end
end

function flag_unlock.selection(room, entity)
    if killswitch and entity.hider ~= nil then return nil end
    return rectangle.create(entity.x-8,entity.y-8,16,16)
end

function flag_unlock.sprite(room, entity)
    local points={}
    
    for i=0,360,10 do
        table.insert(points,math.cos(i/180*math.pi)*entity.radius+entity.x)
        table.insert(points,math.sin(i/180*math.pi)*entity.radius+entity.y)
    end

    if entity.hider ~= nil then 
        ids[entity.hider] = true
        return {}
    end

    return {
        drawableLine.fromPoints(points, {1,1,1}, 1),
    }
end

return flag_unlock