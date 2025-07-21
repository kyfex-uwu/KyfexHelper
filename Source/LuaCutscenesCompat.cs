using System.Net.Http;

namespace Celeste.Mod.KyfexHelper;

public class LuaCutscenesCompat {
    public static void Load() {
        
    }

    public static void HTTPRequest(string path, NLua.LuaFunction onFetch) {
        new HttpClient().GetAsync(path).ContinueWith((task) => {
            task.Result.Content.ReadAsStringAsync().ContinueWith((body) => {
                onFetch.Call(body.Result);
            });
        });
    }
}