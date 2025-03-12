local entity = {
    name="KyfexHelper/SwitchGateIcon",
    depth = -9000,
    placements={
        {
            name = "main",
            data = {
                depth=-9000,
                flag="",
                icon="objects/switchgate/icon",
                inactive="5fcde4",
                active="ffffff",
                finish="f141df",
                
                attached=true,
                time=1.8,
                shake=0.5,
                shakeStrength=1.0,
                color="f141df",
                particles=true,
            },
        }
    },
    
    fieldInformation={
        inactive={fieldType="color",useAlpha=true},
        active={fieldType="color",useAlpha=true},
        finish={fieldType="color",useAlpha=true},
        color={fieldType="color",useAlpha=true},
        depth={fieldTyle="integer"},
    }
}

function entity.texture(room, entity)
    return entity.icon.."00"
end

return entity