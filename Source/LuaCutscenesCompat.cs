using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using NLua;

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

    public static object parseJson(string str) {
        return transform(JsonValue.Parse(str));
    }

    private static object transform(JsonNode node) {
        Logger.Info("KyfexHelper", node.ToString());
        switch (node.GetValueKind()) {
            case JsonValueKind.Array: {
                var toReturn = new object[(node as JsonArray).Count];
                for (int i = 0; i < toReturn.Length; i++) {
                    toReturn[i] = transform(node[i]);
                }

                return toReturn;
            }
            case JsonValueKind.Object: {
                var toReturn = new Dictionary<string, object>();
                foreach (var (key, val) in node as JsonObject) {
                    toReturn[key] = transform(val);
                }
                
                return toReturn;
            }
            case JsonValueKind.False: return false;
            case JsonValueKind.True: return true;
            case JsonValueKind.Null: case JsonValueKind.Undefined: return null;
            case JsonValueKind.Number: return node.AsValue().TryGetValue(out double ret) ? ret : null;
            case JsonValueKind.String: return node.AsValue().TryGetValue(out string ret2) ? ret2 : null;
        }

        return null;
    }
}