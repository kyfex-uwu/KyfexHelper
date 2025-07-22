local cSharpClass = require("#Celeste.Mod.KyfexHelper.LuaCutscenesCompat")

local extensions = {}

function extensions.httpGet(path, onGet)
    cSharpClass.HTTPRequest(path, onGet)
end
function extensions.parseJson(jsonString)
    cSharpClass.parseJson(jsonString)
end

return extensions