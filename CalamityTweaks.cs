using Terraria.ModLoader;
using Microsoft.Xna.Framework;

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
			NPC.value = 100 * 100 * 100 * 100; //100 platinum. TODO: remove magic number
			NPC.aiStyle = -1;

            NPC.boss = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;

			
        }

		public override void AI()
		{
			float lifePct = NPC.GetLifePercent();

			if (lifePct > 60) bossPhase = 1;
			else if (lifePct > 30) bossPhase = 2;
			//TODO: implement: if clones are alive, enter phase 3
			else bossPhase = 4;
		}

		private int bossPhase = 0;

		//These are target damage values for Master Death mode. 
		private static float targetDamage_nonPredictiveCharge = 1250;
        private static float targetDamage_predictiveCharge = 950;
		private static float targetDamage_cloneCharge = 850;
		private static float targetDamage_supremeWaterBolt_contact = 960;
		private static float targetDamage_supremeWaterBolt_ascending = 780;
		private static float targetDamage_waterTide = 1250;
		private static float targetDamage_steamBreath = 1080;
		private static float targetDamage_waterDeathhail = 840;
		private static float targetDamage_predictiveWaterArrow = 910;
    }
}