﻿using System;
using JetBrains.ReSharper.Feature.Services.Tests.CSharp.FeatureServices.CodeCompletion;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ReSharper.ControlFlow.PostfixCompletion
{
  [TestNetFramework4]
  public class PostfixCompletionTest : CodeCompletionTestBase
  {
    protected override bool ExecuteAction { get { return true; } }

    protected override string RelativeTestDataPath
    {
      get { return ReSharperPostfixCompletionTestsAssembly.TestDataPath + @"\Completion"; }
    }

    [Test] public void TestIf01() { DoNamedTest(); }
  }
}