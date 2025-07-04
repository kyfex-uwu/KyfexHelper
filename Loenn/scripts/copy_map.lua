local state = require("loaded_state")
local snapshot = require("structs.snapshot")
local utils = require("utils")
local mapcoder = require("mapcoder")
local mapStruct = require("structs.map")
local mapItemUtils = require("map_item_utils")
local celesteRender = require("celeste_render")

local script = {
    name = "kyfex_copyWholeMap",
    displayName = "Copy Whole Map",
    parameters = {
        from = "",
        prefix = "",
    },
    fieldOrder = { "from", "prefix" },
    fieldInformation = {
        from = {
            fieldType = "loennScripts.directFilepath",
            extension = "bin",
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

    local rooms = {}
    for _, room in ipairs(targetMap.rooms) do
        newRoom = utils.deepcopy(room)
        newRoom.name = args.prefix..room.name
        newRoom.x = room.x+xOffs
        newRoom.y = room.y+yOffs

        table.insert(rooms, newRoom)
    end

    local function forward(data)
        for _, room in ipairs(rooms) do
            mapItemUtils.addRoom(state.map, room)
        end
    end

    local function backward(data)
        for _, room in ipairs(rooms) do
            mapItemUtils.deleteRoom(state.map, room)
        end
    end

    forward()

    return snapshot.create(script.name, {}, backward, forward)
end

return script
