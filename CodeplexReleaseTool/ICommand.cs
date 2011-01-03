using System;

namespace CodeplexReleaseTool
{
  public interface ICommand
  {
    void Execute (string[] args);
  }
}