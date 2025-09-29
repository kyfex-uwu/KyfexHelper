local rect = require("structs.drawable_rectangle")
local drawableText = require("structs.drawable_text")

return {
    name = "KyfexHelper/ZoneTriggerEntity",
    associatedMods={"KyfexHelper", "CrystallineHelper"},
    placements = {
        name = "main",
        data = {
            width = 8,
            height = 8,
            triggerKey="",
        }
    },
    sprite = function(room, entity)
        return {
            rect.fromRectangle("bordered", entity.x, entity.y, entity.width,entity.height, "ffffff44", "ffffff88"),
            drawableText.fromText("triggerKey = "..entity.triggerKey, entity.x, entity.y+3, entity.width, entity.height, nil, nil, "ffffff"),
        }
    end
}