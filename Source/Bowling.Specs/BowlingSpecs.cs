using System;
using Bowling;
using Bowling.Specs.Infrastructure;
using Bowling.Specs;

/*
 * 
when rolling all gutter balls, the score is 0.
when the first frame is a spare and the rest score 2, the score is 48.
when the first 2 frames are spare and the rest score 2, the score is 56.
when 10 frames have been bowled, don't allow any more to be bowled.
when the first frame is a strike and the rest score 2, the score is 50.
when the first 2 frames are strikes and the rest score 2, the score is 68.
when rolling a perfect game, the score is 300.
when rolling alternate strikes and spares, the score is 200.
 * 
 */

namespace specs_for_bowling
{
	public class when_rolling_all_gutter_balls : concerns
	{
		private Game SUT;

		protected override void context()
		{
			SUT = new Game();
			20.times(() => SUT.Roll(0));
		}

		[Specification]
		public void score_is_zero()
		{
			SUT.Score().should_equal(0);
		}
	}

	public class when_first_frame_is_Spare_followed_by_all_2_rolls : concerns
	{
		private Game SUT;

		protected override void context()
		{
			SUT = new Game();
			SUT.Roll(7);
			SUT.Roll(3);
			18.times(() => SUT.Roll(2));
		}

		[Specification]
		public void score_is_48()
		{
			SUT.Score().should_equal(48);
		}
	}

	public class when_first_frame_is_Spare_followed_by_all_4_rolls : concerns
	{
		private Game SUT;

		protected override void context()
		{
			SUT = new Game();
			SUT.Roll(4);
			SUT.Roll(6);
			18.times(() => SUT.Roll(4));
		}

		[Specification]
		public void score_is_86()
		{
			SUT.Score().should_equal(86);
		}
	}

	public class when_first_two_frames_are_Spare_followed_by_all_2_rolls : concerns
	{
		private Game SUT;

		protected override void context()
		{
			SUT = new Game();
			SUT.Roll(4);
			SUT.Roll(6);
			SUT.Roll(2);
			SUT.Roll(8);
			16.times(() => SUT.Roll(2));
		}

		[Specification]
		public void score_is_56()
		{
			SUT.Score().should_equal(56);
		}
	}

	public class when_10_frames_are_bowled : concerns
	{
		private Game SUT;

		protected override void context()
		{
			SUT = new Game();
			20.times(() => SUT.Roll(0));
		}

		[Specification, MbUnit.Framework.ExpectedException(typeof(InvalidOperationException))]
		public void no_more_are_allowed()
		{
			SUT.Roll(0);
		}
	}

	public class when_the_first_frame_is_Strike_and_the_rest_score_2 : concerns
	{
		private Game SUT;

		protected override void context()
		{
			SUT = new Game();
			SUT.Roll(10);
			18.times(() => SUT.Roll(2));
		}

		[Specification]
		public void score_is_50()
		{
			SUT.Score().should_equal(50);
		}
	}
}