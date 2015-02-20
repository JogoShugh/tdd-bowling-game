using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bowling
{
    public class Game
    {
        private List<Frame> _frames = new List<Frame>();

        public void Roll(int pins)
        {
            if (_frames.Count == 10 && _frames.Last().Open == false)
            {
                return;
            }

            if (_frames.Count == 0)
            {
                var frame = new Frame(1, pins, _frames);
                _frames.Add(frame);
            }
            else
            {
                var lastFrame = _frames.Last();

                if (!lastFrame.Open)
                {
                    AddNewFrame(pins);
                }
                else
                {
                    lastFrame.Roll(pins);
                }
            }
        }

        private void AddNewFrame(int pins)
        {
            Frame newFrame;

            newFrame = _frames.Count == 9 ? new TenthFrame(_frames.Count + 1, pins, _frames)
                : new Frame(_frames.Count + 1, pins, _frames);
            _frames.Add(newFrame);
        }

        public int Score()
        {
            return _frames.Sum(frame => frame.Score());
        }
    }

    public class Frame
    {
        public Frame(int frameIndex, int firstRoll, List<Frame> allFrames)
        {
            FirstRoll = firstRoll;
            FrameIndex = frameIndex;
            _allFrames = allFrames;
        }

        private List<Frame> _allFrames;

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

        public virtual int FrameScore()
        {
            return FirstRoll.Value + (SecondRoll.HasValue ? SecondRoll.Value : 0);
        }

        public int Score()
        {
            var score = FrameScore();

            var nextFrame = _allFrames.Find(fr => fr.FrameIndex == FrameIndex + 1);

            if (nextFrame == null) return score;

            if (IsSpare)
            {
                score += nextFrame != null && nextFrame.FirstRoll.HasValue ? nextFrame.FirstRoll.Value : 0;
            }

            else if (IsStrike)
            {
                var strikeBonus = nextFrame.FirstRoll.HasValue ? nextFrame.FirstRoll.Value : 0;
                if (nextFrame.SecondRoll.HasValue)
                {
                    strikeBonus += nextFrame.SecondRoll.Value;
                }
                else
                {
                    var hopefulTurkeyFrame = _allFrames.Find(fr => fr.FrameIndex == FrameIndex + 2);
                    strikeBonus += hopefulTurkeyFrame != null && hopefulTurkeyFrame.FirstRoll.HasValue ?
                        hopefulTurkeyFrame.FirstRoll.Value : 0;
                }

                score += strikeBonus;
            }

            return score;
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
        public TenthFrame(int frameIndex, int pins, List<Frame> allFrames) : base(frameIndex, pins, allFrames) { }

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

        public override int FrameScore()
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