PatternMatcher
==============

What is it?
-----------

PatternMatcher is a C# Pattern Maching Library that adds the `PatternMatcher` class to the `Functional.PatternMatching` namespace. Pattern Matching is a Functional paradigm that allows for the representation of functions that do different things for different specified input types and values.

How can I get it?
-----------------

PatternMatcher is available as a NuGet package: https://www.nuget.org/packages/PatternMatcher

```
PM> Install-Package PatternMatcher
```

Why was it made?
----------------

PatternMatcher is missing as a built in for C#, and while present and excellent in F# using F# PatternMatching in C# is not possible. It is a very useful tool as it can be used as a Type and value dispatcher with compile time guarantees about return types.

Example Usage
-------------

```csharp
public void Example()
{
    var pm = PatternMatcher.MatchWithResult<string>()
        .With<int>(0, () => "Zero")
        .With<int>(x => x.ToString())
        .With<string>(s => s)
        .With<double>(1.0, () => "One Point Zero")
        .Else(() => "Wildcard");
        
    pm.Return();    // "Wildcard"
    pm.Return(0);   // "Zero"
    pm.Return(1);   // "1"
    pm.Return(1.0); // "One Point Zero"
    pm.Return(0.0); // "0.0"
    pm.Return("s"); // "s"
    pm.Return(0f);  // "Wildcard"
}
```

As an alternative, the class `_` can also be used as a wildcard in pattern matching:

```csharp
var pm = PatternMatcher.MatchWithResult<string>().With<_>(() => "Wildcard");
```
