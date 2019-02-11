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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets;
using OpenRA.Primitives;
using OpenRA.Traits;
using OpenRA.Widgets;

namespace OpenRA.Mods.Kknd.Widgets.Ingame
{
	public class RadarWidget : OpenRA.Widgets.Widget
	{
		private readonly IngameUiWidget ingameUi;

		private readonly Sheet radarSheet;
		private readonly byte[] radarData;
		private readonly Sprite terrainSprite;
		private readonly Sprite shroudSprite;

		private int size = 2;
		private bool useStanceColor = false;

		public Stance ShowStances { get; set; }

		public RadarWidget(IngameUiWidget ingameUi)
		{
			this.ingameUi = ingameUi;

			radarSheet = new Sheet(SheetType.BGRA, new Size(ingameUi.World.Map.MapSize.X, ingameUi.World.Map.MapSize.Y * 2));
			radarSheet.CreateBuffer();
			radarData = radarSheet.GetData();

			terrainSprite = new Sprite(radarSheet, new Rectangle(0, 0, ingameUi.World.Map.MapSize.X, ingameUi.World.Map.MapSize.Y), TextureChannel.RGBA);
			shroudSprite = new Sprite(radarSheet, new Rectangle(0, ingameUi.World.Map.MapSize.Y, ingameUi.World.Map.MapSize.X, ingameUi.World.Map.MapSize.Y), TextureChannel.RGBA);

			DrawTerrain();
			Resize();
			Visible = false;
		}

		public override bool HandleKeyPress(KeyInput e)
		{
			if (e.Key != Game.ModData.Hotkeys["TogglePlayerStanceColor"].GetValue().Key || e.IsRepeat || e.Event != KeyInputEvent.Down)
				return false;

			useStanceColor = !useStanceColor;

			return true;
		}

		public void Resize()
		{
			Bounds = new Rectangle(
				0,
				Game.Renderer.Resolution.Height - ingameUi.World.Map.MapSize.Y * size,
				ingameUi.World.Map.MapSize.X * size,
				ingameUi.World.Map.MapSize.Y * size);
		}

		private void DrawTerrain()
		{
			// TODO instead of using this colors, try a correct thumbnail variant.
			for (var y = 0; y < ingameUi.World.Map.MapSize.Y; y++)
			{
				for (var x = 0; x < ingameUi.World.Map.MapSize.X; x++)
				{
					var type = ingameUi.World.Map.Rules.TileSet.GetTileInfo(ingameUi.World.Map.Tiles[new MPos(x, y)]);
					radarData[(y * radarSheet.Size.Width + x) * 4] = type.LeftColor.B;
					radarData[(y * radarSheet.Size.Width + x) * 4 + 1] = type.LeftColor.G;
					radarData[(y * radarSheet.Size.Width + x) * 4 + 2] = type.LeftColor.R;
					radarData[(y * radarSheet.Size.Width + x) * 4 + 3] = 0xff;
				}
			}
		}

		private void UpdateShroud()
		{
			var rp = ingameUi.World.RenderPlayer;

			for (var y = 0; y < ingameUi.World.Map.MapSize.Y; y++)
			{
				for (var x = 0; x < ingameUi.World.Map.MapSize.X; x++)
				{
					var color = Color.FromArgb(0, Color.Black);

					if (rp != null)
					{
						var pos = new MPos(x, y);
						if (!rp.Shroud.IsExplored(pos))
							color = Color.FromArgb(255, Color.Black);
						else if (!rp.Shroud.IsVisible(pos))
							color = Color.FromArgb(128, Color.Black);
					}

					radarData[radarSheet.Size.Width * ingameUi.World.Map.MapSize.Y * 4 + (y * radarSheet.Size.Width + x) * 4] = color.B;
					radarData[radarSheet.Size.Width * ingameUi.World.Map.MapSize.Y * 4 + (y * radarSheet.Size.Width + x) * 4 + 1] = color.G;
					radarData[radarSheet.Size.Width * ingameUi.World.Map.MapSize.Y * 4 + (y * radarSheet.Size.Width + x) * 4 + 2] = color.R;
					radarData[radarSheet.Size.Width * ingameUi.World.Map.MapSize.Y * 4 + (y * radarSheet.Size.Width + x) * 4 + 3] = color.A;
				}
			}
		}

		public override string GetCursor(int2 pos)
		{
			var cell = new MPos((pos.X - RenderBounds.X) / size, (pos.Y - RenderBounds.Y) / size).ToCPos(ingameUi.World.Map);
			var worldPixel = ingameUi.WorldRenderer.ScreenPxPosition(ingameUi.World.Map.CenterOfCell(cell));
			var location = ingameUi.WorldRenderer.Viewport.WorldToViewPx(worldPixel);

			var mi = new MouseInput
			{
				Location = location,
				Button = Game.Settings.Game.MouseButtonPreference.Action,
				Modifiers = Game.GetModifierKeys()
			};

			var cursor = ingameUi.World.OrderGenerator.GetCursor(ingameUi.World, cell, worldPixel, mi);
			return cursor ?? "default";
		}

