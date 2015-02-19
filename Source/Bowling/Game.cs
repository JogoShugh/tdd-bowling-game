using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bowling
{
    public class Game
    {
        private List<Frame> _frames = new List<Frame>();
        /// <summary>
        /// Mehod that implements a bowling ball roll
        /// </summary>
        /// <param name="pins"></param>
        public void Roll(int pins)
        {
            //if more than 10 frames have been rolled then skip rolling of the ball; the lane is closed
            if (_frames.Count == 10 && _frames.Last().Open == false)
            {
                return;
            }

            // if no frames have been added then intitate a new list and add a frame to it
            if (_frames.Count == 0)
            {
                var frame = new Frame(1, pins);
                _frames.Add(frame);
            }
            else //for all other rolls
            {
                //get last rolled frame
                var lastFrame = _frames.Last();

                //if last frame is still open; aka not a strike
                if (!lastFrame.Open)
                {
                    Frame newFrame;

                    //if this is the last frame then special rules apply
                    newFrame = _frames.Count == 9 ? new TenthFrame(_frames.Count + 1, pins) : new Frame(_frames.Count + 1, pins);
                    _frames.Add(newFrame);
                }
                else //if the last roll is still open; aka first roll in frame was not a strike, then add the value of pins to the 2nd roll
                {
                    lastFrame.Roll(pins);
                }
            }
        }

        /// <summary>
        /// Method to calculate the final score of the game
        /// </summary>
        /// <returns></returns>
        public int Score()
        {
            return _frames.Sum(
                frame =>
                {
                    var frameScore = frame.Score();

                    if (frame.IsSpare)
                    {
                        frameScore += AddSpareBonus(frame);
                    }
                    else if (frame.IsStrike)
                    {
                        frameScore += AddStrikeBonus(frame);
                    }

                    return frameScore;
                }
            );
        }

        /// <summary>
        /// Add a bonus on a spare for the first ball of the very next frame, if it exists.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        private int AddSpareBonus(Frame frame)
        {
            var nextFrame = _frames.Find(fr => fr.FrameIndex == frame.FrameIndex + 1);
            return nextFrame != null && nextFrame.FirstRoll.HasValue ? nextFrame.FirstRoll.Value : 0;
        }

        /// <summary>
        /// Calculates a bonus score for a strike based on the next two rolled balls. These rolls
        /// could come from the very next frame, if it's not a strike. But, if the next rolled ball
        /// is a strike, then it's possible this player is about to get a turkey, or 
        /// three-strikes-in-a-row, so in that case, we check that hopeful turkey frame
        /// for its first roll's pin total.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        private int AddStrikeBonus(Frame frame)
        {
            var nextFrame = _frames.Find(fr => fr.FrameIndex == frame.FrameIndex + 1);
            
            if (nextFrame == null) return 0;
            
            var strikeBonus = nextFrame.FirstRoll.HasValue ? nextFrame.FirstRoll.Value : 0;
            if (nextFrame.SecondRoll.HasValue) {
                strikeBonus += nextFrame.SecondRoll.Value;
            } else {
                var hopefulTurkeyFrame = _frames.Find(fr => fr.FrameIndex == frame.FrameIndex + 2);
                strikeBonus += hopefulTurkeyFrame != null && hopefulTurkeyFrame.FirstRoll.HasValue ? 
                    hopefulTurkeyFrame.FirstRoll.Value : 0;
            }

            return strikeBonus;
        }
    }

    public class Frame
    {
        public Frame(int frameIndex, int firstRoll)
        {
            FirstRoll = firstRoll;
            FrameIndex = frameIndex;
        }

        public int FrameIndex { get; set; }

        public int? FirstRoll { get; set; }
        public int? SecondRoll { get; set; }

        public virtual bool Open
        {
            get
            {
                return !FirstRoll.HasValue || (
                    !SecondRoll.HasValue && !IsStrike);
            }
        }

        public virtual int Score()
        {
            return FirstRoll.Value + (SecondRoll.HasValue ? SecondRoll.Value : 0);
        }

        public virtual void Roll(int pins)
        {
            SecondRoll = pins;
        }

        public bool IsStrike
        {
            get
            {
                return FirstRoll == 10;
            }
        }

        public bool IsSpare
        {
            get
            {
                return !IsStrike && FirstRoll + SecondRoll == 10;
            }
        }
    }

    public class TenthFrame : Frame
    {
        public TenthFrame(int frameIndex, int pins) : base(frameIndex, pins) { }

        public int? ThirdRoll { get; set; }

        public override bool Open
        {
            get
            {
                return (
                    ( (IsSpare || IsStrike) && !ThirdRoll.HasValue )
                    ||
                    !SecondRoll.HasValue
                );
            }
        }

        public override int Score()
        {
            return FirstRoll.Value + (SecondRoll.HasValue ? SecondRoll.Value : 0)
                + (ThirdRoll.HasValue ? ThirdRoll.Value : 0);
        }

        public override void Roll(int pins)
        {
            if (!SecondRoll.HasValue) SecondRoll = pins;
            else ThirdRoll = pins;
        }
    }
}