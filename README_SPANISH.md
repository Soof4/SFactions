
# SFactions
Un Plugin de Facciones para Tshock. (Traducido por [FrankV22](https://github.com/itsFrankV22))

## Como Instalar
Descargar **SFactions.dll** y poner en la carpeta **ServerPlugins** de su servidor. <br>
Descargar **Abilities.dll** and place it in **bin** folder. <br>
_**Nota:** Ver [Abilities repository](https://github.com/Soof4/Abilities) Si quieres mantenerte al dia sobre actualizaciones etc._

## Permisos y Comandos
Nececitas darle este permiso `sfactions.faction` al grupo de jugadores si no lo has cambiado es "default", deben usar `/faction` o `/f`. <br>
_**Nota:** No dar el permiso a jugadores sin registrarse._

### Sbu-Comandos y Descripcion
* `help` : Muestra la lista de comandos. <br>
ej: `/f help`

* `create` : Crrear una nueva faccion. <br>
eg: `/f create The Furnitures`

* `join` : Unirse a una Faccion. <br>
eg: `/f join The Furnitures`

* `leave` : Salirse de una Faccion. <br>
eg: `/f leave`

* `rename` : Renombrar Faccion. (Solo Lider) <br>
eg: `/f rename The Mafia`

* `lead` : Convertir a otro jugador en Lider de la faccion. <br>
eg: `/f lead`

* `ability` : Cambiar la abilidad de la faccion (Solo Lider) <br>
eg: `/f ability harvest` <br>
eg: `/f ability` (Aquí los nombres de las abilidades)

* `region` : Asignar o Eliminar regiones de la Faccion. (Solo Lider) <br>
eg: `/f set` (Asignar la Facción donde el jugador este dentro en ese momento. Automáticamente se comparte a otros miembros.) <br>
eg: `/f del` (Eliminar vinculo entre la facción y la region.)

* `invitetype` : Muestra el tipo de invitación de tu facción si no se dieron argumentos. Pero si el jugador es el líder y se le da un número, entonces establece el tipo de invitación de la facción a ese número. <br>
eg: `/f invitetype` <br>
eg: `/f invitetype 0` (Esta facción se encontrara "Abierta" a otros jugadores.) <br>
0 = Abierta, no nececita invitacion <br>
1 = Todos los miembros pueden invitar jugadores <br>
2 = Solo líder puede invitar jugadores <br>

* `invite` : invitar jugadores a la faccion. <br>
eg: `/f invite Soofa`

* `accept` : Aceptar invitacion a la faccion. <br>
eg: `/f accept`

* `info` : Mostrar la informacion de la faccion. <br>
eg: `/f info The Furnitures`

## Otras informaciones
* Para usar las abilidades, Nececitas usar el arpa.
* Para alternar habilidades que tienen subhabilidades, los jugadores deben usar el item Reloj de cobre de Bolsillo.
* Estas son las siguientes abilidades disponibles, (deberás pegarlos en la propiedad EnabledAbilities en el archivo de configuración; de forma predeterminada, ya estarán agregados)
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
