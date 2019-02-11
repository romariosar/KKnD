#region Copyright & License Information
/*
 * Copyright 2016-2019 The KKnD Developers (see AUTHORS)
 * This file is part of KKnD, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System;
using System.Drawing;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Kknd.Traits.Render;
using OpenRA.Mods.Kknd.Traits.Research;
using OpenRA.Mods.Kknd.Traits.Veterancy;
using OpenRA.Traits;

namespace OpenRA.Mods.Kknd.Graphics
{
	public class StatusBar : IRenderable, IFinalizedRenderable
	{
		private Actor actor;
		private AdvancedSelectionDecorationsInfo info;

		private Health health;
		//private SaboteurConquerable saboteurs;
		//private SaboteurConquerableInfo saboteursInfo;
		//private IHaveOil oil;
		private Researchable researchable;
		private ResearchableInfo researchableInfo;
		private TechLevel techLevel;
		private Veterancy veteran;
		private VeterancyInfo veteranInfo;

		public StatusBar(Actor actor, AdvancedSelectionDecorationsInfo info)
		{
			this.actor = actor;
			this.info = info;

			var isAlly = actor.Owner.IsAlliedWith(actor.World.LocalPlayer);

			health = actor.TraitOrDefault<Health>();
			//saboteurs = isAlly ? actor.TraitOrDefault<SaboteurConquerable>() : null;
			//saboteursInfo = actor.Info.TraitInfoOrDefault<SaboteurConquerableInfo>();
			//oil = actor.TraitOrDefault<IHaveOil>();
			researchable = isAlly ? actor.TraitOrDefault<Researchable>() : null;
			researchableInfo = actor.Info.TraitInfoOrDefault<ResearchableInfo>();
			techLevel = actor.World.WorldActor.Trait<TechLevel>();
			veteran = actor.TraitOrDefault<Veterancy>();
			veteranInfo = actor.Info.TraitInfoOrDefault<VeterancyInfo>();
		}

		public WPos Pos { get { return WPos.Zero; } }
		public PaletteReference Palette { get { return null; } }
		public int ZOffset { get { return 0; } }
		public bool IsDecoration { get { return true; } }

		public IRenderable WithPalette(PaletteReference newPalette) { return this; }
		public IRenderable WithZOffset(int newOffset) { return this; }
		public IRenderable OffsetBy(WVec offset) { return this; }
		public IRenderable AsDecoration() { return this; }
		public IFinalizedRenderable PrepareRender(WorldRenderer wr) { return this; }

		public void Render(WorldRenderer wr)
		{
			if (health == null && /*saboteurs == null && oil == null && */researchable == null)
				return;

			var bounds = actor.TraitsImplementing<IDecorationBounds>().FirstNonEmptyBounds(actor, wr);

			var thickness = info.BigVariant ? 4 : 3;
			var height = (health != null ? thickness : 0) + /*(saboteurs != null ? thickness : 0) + */(researchable != null ? thickness : 0) /*+ (oil != null ? thickness : 0) */- 1;
			var width = info.Width == 0 ? bounds.Width : info.Width;

			DrawRect(bounds, 0, -height - 4, width, height + 4, veteran != null && veteran.Level > 0 ? veteranInfo.Levels[veteran.Level - 1] : Color.FromArgb(255, 206, 206, 206));
			DrawRect(bounds, 1, -height - 3, width - 2, height + 2, Color.FromArgb(255, 16, 16, 16));

			var current = 0;

			if (health != null)
			{
				var progress = (width - 4) * health.HP / health.MaxHP;

				DrawRect(bounds, 2, -height - 2 + current * thickness, width - 4, 1, Color.FromArgb(255, 206, 206, 206));
				DrawRect(bounds, 2, -height - 1 + current * thickness, width - 4, thickness - 2, Color.FromArgb(255, 49, 49, 49));

				switch (health.DamageState)
				{
					case DamageState.Undamaged:
						DrawRect(bounds, 2, -height - 2 + current * thickness, progress, 1, Color.FromArgb(255, 0, 255, 0));
						DrawRect(bounds, 2, -height - 1 + current * thickness, progress, thickness - 2, Color.FromArgb(255, 0, 181, 0));
						break;

					case DamageState.Light:
						DrawRect(bounds, 2, -height - 2 + current * thickness, progress, 1, Color.FromArgb(255, 255, 255, 0));
						DrawRect(bounds, 2, -height - 1 + current * thickness, progress, thickness - 2, Color.FromArgb(255, 141, 184, 28));
						break;

					case DamageState.Medium:
						DrawRect(bounds, 2, -height - 2 + current * thickness, progress, 1, Color.FromArgb(255, 255, 156, 0));
						DrawRect(bounds, 2, -height - 1 + current * thickness, progress, thickness - 2, Color.FromArgb(255, 178, 122, 51));
						break;

					case DamageState.Heavy:
						DrawRect(bounds, 2, -height - 2 + current * thickness, progress, 1, Color.FromArgb(255, 230, 0, 0));
						DrawRect(bounds, 2, -height - 1 + current * thickness, progress, thickness - 2, Color.FromArgb(255, 123, 0, 0));
						break;

					case DamageState.Critical:
						DrawRect(bounds, 2, -height - 2 + current * thickness, progress, 1, Color.FromArgb(255, 123, 0, 0));
						DrawRect(bounds, 2, -height - 1 + current * thickness, progress, thickness - 2, Color.FromArgb(255, 82, 0, 0));
						break;
				}

				current++;
			}

			/*if (saboteurs != null)
			{
				var progress = (width - 4) * saboteurs.Population / saboteursInfo.MaxPopulation;

				DrawRect(bounds, 2, -height - 2 + current * thickness, width - 4, 1, Color.FromArgb(255, 206, 206, 206));
				DrawRect(bounds, 2, -height - 1 + current * thickness, width - 4, thickness - 2, Color.FromArgb(255, 49, 49, 49));

				DrawRect(bounds, 2, -height - 2 + current * thickness, progress, 1, Color.FromArgb(255, 230, 0, 0));
				DrawRect(bounds, 2, -height - 1 + current * thickness, progress, thickness - 2, Color.FromArgb(255, 123, 0, 0));

				for (var i = 1; i < saboteursInfo.MaxPopulation; i++)
					DrawRect(bounds, 2 + (width - 4) * i / saboteursInfo.MaxPopulation, -height - 2 + current * thickness, 1, thickness - 1, Color.FromArgb(255, 16, 16, 16));

				current++;
			}*/

			if (researchable != null)
			{
				var progress = (width - 4) * researchable.Level / researchableInfo.MaxLevel;
				var unavailable = (width - 4) * Math.Max(0, researchableInfo.MaxLevel - techLevel.TechLevels) / researchableInfo.MaxLevel;

				DrawRect(bounds, 2, -height - 2 + current * thickness, width - 4, 1, Color.FromArgb(255, 206, 206, 206));
				DrawRect(bounds, 2, -height - 1 + current * thickness, width - 4, thickness - 2, Color.FromArgb(255, 49, 49, 49));

				DrawRect(bounds, 2, -height - 2 + current * thickness, progress, 1, Color.FromArgb(255, 0, 165, 255));
				DrawRect(bounds, 2, -height - 1 + current * thickness, progress, thickness - 2, Color.FromArgb(255, 0, 66, 255));

				DrawRect(bounds, width - 2 - unavailable, -height - 2 + current * thickness, unavailable, thickness, Color.FromArgb(255, 16, 16, 16));

				for (var i = 1; i < researchableInfo.MaxLevel; i++)
					DrawRect(bounds, 2 + (width - 4) * i / researchableInfo.MaxLevel, -height - 2 + current * thickness, 1, thickness - 1, Color.FromArgb(255, 16, 16, 16));

				current++;
			}

			/*if (oil != null)
			{
				var progress = (width - 4) * oil.Current / oil.Maximum;

				DrawRect(bounds, 2, -height - 2 + current * thickness, width - 4, 1, Color.FromArgb(255, 206, 206, 206));
				DrawRect(bounds, 2, -height - 1 + current * thickness, width - 4, thickness - 2, Color.FromArgb(255, 49, 49, 49));

				DrawRect(bounds, 2, -height - 2 + current * thickness, progress, 1, Color.FromArgb(255, 0, 165, 255));
				DrawRect(bounds, 2, -height - 1 + current * thickness, progress, thickness - 2, Color.FromArgb(255, 0, 66, 255));
			}*/
		}

		private void DrawRect(Rectangle bounds, int x, int y, int w, int h, Color c)
		{
			var width = info.Width == 0 ? bounds.Width : info.Width;
			var center = (bounds.Width - width) / 2;
			Game.Renderer.WorldRgbaColorRenderer.FillRect(
				new float3(bounds.X + x + info.Offset.X + center, bounds.Y + y + info.Offset.Y, 0),
				new float3(bounds.X + x + info.Offset.X + center + w, bounds.Y + y + info.Offset.Y + h, 0),
				c);
		}

		public void RenderDebugGeometry(WorldRenderer wr) { }

		public Rectangle ScreenBounds(WorldRenderer wr) { return Rectangle.Empty; }
	}
}
