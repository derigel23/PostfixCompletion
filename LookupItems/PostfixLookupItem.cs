﻿using System;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Feature.Services.Resources;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Services;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;
using JetBrains.TextControl;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;
using JetBrains.Util;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util.Logging;

namespace JetBrains.ReSharper.ControlFlow.PostfixCompletion.LookupItems
{
  public abstract class PostfixLookupItem : PostfixLookupItemBase, ILookupItem
  {
    [NotNull] private readonly string myShortcut;
    [NotNull] private readonly string myIdentifier;

    private readonly string myReparseString;
    private readonly DocumentRange myExpressionRange;
    private readonly Type myExpressionType;
    private readonly int myContextIndex;

    protected const string PostfixMarker = "POSTFIX_COMPLETION_MARKER";
    protected const string CaretMarker = "POSTFIX_COMPLETION_CARET";

    protected PostfixLookupItem(
      [NotNull] string shortcut, [NotNull] PrefixExpressionContext context)
    {
      myIdentifier = shortcut;
      myShortcut = shortcut.ToLowerInvariant();
      myExpressionType = context.Expression.GetType();
      myExpressionRange = context.ExpressionRange;
      myContextIndex = context.Parent.Expressions.IndexOf(context);
      myReparseString = context.Parent.ExecutionContext.ReparseString;
    }

    public MatchingResult Match(string prefix, ITextControl textControl)
    {
      return LookupUtil.MatchPrefix(new IdentifierMatcher(prefix), myIdentifier);
    }

    protected virtual bool RemoveSemicolon { get { return false; } }

    public void Accept(
      ITextControl textControl, TextRange nameRange, LookupItemInsertType insertType,
      Suffix suffix, ISolution solution, bool keepCaretStill)
    {
      // inserting dummy identifier...
      textControl.Document.InsertText(
        nameRange.EndOffset, myReparseString, TextModificationSide.RightSide);

      solution.GetPsiServices().Files.CommitAllDocuments();

      var position = TextControlToPsi.GetElementFromCaretPosition<ITreeNode>(solution, textControl);

      var itemsOwnerFactory = solution.GetComponent<LookupItemsOwnerFactory>();
      var lookupItemsOwner = itemsOwnerFactory.CreateLookupItemsOwner(textControl);

      var templatesManager = solution.GetComponent<PostfixTemplatesManager>();
      var psiModule = position.GetPsiModule();
      var executionContext = new PostfixExecutionContext(
        false, psiModule, lookupItemsOwner, myReparseString);

      var context = templatesManager.IsAvailable(position, executionContext);
      if (context == null)
      {
        Logger.LogError("Should not happens normally");
        return;
      }

      var expressionContext = FindOriginalContext(context);
      if (expressionContext == null)
      {
        Logger.LogError("Should not happens normally");
        return;
      }

      using (WriteLockCookie.Create())
      {
        var commandName = GetType().FullName + " expansion";
        solution.GetPsiServices().DoTransaction(commandName, () =>
        {
          var fixedContext = context.FixExpression(expressionContext);

          ExpandPostfix(
            textControl, suffix, solution, psiModule, fixedContext.Expression);
        });
      }
    }

    [CanBeNull]
    private PrefixExpressionContext FindOriginalContext([NotNull] PostfixTemplateContext context)
    {
      var startOffset = myExpressionRange.TextRange.StartOffset;
      foreach (var expressionContext in context.Expressions)
      {
        if (expressionContext.Expression.GetType() == myExpressionType &&
            expressionContext.ExpressionRange.TextRange.StartOffset == startOffset)
        {
          return expressionContext;
        }
      }

      if (context.Expressions.Count < myContextIndex)
      {
        return context.Expressions[myContextIndex];
      }

      return null;
    }

    protected abstract void ExpandPostfix(
      [NotNull] ITextControl textControl, [NotNull] Suffix suffix,
      [NotNull] ISolution solution, 
      [NotNull] IPsiModule psiModule, [NotNull] ICSharpExpression expression);

    protected void AfterComplete(
      [NotNull] ITextControl textControl, [NotNull] Suffix suffix, int? caretPosition)
    {
      if (caretPosition != null)
      {
        textControl.Caret.MoveTo(
          caretPosition.Value, CaretVisualPlacement.DontScrollIfVisible);
      }

      ReplaySuffix(textControl, suffix);
    }

    protected virtual void ReplaySuffix(
      [NotNull] ITextControl textControl, [NotNull] Suffix suffix)
    {
      suffix.Playback(textControl);
    }

    public IconId Image
    {
      get { return ServicesThemedIcons.LiveTemplate.Id; }
    }

    public RichText DisplayName { get { return myShortcut; } }
    public RichText DisplayTypeName { get { return null; } }
    public string OrderingString { get { return myShortcut; } }
    public string Identity { get { return myShortcut; } }
  }
}