using Terraria;
using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using System;

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
			NPC.width = 365;
			NPC.height = 236;
			NPC.damage = (int)(targetDamage_nonPredictiveCharge * targetDamage_mult);
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
			NPC.TargetClosestUpgraded();
			ticksInCurrentPhase++;

			float lifePct = NPC.GetLifePercent();

			if (lifePct > 0.60) TransitionToPhase(1);
			else if (lifePct > 0.30) TransitionToPhase(2);
            //TODO: implement: if clones are alive, enter phase 3
            else TransitionToPhase(4);

            currentAttackTickCounter++;
			NonPredictiveCharge(80);
        }

		private void TransitionToPhase(int phase)
		{
			prevBossPhase = currBossPhase;
            currBossPhase = phase;

			if (prevBossPhase != currBossPhase)
			{
				ticksInCurrentPhase = 0;

				if (phase == 1) Talk("Long have I waited for this. I heard rumors about you. Nobody took me seriously, they kept laughing at me, saying I'm a weakling mini-boss! Now, I will prove everyone wrong by defeating you!");
				if (phase == 2) Talk("Your performance is surprising. Given how much I worked on myself, you are a powerful opponent. I can respect that, but I have a few tricks too.");
				if (phase == 3) Talk("Alright, I'll leave fighting to little ones for now");
				if (phase == 4) Talk("NOOO! I just can't die like this!");
			}
		}
		private void NonPredictiveCharge(int targetTickDuration)
		{
			if (currentAttackTickCounter == 0)
			{
				Vector2 targetPos = Main.player[NPC.target].position;
				Vector2 dir = targetPos - NPC.position;
				this.currentChargeVelocity = dir * 10f / targetTickDuration;
			}

			if (currentAttackTickCounter < targetTickDuration / 3 || currentAttackTickCounter > 1 - targetTickDuration / 3) NPC.velocity = Vector2.Zero;
			else NPC.velocity = this.currentChargeVelocity;

			if (currentAttackTickCounter == targetTickDuration) currentAttackTickCounter = -1;
        }

        private void Talk(string message)
        {
            if (Main.netMode != NetmodeID.Server)
            {
				//string text = Language.GetTextValue("Mods.ExampleMod.NPCTalk", Lang.GetNPCNameValue(npc.type), message);
				string text = message;
                Main.NewText(text, 150, 250, 150);
            }
            else
            {
				//Terraria.Localization.NetworkText text = Terraria.Localization.NetworkText.FromKey("Mods.ExampleMod.NPCTalk", Lang.GetNPCNameValue(npc.type), message);
				Terraria.Localization.NetworkText text = Terraria.Localization.NetworkText.FromLiteral(message);
                Terraria.Chat.ChatHelper.BroadcastChatMessage(text, new Color(150, 250, 150));
            }
        }

        private int currBossPhase = -1;
        private int prevBossPhase = -2;
        private int ticksInCurrentPhase = 0;
		private int currentAttackTickCounter = -1;

		private Vector2 currentChargeVelocity;

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
		private static float targetDamage_mult = 350.0f / 1250;
    }
}