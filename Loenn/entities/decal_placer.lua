local rect = require("structs.drawable_rectangle")

return {
    name="KyfexHelper/DecalPlacer",
    placements={
        name="main",
        data={
            width=8,
            height=8,
            count=0,
            depth=9000,
            random=0,
            decals="",
        },
    },
    fieldInformation={
        count={ fieldType="integer" },
        depth={ 
            fieldType="integer",
            options={
                9000,
                -10500,    
            },
            editable=true,
        },
        random={ fieldType="integer" },
        
        decals={
            fieldType="list",
            elementDefault = ":0",
            elementSeparator=",",
        },
    },
    depth = function(room, entity) return entity.depth end,
    
    sprite = function(room, entity, viewport)
        return {
            rect.fromRectangle("bordered", entity.x, entity.y, entity.width, entity.height, "ffffff44", "ffffff88")    
        }
    end
}
