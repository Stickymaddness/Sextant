// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using Sextant.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Sextant.Tests.Builders;
using Sextant.Tests;
using Sextant.Infrastructure.Repository;

namespace Sextant.Infrastructure.Tests
{
    public class NavigatorTests
    {
        Navigator CreateSut() => new Navigator(new NavigationRepository(new MemoryDataStore<StarSystemDocument>()));

        [Fact]
        public void GetNextSystem_Returns_First_Unscanned_System()
        {
            Navigator sut = CreateSut();

            Celestial celestial          = Build.A.Celestial;
            List<StarSystem> starSystems = Build.A.StarSystem.WithCelestial(celestial).InAList();

            sut.PlanExpedition(starSystems);

            sut.GetNextSystem().Name.Should().Be(starSystems.Single().Name);
        }

        [Fact]
        public void GetNextCelestial_Returns_First_Unscanned_Celestial()
        {
            Navigator sut = CreateSut();

            Celestial unscannedCelestial = Build.A.Celestial.ThatHasNotBeenScanned();
            Celestial scannedCelestial   = Build.A.Celestial.ThatHasBeenScanned();
            List<StarSystem> starSystems = Build.A.StarSystem.WithCelestials(unscannedCelestial, scannedCelestial).InAList();

            sut.PlanExpedition(starSystems);

            sut.GetNextCelestial().Name.Should().Be(unscannedCelestial.Name);
        }

        [Fact]
        public void GetNextCelestial_With_Empty_System_Returns_Null_With_Expedition_Complete()
        {
            Navigator sut = CreateSut();

            List<StarSystem> starSystems = Build.A.StarSystem.InAList();

            sut.PlanExpedition(starSystems);

            sut.GetNextCelestial().Should().BeNull();
            sut.ExpeditionComplete.Should().BeTrue();
        }

        [Fact]
        public void ExpeditionComplete_With_All_Systems_Scanned_Returns_True()
        {
            Navigator sut = CreateSut();

            Celestial scannedCelestial   = Build.A.Celestial.ThatHasBeenScanned();
            StarSystem firstSystem       = Build.A.StarSystem.WithCelestial(scannedCelestial);
            StarSystem secondSystem      = Build.A.StarSystem.WithCelestial(scannedCelestial);
            List<StarSystem> starSystems = Build.Many.StarSystems(firstSystem, secondSystem);

            sut.PlanExpedition(starSystems);

            sut.ExpeditionComplete.Should().BeTrue();
        }

        [Fact]
        public void ExpeditionComplete_With_Systems_Unscanned_Returns_False()
        {
            Navigator sut = CreateSut();

            Celestial unscannedCelestial = Build.A.Celestial.ThatHasNotBeenScanned();
            Celestial scannedCelestial   = Build.A.Celestial.ThatHasBeenScanned();
            List<Celestial> celestials   = Build.Many.Celestials.With(unscannedCelestial, scannedCelestial);
            List<StarSystem> starSystems = Build.A.StarSystem.WithCelestials(celestials).InAList();

            sut.PlanExpedition(starSystems);

            sut.ExpeditionComplete.Should().BeFalse();
        }

        [Fact]
        public void ScanSystem_Sets_All_Celetials_In_That_System_To_Scanned()
        {
            Navigator sut = CreateSut();

            Celestial celestial          = Build.A.Celestial.ThatHasNotBeenScanned();
            StarSystem firstSystem       = Build.A.StarSystem.WithCelestial(celestial);
            StarSystem secondSystem      = Build.A.StarSystem.WithCelestial(celestial);
            List<StarSystem> starSystems = Build.Many.StarSystems(firstSystem, secondSystem);

            sut.PlanExpedition(starSystems);

            sut.ScanSystem(firstSystem.Name);

            StarSystem firstStoredSystem = sut.GetSystem(firstSystem.Name);
            firstStoredSystem.Scanned.Should().BeTrue();
            firstStoredSystem.Celestials.All(c => c.Scanned).Should().BeTrue();

            StarSystem secondStoredSystem = sut.GetSystem(secondSystem.Name);
            secondStoredSystem.Scanned.Should().BeFalse();
            secondStoredSystem.Celestials.All(c => c.Scanned).Should().BeFalse();
        }

