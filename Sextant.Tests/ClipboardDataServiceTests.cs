// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Collections.Generic;
using Sextant.Domain.Entities;
using Sextant.Infrastructure;
using Xunit;
using FluentAssertions;
using Serilog;
using Serilog.Sinks.TestCorrelator;

namespace Sextant.Tests
{
    public class ClipboardDataServiceTests
    {
        private class ClipboardDataServiceStub : ClipboardDataService
        {
            private readonly string _clipboardText;

            protected override string GetClipboard() => _clipboardText;

            public ClipboardDataServiceStub(string clipboardText)
                : base(new SerilogWrapper())
            {
                Log.Logger = new LoggerConfiguration().WriteTo.TestCorrelator().CreateLogger();
                _clipboardText = clipboardText;
            }
        }

        [Fact(Skip = "appveyor incorrectly parsing expeditionText")]
        public void GetExpeditionData_With_Valid_ExpeditionText_Returns_StarSystems()
        {
            const string expeditionText =
@"  #   Jump System/Planets
--- ------ ---------------
  1  69.53 Test System One
	           1 (79) TWW
  2  3.62  Test System Two
	           1 (267) ELW
  3  56.80 Test System Three
	           2 (393) TWW
	           3 (493) HMC
";

            ClipboardDataServiceStub sut = new ClipboardDataServiceStub(expeditionText);

            List<StarSystem> results     =  sut.GetExpeditionData().ToList();

            StarSystem systemOne         = results[0];
            StarSystem systemTwo         = results[1];
            StarSystem systemThree       = results[2];

            Celestial planetOne          = systemOne.Celestials[0];
            Celestial planetTwo          = systemTwo.Celestials[0];
            Celestial planetThree        = systemThree.Celestials[1];

            systemOne.Name.Should().Be("Test System One");
            systemTwo.Name.Should().Be("Test System Two");
            systemThree.Name.Should().Be("Test System Three");

            systemOne.Scanned.Should().BeFalse();
            systemTwo.Scanned.Should().BeFalse();
            systemThree.Scanned.Should().BeFalse();

            planetOne.Name.Should().Be("Test System One 1");
            planetOne.Scanned.Should().BeFalse();
            planetOne.Clasification.Should().Be("Water World");

            planetTwo.Name.Should().Be("Test System Two 1");
            planetTwo.Scanned.Should().BeFalse();
            planetTwo.Clasification.Should().Be("Earth world");

           planetThree.Name.Should().Be("Test System Three 3");
           planetThree.Scanned.Should().BeFalse();
           planetThree.Clasification.Should().Be("High metal content world");
        }

        [Fact]
        public void GetExpeditionData_With_Null_ExpeditionText_Logs_Error_And_Returns_Null()
        {
            using (var context = TestCorrelator.CreateContext())
            {
                const string expeditionText = null;

                ClipboardDataServiceStub sut    = new ClipboardDataServiceStub(expeditionText);
                IEnumerable<StarSystem> results = sut.GetExpeditionData();

                results.Should().BeNull();

                TestCorrelator.GetLogEventsFromCurrentContext()
                              .Should().ContainSingle()
                              .Which.MessageTemplate.Text
                              .Should().Be("Exception parsing expedition data");
            }
        }

        [Fact]
        public void GetExpeditionData_With_Invalid_ExpeditionText_Returns_Null()
        {
            const string expeditionText = "Test";

            ClipboardDataServiceStub sut = new ClipboardDataServiceStub(expeditionText);
            IEnumerable<StarSystem> results = sut.GetExpeditionData();

            results.Should().BeNull();
        }
    }
}
