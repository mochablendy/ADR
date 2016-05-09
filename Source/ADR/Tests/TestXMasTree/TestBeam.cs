using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ADR.Common;
using ADR.Physics;
using XMasTree;

namespace TestXMasTree
{
    //[Ignore]
    [TestClass]
    public class TestBeam
    {
        [TestMethod]
        public void TestMethod1()
        {
            var lcar = new Car(0) { Lane = Lane.LeftHand };
            var rcar = new Car(0) { Lane = Lane.RightHand };

            var preStageBeam = new PreStageBeam() { Y = 10 };
            preStageBeam.Watch(lcar, rcar);

            var stageBeam = new StageBeam() { Y = 15 };
            stageBeam.Watch(lcar, rcar);

            var xtree = new XMasTree.XMasTree(preStageBeam, stageBeam);
            xtree.Setup();

            lcar.MoveForward(10);
            rcar.MoveForward(10);
            lcar.MoveForward(1);
            rcar.MoveForward(1);

            lcar.MoveForward(5);
            Thread.Sleep(2500);
            rcar.MoveForward(5);
            lcar.MoveForward(10);
            Thread.Sleep(3000);
        }

    }
}
