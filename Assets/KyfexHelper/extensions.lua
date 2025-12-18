local cSharpClass = require("#Celeste.Mod.KyfexHelper.LuaCutscenesCompat")

local extensions = {}

function extensions.httpGet(path, onGet)
    cSharpClass.HTTPRequest(path, onGet)
end
function extensions.parseJson(jsonString)
    return cSharpClass.parseJson(jsonString)
end
function extensions.getField(getOn, name, bindingFlags)
    return cSharpClass.getField(getOn, name, bindingFlags)
end
function extensions.getMethod(getOn, name, bindingFlags, parameters)
    return cSharpClass.getMethod(getOn, name, bindingFlags, parameters)
end

return extensions