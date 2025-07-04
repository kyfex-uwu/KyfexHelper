local state = require("loaded_state")
local snapshot = require("structs.snapshot")
local utils = require("utils")
local mapcoder = require("mapcoder")
local mapStruct = require("structs.map")
local mapItemUtils = require("map_item_utils")
local celesteRender = require("celeste_render")
local logging = require("logging")

local script = {
    name = "kyfex_copyStylegrounds",
    displayName = "Copy Stylegrounds From Map",
    parameters = {
        from = "",
        group = "",
        tagsToAdd = "",
        onlyPrefix = "",

        offsetX = 0,
        offsetY = 0,
    },
    fieldInformation = {
        from = {
            fieldType = "loennScripts.directFilepath",
            extension = "bin",
        },
        tagsToAdd = {
            fieldType = "list"
        },
    },
    tooltip = "Copies all of a map's stylegrounds into this one",
    tooltips = {
        from = "The file to copy",
        group = "The name of the group to put all these stylegrounds in (leave this blank, and they won't go in a group)",
        tagsToAdd = "A comma-separated list of tags to add to the stylegrounds",
        onlyPrefix = "The prefix to add to all entries in the \"Only\" list",
        offsetX = "The destination map X minus the source map X",
        offsetY = "The destination map Y minus the source map Y",
    },
}

function process(styleground, args)
    if args.tagsToAdd ~= "" then
        styleground.tag = styleground.tag or ""
        if styleground.tag ~= "" then
            styleground.tag = styleground.tag .. ","
        end
        styleground.tag = styleground.tag .. args.tagsToAdd
    end

    if styleground._type == "parallax" then
        styleground.x = (styleground.x or 0) + args.offsetX * 8 * (styleground.scrollX or 0)
        styleground.y = (styleground.y or 0) + args.offsetY * 8 * (styleground.scrollY or 0)
    end

    if styleground.only ~= nil then
        local newOnly = ""
        for part in string.gmatch(styleground.only, "([^,]+)") do
            if newOnly ~= "" then newOnly = newOnly .. "," end
            newOnly = args.onlyPrefix .. part
        end
        styleground.only = newOnly
    end
end

function script.prerun(args, mode, ctx)
    local targetMap = mapStruct.decode(mapcoder.decodeFile(args.from))
    local stylesFg = utils.deepcopy(targetMap.stylesFg) or {}
    local stylesBg = utils.deepcopy(targetMap.stylesBg) or {}

    for _, styleground in ipairs(stylesFg) do
        process(styleground, args)
    end
    for _, styleground in ipairs(stylesBg) do
        process(styleground, args)
    end

    local function forward(data)
        local fg = args.group == "" and state.map.stylesFg or {}
        local bg = args.group == "" and state.map.stylesBg or {}

        for _, style in ipairs(stylesFg) do
            table.insert(fg, style)
        end

        for _, style in ipairs(stylesBg) do
            table.insert(bg, style)
        end

        if args.group ~= "" then
            table.insert(state.map.stylesFg, {
                color = "ffffff",
                _name = args.group,
                _type = "apply",
                _collapsed = false,
                children = fg,
            })
            table.insert(state.map.stylesBg, {
                color = "ffffff",
                _name = args.group,
                _type = "apply",
                _collapsed = false,
                children = bg,
            })
        end
    end

    local function backward(data)
        if args.group ~= "" then
            for i, style in ipairs(state.map.stylesFg) do
                if style._type == "apply" and style._name == args.group then
                    table.remove(state.map.stylesFg, i)
                    break
                end
            end

            for i, style in ipairs(state.map.stylesBg) do
                if style._type == "apply" and style._name == args.group then
                    table.remove(state.map.stylesBg, i)
                    break
                end
            end

            return
        end

        for _, remove in ipairs(stylesFg) do
            for i, style in ipairs(state.map.stylesFg) do
                if style == remove then
                    table.remove(state.map.stylesFg, i)
                end
            end
        end

        for _, remove in ipairs(stylesBg) do
            for i, style in ipairs(state.map.stylesBg) do
                if style == remove then
                    table.remove(state.map.stylesBg, i)
                end
            end
        end
    end

    forward()

    return snapshot.create(script.name, {}, backward, forward)
end

return script
