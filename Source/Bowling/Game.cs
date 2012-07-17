using System.Collections.Generic;
using System.Linq;

namespace Bowling
{
	public class Game
	{
		private int _score;
		private List<Frame> _frames = new List<Frame>();

		public bool IsComplete
		{
			get { return _frames.Count == 10; }
		}

		public int Score()
		{
			return _score;
		}

		public void RollFrame(int numberOfPinsKnockedDownOnRollOne, int numberOfPinsKnockedDownOnRollTwo)
		{
			var frame = new Frame();
			frame.RollOne = numberOfPinsKnockedDownOnRollOne;
			frame.RollTwo = numberOfPinsKnockedDownOnRollTwo;
			_score += frame.Score();

			// calculate bonus pins if we have more than one frame completed
			if (_frames.Count > 0)
			{
				// last frame
				var frameOne = _frames[_frames.Count - 1];
				var frameTwo = frame;
				_score += CalculateBonusPins(frameOne, frameTwo);
			}

			_frames.Add(frame);
		}

		private int CalculateBonusPins(Frame frameOne, Frame frameTwo)
		{
			var bonus = 0;
			if (!frameOne.IsOpen)
			{
				bonus += frameTwo.RollOne;
			}

			if (frameOne.IsStrike)
			{
				bonus += frameTwo.RollTwo;
			}

			return bonus;
		}

	}

	internal class Frame
	{
		private int _score;

		public int RollOne { get; set; }

		public int RollTwo { get; set; }

		public bool IsOpen
		{
			get { return Score() < 10; }
		}

		public bool IsStrike
		{
			get { return RollOne == 10; }
		}

		public int Score()
		{
			return RollOne + RollTwo;
		}
	}
}