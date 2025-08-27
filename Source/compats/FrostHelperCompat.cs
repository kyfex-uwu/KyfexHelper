using FrostHelper.Entities.Boosters;
using Monocle;

namespace Celeste.Mod.KyfexHelper;

public class FrostHelperCompat {
    public static void Load() {
        BubbleRedirector.checkFrostHelperBoosters = true;
    }

    public static Entity PlayerIsInBubble(Player player) {;
        var toReturn = GenericCustomBooster.GetBoosterThatIsBoostingPlayer(player);
        return (toReturn != null && toReturn.BoostingPlayer)?toReturn:null;
    }
}