return {
    name="KyfexHelper/AubreyHelperPosSettingTrigger",
    associatedMods={"KyfexHelper", "AubreyHelper"},
    placements={
        {
            name = "main",
            data = {
                posSwap="false"
            },
        }
    },
    fieldInformation={
        posSwap={
            options={["Force Swap"]="true", ["Force Don't Swap"]="false", ["Remove Override"]= "null"},
            editable=false,    
        }
    },
}