local logging = require("logging")

function recolor(decal, data)
    if not data.easierDecalNames[string.sub(decal.texture, #"decals/"+1)] then return end
    decal.color = data.easierDecalColors[math.random(1,#data.easierDecalColors)]
end

return {
  name="kyfex_randomize_decal_colors",
  displayName="Randomize Decal Colors",
  tooltip="Randomizes the colors of specific decals",
  parameters = {
    decalNames = "placeholder",
    decalColors = "ffffff",
  },
  fieldInformation = {
    decalNames = {
      fieldType = "list",
      elementOptions = {
        fieldType = "string",
      },
    },
    decalColors = {
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

    ctx.easierDecalColors = {}
    for color in string.gmatch(args.decalColors, "([^,]+)") do
        ctx.easierDecalColors[#ctx.easierDecalColors+1] = color
    end
  end,
  run = function(room, args, ctx)
    for _,decal in ipairs(room.decalsFg) do
      recolor(decal, ctx)
    end
    for _,decal in ipairs(room.decalsBg) do
      recolor(decal, ctx)
    end
  end,
}
