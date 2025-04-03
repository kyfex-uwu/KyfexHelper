local rectangle = require("structs.rectangle")

local renderer = {
    name="KyfexHelper/PlayerCloneRenderer",
    placementType="point",
    
    placements={
        {
            name = "main",
            data = {
                xOffset=0,
                yOffset=0,
            },
        }
    },
    texture="loenn/kyfexuwu/clone",
}

return renderer