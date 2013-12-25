﻿using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.I18n.Services.Refactoring;
using JetBrains.ReSharper.LiveTemplates;
using JetBrains.ReSharper.PostfixTemplates.LookupItems;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Naming.Extentions;
using JetBrains.ReSharper.Psi.Naming.Impl;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace JetBrains.ReSharper.PostfixTemplates.Templates
{
  public abstract class IntroduceMemberTemplateBase : IPostfixTemplate
  {
    public ILookupItem CreateItem(PostfixTemplateContext context)
    {
      var functionDeclaration = context.ContainingFunction;
      if (functionDeclaration == null) return null;

      var classDeclaration = functionDeclaration.GetContainingNode<IClassDeclaration>();
      if (classDeclaration == null) return null;

      if (!context.IsAutoCompletion || functionDeclaration.DeclaredElement is IConstructor)
      {
        foreach (var expression in context.Expressions)
        {
          if (expression.Type.IsUnknown) continue;
          if (!expression.CanBeStatement) continue;

          var reference = expression.Expression as IReferenceExpression;
          if (reference != null && reference.QualifierExpression == null)
          {
            // filter out other fields and properties
            var target = expression.ReferencedElement;
            if (target == null || target is IField || target is IProperty) continue;
          }

          return CreateItem(expression, expression.Type, functionDeclaration.IsStatic);
        }
      }

      return null;
    }

    protected abstract IntroduceMemberLookupItem CreateItem([NotNull] PrefixExpressionContext expression,
                                                            [NotNull] IType expressionType, bool isStatic);

    protected abstract class IntroduceMemberLookupItem : StatementPostfixLookupItem<IExpressionStatement>
    {
      [NotNull] protected readonly IType ExpressionType;
      protected readonly bool IsStatic;

      [NotNull] private ICollection<string> myMemberNames;
      [NotNull] private readonly LiveTemplatesManager myTemplatesManager;
      [CanBeNull] private ITreeNodePointer<IClassMemberDeclaration> myMemberPointer;

      protected IntroduceMemberLookupItem([NotNull] string shortcut,
                                          [NotNull] PrefixExpressionContext context,
                                          [NotNull] IType expressionType, bool isStatic)
        : base(shortcut, context)
      {
        IsStatic = isStatic;
        ExpressionType = expressionType;
        myMemberNames = EmptyList<string>.InstanceList;
        myTemplatesManager = context.PostfixContext.ExecutionContext.LiveTemplatesManager;
      }

      protected override IExpressionStatement CreateStatement(CSharpElementFactory factory,
                                                              ICSharpExpression expression)
      {
        var statement = (IExpressionStatement) factory.CreateStatement("__ = expression;");

        var classDeclaration = expression.GetContainingNode<IClassDeclaration>().NotNull();
        var anchor = GetAnchorMember(classDeclaration.MemberDeclarations.ToList());

        var newDeclaration = CreateMemberDeclaration(factory);
        var newMember = classDeclaration.AddClassMemberDeclarationAfter(
          newDeclaration, (IClassMemberDeclaration) anchor);

        var assignment = (IAssignmentExpression) statement.Expression;
        assignment.SetSource(expression);

        var suggestionManager = statement.GetPsiServices().Naming.Suggestion;
        var collection = suggestionManager.CreateEmptyCollection(
          PluralityKinds.Unknown, classDeclaration.Language, true, statement);

        collection.Add(assignment.Source, new EntryOptions());
        collection.Prepare(newMember.DeclaredElement,
          new SuggestionOptions { UniqueNameContext = classDeclaration.Body });

        newMember.SetName(collection.FirstName());
        myMemberNames = collection.AllNames();
        myMemberPointer = newMember.CreatePointer();

        return statement;
      }

      [CanBeNull]
      protected abstract ICSharpTypeMemberDeclaration GetAnchorMember(
        [NotNull] IList<ICSharpTypeMemberDeclaration> members);

      [NotNull]
      protected abstract IClassMemberDeclaration CreateMemberDeclaration([NotNull] CSharpElementFactory factory);

      protected override void AfterComplete(ITextControl textControl, IExpressionStatement statement)
      {
        if (myMemberPointer == null) return;

        var memberDeclaration = myMemberPointer.GetTreeNode();
        if (memberDeclaration == null) return;

        var assignment = (IAssignmentExpression) statement.Expression;
        var memberIdentifier = ((IReferenceExpression) assignment.Dest).NameIdentifier;

        var hotspotInfo = new HotspotInfo(
          new TemplateField("memberName", new NameSuggestionsExpression(myMemberNames), 0),
          memberIdentifier.GetDocumentRange(), memberDeclaration.GetNameDocumentRange());

        var endRange = statement.GetDocumentRange().EndOffsetRange().TextRange;
        var session = myTemplatesManager.CreateHotspotSessionAtopExistingText(
          statement.GetSolution(), endRange, textControl,
          LiveTemplatesManager.EscapeAction.LeaveTextAndCaret, hotspotInfo);

        session.Execute();
      }
    }
  }
}