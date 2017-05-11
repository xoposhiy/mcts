using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MctsLib;
using MctsLib.Gomoku;

namespace GomokuApp
{
	public partial class GameForm : Form
	{
		private readonly Mcts<GomokuGame> ai = new Mcts<GomokuGame>
		{
			MaxTime = TimeSpan.FromSeconds(3),
			MaxSimulationsCount = int.MaxValue,
			ExplorationConstant = 0.5
		};

		private GomokuGame game = new GomokuGame();
		private (int x, int y) hoveredCell = (0, 0);
		private Dictionary<(int x, int y), Node<GomokuGame>> stats;

		public GameForm()
		{
			DoubleBuffered = true;
			InitializeComponent();
		}

		private void GameForm_Paint(object sender, PaintEventArgs e)
		{
			var w = ClientRectangle.Width;
			var cw = w / game.Size;
			var h = ClientRectangle.Height;
			var ch = h / game.Size;
			var xPen = new Pen(Color.Blue, 3);
			var oPen = new Pen(Color.DarkRed, 3);
			for (var x = 0; x < game.Size; x++)
			for (var y = 0; y < game.Size; y++)
				RenderCell(e, x, y, xPen, cw, ch, oPen);

			for (var x = 1; x < game.Size; x++)
				e.Graphics.DrawLine(Pens.Black, x * cw, 0, x * cw, h);
			for (var y = 1; y < game.Size; y++)
				e.Graphics.DrawLine(Pens.Black, 0, y * ch, w, y * ch);
			e.Graphics.DrawRectangle(new Pen(Color.Blue, 2), hoveredCell.x * cw, hoveredCell.y * ch, cw, ch);
		}

		private void RenderCell(PaintEventArgs e, int x, int y, Pen xPen, int cw, int ch, Pen oPen)
		{
			if (stats != null && stats.TryGetValue((x, y), out var node))
			{
				var score = node.GetExpectedScore(1);
				var color = GetColor(score);
				e.Graphics.FillRectangle(new SolidBrush(color), x * cw, y * ch, cw, ch);
				e.Graphics.DrawString(node.TotalPlays.ToString(), SystemFonts.DialogFont, Brushes.Black, x * cw, y * ch);
			}
			if (game[x, y] == 1)
			{
				e.Graphics.DrawLine(xPen, (x + 0.2f) * cw, (y + 0.2f) * ch, (x + 0.8f) * cw, (y + 0.8f) * ch);
				e.Graphics.DrawLine(xPen, (x + 0.8f) * cw, (y + 0.2f) * ch, (x + 0.2f) * cw, (y + 0.8f) * ch);
			}
			else if (game[x, y] == 2)
			{
				e.Graphics.DrawEllipse(oPen, (x + 0.2f) * cw, (y + 0.2f) * ch, cw * 0.6f, ch * 0.6f);
			}
		}

		private Color GetColor(double score)
		{
			if (score <= 0.5)
			{
				var other = (int) (510 * score);
				return Color.FromArgb(255, 255, other, other);
			}
			else
			{
				var other = 255 - (int) (510 * (score - 0.5));
				return Color.FromArgb(255, other, 255, other);
			}
		}

		private void GameForm_MouseMove(object sender, MouseEventArgs e)
		{
			hoveredCell = GetCell(e.Location);
			Invalidate();
		}

		private (int x, int y) GetCell(Point location)
		{
			var w = ClientRectangle.Width;
			var cw = w / game.Size;
			var h = ClientRectangle.Height;
			var ch = h / game.Size;
			return (location.X / cw, location.Y / ch);
		}


		private void GameForm_MouseUp(object sender, MouseEventArgs e)
		{
			var (x, y) = GetCell(e.Location);
			if (!game.GetPossibleMoves().Contains(new GomokuMove(x, y))) return;
			game.MakeMove(x, y);
			Invalidate();
			Application.DoEvents();
			if (!game.IsFinished)
			{
				var root = ai.BuildGameTree(game);
				stats = root.GetChildren()
					.ToDictionary(
						c => ((GomokuMove) c.Move).ToCoord(),
						c => c);
				var move = ai.GetBestMove(game, root);
				move.ApplyTo(game);
				Invalidate();
			}
			if (game.IsFinished)
			{
				MessageBox.Show(string.Join(":", game.GetScores().Select(s => s.ToString("0.#"))), "Finished!");
				game = new GomokuGame();
				stats = null;
				Invalidate();
			}
		}
	}
}