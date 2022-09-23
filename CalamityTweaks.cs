using Terraria.ModLoader;

namespace CalamityTweaks
{
	public class CalamityTweaks : Mod
	{
	}
}

namespace CalamityTweaks.Enemies
{
	public class SupremeCnidrion : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Supreme Cnidrion");
			
		}

		public override void SetDefaults()
		{
			NPC.width = 300;
			NPC.height = 120;
			NPC.damage = 350;
			NPC.defense = 150;
			NPC.lifeMax = 3000000;
			NPC.knockBackResist = 0;
			NPC.boss = true;
			NPC.noGravity = true;
			NPC.value = 100 * 100 * 100 * 100; //100 platinum. TODO: remove magic number
			NPC.aiStyle = -1;
		}
	}
}