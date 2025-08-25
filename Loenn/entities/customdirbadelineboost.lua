local badelineBoost = {}

badelineBoost.name = "KyfexHelper/CustomDirectionBadelineBoost"
badelineBoost.depth = -1000000
badelineBoost.nodeLineRenderType = "line"
badelineBoost.texture = "objects/badelineboost/idle00"
badelineBoost.nodeLimits = {0, -1}
badelineBoost.placements = {
    name = "main",
    data = {
        lockCamera = true,
        canSkip = false,
        finalCh9Boost = false,
        finalCh9GoldenBoost = false,
        finalCh9Dialog = false,
        
        direction = "1.570796327",
    }
}
local pi = 3.1415926536
badelineBoost.fieldInformation = {
    direction = {
        valueTransformer = function(val)
            return tostring((tonumber(val) or 0)/360*2*pi)
        end,
        displayTransformer = function(val)
            return tostring(math.floor((tonumber(val) or 0)/(2*pi)*360*1000)/1000)
        end,
    }    
}

return badelineBoost