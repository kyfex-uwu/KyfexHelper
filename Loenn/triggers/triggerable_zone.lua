return {
    name="KyfexHelper/TriggerableZone",
    associatedMods={"KyfexHelper", "CrystallineHelper"},
    nodeLimits={1,-1},
    placements={
        {
            name = "main",
            data = {
                oneUse=false,
                timeToWait=0,
                triggerKey="",
                collideCount=1,
            },
        }
    },
    fieldInformation = {
        collideCount = {
            type="integer",
            minimumValue=1,
        },
    },
    triggerText = function(room, self)
        return "accepts triggerKey: "..self.triggerKey
    end,
}