        [Fact]
        public void UnscanSystem_Sets_All_Celetials_In_That_System_To_Unscanned()
        {
            Navigator sut = CreateSut();

            Celestial celestial          = Build.A.Celestial.ThatHasBeenScanned();
            StarSystem firstSystem       = Build.A.StarSystem.WithCelestial(celestial);
            StarSystem secondSystem      = Build.A.StarSystem.WithCelestial(celestial);
            List<StarSystem> starSystems = Build.Many.StarSystems(firstSystem, secondSystem);

            sut.PlanExpedition(starSystems);

            sut.UnscanSystem(firstSystem.Name);

            StarSystem firstStoredSystem = sut.GetSystem(firstSystem.Name);
            firstStoredSystem.Scanned.Should().BeFalse();
            firstStoredSystem.Celestials.All(c => c.Scanned).Should().BeFalse();

            StarSystem secondStoredSystem = sut.GetSystem(secondSystem.Name);
            secondStoredSystem.Scanned.Should().BeTrue();
            secondStoredSystem.Celestials.All(c => c.Scanned).Should().BeTrue();
        }

        [Fact]
        public void SystemScanned_Returns_If_System_Was_Scanned()
        {
            Navigator sut = CreateSut();

            Celestial celestial = Build.A.Celestial.ThatHasBeenScanned();
            List<StarSystem> starSystems = Build.A.StarSystem.WithCelestial(celestial).InAList();

            sut.PlanExpedition(starSystems);

            sut.SystemScanned(starSystems.Single().Name).Should().BeTrue();
        }

        [Fact]
        public void GetAllExpeditionSystems_Gets_All_Systems()
        {
            Navigator sut = CreateSut();

            StarSystem firstSystem       = Build.A.StarSystem;
            StarSystem secondSystem      = Build.A.StarSystem;
            List<StarSystem> starSystems = Build.Many.StarSystems(firstSystem, secondSystem);

            sut.PlanExpedition(starSystems);

            sut.GetAllExpeditionSystems().ShouldBeEquivalentTo(starSystems);
        }

        [Fact]
        public void GetRemaining_Returns_Correct_Remaining()
        {
            Navigator sut = CreateSut();

            Celestial scannedCelestial   = Build.A.Celestial.ThatHasBeenScanned();
            Celestial unscannedCelestial = Build.A.Celestial.ThatHasNotBeenScanned();
            StarSystem firstSystem       = Build.A.StarSystem.WithCelestial(scannedCelestial);
            StarSystem secondSystem      = Build.A.StarSystem.WithCelestial(unscannedCelestial);
            List<StarSystem> starSystems = Build.Many.StarSystems(firstSystem, secondSystem);

            sut.PlanExpedition(starSystems);

            sut.SystemsRemaining().Should().Be(1);
            sut.CelestialsRemaining().Should().Be(1);
            sut.GetAllRemainingCelestials().ShouldAllBeEquivalentTo(unscannedCelestial);
        }

        [Fact]
        public void CancelExpedition_Clears_All_Data()
        {
            Navigator sut = CreateSut();

            Celestial celestial          = Build.A.Celestial.ThatHasBeenScanned();
            StarSystem firstSystem       = Build.A.StarSystem.WithCelestial(celestial);
            StarSystem secondSystem      = Build.A.StarSystem.WithCelestial(celestial);
            List<StarSystem> starSystems = Build.Many.StarSystems(firstSystem, secondSystem);

            sut.PlanExpedition(starSystems);
            sut.CancelExpedition();

            sut.ExpeditionStarted.Should().BeFalse();
            sut.SystemsRemaining().Should().Be(0);
            sut.CelestialsRemaining().Should().Be(0);
            sut.GetAllRemainingCelestials().Should().BeEmpty();
        }
    }
}
