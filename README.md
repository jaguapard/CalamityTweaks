# CalamityTweaks mod

Requires Calamity Mod to function properly. Design doc below describes current end goals to make (subject to change). Implemented ones will be marked with ✅ mark.

# General Changes

Adrenaline now reduces damage you take linearly as it's accumulated when hit. For example, if you have 65% adrenaline damage reduction and got hit when it is 50% full, you will receive 32.5% damage

Rage now increses damage you deal if not triggered by up to 7% multiplicatively 

All health bonuses now stack multiplicatively and take your total HP into account (default behavior is that some items used vanilla's 500 limit, and other - your base HP (up to 600 with fruits), and add up additively). Numbers are tweaked according to it.

Damage modifiers on accessories now stack multiplicatively rather than additively
Any crit chance above 100% will now additively increase crit damage by 1% per each excessive percent. Example: 150% crit chance = 100% chance for 250% damage

Warding modifier now provides 13/14 defense once one/two of this bosses defeated: Supreme Calamitas, Exo Mechs, and 16 and 1.25% DR if Adult Eidolon Wyrm is defeated 

Evading attack with a shield or sash dash will no longer trigger any debuffs and will not put on take damage items on cooldown (like Flesh Totem)

Fixed Draconic Elixir not giving 16 defense as stated on tooltip

# Accessories
All "raining X when hit" now deal 200% more damage. Additionaly, they will deal 1% more damage for every damage point you take (calculated before defense and DR).

The Community: defeating a boss will now bring bonuses for all the previous ones too (order on wiki). Now reduces duration of negative DoTs by half rather than reducing their damage

Nebulous Core: adds 2 life regen at all times. Upon triggering restores 150 hp and makes you fully invulnerable for 3 s. When on cooldown, also provides 2 more life regen and 10 defense

Silva/Auric tesla armor revive: upon triggering, your are invulnerable for 9 s, restores one/two thirds your max health (without/with draconic elixir). Cooldown now doesn't freeze when boss is alive and provides extra 4 life regen, 20 defense and 15% damage

Shattered Community: now doesn't lose some bonuses from fully charged community and doesn't require charging (starts at lvl 25 immediately) (now is a complete direct upgrade to normal community)

Blood Pact: reduced HP bonus from 100% to 50%
Crit damage is now calculated after all reductions rather than before
Crit multiplier reduced from 2.25 to 2

Regenerator: defense increased from 10 to 20, regen bonus from 12 to 20

The Camper: standing still now reduces damage you take by 33% and defense damage by 66%.

# Existing enemies
## Supreme Calamitas
Increased damage of all attacks by 15%, HP by 20%, Brothers, Moons and Soul Seekers unaffected. 
Now summons 1 soul seeker upon each 100,000 HP lost. This Soul Seekers turn passive, invulnerable and stop any actions during bullet hells, when Sepulcher or Brothers are present. 
This Seekers look different to normal ones, stay close to Scal and cycles between firing brimstone dart with reduced damage and healing 0.25% of Scal max HP. If a seeker healed her 4 times, it loses the ability to heal and fires a dart instead 

## Adult Eidolon Wyrm
Adult Eidolon Wyrm now drops treasure bag when defeated. After AEW is defeated, Brimstone Witch begins to sell Shadowspec and Auric Tesla bars

## Enchantments
Most of Calamity's Enchantments are rather poorly balanced in a bad way: downsides are too strong and make weapons less effective on average. 
Oblatory Enchantment is busted on slowly firing weapons due to very negligible downside for a gigantic upside.

Aflame: no longer disables your natural life regeneration, instead reducing it by 5. Debuff DPS increased from 2664 to 3000, enemies aflame recieve 15% more damage.

Lecherous: orb health reduced from 184,445 to 120,000, increased Orb's speed and turn rate by 50%, reduced hearts dropped on death from 7 to 6

Oblatory: self-damage chance increased from 25% to 100% and damage now proportionally scales with use time of the weapon, 60 use time = 5 health lost, 30 - 2.5, 15 - 1.25, etc. Any fractional damage is moved to next use.

Ephemeral: reduced charge usage by 50% (??)

Tainted: now increases weapon damage by 20% and range by 50% multiplicatively as well

## New items
AEW bag: 40 ancient wyrm soul fragments

Adrenaline booster: Crafted with 10 Shadowspec Bars, 5 AEW souls @ Draedon's Forge. Consumable. Permanently makes adrenaline accumulate 33% faster, deal 25% damage, last 2 seconds longer and increases damage reduction by 10%. Allows adrenaline to continue accumulating while it's active, increasing the duration. Nanomachines only get speedup bonus

### Core of the Undying
Nebulous Core, Draedon's Heart, Yharim's Gift, Core of the Blood God, 10 shadowspec, 10 aew souls @ Draedon's Forge: Combines effects of the all accessories.

75 defense
25% increased damage, 15% movement speed, 2 life regen, leave behind exploding dragon dust as you move and rain meteors when damaged

When visibility is on, replaces Adrenaline with Advanced Nanomachines, that heal 500 health over 5 seconds. Damage taken reduced by 55% when healing, continue to accumulate when healing and do not overuse. If visibility is off, player gets 50% multiplicative damage buff on Adrenaline activation for it's duration 

Increases the efficiency of healing potions by 35% and max HP by 12%.

Every 18 seconds reduces enemy contact damage by half. If this effect is triggered before cooldown has passed, it will reduce less damage (linearly scale from 0 to 50%). All contact damage is reduced by another 15% at all times 

Survive a fatal hit every 80 seconds. Upon triggering you become fully invulnerable for 4 s and heal 250 hp. While on cooldown, you get extra 3 life regen, 25 defense and 10% damage

## Raging King Crown
Occult skull crown, calamity, shattered community, 5 shadowspec: combine effects. 

## New enemies 
### Supreme Cnidrion
Red eyed Cnidrion, clad in Exo armor, intended to be fought after AEW (Exo mech+Scal maybe?). All stats for master death mode. Summoned by using Seahorse Terror Beacon (crafted with 10 Shadowspec and Amidas' Spark @ Draedon's Forge) :

Health: 12,000,000
DR: 20%
Defense: 150

Damage:
Contact (non-predictive charge): 1250
Predictive charge: 950
Clone charge: 850
Supreme Water Bolt: 960
Supreme Splitting Water Bolt (ascending): 780
Water Tide: 1250
Steam Breath: 1080
Water deathhail: 840
Predictive water arrow: 910

Attack pattern:
Fires 20 supreme water bolts in a 20 degree spread from the player going up and down every 0.1666 sec (10 ticks)
Charges the player 3 times not predictively and roars before each one
Fires telegraphed big water tides to it's left and right every 2 seconds 5 times
Fires 30 water deathhails around itself each 0.2 s
Charges 4 times predictively, making high pitch roar before every charge 
Idles for 1.5 s
Chases the player breathing steam for 7 seconds 
5 times fires normal, predictive and mirrored predicive water arrows at the player with 1.5 second interval
Repeat

Phase 2: upon reaching 50/60% hp: summons 2(rev mode)/3(death mode) smaller clones of itself, that orbit it and don't deal contact damage. Instead, they fire supreme water bolts at the player in 3 different ways: shotgun blast, a bunch of direct ones and sequential. Main continues all attacks from phase 1. Clones each have 50% DR, 1,000,000 HP, 110 defense. Clones increase main's projectile count by 20/30/40% and reduce attack interval by 5/15/25% when defeated. 

Phase 3: The main one will become invulnerable, fully passive and invisible upon reaching 20/30% health, leaving remaining clones to fight, spawning one more, and granting them all ability to charge the player non-predictively and with short range. Once all are defeated, transitions into phase 4

Phase 4: turns visible over the course of 5 seconds, being unable to move, attack, being attacked or dealing contact damage while doing so. When fully visible, goes through attack pattern (gains 50% DR when idling):

Charge 4 times unpredictively and 2 predictevely telegraphed, launching supreme splitting water bolts above itself every 0.33 s, which each split into 2/3 supreme water bolts aimed at the player after ascending for 3 seconds. 
Idles for 1 s
Chases the player with steam breath for 10 s
Idles for 1 s
Fires 5 blasts of 10 telegraphed supreme water bolts in 60 degree spread around the player, adjusting it's position randomly by up to 10 tiles each time each 0.8 seconds
Idles for 1 s
Repeat until defeated

Drops: Supreme Seahorse Steak - consumable, permanently adds an accessory slot.
