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
}

function flag_unlock.selection(room, entity)
    return rectangle.create(entity.x-8,entity.y-8,16,16)
end

function flag_unlock.sprite(room, entity)
    local points={}
    for i=0,360,10 do
        table.insert(points,math.cos(i/180*math.pi)*entity.radius+entity.x)
        table.insert(points,math.sin(i/180*math.pi)*entity.radius+entity.y)
    end
    return {
        drawableLine.fromPoints(points, {1,1,1}, 1),
        
    }
end

return flag_unlock