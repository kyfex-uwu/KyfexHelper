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
                flag=""
            },
        }
    },
    texture="loenn/kyfexuwu/clone",
    fieldOrder={
        "x","y",
        "xOffset", "yOffset",
        "flag"
    },
    fieldInformation={
        flag={default=""}
    }
}

return renderer