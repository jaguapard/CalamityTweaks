using Terraria;
using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using System.Collections.Generic;
using CalamityTweaks.Enemies;

namespace CalamityTweaks
{
	public class CalamityTweaks : Mod
	{
	}

	namespace Items
	{
		class SeahorseTerrorBeacon : ModItem
		{
			public override void SetStaticDefaults()
			{
				base.SetStaticDefaults();

				DisplayName.SetDefault("Seahorse Terror Beacon");
				Tooltip.SetDefault("Summons the Thiccc Supreme Seahorse\nDon't stray too far away and pray you're able\nNot consumable");
			}

			public override void SetDefaults()
			{
				base.SetDefaults();

				Item.useTime = 60;
				Item.useAnimation = 17;
				Item.consumable = false;
                Item.useStyle = ItemUseStyleID.EatFood;
            }
			public override void AddRecipes()
			{
                var CalamityMod = ModLoader.GetMod("CalamityMod");
                Recipe recipe = CreateRecipe() //TODO: change it to Seahorse Terror Beacon
                .AddIngredient(CalamityMod, "ShadowspecBar", 10)
                //.AddIngredient(CalamityMod, "AmidasSpark") //TODO: find real item name for Amidas' Spark
                .Register();
            }

            public override bool? UseItem(Player player)
			{
                if (player.itemAnimation > 0 && player.itemTime == 0)
                {
					NPC.NewNPC(null, (int)player.position.X + 600, (int)player.position.Y, ModContent.NPCType<SupremeCnidrion>());
                }
                return true;
            }

        }
	}
}