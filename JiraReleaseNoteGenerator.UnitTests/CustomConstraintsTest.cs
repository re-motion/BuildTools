// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using NUnit.Framework;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  [TestFixture]
  public class CustomConstraintsTest
  {
    [Test]
    public void CustomConstraints_additionalConstraintIsNull ()
    {
      var customConstraints = new CustomConstraints ("1.2", null);

      Assert.That (customConstraints.Version, Is.EqualTo ("1.2"));
      Assert.That (customConstraints.AdditionalConstraint, Is.Null);
    }

    [Test]
    public void CustomConstraints_additionalConstraintNotNull ()
    {
      var customConstraints = new CustomConstraints ("1.2", "abc");

      Assert.That (customConstraints.Version, Is.EqualTo ("1.2"));
      Assert.That (customConstraints.AdditionalConstraint, Is.EqualTo ("abc"));
    }

    [Test]
    public void CustomConstraints_Fail ()
    {
      try
      {
        new CustomConstraints (null, "abc");
        Assert.Fail();
      }
      catch (ArgumentNullException exception)
      {
      }
    }
  }
}