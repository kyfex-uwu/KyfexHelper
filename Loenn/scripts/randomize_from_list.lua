local logging = require("logging")

function recolor(decal, data)
    if not data.easierDecalNames[string.sub(decal.texture, #"decals/"+1)] then return end
    decal.color = data.easierColors[math.random(1,#data.easierColors)]
end
function recolorEntity(entity, data)
    if not data.easierEntityNames[entity._name] then return end
    entity[data.entityColorField] = data.easierColors[math.random(1,#data.easierColors)]
end

return {
  name="kyfex_randomize_colors_from_list",
  displayName="Randomize Decal/Entity Colors From List",
  tooltip="Randomizes the colors of specific decals/entities",
  parameters = {
    entityNames = "entity",
    decalNames = "decal",
    colors = "ffffff",
    entityColorField = "color",
  },
  fieldInformation = {
    entityNames = {
      fieldType = "list",
      elementOptions = {
        fieldType = "string",
      },
    },
    decalNames = {
      fieldType = "list",
      elementOptions = {
        fieldType = "string",
      },
    },
    colors = {
      fieldType = "list",
      elementOptions = {
        fieldType = "color",
      },
    },
  },
  prerun = function(args, mode, ctx)
    ctx.easierDecalNames = {}
    for name in string.gmatch(args.decalNames, "([^,]+)") do
      ctx.easierDecalNames[name] = true
    end
    ctx.easierEntityNames = {}
    for name in string.gmatch(args.entityNames, "([^,]+)") do
      ctx.easierEntityNames[name] = true
    end

    ctx.easierColors = {}
    for color in string.gmatch(args.colors, "([^,]+)") do
        ctx.easierColors[#ctx.easierColors+1] = color
    end
    
    ctx.entityColorField=args.entityColorField
  end,
  run = function(room, args, ctx)
    for _,decal in ipairs(room.decalsFg) do
      recolor(decal, ctx)
    end
    for _,decal in ipairs(room.decalsBg) do
      recolor(decal, ctx)
    end

    for _,entity in ipairs(room.entities) do
      recolorEntity(entity, ctx)
    end
    for _,trigger in ipairs(room.triggers) do
      recolorEntity(trigger, ctx)
    end
  end,
}
