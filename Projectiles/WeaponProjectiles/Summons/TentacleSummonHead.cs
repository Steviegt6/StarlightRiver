﻿using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using StarlightRiver.NPCs;
using StarlightRiver.Core;
using StarlightRiver.Buffs.Summon;
using static Terraria.ModLoader.ModContent;

namespace StarlightRiver.Projectiles.WeaponProjectiles.Summons
{

	public class TentacleSummonHead : ModProjectile
	{

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tentacle");
			Main.projFrames[projectile.type] = 1;
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
			ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
			ProjectileID.Sets.Homing[projectile.type] = true;
		}

		public sealed override void SetDefaults()
		{
			projectile.width = 24;
			projectile.height = 24;
			projectile.tileCollide = false;
			projectile.friendly = false;
			projectile.hostile = false;
			projectile.minion = true;
			projectile.minionSlots = 1f;
			projectile.penetrate = -1;
			projectile.timeLeft = 60;

		}

		bool attacking = false;
		Vector2 control1;
        Vector2 control2;
		Vector2 posToBe = Vector2.Zero;
		float rotationCounter = 0f;
		float attackCounter = 0f;
		Vector2 rotationVector = Vector2.UnitX;
		int circleX = 0;
		int circleY = 0;
		float circleSpeed = 0.05f;
		public override void AI()
		{
			//if (projectile.owner == null || projectile.owner < 0)
			//return;
			if (circleX == 0)
			{
				circleSpeed = Main.rand.NextFloat(0.03f, 0.06f);
				circleX = Main.rand.Next(40, 80);
				circleY = Main.rand.Next(20, 40);
			}
			Player player = projectile.Owner();
			attackCounter = (Main.GameUpdateCount / 120f * 6.28f) + projectile.ai[0];
			rotationCounter += circleSpeed;
			if (player.dead || !player.active)
				player.ClearBuff(ModContent.BuffType<TentacleSummonBuff>());

			if (player.HasBuff(ModContent.BuffType<TentacleSummonBuff>()))
				projectile.timeLeft = 2;

			float maxRange = 10f;
			int range = 10;
			if (projectile.ai[1] == -1)//boilerplate, sorry
			{
				float lowestDist = float.MaxValue;
				for (int i = 0; i < 200; ++i)
				{
					NPC npc = Main.npc[i];
					if (npc.active && npc.CanBeChasedBy(projectile) && !npc.friendly)
					{
						float dist = projectile.Distance(npc.Center);
						if (dist / 16 < range)
						{
							if (dist < lowestDist)
							{
								lowestDist = dist;

								projectile.ai[1] = npc.whoAmI;
								projectile.netUpdate = true;
							}
						}
					}
				}
			}
			NPC target = projectile.OwnerMinionAttackTargetNPC;
			if (target != null && projectile.Distance(target.Center) < maxRange * 16)
			{
				projectile.ai[1] = target.whoAmI;
			}
			if (projectile.ai[1] != -1)
			{
				target = (Main.npc[(int)projectile.ai[1]] ?? new NPC());
				rotationVector = projectile.Center - target.Center;
			}
			projectile.rotation = rotationVector.ToRotation() - 1.57f;
			//Vector2 rotationVector = projectile.Center - target.Center;
			Vector2 circle = new Vector2(circleX * (float)Math.Sin((double)rotationCounter), circleY * (float)Math.Cos((double)rotationCounter));
			float speed = 0.5f;
			if (!attacking)
			{
				projectile.friendly = false;
				posToBe = Vector2.UnitY * -1;
				projectile.ai[1] = -1;
				int offset = 80;
				double angle;
				if (player.direction == 1)
				{
					angle = 1.3;
					control1 = (Vector2.UnitY * -120).RotatedBy(6.28 - 1.4) + player.Center+ (circle.RotatedBy(angle) / 2);
					control2 = (Vector2.UnitY * -80).RotatedBy(0.4) + player.Center + (circle.RotatedBy(angle) / 5);
				}
				else
				{
					angle = 6.28 - 1.3;
					control1 = (Vector2.UnitY * -120).RotatedBy(1.4) + player.Center + (circle.RotatedBy(angle) / 2);
					control2 = (Vector2.UnitY * -80).RotatedBy(6.28 - 0.4) + player.Center + (circle.RotatedBy(angle) / 5);
				}
				posToBe *= offset;
				posToBe += circle;
				posToBe = posToBe.RotatedBy(angle);
				posToBe.Y -= 30;
				posToBe += player.Center;
				if (attackCounter % 7 > 6.8)
				{
					attacking = true;

				}
			}
			else
			{
				projectile.friendly = true;
				if (projectile.ai[1] != -1)
				{
					//target = (Main.npc[(int)projectile.ai[1]] ?? new NPC());
					if (target.active && !target.friendly)
                    {
						posToBe = target.Center;
						speed = 2;
					}
					else
                    {
						attacking = false;
					}
				}
				else
                {
					attacking = false;
                }
			}

			Vector2 direction = posToBe - projectile.position;
			if (direction.Length() > 1000)
            {
				projectile.position = posToBe;
            }
			else
			{
				speed *= (float)Math.Sqrt(direction.Length());
				direction.Normalize();
				direction *= speed;

				projectile.velocity = direction;
			}
		}
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			attacking = false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Player player = projectile.Owner();
			Texture2D tex = mod.GetTexture("Projectiles/WeaponProjectiles/Summons/TentacleSummonTail2");
			float dist = (projectile.position - player.Center).Length();
			DrawBezier(spriteBatch, lightColor, tex, projectile.Center, player.Center, control1, control2, tex.Height / dist / 2, projectile.rotation);
			return true;
		}
		public static void DrawBezier(SpriteBatch spriteBatch, Color lightColor, Texture2D texture, Vector2 endpoint, Vector2 startPoint, Vector2 c1, Vector2 c2, float chainsPerUse, float rotDis = 0f)
		{
			float width = texture.Width;
			float length = (startPoint - endpoint).Length();
			for (float i = 0; i <= 1; i += chainsPerUse)
			{
				Vector2 distBetween;
				float projTrueRotation;
				if (i != 0)
				{
					float x = TentacleDraw.EX(i, startPoint.X, c1.X, c2.X, endpoint.X);
					float y = TentacleDraw.WHY(i, startPoint.Y, c1.Y, c2.Y, endpoint.Y);
					distBetween = new Vector2(x -
				   TentacleDraw.EX(i - chainsPerUse, startPoint.X, c1.X, endpoint.X),
				   y -
				   TentacleDraw.WHY(i - chainsPerUse, startPoint.Y, c1.Y, endpoint.Y));
					projTrueRotation = distBetween.ToRotation() - MathHelper.PiOver2 + rotDis;
					Main.spriteBatch.Draw(texture, new Vector2(x - Main.screenPosition.X, y - Main.screenPosition.Y),
				   new Rectangle(0, 0, texture.Width, texture.Height), lightColor, projTrueRotation,
				   new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), 1, SpriteEffects.None, 0);
				}
			}
		}
	}
	internal static class TentacleDraw 
	{ 
			#region os's shit
		public static float EX(float t,
		float x0, float x1, float x2, float x3)
		{
			return (float)(
				x0 * Math.Pow(1 - t, 3) +
				x1 * 3 * t * Math.Pow(1 - t, 2) +
				x2 * 3 * Math.Pow(t, 2) * (1 - t) +
				x3 * Math.Pow(t, 3)
			);
		}

		public static float WHY(float t,
			float y0, float y1, float y2, float y3)
		{
			return (float)(
				 y0 * Math.Pow(1 - t, 3) +
				 y1 * 3 * t * Math.Pow(1 - t, 2) +
				 y2 * 3 * Math.Pow(t, 2) * (1 - t) +
				 y3 * Math.Pow(t, 3)
			 );
		}
		public static float EX(float t,
   float x0, float x1, float x2)
		{
			return (float)(
				x0 * Math.Pow(1 - t, 2) +
				x1 * 2 * t * (1 - t) +
				x2 * Math.Pow(t, 2)
			);
		}

		public static float WHY(float t,
			float y0, float y1, float y2)
		{
			return (float)(
				y0 * Math.Pow(1 - t, 2) +
				y1 * 2 * t * (1 - t) +
				y2 * Math.Pow(t, 2)
			);
		}
		#endregion

	}
}


	