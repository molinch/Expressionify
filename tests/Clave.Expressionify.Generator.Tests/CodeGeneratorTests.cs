﻿using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using Verify = Clave.Expressionify.Generator.Tests.Verifiers.CSharpSourceGeneratorVerifier<Clave.Expressionify.Generator.ExpressionifySourceGenerator>;

namespace Clave.Expressionify.Generator.Tests
{
    [TestFixture]
    public class CodeGeneratorTests
    {
        private const string AttributeCode = @"
        namespace ConsoleApplication1
        {
            using System;

            [AttributeUsage(AttributeTargets.Method)]
            public class ExpressionifyAttribute : Attribute
            {
            }
        }";

        [TestCase(@"namespace ConsoleApplication1
{
    public partial class Extensions
    {
        [Expressionify]
        public static int Foo(int x) => 8;
    }
}",
@"namespace ConsoleApplication1
{
    public partial class Extensions
    {
        private static System.Linq.Expressions.Expression<System.Func<int, int>> Foo_Expressionify_0 { get; } = (int x) => 8;
    }
}", TestName = "Normal scenario")]
        [TestCase(@"namespace ConsoleApplication1;
    public partial class Extensions
    {
        [Expressionify]
        public static int Foo(int x) => 8;
    }",
@"namespace ConsoleApplication1
{
    public partial class Extensions
    {
        private static System.Linq.Expressions.Expression<System.Func<int, int>> Foo_Expressionify_0 { get; } = (int x) => 8;
    }
}", TestName = "Nested class")]
        [TestCase(@"namespace ConsoleApplication1
{
    public partial class Extensions
    {
        [Expressionify]
        public static int Foo(int x) => 8;

        public partial class Nested
        {
            [Expressionify]
            public static int Foo(int x) => 8;
        }
    }
}",
@"namespace ConsoleApplication1
{
    public partial class Extensions
    {
        private static System.Linq.Expressions.Expression<System.Func<int, int>> Foo_Expressionify_0 { get; } = (int x) => 8;
        public partial class Nested
        {
            private static System.Linq.Expressions.Expression<System.Func<int, int>> Foo_Expressionify_0 { get; } = (int x) => 8;
        }
    }
}", TestName = "File scoped namespace")]
        public async Task TestGenerator(string source, string generated)
        {
            await VerifyGenerated(source, generated);
        }

        public async Task VerifyGenerated(string source, string generated)
        {
            await new Verify.Test
            {
                TestState =
                {
                    Sources = { source, AttributeCode },
                    GeneratedSources =
                    {
                        (typeof(ExpressionifySourceGenerator), "Test0_expressionify_0.cs", SourceText.From(generated.Replace("\r\n", Environment.NewLine), Encoding.UTF8, SourceHashAlgorithm.Sha1)),
                    },
                },
            }.RunAsync();
        }
    }
}
