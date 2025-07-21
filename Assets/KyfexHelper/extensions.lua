local cSharpClass = require("#Celeste.Mod.KyfexHelper.LuaCutscenesCompat")

local extensions = {}

function extensions.httpGet(path, onGet)
    cSharpClass.HTTPRequest(path, onGet)
end

return extensions