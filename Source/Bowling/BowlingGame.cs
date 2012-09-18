using System.Collections.Generic;
using System.Linq;

namespace Bowling
{
	public class BowlingGame
	{
		private IList<Frame> frames = new List<Frame>();
		public void Roll(int pins)
		{
			GetCurrentFrame().AddRoll(pins);
		}

		private Frame GetCurrentFrame()
		{
			var frame = frames.LastOrDefault();
			
			if (frame== null || frame.IsComplete)
			{
				frame = new Frame();
				frames.Add(frame);
			}
			return frame;
		}


		public int CalculateScore()
		{
			var score = 0;
			for (int i = 0; i < 10; i++)
			{
				Frame f = frames[i];
				score += f.CalculateScore(frames.ElementAtOrDefault(i+1), frames.ElementAtOrDefault(i+2));
			}
			return score;
		}
	}

	internal class Frame
	{
		private List<int> rolls = new List<int>();
		public void AddRoll(int pins)
		{

			rolls.Add(pins);
		}

		public int FirstRoll {
			get { return rolls[0]; }
		}

		public int Pins {
			get { return rolls.Sum(); }
		}

		public bool IsSpare
		{
			get { return Pins == 10 && rolls.Count == 2; }
		}
		public bool IsStrike
		{
			get { return Pins == 10 && rolls.Count == 1; }
		}

		public bool IsComplete
		{
			get { return Pins == 10 || rolls.Count == 2; }
		}

		public int CalculateScore(Frame nextFrame, Frame furtherFrame)
		{
			int score = Pins;
			if (IsSpare)
			{
				score += nextFrame.FirstRoll;
			}
			else if (IsStrike)
			{
				score += nextFrame.Pins;
				if (nextFrame.IsStrike)
				{
					score += furtherFrame.FirstRoll;
				}
			}
			return score;
		}
	}
}