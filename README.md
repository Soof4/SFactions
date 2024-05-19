
# SFactions
A TShock factions plugin.

## Setting Up
Download **SFactions.dll** and place it in **ServerPlugins** folder. <br>
Download **Abilities.dll** and place it in **bin** folder. <br>
_**Note:** Check [Abilities repository](https://github.com/Soof4/Abilities) regularly, if you want to keep up with balancing updates etc._

## Permissions and Commands
You need to grant `sfactions.faction` permisson to players' group. Then players will be able to use `/faction` command. <br>
_**Note:** Do not grant this permisson to a unregistered player groups._

### Sub-Commands and Descriptions
* `help` : Shows the help text. <br>
eg: `/f help`

* `create` : Creates a new faction. <br>
eg: `/f create The Furnitures`

* `join` : Join a faction. <br>
eg: `/f join The Furnitures`

* `leave` : Leave the faction. <br>
eg: `/f leave`

* `rename` : Rename the faction. (Leader only) <br>
eg: `/f rename The Mafia`

* `lead` : Make yourself the leader of the  faction, if the leader has left the faction. <br>
eg: `/f lead`

* `ability` : Changes the faction's ability (Leader only) <br>
eg: `/f ability harvest` <br>
eg: `/f ability` (This will show all the ability names)

* `region` : Sets or deletes the faction's region. (Leader only) <br>
eg: `/f set` (This will set the region that player is inside of as the faction's region. Faction regions are automatically shared with members.) <br>
eg: `/f del` (Deletes the link between region and the faction.)

* `invitetype` : Shows your faction's invite type if no arguments were given. But if the player is the leader and a number is given then sets the faction's invite type to that numebr. <br>
eg: `/f invitetype` <br>
eg: `/f invitetype 0` (This will set faction as "Open".) <br>
0 = Open, no need for invite <br>
1 = All members can invite new players <br>
2 = Only leader can invite new players <br>

* `invite` : Invites a player to your faction. <br>
eg: `/f invite Soofa`

* `accept` : Accepts invitation to join a faction. <br>
eg: `/f accept`

* `info` : Shows the faction's information. <br>
eg: `/f info The Furnitures`

## Other Informations
* In order to cast abilities, players need to use Harp item.
* In order to cycle abilities that have sub-abilities, players need to use Copper Watch item.
* Currently supported abilities are as follow, (you'll need to paste these into EnabledAbilities property in the config file, by default they'll come added already)
  1. dryadsringofhealing
  2. ringofdracula
  3. setsblessing
  4. adrenaline
  5. witch
  6. marthymr
  7. randomtp
  8. fairyoflight
  9. twilight
  10. harvest
  11. icegolem
  12. magicdice
  13. thebound
  14. alchemist
  15. paranoia
  16. hypercrit