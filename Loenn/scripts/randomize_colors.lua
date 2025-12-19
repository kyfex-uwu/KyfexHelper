local logging = require("logging")
local utils = require("utils.utils")

function recolor(decal, data)
    if not data.easierDecalNames[string.sub(decal.texture, #"decals/"+1)] then return end
    decal.color = data.easierDecalColors[math.random(1,#data.easierDecalColors)]
end
function recolorEntity(entity, data)
    if not data.easierEntityNames[entity._name] then return end
    logging.log(data.targetColor[1])
    logging.log(data.targetColor[2])
    logging.log(data.targetColor[3])
    logging.log(data.targetColor[4])
    entity[data.entityColorField] = utils[(data.targetColor[4] == nil) and "rgbToHex" or "rgbaToHex"](utils.hsvToRgb(
        math.fmod(data.targetColor[1]*360 + (math.random()-0.5)*data.hueDifference, 360),
        math.max(0,math.min(1,data.targetColor[2] + (math.random()-0.5)*data.satDifference)),
        math.max(0,math.min(1,data.targetColor[3] + (math.random()-0.5)*data.valDifference))
    ), data.targetColor[4])
end

return {
  name="kyfex_randomize_colors",
  displayName="Randomize Entity Colors",
  tooltip="Randomizes the colors of specific entities",
  parameters = {
    entityNames = "entity",
    entityColorField = "color",
    
    targetColor = "ffffff",
    hueDifference=0,
    saturationDifference=0,
    valueDifference=0,
  },
  tooltips={
      hueDifference = "Hue is from 0-359",
      saturationDifference = "Saturation is from 0-99",
      valueDifference = "Hue is from 0-99",
  },
  fieldInformation = {
    entityNames = {
      fieldType = "list",
      elementOptions = {
        fieldType = "string",
      },
    },
    targetColor={
      fieldType = "color",
    }
  },
  fieldOrder={
    "entityNames",
    "entityColorField",
    
    "targetColor",
    "hueDifference",
    "saturationDifference",
    "valueDifference",
  },
  prerun = function(args, mode, ctx)
    ctx.easierEntityNames = {}
    for name in string.gmatch(args.entityNames, "([^,]+)") do
      ctx.easierEntityNames[name] = true
    end

    local parsed, r, g, b, a = utils.parseHexColor(args.targetColor)
    if parsed then
        ctx.targetColor = {utils.rgbToHsv(r, g, b), a}
    end
    ctx.hueDifference=args.hueDifference
    ctx.satDifference=args.saturationDifference
    ctx.valDifference=args.valueDifference
    ctx.entityColorField = args.entityColorField
  end,
  run = function(room, args, ctx)
--     if ctx.targetColor == nil then error("Invalid target color") end
      
--     for _,decal in ipairs(room.decalsFg) do
--       recolor(decal, ctx)
--     end
--     for _,decal in ipairs(room.decalsBg) do
--       recolor(decal, ctx)
--     end

    for _,entity in ipairs(room.entities) do
      recolorEntity(entity, ctx)
    end
  end,
}
