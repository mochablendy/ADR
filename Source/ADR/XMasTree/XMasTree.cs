using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniRx;

using ADR.Common;
using ADR.Physics;

namespace XMasTree
{
    public class XMasTree
    {
        public enum State
        {
            PreStaged,
            Staged,
            Started
        }

        public double StagingWindowTime { get; set; } = 3.0D;

        public PreStageBeam PreStageBeam { get; set; }
        public StageBeam StageBeam { get; set; }

        private IDisposable _stagingTimer;

        public XMasTree(PreStageBeam psb, StageBeam sb)
        {
            this.PreStageBeam = psb;
            this.StageBeam = sb;
        }

        public void Setup()
        {
            Observable.FromEvent<Car>(
                h => this.PreStageBeam.OnBreak += h,
                h => this.PreStageBeam.OnBreak -= h)
                .Do(c => this.LitPreStageLamp(c.Lane))
                .Buffer(2)
                .Where(cars => cars[0].Lane.AnotherHand() == cars[1].Lane)
                .Subscribe(_ => Logger.Log("Both cars break prestage beam"));

            Observable.FromEvent<Car>(
                h => this.StageBeam.OnBreak += h,
                h => this.StageBeam.OnBreak -= h)
                .Do(c => this.LitStageLamp(c.Lane))
                .Buffer(2)
                .Where(cars => cars[0].Lane.AnotherHand() == cars[1].Lane)
                .Subscribe(_ =>
                {
                    Logger.Log("Both cars break stage beam");
                    this.ProceedGreen();
                });

            Observable.FromEvent<Car>(
                h => this.StageBeam.OnLeave += h,
                h => this.StageBeam.OnLeave -= h)
                .Subscribe(c => Logger.Log($"{c.Lane} leaves stage beam"));
        }

        private void LitPreStageLamp(Lane lane)
        {
            Logger.Log($"prestage lamp lit: {lane}");
        }

        private void LitStageLamp(Lane lane)
        {
            Logger.Log($"stage lamp lit: {lane}");
            if (_stagingTimer == null)
            {
                _stagingTimer = Observable.Timer(TimeSpan.FromSeconds(this.StagingWindowTime))
                .Take(1)
                .Subscribe(_ => this.LitRedLamp(lane.AnotherHand()));
            }
        }

        private void LitGreenLamp()
        {
            Logger.Log("green lamp lit");
        }

        private void LitRedLamp(Lane lane)
        {
            Logger.Log($"red lamp lit: {lane}");
        }

        private void LitAmberLamp()
        {
            Logger.Log("amber lamp lit");
        }

        private void ProceedGreen()
        {
            Task.Run(() =>
            {
                Thread.Sleep(this.RandomWait());
                this.LitAmberLamp();
                Thread.Sleep(500);
                this.LitGreenLamp();
            });
        }

        private TimeSpan RandomWait()
        {
            double minWait = 1.0;
            Random rnd = new Random();
            double rndWait = rnd.Next(2000) / 1000D;

            return TimeSpan.FromSeconds(minWait + rndWait);
        }
    }

    public class PreStageBeam
    {
        public int Y { get; set; }

        public event Action<Car> OnBreak;

        public void Watch(Car lc, Car rc)
        {
            lc.LocationY.Where(y => y > this.Y).Take(1).Subscribe(_ => OnBreak(lc));
            rc.LocationY.Where(y => y > this.Y).Take(1).Subscribe(_ => OnBreak(rc));
        }
    }

    public class StageBeam
    {
        public int Y { get; set; }

        public event Action<Car> OnBreak;
        public event Action<Car> OnLeave;

        public void Watch(Car lc, Car rc)
        {
            lc.LocationY.Where(y => y > this.Y).Take(1).Subscribe(_ => OnBreak(lc));
            rc.LocationY.Where(y => y > this.Y).Take(1).Subscribe(_ => OnBreak(rc));

            lc.LocationY.Where(y => y > this.Y + 10).Take(1).Subscribe(_ => OnLeave(lc));
            rc.LocationY.Where(y => y > this.Y + 10).Take(1).Subscribe(_ => OnLeave(rc));
        }
    }
}
