using Terraria;
using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using System.Collections.Generic;

namespace CalamityTweaks
{
	public class CalamityTweaks : Mod
	{
	}
}

namespace CalamityTweaks.Enemies
{
    [AutoloadBossHead]
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
			NPC.damage = targetDamage_nonPredictiveCharge;
			NPC.defense = 150;
			NPC.lifeMax = 5500000; 
			NPC.knockBackResist = 0;			
			NPC.value = Item.buyPrice(platinum: 50);
			NPC.aiStyle = -1;

            NPC.boss = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;			
        }

		public override void AI()
		{
            NPC.TargetClosestUpgraded(true);
			if (!NPC.HasValidTarget) this.NPC.velocity.Y += 1f;

			NPC.FaceTarget();
            this.targetPlayer = Main.player[NPC.target];
            ticksInCurrentPhase++;

			float lifePct = NPC.GetLifePercent();

			if (lifePct > 0.60) SetTargetPhase(1);
			else if (lifePct > 0.30) SetTargetPhase(2);
			else //TODO: FIX: transition to phase 4 doesn't work
			{
				if (currBossPhase == 3)
				{
					if (!IsAnySpawnAlive()) SetTargetPhase(4);
                }				
				else if (currBossPhase < 3) SetTargetPhase(3);				
			}

            currentAttackTickCounter++;
			int patternDurationTicks = 860;
			int currentPatternTick = ticksInCurrentPhase % patternDurationTicks;

			if (NPC.HasValidTarget && currBossPhase != 3)
			{
				if (currentPatternTick >= 0 && currentPatternTick < 320) ChargeAttack(80, 600, 400, 2000, 0);
				else if (currentPatternTick < 520) WaterBoltAttack(50, (20 * MathF.PI / 180), 4);
				else if (currentPatternTick < 700) ChargeAttack(90, 400, 300, 1800, 1.5f);
				else if (currentPatternTick < 860) WaterDeathHailAttack(0.1f, 0.7f, 3, 80, 40);
			}

			if (currBossPhase != 3)
			{
				int orbitTick = ticksSinceSpawn % 600;
				for (int i = 0; i < spawns.Count; ++i) //TODO: add Spawn death handling
				{
					if (Main.npc[spawns[i]].netID != ModContent.NPCType<SupremeCnidrionClone>()) continue;

					float currentAngle = 2 * i * MathF.PI / 3.0f + orbitTick / 300.0f * MathF.PI;
					Main.npc[spawns[i]].position = this.NPC.position + new Vector2(400.0f * MathF.Sin(currentAngle), 400.0f * MathF.Cos(currentAngle));
				}
			}

            ticksSinceSpawn++;
        }

		public bool IsAnySpawnAlive()
		{
			foreach (var s in spawns)
			{
                if (Main.npc[s].netID == ModContent.NPCType<SupremeCnidrionClone>() && Main.npc[s].life > 0) return true;
            }
			return false;
		}

		protected void SetTargetPhase(int phase)
		{
			prevBossPhase = currBossPhase;
            currBossPhase = phase;

			if (prevBossPhase != currBossPhase) //on phase transition
			{
				ticksInCurrentPhase = 0;

				if (phase == 1) Talk("Long have I waited for this. I've heard rumors about you. Nobody took me seriously, they kept laughing at me, saying I'm a weakling mini-boss! Now, I will prove everyone wrong by defeating you!");

				if (phase == 2)
				{
					Talk("Your performance is surprising. Given how much I worked on myself, you are a powerful opponent. I can respect that, but I have a few tricks too.");
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						for (int i = 0; i < 3; ++i)
						{
							spawns.Add(NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + i * 100, (int)NPC.position.Y, ModContent.NPCType<SupremeCnidrionClone>(), 1, ai0: i, ai1: 0));
						}						
					}
				}

				if (phase == 3)
				{
					Talk("Alright, I'll leave fighting to the little ones for now");
					NPC.immortal = true;
					NPC.alpha = 1;
					NPC.damage = 0;
                    for (int i = 0; i < 3; ++i)
                    {
                        spawns.Add(NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + i * 200, (int)NPC.position.Y, ModContent.NPCType<SupremeCnidrionClone>(), 1, ai0: i, ai1: 0));
                    }

                    foreach (var s in spawns)
                    {
						if (Main.npc[s].netID == ModContent.NPCType<SupremeCnidrionClone>()) Main.npc[s].ai[1] = 1f;
                    }
                }

				if (phase == 4)
				{
					Talk("Time to get serious!");
					NPC.alpha = 255;
					NPC.damage = targetDamage_nonPredictiveCharge;
					NPC.immortal = false;
				}
			}
		}
		protected void ChargeAttack(int targetTickDuration, float minDist, float playerOffset, float maxDist, float predictiveness)
		{
			int idleTicks = targetTickDuration / 3;

            if (currentAttackTickCounter == idleTicks)
			{
                int chargeTickDuration = targetTickDuration / 3;
                Vector2 targetPos = targetPlayer.Center + targetPlayer.velocity * (chargeTickDuration*0.5f) * predictiveness;
				Vector2 dir = targetPos - NPC.Center;
				Vector2 unitDir = dir.SafeNormalize(Vector2.Zero);

				float dirLen = dir.Length();
				Vector2 chargeTargetPoint = NPC.Center + unitDir * MathF.Max(MathF.Min(dirLen + playerOffset, maxDist), minDist);
				
				this.currentChargeVelocity = (chargeTargetPoint - NPC.Center) / chargeTickDuration;
			}

			if (currentAttackTickCounter < idleTicks || currentAttackTickCounter > targetTickDuration - idleTicks) NPC.velocity = Vector2.Zero;
			else
			{
				NPC.velocity = this.currentChargeVelocity;
            }

			if (currentAttackTickCounter == targetTickDuration)
			{
                currentAttackTickCounter = 0;
			}
        }

		protected void WaterBoltAttack(int boltCount, float maxSpreadRadians, int ticksPerBolt)
		{
			NPC.velocity = Vector2.Zero;

            int fullAttackDuration = ticksPerBolt * (boltCount-1);
			if (currentAttackTickCounter > fullAttackDuration)
			{
				currentAttackTickCounter = 0;
				return;
			}

			if (currentAttackTickCounter % ticksPerBolt == 0)
			{
                if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {					 
                    var source = NPC.GetSource_FromAI();
                    Vector2 position = NPC.Center;
                    Vector2 targetPosition = targetPlayer.Center;
                    Vector2 direction = targetPosition - position;

					float radius = direction.Length();
					float targetAngle = position.AngleTo(targetPosition);
					float currentSpreadPhase = (float)(currentAttackTickCounter) / fullAttackDuration; //from 0 to 1
					float spreadPhaseAdjusted = currentSpreadPhase - 0.5f; //from -0.5 to 0.5
					float alterationModifier = ((currentAttackTickCounter / ticksPerBolt) % 2) * 2 - 1; //-1 and 1. Makes bolts converge onto the player
					float projectileAngle = targetAngle + maxSpreadRadians * spreadPhaseAdjusted * alterationModifier;

					direction = new Vector2(radius * MathF.Cos(projectileAngle), radius * MathF.Sin(projectileAngle));
                    direction.Normalize();
                    float speed = 14f;
                    int type = ProjectileID.PinkLaser; //TODO: change it to something watery
					int damage = targetDamage_supremeWaterBolt_contact;
                    Projectile.NewProjectile(source, position, direction * speed, type, damage, 0f, Main.myPlayer);
                }
            }
		}

		protected void WaterDeathHailAttack(float safeSpaceSize, float maxFirstRoll, int ticksPerBolt, int boltCount, int delayTicks) //boltCount must be even.
		{
            int fullAttackDuration = boltCount / 2 * ticksPerBolt;
            NPC.velocity = Vector2.Zero;
            if (currentAttackTickCounter < delayTicks) return;

			int attackCycleTick = currentAttackTickCounter - delayTicks;
			if (attackCycleTick % ticksPerBolt != 0) return;

			if (currentAttackTickCounter == delayTicks)
			{
				Random r = new();
				this.NPC.ai[0] = (float)r.NextDouble() * maxFirstRoll;
			}

			int projectileNumber = attackCycleTick / ticksPerBolt;
			float leftProjectileSpacing = 1920 * NPC.ai[0] / (boltCount*0.5f);
			float rightProjectileSpacing = 1920 * (1 - NPC.ai[0] - safeSpaceSize) / (boltCount * 0.5f);

            if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
            {
                var source = NPC.GetSource_FromAI();
                Vector2 position = targetPlayer.Center - new Vector2(960f, 540f);
				Vector2 direction = new(0f, 1f);

                direction.Normalize();
                float speed = 6f;
                int type = ProjectileID.PinkLaser; //TODO: change it to something watery
                int damage = targetDamage_waterDeathhail;
				Vector2 adjDir = direction * speed;

				Vector2 leftOffset = new(leftProjectileSpacing * projectileNumber, 0f);
				Vector2 rightOffset = new(1920f - rightProjectileSpacing * projectileNumber, 0f);
                Projectile.NewProjectile(source, position + leftOffset, adjDir, type, damage, 0f, Main.myPlayer);
                Projectile.NewProjectile(source, position + rightOffset, adjDir, type, damage, 0f, Main.myPlayer);
            }
        }

        protected void Talk(string message) //TODO: add localization stuff?
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

        protected int currBossPhase = -1;
        protected int prevBossPhase = -2;
        protected int ticksInCurrentPhase = 0;
		protected int currentAttackTickCounter = 0; //how much ticks from beginning of last attack. Reset it to 0 when attack is completed
		protected int ticksSinceSpawn = 0;
		protected List<int> spawns = new();

		protected Vector2 deathHailTargetPos;
        protected Vector2 currentChargeVelocity;
		protected Player targetPlayer;

		//Damage values are designed for Master Death mode originally (first number) and are scaled appropriately (second number, the multiplier) 
		protected static int targetDamage_nonPredictiveCharge = (int)(1250*0.4);
        protected static int targetDamage_predictiveCharge = (int)(950*0.4);
		protected static int targetDamage_cloneCharge = (int)(850 * 0.4);
        protected static int targetDamage_supremeWaterBolt_contact = (int)(860 * 0.2);
		protected static int targetDamage_supremeWaterBolt_ascending = (int)(690 * 0.2);
		protected static int targetDamage_waterTide = (int)(1250 * 0.2);
		protected static int targetDamage_steamBreath = (int)(1250 * 0.2);
		protected static int targetDamage_waterDeathhail = (int)(840*0.2);
		protected static int targetDamage_predictiveWaterArrow = (int)(910 * 0.2);
    }

    [AutoloadBossHead]
    public class SupremeCnidrionClone : SupremeCnidrion
	{
        protected float orbitRadianOffset;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Supreme Cnidrion Spawn");
        }
        public override void SetDefaults()
        {
            NPC.width = 365/3;
            NPC.height = 236/3;
			NPC.damage = 0;
            NPC.defense = 110;
            NPC.lifeMax = 1500000;
            NPC.knockBackResist = 0;
            NPC.value = Item.buyPrice(platinum: 3);
            NPC.aiStyle = -1;

            NPC.boss = true;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;

			orbitRadianOffset = NPC.ai[0] * 120.0f * MathF.PI / 180.0f;			
        }

        public override void AI()
        {
            NPC.TargetClosestUpgraded(true);
            NPC.FaceTarget();
            this.targetPlayer = Main.player[NPC.target];

			ticksInCurrentPhase++;
			bool isFreeMoving = (NPC.ai[1] > 0);
			if (isFreeMoving) NPC.damage = targetDamage_cloneCharge;

			currentAttackTickCounter = ticksSinceSpawn % (isFreeMoving ? 200 : 120);
			int attackType = (int)NPC.ai[0] % 3;

			if (currentAttackTickCounter < 120) //TODO: stop attacking when main one is performing water deathhail
			{
				if (attackType == 0) waterBoltSequence(3, 10, 0);
				if (attackType == 1) waterBoltShotgun(5, 60, 40);
				if (attackType == 2) waterBoltWall(5, 60, 80);
			}
			else ChargeAttack(80, 200, 250, 1000, 0);

            ticksSinceSpawn++;
        }

		protected void waterBoltSequence(int projectileCount, int ticksPerBolt, int delayTicks)
		{
			if (currentAttackTickCounter < delayTicks) return;
			if ((currentAttackTickCounter - delayTicks) % ticksPerBolt != 0) return;
			if (currentAttackTickCounter > delayTicks + ticksPerBolt * projectileCount) return;

            if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
            {
                var source = NPC.GetSource_FromAI();
                Vector2 position = NPC.Center;
                Vector2 targetPosition = targetPlayer.Center;
                Vector2 direction = targetPosition - position;

                direction.Normalize();
                float speed = 14f;
                int type = ProjectileID.PinkLaser; //TODO: change it to something watery
                int damage = targetDamage_supremeWaterBolt_contact;
                Projectile.NewProjectile(source, position, direction * speed, type, damage, 0f, Main.myPlayer);
            }
        }

		protected void waterBoltShotgun(int projectileCount, float maxSpreadDegrees, int delayTicks)
		{	
            if (currentAttackTickCounter == delayTicks && NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
            {
				float maxSpreadRadians = maxSpreadDegrees * MathF.PI / 180;
                var source = NPC.GetSource_FromAI();
                Vector2 position = NPC.Center;
                Vector2 targetPosition = targetPlayer.Center;
                Vector2 direction = targetPosition - position;
                float targetAngle = position.AngleTo(targetPosition);

				for (int i = 0; i < projectileCount; ++i)
				{
					float angleScale = (float)i / projectileCount - 0.5f;
					float projectileAngle = targetAngle + maxSpreadRadians * angleScale;
					direction = new Vector2(MathF.Cos(projectileAngle), MathF.Sin(projectileAngle));
					direction.Normalize();
					float speed = 14f;
					int type = ProjectileID.PinkLaser; //TODO: change it to something watery
					int damage = targetDamage_supremeWaterBolt_contact;
					Projectile.NewProjectile(source, position, direction * speed, type, damage, 0f, Main.myPlayer);
				}
            }         
		}

		protected void waterBoltWall(int projectileCount, int maxPixelSeparation, int delayTicks)
		{
            if (currentAttackTickCounter == delayTicks && NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
            {
                var source = NPC.GetSource_FromAI();
                Vector2 position = NPC.Center;
                Vector2 targetPosition = targetPlayer.Center;
                Vector2 direction = targetPosition - position;
				Vector2 perpendicular = new(direction.Y, -direction.X);
				perpendicular.Normalize();

				for (int i = 0; i < projectileCount; ++i)
				{
					Vector2 direction2 = direction + perpendicular * ((float)i / projectileCount - 0.5f) * maxPixelSeparation;
					direction2.Normalize();
					float speed = 14f;
					int type = ProjectileID.PinkLaser; //TODO: change it to something watery
					int damage = targetDamage_supremeWaterBolt_contact;
					Projectile.NewProjectile(source, position, direction2 * speed, type, damage, 0f, Main.myPlayer);
				}
            }
        }
    }

	
}