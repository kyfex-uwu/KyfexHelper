local enums = require("consts.celeste_enums")

local badelineBoss = {}

local hitOptions = {
    "damage",
    "kill",
    "bounce_away",
    "none"
}

badelineBoss.name = "KyfexHelper/CustomCollidableBadeline"
badelineBoss.depth = 0
badelineBoss.nodeLineRenderType = "line"
badelineBoss.texture = "characters/badelineBoss/charge00"
badelineBoss.nodeLimits = {0, -1}
badelineBoss.fieldInformation = {
    patternIndex = {
        fieldType = "integer",
        options = enums.badeline_boss_shooting_patterns,
        editable = false
    },
    playerHitType={
        options = hitOptions,
        editable = false,
    },
    holdableHitType={
        options = hitOptions,
        editable = false,
    },
    customSprite={
        options={"badeline_boss"},
        editable=true,
    },
    flagList = {
        fieldType = "list",
        elementDefault = "",
    }
}
badelineBoss.placements = {
    name = "boss",
    data = {
        patternIndex = 1,
        startHit = false,
        cameraPastY = 120.0,
        cameraLockY = true,
        canChangeMusic = true,
        
        playerHitType=hitOptions[1],
        holdableHitType=hitOptions[1],
        customSprite="badeline_boss",
        flagList="",
        bounceStrength=1,
    }
}

return badelineBoss