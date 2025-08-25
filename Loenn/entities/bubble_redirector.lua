local rectangle = require("structs.rectangle")
local drawableSprite = require("structs.drawable_sprite")

local entity = {
    name="KyfexHelper/BubbleRedirector",
    placementType="point",
    
    placements={
        {
            name = "green",
            data = {
                direction="any",
                color="green"
            },
        },
        {
            name = "red",
            data = {
                direction="any",
                color="red"
            },
        },
        {
            name = "custom",
            data = {
                direction="any",
                color="custom",
                tint="ffffff"
            },
        },
    },
    fieldInformation={
        direction={
            options = {
                "up",
                "down",
                "left",
                "right",
                "any"    
            },
            editable=false,
        },
        color={
            options = {
                "red",
                "green",
                "custom"    
            }    
        },
        tint={
            fieldType="color",
        }
    }
}

function entity.sprite(room, entity)
    return {
        drawableSprite.fromTexture("objects/KyfexHelper/bubble_redirector/"..entity.color.."_00", {
            x=entity.x,
            y=entity.y,
            color=entity.tint,
        }),
        drawableSprite.fromTexture("objects/KyfexHelper/bubble_redirector/"..entity.color.."_arrow"..entity.direction.."_00", {
            x=entity.x,
            y=entity.y,
            color=entity.tint, 
        }),
    }
end

return entity