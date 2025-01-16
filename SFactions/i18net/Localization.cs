namespace SFactions.i18net 
{
    public static class Localization
    {
        public static string PluginDescription = "Soofa\u0027s factions.";
        public static string FactionCommandHelpText = "To see the detailed help message do /faction help";
        public static string ReloadSuccessMessage = "[SFactions] Plugin has been reloaded.";
        public static string UpdateManager_ErrorMessage = "[SFactions] An error occurred while checking for updates.";
        public static string UpdateManager_CheckingMessage = "[SFactions] Checking for updates...";
        public static string UpdateManager_UpToDate = "[SFactions] The plugin is up to date!";
        public static string UpdateManager_NotUpToDate = "[SFactions] The plugin is not up to date.\nPlease visit https://github.com/Soof4/SFactions/releases/latest to download the latest version.";
        public static string War_StartAnnouncement = "{0} vs {1} war has started!";
        public static string War_EndTie = "The war between {0} and {1} ended as a TIE!";
        public static string War_NoTie = "The war between {0} and {1} ended as {2} being victorious!";
        public static string ErrorMessage_SpecifySubCommand = "You need to specify a subcommand. Do \u0027/faction help\u0027 to see all subcommands.";
        public static string ErrorMessage_NotInFaction = "You\u0027re not in a faction.";
        public static string ErrorMessage_InFaction = "You\u0027re in a faction.";
        public static string ErrorMessage_LeaderOnly = "Only leaders can use this command.";
        public static string ErrorMessage_MissingArgument = "Missing an argument.";
        public static string ErrorMessage_FactionNotFound = "Faction not found.";
        public static string ErrorMessage_PlayerNotFound = "Player not found.";
        public static string ErrorMessage_OnlyInGameCommand = "You can only use this command in game.";
        public static string ErrorMessage_GenericFail = "Command failed, check logs for more details.";
        public static string ErrorMessage_MissingFactionName = "You need to specify a faction name.";
        public static string ErrorMessage_NameTaken = "A faction with this name already exists.";
        public static string ErrorMessage_FactionNameTooShort = "Faction name needs to be at least {0} characters long.";
        public static string ErrorMessage_FactionNameTooLong = "Faction name needs to be at most {0} characters long.";
        public static string ErrorMessage_ColorCodesNotAllowed = "Color codes are not allowed in faction names.";
        public static string ErrorMessage_NoInviteFound = "Couldn\u0027t find a pending invitation.";
        public static string AbilityCommand_HelpText = "Changes faction\u0027s ability.";
        public static string AbilityCommand_SyntaxHelp = "/faction ability \u003Cability name\u003E";
        public static string AbilityCommand_SuccessMessage = "Your faction\u0027s ability is now {0}.";
        public static string AbilityCommand_ErrorMessage_MissingAbilityName = "Missing ability name. Valid ability types are: {0}";
        public static string AbilityCommand_ErrorMessage_InvalidAbilityName = "Invalid ability name. Valid ability types are: {0}";
        public static string AcceptCommand_HelpText = "Accepts a faction invite.";
        public static string AcceptCommand_SyntaxHelp = "/faction accept";
        public static string AcceptCommand_SuccessMessage = "You\u0027ve joined {0}.";
        public static string AcceptCommand_ErrorMessage_MustLeaveFaction = "You need to leave your current faction to join another.";
        public static string BaseCommand_HelpText = "Teleports player to the faction base";
        public static string BaseCommand_SyntaxHelp = "/faction base";
        public static string BaseCommand_ErrorMessage_NoBase = "Your faction doesn\u0027t have a base!";
        public static string CreateCommand_HelpText = "Creates a new faction.";
        public static string CreateCommand_SyntaxHelp = "/faction create \u003Cfaction name\u003E";
        public static string CreateCommand_ErrorMessage_MustLeaveFaction = "You need to leave your current faction first to create new.\nIf you want to leave your current faction do \u0027/faction leave\u0027";
        public static string CreateCommand_SuccessMessage = "You\u0027ve created {0}.";
        public static string HelpCommand_HelpText = "Shows help texts for faction commands.";
        public static string HelpCommand_SyntaxHelp = "/faction help [page number / command name]";
        public static string HelpCommand_Header = "Sub-commands (Page: {0}/{1}):";
        public static string InfoCommand_HelpText = "Shows information about the faction.";
        public static string InfoCommand_SyntaxHelp = "/faction info \u003Cfaction name\u003E";
        public static string InfoCommand_Result = "Faction ID: {0}\nFaction Name: {1}\nFaction Leader: {2}\nFaction Ability: {3}\nHas Base: {4}\nInvite Type: {5}";
        public static string InviteCommand_HelpText = "Sends an invitation to another player.";
        public static string InviteCommand_SyntaxHelp = "/faction invite \u003Cplayer name\u003E";
        public static string InviteCommand_ErrorMessage_MissingPlayerName = "You need to specify a player name.";
        public static string InviteCommand_ErrorMessage_AlreadyHasAnInvite = "This player already has a pending invitation from your faction.";
        public static string InviteCommand_NotificationMessage = "{0} has invited you to {1}. Type \u0022/faction accept\u0022 to join. Do nothing if you don\u0027t want to join.";
        public static string InviteCommand_SuccessMessage = "You\u0027ve successfully invited {0} to your faction.";
        public static string InviteTypeCommand_HelpText = "Shows and changes your faction\u0027s invite type.";
        public static string InviteTypeCommand_SyntaxHelp = "/faction invitetype [open / inviteonly / closed]";
        public static string InviteTypeCommand_GetResult = "Your faction is {0}.";
        public static string InviteTypeCommand_ErrorMessage_InvalidType = "Invalid invite type were given. Valid types are open, inviteonly, closed.";
        public static string InviteTypeCommand_SetResult = "You\u0027ve successfully changed you faction\u0027s invite type to {0}.";
        public static string JoinCommand_HelpText = "Used for joining an open faction.";
        public static string JoinCommand_SyntaxHelp = "/faction join \u003Cfaction name\u003E";
        public static string JoinCommand_ErrorMessage_InviteOnly = "{0} is an invite only faction.";
        public static string JoinCommand_SuccessMessage = "You\u0027ve joined {0}.";
        public static string LeadCommand_HelpText = "Used for becoming the leader of your faction if leader quits the faction.";
        public static string LeadCommand_SyntaxHelp = "/faction lead";
        public static string LeadCommand_ErrorMessage_LeaderExists = "{0} is your faction\u0027s leader already.";
        public static string LeadCommand_SuccessMessage = "You\u0027re the leader of your faction now.";
        public static string LeaveCommand_HelpText = "Used for leaving your current faction.";
        public static string LeaveCommand_SyntaxHelp = "/faction leave";
        public static string LeaveCommand_SuccessMessage = "You\u0027ve left your faction.";
        public static string RegionCommand_HelpText = "Sets or deletes faction\u0027s region. (You must be inside an already defined region before setting it.)";
        public static string RegionCommand_SyntaxHelp = "/faction \u003Cset / del\u003E";
        public static string RegionCommand_MissingSubCommand = "You need to specify set or del";
        public static string RegionCommand_ErrorMessage_InvalidSubCommand = "Invalid region subcommand. (Please use either set or del)";
        public static string RegionCommand_ErrorMessage_MustDeleteOldOne = "You need to delete the old region before setting new one. (Do \u0022/faction region del\u0022 to delete old region.)";
        public static string RegionCommand_ErrorMessage_NotInARegion = "You\u0027re not in a protected region.";
        public static string RegionCommand_ErrorMessage_NotOwner = "You\u0027re not the owner of this region.";
        public static string RegionCommand_SuccessMessage_Set = "Successfully set the region.";
        public static string RegionCommand_ErrorMessage_NoFactionRegion = "Your faction doesn\u0027t have a region set.";
        public static string RegionCommand_SuccessMessage_Del = "Successfully deleted the region.";
        public static string RenameCommand_HelpText = "Renames the faction";
        public static string RenameCommand_SyntaxHelp = "/faction rename \u003Cnew name\u003E";
        public static string RenameCommand_SuccessMessage = "Successfully changed faction name to {0}.";
        public static string WarCommand_HelpText = "Invites, accepts or declines war invitations.";
        public static string WarCommand_SyntaxHelp = "/faction war \u003Cinvite / accept / decline\u003E [faction name]";
        public static string WarCommand_ErrorMessage_MissingSubCommand = "You need to specify invite, accept or decline.";
        public static string WarCommand_ErrorMessage_InvalidSubCommand = "Invalid war subcommand. (Please use one of invite, accept or decline)";
        public static string WarCommand_ErrorMessage_FactionNotOnline = "A faction with specified name couldn\u0027t be found online.";
        public static string WarCommand_ErrorMessage_EnemyLeaderNotOnline = "Enemy faction\u0027s leader is not online.";
        public static string WarCommand_ErrorMessage_CantAttackYourself = "You can\u0027t start a war with yourself!";
        public static string WarCommand_ErrorMessage_OngoingWar = "There is another war ongoing right now. Please wait till it ends.";
        public static string WarCommand_ErrorMessage_PendingInvite = "There is already a pending invitation to this faction.";
        public static string WarCommand_NotificationMessage_GotInvite = "{0} has invited your faction to a war with {1}.\nDo [c/ffffff:/faction war accept] to accept, [c/ffffff:/faction war decline] to decline.";
        public static string WarCommand_NotificationMessage_Declined = "{0} declined your war invitation.";
        public static string WarCommand_SuccessMessage_Declined = "You\u0027ve declined the war invitation.";
        public static string DeleteCommand_HelpText = "Deletes the faction.";
        public static string DeleteCommand_SyntaxtHelp = "/faction delete";
        public static string DeleteCommand_SuccessMessage = "Successfully deleted the faction.";
        public static string ListCommand_HelpText = "Lists all of the factions.";
        public static string ListCommand_SyntaxHelp = "/faction list [page number]";
        public static string ListCommand_Header = "Factions (Page: {0}/{1})";
        public static string ListCommand_EntryFormat = "\nID: {0} - Name: {1} - Leader: {2}";
        public static string ListCommand_PageEnd = "\nFor more information about a faction do /faction info \u003Cfaction name\u003E.";
    }
}
