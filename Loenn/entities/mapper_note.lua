local utils = require("utils")
local drawableText = require("structs.drawable_text")
local drawableRectangle = require("structs.drawable_rectangle")
local drawableLine = require("structs.drawable_line")
local modHandler = require("mods")

local hiresFont = love.graphics.newImageFont(
    string.format("%s/Graphics/Atlases/%s/%s.png", modHandler.commonModContent, "Gameplay/loenn/KyfexHelper", "hires-font"), 
    [=[abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"'`-_/1234567890!?[](){}.,;:<>+=%#^*~ @$|\▽&]=], 1)
    
local logger = require("logging")
local state = require("loaded_state")
local selectionUtils = require("selections")

local keyData={
    lshift=false,
    rshift=false,
    lctrl=false,
    rctrl=false,
    capslock=false
}
local shiftLookups = {
    ["`"]="~",
    ["1"]="!",
    ["2"]="@",
    ["3"]="#",
    ["4"]="$",
    ["5"]="%",
    ["6"]="^",
    ["7"]="&",
    ["8"]="*",
    ["9"]="(",
    ["0"]=")",
    ["-"]="_",
    ["="]="+",
    ["["]="{",
    ["]"]="}",
    ["\\"]="|",
    [";"]=":",
    ["'"]="\"",
    [","]="<",
    ["."]=">",
    ["/"]="?"
}
local oldKeyPressed = love.keypressed
local oldKeyReleased = love.keyreleased
love.keypressed = function(key, scancode, isrepeat)
    if key == "lshift" then keyData.lshift=true end
    if key == "rshift" then keyData.lshift=true end
    if key == "lctrl" then keyData.lctrl=true end
    if key == "rctrl" then keyData.rctrl=true end
    if key == "capslock" then keyData.capslock=not keyData.capslock end
    
    if key == "delete" or (keyData.lctrl or keyData.rctrl) then return oldKeyPressed(key, scancode, isrepeat) end
    
    if state.selectionToolTargets ~= nil and
        #state.selectionToolTargets == 1 and 
        state.selectionToolTargets[1].item._name == "KyfexHelper/MapperNote" then
            
        if key == "return" then key = "\n" end
        if key == "backspace" then 
            state.selectionToolTargets[1].item.text = string.sub(state.selectionToolTargets[1].item.text,1,-2)
            selectionUtils.redrawTargetLayers(state.getSelectedRoom(), state.selectionToolTargets)
            return
        end
        if key == "space" then key = " " end
        if #key > 1 then key = "" end
        
        if string.match(key, "[a-z]") and (keyData.lshift or keyData.rshift or keyData.capslock) then
            key=string.upper(key)
        end
        if (keyData.lshift or keyData.rshift) and shiftLookups[key] ~= nil then
            key=shiftLookups[key]
        end
            
        logger.error("kyfex "..key)
        state.selectionToolTargets[1].item.text = state.selectionToolTargets[1].item.text..key
        selectionUtils.redrawTargetLayers(state.getSelectedRoom(), state.selectionToolTargets)
        return
    end
    
    return oldKeyPressed(key, scancode, isrepeat)
end
love.keyreleased = function(key, scancode, isrepeat)
    if key == "lshift" then keyData.lshift=false end
    if key == "rshift" then keyData.lshift=false end
    if key == "lctrl" then keyData.lctrl=false end
    if key == "rctrl" then keyData.rctrl=false end
    
    return oldKeyReleased(key, scancode, isrepeat)
end

return {
    name="KyfexHelper/MapperNote",
    associatedMods={},--this specifically is a loenn only "entity"
    placements={{
        name = "main",
        data = {
            width=8,
            height=8,
            text="Note",
            textColor="ffffff",
            rectangleColor="ffff99"
        },
    }},
    fieldInformation={
        textColor={
            fieldType="color",
            useAlpha=true,
        },
        rectangleColor={
            fieldType="color",
            useAlpha=true,
        },
    },
    fieldOrder={
        "x", "y", "width", "height",
        "rectangleColor", "textColor",
        "text"    
    },

    sprite = function(room, entity)
        local rectColor = utils.getColor(entity.rectangleColor)
        return {
            drawableRectangle.fromRectangle("fill", entity.x, entity.y+8, entity.width, entity.height-8, 
                {rectColor[1], rectColor[2], rectColor[3], (rectColor[4] or 1)*0.3}),
            drawableRectangle.fromRectangle("fill", entity.x+8, entity.y, entity.width-8, 8, 
                {rectColor[1], rectColor[2], rectColor[3], (rectColor[4] or 1)*0.3}),
            drawableLine.fromPoints({
                    entity.x+8,entity.y,
                    entity.x+entity.width,entity.y,
                    entity.x+entity.width,entity.y+entity.height,
                    entity.x,entity.y+entity.height,
                    entity.x,entity.y+8,
                    entity.x+8,entity.y,
                    entity.x+8,entity.y+8,
                    entity.x,entity.y+8}, 
                {rectColor[1]*0.65, rectColor[2]*0.65, rectColor[3]*0.65,(rectColor[4] or 1)*0.8}),
            drawableText.fromText(entity.text, 
                entity.x, entity.y, 
                entity.width, entity.height, 
                hiresFont, 0.5, 
                entity.textColor)
        }
    end
}
