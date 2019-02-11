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
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common;
using OpenRA.Mods.Common.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Traits.Render;
using OpenRA.Mods.Common.Widgets;
using OpenRA.Primitives;
using OpenRA.Widgets;

namespace OpenRA.Mods.Kknd.Widgets.Ingame.Buttons
{
	public class ProductionItemButtonWidget : ButtonWidget
	{
		public string Item;
		public string Icon;
		public Action<MouseInput> ClickedLeft;
		public Action<MouseInput> ClickedRight;
		public Func<bool> IsActive;
		public Func<bool> IsFocused;
		public Func<int> Progress;
		public Func<int> Amount;

		private ActorPreviewWidget actorPreviewWidget;
		private bool isHovered;
		private bool initialized;
		private Sprite image;

		public ProductionItemButtonWidget(SidebarWidget sidebar) : base(sidebar, "unit") { }

		protected override bool HandleLeftClick(MouseInput mi)
		{
			ClickedLeft(mi);
			return true;
		}

		protected override bool HandleRightClick(MouseInput mi)
		{
			if (ClickedRight != null)
				ClickedRight(mi);

			return true;
		}

		public override void MouseEntered()
		{
			base.MouseEntered();
			isHovered = true;
		}

		public override void MouseExited()
		{
			base.MouseExited();
			isHovered = false;
		}

		public override void Tick()
		{
			Active = IsActive();

			if (actorPreviewWidget != null && isHovered)
			{
				actorPreviewWidget.Tick();
			}
		}

		public override void Draw()
		{
			base.Draw();

			if (Progress != null)
			{
				var progress = Progress();
				if (progress != -1)
				{
					progress = progress * (Size - 10) / 100;
					var o = Size - 10 - progress;
					WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X + 2, RenderBounds.Y + 4, 7, Size - 6), Color.Black);
					WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X + 3, RenderBounds.Y + 5, 5, Size - 8), sidebar.IngameUi.Palette.Palette.GetColor(10));
					WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X + 4, RenderBounds.Y + 6, 3, Size - 10), sidebar.IngameUi.Palette.Palette.GetColor(8));
					WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X + 4, RenderBounds.Y + 6 + o, 3, progress), sidebar.IngameUi.Palette.Palette.GetColor(12));

					var amount = Amount();
					if (amount == -1)
					{
						sidebar.Font.PlayFetchIndex("production", () => 10);
						WidgetUtils.DrawSHPCentered(sidebar.Font.Image, new int2(RenderBounds.X + 14 + 4, RenderBounds.Y + 40), sidebar.IngameUi.Palette);
					}
					else if (amount > 1)
					{
						var numberString = amount.ToString();
						for (var i = 0; i < numberString.Length; i++)
						{
							sidebar.Font.PlayFetchIndex("production", () => numberString[i] - 0x30);
							WidgetUtils.DrawSHPCentered(sidebar.Font.Image, new int2(RenderBounds.X + 14 + i * 8, RenderBounds.Y + 40), sidebar.IngameUi.Palette);
						}
					}
				}
			}
		}

		public override void PrepareRenderables()
		{
			if (!initialized)
			{
				initialized = true;

				var actor = new Animation(sidebar.IngameUi.World, Icon);

				if (actor.HasSequence("icon"))
				{
					actor.PlayFetchIndex("icon", () => 0);
					image = actor.Image;
					return;
				}

				if (actorPreviewWidget == null)
				{
					actorPreviewWidget = new ActorPreviewWidget(sidebar.IngameUi.WorldRenderer)
					{
						Animate = true
					};
					actorPreviewWidget.SetPreview(sidebar.IngameUi.World.Map.Rules.Actors[Item], new TypeDictionary
					{
						new FacingInit(96),
						new TurretFacingInit(96),
						new OwnerInit(sidebar.IngameUi.World.LocalPlayer),
						new FactionInit(sidebar.IngameUi.World.LocalPlayer.Faction.Name)
					});

					/*
					// TODO implement per actor offsets
					// TODO implement per ui inner regions
					// TODO fix turret palettes!
					*/

					var factorX = sidebar.ButtonArea.Width / (float)actorPreviewWidget.IdealPreviewSize.X;
					var factorY = sidebar.ButtonArea.Height / (float)actorPreviewWidget.IdealPreviewSize.Y;

					if (factorX <= 1 && factorY <= 1)
					{
						var factor = Math.Max(factorX, factorY);
						actorPreviewWidget.GetScale = () => factor;
					}
				}
			}

			if (actorPreviewWidget != null)
				actorPreviewWidget.PrepareRenderables();
		}

		protected override void DrawContents()
		{
			if (IsFocused())
				WidgetUtils.FillRectWithColor(RenderBounds, Color.FromArgb(25, 255, 255, 255));

			if (image != null)
				WidgetUtils.DrawSHPCentered(image, center + new int2(0, Active ? 1 : 0), sidebar.IngameUi.Palette);
			else
			{
				actorPreviewWidget.Bounds = RenderBounds;

				Game.Renderer.EnableScissor(new Rectangle(
					RenderBounds.X + sidebar.ButtonArea.X,
					RenderBounds.Y + sidebar.ButtonArea.Y,
					sidebar.ButtonArea.Width,
					sidebar.ButtonArea.Height));

				actorPreviewWidget.Draw();
				Game.Renderer.DisableScissor();
			}
		}
	}
}
