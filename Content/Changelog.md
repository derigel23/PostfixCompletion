#### Changelog

*2.x*

* Parameter info is turned off by default (too annoying);
* Respect "Automatically insert parentheses after completion" ReSharper setting in `.new`/`.throw` templates and static members completion;
* Fix lookup list placement of enum cases items in 9.0 build;
* Fix static member completion with "Automatically insert parentheses after completion" ReSharper setting turned off;

TODO * Fix razor?
TODO * Support code style var setting in 9.0 build;
TODO * New .cw template for Console.WriteLine() invocation;
TODO * New .push/.pull templates?

*2.0.9*

* Fix solution leak in ReSharper 9.0 version;
* Trying to fix postfix items ordering (should no longer be on top or take preselection).

*2.0.8*

* Build against R# 9.0 RTM SDK binaries;

*2.0.7*

* Fix static/enum members completion in 9.0 build;
* Enable `.await` template over awaitable expressions with `.ConfigureAwait(false)`;
* Fix `.if`/`.not` templates over awaited boolean expressions.

*2.0.6-beta*

* First ReSharper 9.0 EAP SDK build;

*2.0.5-beta*

* Build against R# 9.0 latest binaries;

*2.0.4*

* Template `.var` now available in auto completion inside invocation arguments;
* Static method completion now work well with the existing arguments (replace completion);
* Template `.var` now can work over expressions like `StringBuilder("abc").var` like `.new`/`.throw`;
* Align with ReSharper 9.0 changes.

*2.0.3-beta*

* Double basic postfix items are moved to top of lookup list (make `.arg`/`.to`/`.sel` usable from code completion);
* Template `.if` automatically replaces `if (x as T)` expression with `if (x is T)` form;
* Template `.throw` now can work over expressions like `Exception(message).throw` like `.new`;
* Templates `.notnull`/`.null`/`.field`/`.prop` no longer available over assignment expressions in automatic completion;
* Fix caret placement in `.arg` template when completing generic method with uninferable type arguments (when "<>" is inserted);
* Template `.not` no longer produces expression selector over "!a" expression;
* Missing read lock in `.parse` and `.tryparse` templates;
* Template `.yield` now works in iterators of enumerator types;
* Fix static methods completion with existing arguments ('RSPL-6167 Invalid completion');
* Align with ReSharper 9.0 APIs.

*2.0.2-beta*

* New feature: `.foreach` template now also available inside expressions (`Write(xs.foreach)` => `foreach (var x in xs) Write(x)`);
* Align with ReSharper 9.0 API changes :)

*2.0*

* Release version. ReSharper 8.2 RTM support.

*1.9.3*

* New feature: expression chooser popup to resolve ambiguity in `.not`/`.par`/`.cast`/`.arg` templates;
* New experimental feature: `.sel` template to select expression in editor (only in double completion/expansion by `tab`);
* Unwrap expression from parentheses in all templates.

*1.9.2-beta*

* ReSharper 8.2 EAP support + fix breaking changes in R# 9.0;
* Project structure reworked to use NuGet 2.8 + multiple ProjectName.packages.config.

*1.9.1-beta*

* ReSharper 9.0 support :)
* Missing locks in parameters info.

*1.9-beta*

* New feature: searching for occurrences in `.var` template (optional);
* New feature: formatting on semicolon;
* Template .new enabled over unresolved references and expression like `StringBuilder().new`;
* Fix `.arg` locking issues, tested with R# 8.1.1 EAP;
* Template `.var` enabled in auto completion `Inside.Complex.var.Expressions`;
* Templates `.for`/`.forr` enabled over integral numerical types other than `int`;
* Template `.foreach` now matches first when typing `.for` prefix;
* Fix `.field`/`.prop` templates naming suggestion issues.

*1.8.4*

* New feature: code completion helper to prevent mistyping `Length` property as `Count` or vice versa;
* New feature: static methods from `System.Array` type available over arrays (`xs.Resize` => `Array.Resize(xs, |)`);
* New feature: static methods from `System.Enum` type available over enumeration types (`SomeEnum.gv` => `Enum.GetValues(typeof(SomeEnum))`);
* Fix naming suggestions for `.foreach`/`.using` when braces insertion is turned off and there is `foreach`/`using` statement over the same collection/resource in the same block;
* Templates `.notnull`/`.null` disabled over as-expressions (producing `expr as T != null` is stupid);
* Fix `.arg` template scope inside assignment expresions;

*1.8.3*

* ReSharper 8.1 RTM support;
* New `.to` template for assingments (only in basic completion/tab);
* Option to turn off parameter info invokation;
* Various fixes: `null.return` and `x as T.return` now works;
* Refined interator variable naming in `.foreach`;
* Template `.if` now works with convertible-to-boolean expressions;
* Template `.using` now generates variable only when needed;
* Template `.arg` now places caret after invocation when invocation resolves fine.

*1.8*

* Many small bugfixes;
* Ability to turn off code completion integration (only expand templates by `Tab`);
* All templates disabled over namespaces, some Razor and availability fixes;
* Template `.arg` moved away from auto completion (too invasive).

*1.5*

* ReSharper 8.1 RTM beta support;
* EPIC REFACTORING;
* Core rewritten to match ideas from IntelliJ version;
* Much better work over broken code;
* Plugin rename to "Postfix Templates".

*1.2.1*

* Fix exceptions from settings;
* Better work on broken/incomplete code.

*1.1.2*

* ReSahrper SDK 8.1 EAP version support;
* New `.typeof` template over types;
* Better work on broken/incomplete code;
* Templates `.ifnot`/`.paren` renamed to `.else`/`.par` respectively;
* All templates now splitted (`.notnull`/`.null`, `.if`/`.ifnot`, .`.return`/`.yield`), so can be disabled separately;
* Switch template disabled on non-expression-statements;
* Template `.arg` enabled in automatic completion;
* Fix `.par` scope in assignment expressions;
* Fix Razor support issues.

*1.0*

* Fix duplicate items in double completion;
* Disable `.var` on namespaces;
* Enum helpers/static methods completion fixes.

*0.9.9*

* Enumeration types helpers (simplifies equality/flags checks);
* Static methods completion (`text.IsNullOrEmpty` expands into `string.IsNullOrEmpty(text)`);
* New `.parse`/`.tryparse` templates on strings;
* New `.switch` template on integral/string expressions;
* Template `.var` now works directly on types (`StringBuilder.var` produces `var sb = new StringBuilder();`);
* Better caret position after executing `.var`/`.new`/`.throw` on types;
* Small fixes.

*0.9.8*

* New `.lock` template;
* Template `.throw` now works directly on types (`ArgumentException.throw` => `throw new ArgumentException();`);
* Template `.for` now works on integers (kind of n.times) or any other objects with integer `.Count` property;
* Fix various corruptions of next lines of code.