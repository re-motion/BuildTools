// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using NUnit.Framework;
using Remotion.Text.CommandLine;
using Rhino.Mocks;

namespace CodeplexReleaseTool.UnitTests
{
  [TestFixture]
  public class CreateReleaseCommandTest
  {
    private ICodeplexWebService _serviceMock;
    private CreateReleaseCommand _command;

    [SetUp]
    public void SetUp ()
    {
      _serviceMock = MockRepository.GenerateMock<ICodeplexWebService>();
      _command = new CreateReleaseCommand (_serviceMock);
    }
    
    [Test]
    public void ValidParameters ()
    {
      var args = "/projectName:ProjectName /releaseName:ReleaseName /description:Description /status:Status /releaseDate:ReleaseDate "
        +"/showToPublic:+ /isDefaultRelease:- /username:Username /password:Password";

      _serviceMock.Expect (
          mock =>
          mock.CreateARelease (
              Arg.Is("ProjectName"),
              Arg.Is ("ReleaseName"),
              Arg.Is ("Description"),
              Arg.Is ("ReleaseDate"),
              Arg.Is ("Status"),
              Arg.Is (true),
              Arg.Is (false),
              Arg.Is ("Username"),
              Arg.Is ("Password"))).Return (0);
      _serviceMock.Replay();

      _command.Execute (args.Split (' '));

      _serviceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (MissingRequiredCommandLineParameterException))]
    public void MissingParameters ()
    {
      _command.Execute (new string[0]);
    }

    [Test]
    [ExpectedException (typeof (InvalidCommandLineArgumentNameException))]
    public void InvalidParameters ()
    {
      _command.Execute (new[]{"/sdfsd"});
    }
  }
}