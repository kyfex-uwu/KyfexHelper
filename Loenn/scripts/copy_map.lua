local state = require("loaded_state")
local snapshot = require("structs.snapshot")
local utils = require("utils")
local mapcoder = require("mapcoder")
local mapStruct = require("structs.map")
local mapItemUtils = require("map_item_utils")
local celesteRender = require("celeste_render")

local script = {
    name = "copyWholeMap",
    displayName = "Copy Whole Map",
    parameters = {
        from = "",
        prefix = ""
    },
    fieldOrder = { "from", "prefix" },
    fieldInformation = {
        from = {
            fieldType = "loennScripts.directFilepath",
            extension = "bin"
        }
    },
    tooltip = "Copies a whole map into this one",
    tooltips = {
        from = "The file to copy",
        prefix = "The prefix to put on all the rooms",
    },
}

function script.prerun(args, mode, ctx)
    local targetMap = mapStruct.decode(mapcoder.decodeFile(args.from))
    local xOffs = math.round(ctx.mouseMapX / 8) * 8
    local yOffs = math.round(ctx.mouseMapY / 8) * 8

    local function forward(data)
        local map = state.map

        for i, room in ipairs(targetMap.rooms) do
            newRoom = utils.deepcopy(room)
            newRoom.name = args.prefix..room.name
            newRoom.x = room.x+xOffs
            newRoom.y = room.y+yOffs

            mapItemUtils.addItem(map, newRoom)
        end
    end

    local prefixLen = args.prefix:len()
    local function backward(data)
        local map = state.map

        for i, room in ipairs(state.map.rooms) do
            if room.name:sub(0, prefixLen) == args.prefix then
                mapItemUtils.deleteRoom(map, room)
            end
        end

        mapItemUtils.deleteRoom(map, newRoom)
    end

    forward()

    return snapshot.create(script.name, {}, backward, forward)
end

return script
