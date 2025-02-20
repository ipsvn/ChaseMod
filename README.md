# ChaseMod
Some gameplay/utility features for CS2 HNS servers.

## Features

### Freezetime
Freeze the CTs at the start of the round. Configurable with `ctStartFreezeTime` or set to `0` to disable.

### Freeze/stun grenade
Smoke grenade is a freeze grenade, with configurable explode time (`stunThrowTime`), radius (`stunFreezeRadius`) and time frozen (`stunFreezeTime`).

### Antiflash
Makes flashbangs only flash the enemy team.

### Knife damage modification
Override the amount of damage the knife does, along with the possibility of adding an invincibility cooldown to players being stabbed. Ensure `enableKnifeHook` and `knifeDamageModify` is enabled in config, and tune `knifeDamage` and `knifeCooldown` to your liking. 


> [!CAUTION]
> The `knifeDamage` value is still affected by armor, so ensure your server `mp_max_armor` convar is set to `0` if you expect it to be identical to the value in the config.

### Team switching
Switch teams on CT win, optionally on T winstreak (with `maxTerroristWinStreak` option). Enable/disable entirely with the `enableTeamSwitchingConditions` config option.

## Installation
Download the .zip from the [releases](https://github.com/ipsvn/ChaseMod/releases) and extract the contents into `counterstrikesharp/plugins/` on your server.
