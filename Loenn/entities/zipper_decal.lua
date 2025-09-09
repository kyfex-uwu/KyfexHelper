local rect = require("structs.rectangle")

return {
    name="KyfexHelper/ZipperTrafficLight",
    associatedMods={"KyfexHelper", "auspicioushelper"},
    depth=-10001,
    placements={
        name="main",
        data={
            decal1="KyfexHelper/ZipperTrafficLight/redlight",
            decal2="KyfexHelper/ZipperTrafficLight/yellowlight",
            decal3="KyfexHelper/ZipperTrafficLight/greenlight",
        },
    },
    fieldInformation={
        decal1={
            options={"KyfexHelper/ZipperTrafficLight/redlight"},
            editable=true,
        },
        decal2={
            options={"KyfexHelper/ZipperTrafficLight/yellowlight"},
            editable=true,
        },
        decal3={
            options={"KyfexHelper/ZipperTrafficLight/greenlight"},
            editable=true,
        },    
    },
    rectangle=function(room, entity) return rect.create(-4+entity.x,-4+entity.y,8,8) end,
}