using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace SFactions {
    public class PvPManager {
        public static void OnNetGetData(GetDataEventArgs args) {
            if (args.MsgID == PacketTypes.PlayerTeam) {
                args.Handled = true;
                ChangeTeam(TShock.Players[args.Msg.whoAmI]);
            }
        }

        public static void ChangeTeam(TSPlayer player) {
            player.SetTeam(SFactionsMain.db.players[player.Name]);
        }
    }
}
