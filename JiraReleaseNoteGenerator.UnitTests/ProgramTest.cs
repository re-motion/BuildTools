// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  [TestFixture]
  public class ProgramTest
  {
    [Test]
    public void CheckArguments_False ()
    {
      var result = Program.CheckArguments (new string[] { });

      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void CheckArguments_True ()
    {
      var result = Program.CheckArguments (new string[] {"2.0.2"});

      Assert.That (result, Is.EqualTo (0));
    }
  }
}