		public override bool HandleMouseInput(MouseInput mi)
		{
			var cell = new MPos((mi.Location.X - RenderBounds.X) / size, (mi.Location.Y - RenderBounds.Y) / size).ToCPos(ingameUi.World.Map);
			var pos = ingameUi.World.Map.CenterOfCell(cell);

			if ((mi.Event == MouseInputEvent.Down || mi.Event == MouseInputEvent.Move) && mi.Button == Game.Settings.Game.MouseButtonPreference.Cancel)
				ingameUi.WorldRenderer.Viewport.Center(pos);

			if (mi.Event == MouseInputEvent.Down && mi.Button == Game.Settings.Game.MouseButtonPreference.Action)
			{
				var location = ingameUi.WorldRenderer.Viewport.WorldToViewPx(ingameUi.WorldRenderer.ScreenPxPosition(pos));
				var fakemi = new MouseInput
				{
					Event = MouseInputEvent.Down,
					Button = Game.Settings.Game.MouseButtonPreference.Action,
					Modifiers = mi.Modifiers,
					Location = location
				};

				var controller = Ui.Root.Get<WorldInteractionControllerWidget>("INTERACTION_CONTROLLER");
				controller.HandleMouseInput(fakemi);
				fakemi.Event = MouseInputEvent.Up;
				controller.HandleMouseInput(fakemi);
			}

			return true;
		}

		public override void Draw()
		{
			UpdateShroud();

			WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X - size, RenderBounds.Y - size, RenderBounds.Width + size * 2, RenderBounds.Height + size * 2), Color.White);

			radarSheet.CommitBufferedData();
			Game.Renderer.RgbaSpriteRenderer.DrawSprite(terrainSprite, new int2(RenderBounds.X, RenderBounds.Y), new int2(RenderBounds.Width, RenderBounds.Height));
			Game.Renderer.RgbaSpriteRenderer.DrawSprite(shroudSprite, new int2(RenderBounds.X, RenderBounds.Y), new int2(RenderBounds.Width, RenderBounds.Height));

			var cells = new List<Pair<CPos, Color>>();

			foreach (var e in ingameUi.World.ActorsWithTrait<IRadarSignature>())
			{
				if (!e.Actor.IsInWorld || e.Actor.IsDead || ingameUi.World.FogObscures(e.Actor) || e.Actor.Owner == null)
					continue;

				if (!ShowStances.HasStance(Stance.Ally) && e.Actor.Owner.Stances[ingameUi.World.LocalPlayer].HasStance(Stance.Ally))
					continue;

				if (!ShowStances.HasStance(Stance.Enemy) && e.Actor.Owner.Stances[ingameUi.World.LocalPlayer].HasStance(Stance.Enemy))
					continue;

				cells.Clear();
				e.Trait.PopulateRadarSignatureCells(e.Actor, cells);

				foreach (var cell in cells)
				{
					if (!ingameUi.World.Map.Contains(cell.First))
						continue;

					var pos = cell.First.ToMPos(ingameUi.World.Map.Grid.Type);
					var color = useStanceColor ? Color.FromArgb(e.Actor.Owner.PlayerStanceColor(e.Actor).ToArgb()) : e.Actor.Owner.Color.RGB;

					WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X + pos.U * size, RenderBounds.Y + pos.V * size, size, size), color);
				}
			}

			Game.Renderer.EnableScissor(RenderBounds);

			Game.Renderer.RgbaColorRenderer.DrawRect(
				new int2(RenderBounds.X, RenderBounds.Y) + ingameUi.WorldRenderer.Viewport.TopLeft / 32 * size,
				new int2(RenderBounds.X, RenderBounds.Y) + ingameUi.WorldRenderer.Viewport.BottomRight / 32 * size, size,
				Color.White);

			foreach (var ping in ingameUi.RadarPings.Pings)
			{
				if (!ping.IsVisible())
					continue;

				var center = ingameUi.World.Map.CellContaining(ping.Position).ToMPos(ingameUi.World.Map.Grid.Type);
				var points = ping.Points(new int2(RenderBounds.X + center.U * size, RenderBounds.Y + center.V * size)).ToArray();
				Game.Renderer.RgbaColorRenderer.DrawPolygon(points, 2, ping.Color);
			}

			Game.Renderer.DisableScissor();
		}
	}
}
