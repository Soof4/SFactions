using System.Reflection;
using Org.BouncyCastle.Asn1.X509.Qualified;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Configuration;
using TShockAPI.Hooks;

namespace SFactions
{
    public static class ChatManager
    {
        public static void OnPlayerChat(PlayerChatEventArgs args)
        {
            // Format the message
            string message = string.Format(SFactions.Config.ChatFormat,
                                           "",                                               // {0}
                                           args.Player.Group.Prefix,                         // {1}
                                           args.Player.Name,                                 // {2}
                                           args.Player.Group.Suffix,                         // {3}
                                           args.RawText,                                     // {4}
                                           GetFactionNameWithParanthesis(args.Player.Index)  // {5}
                                           );

            TSPlayer.All.SendMessage(message,
                                     args.Player.Group.R,
                                     args.Player.Group.G,
                                     args.Player.Group.B);

            TSPlayer.Server.SendConsoleMessage(message,
                                               args.Player.Group.R,
                                               args.Player.Group.G,
                                               args.Player.Group.B);

            args.Handled = true;
        }

        private static string GetFactionNameWithParanthesis(int playerIndex)
        {
            string result = "";

            if (OnlineFactions.IsPlayerInAnyFaction(playerIndex))
            {
                result = SFactions.Config.ChatFactionNameOpeningParenthesis;
                result += OnlineFactions.GetPlayerFaction(playerIndex).Name;
                result += SFactions.Config.ChatFactionNameClosingParanthesis;
            }
            
            return result;
        }
    }
}
