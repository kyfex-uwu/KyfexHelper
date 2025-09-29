local rect = require("structs.drawable_rectangle")

return {
    name="KyfexHelper/DecalPlacer",
    associatedMods=function(entity)
        if entity.density ~= "" then
            local density
            pcall(function() 
                density = tonumber(entity.density)    
            end)
            if density ~= nil and density >=0 and density <= 10 then
                local size = entity.width * entity.height
                entity.count = math.floor(size * density)
                entity.density=""
            end
        end
        
        return {"KyfexHelper"}    
    end,
    placements={
        {
            name="main",
            data={
                width=8,
                height=8,
                count=0,
                depth=9000,
                random=0,
                decals="",
                density="",
            },
        },{
            name="fg",
            data={
                width=8,
                height=8,
                count=0,
                depth=-10500,
                random=0,
                decals="",
                density="",
            },
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
        density={
            default="",
        },
    },
    fieldOrder={
        "x", "y", "width", "height",
        "count", "density",
        "decals", "depth",
        "random"
    },
    depth = function(room, entity) return entity.depth end,
    
    sprite = function(room, entity, viewport)
        return {
            rect.fromRectangle("bordered", entity.x, entity.y, entity.width, entity.height, "ffffff44", "ffffff88")    
        }
    end
}
