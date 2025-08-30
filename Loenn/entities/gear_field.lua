local entity = {
    name="KyfexHelper/GearField",
    associatedMods={"KyfexHelper", "auspicioushelper"},
    depth=-10000,
    placements = {
        name = "main",
        data = {
            width = 8,
            height = 8,
            gearPath="objects/zipmover/innercog",
            bgColor="000000"
        }
    },
    fieldInformation={
        gearPath={
            options={"objects/zipmover/innercog"},
            editable=true,
        },
        bgColor={
            fieldType="color",
        }
    }
}

return entity