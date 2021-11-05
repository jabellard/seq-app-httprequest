// Copyright © Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using Seq.App.Http.Expressions;
using Seq.App.Http.Expressions.Ast;
using Seq.App.Http.Expressions.Compilation;
using Seq.App.Http.Templates.Ast;
using Seq.App.Http.Templates.Encoding;

namespace Seq.App.Http.Templates.Compilation
{
    static class TemplateCompiler
    {
        public static CompiledTemplate Compile(Template template,
            IFormatProvider? formatProvider, NameResolver nameResolver,
            EncodedTemplateFactory encoder)
        {
            return template switch
            {
                LiteralText text => new CompiledLiteralText(text.Text),
                FormattedExpression { Expression: AmbientNameExpression { IsBuiltIn: true, PropertyName: BuiltInProperty.Level} } level =>
                    encoder.Wrap(new CompiledLevelToken(level.Format, level.Alignment)),
                FormattedExpression
                {
                    Expression: AmbientNameExpression { IsBuiltIn: true, PropertyName: BuiltInProperty.Exception },
                    Alignment: null,
                    Format: null
                } => encoder.Wrap(new CompiledExceptionToken()),
                FormattedExpression
                {
                    Expression: AmbientNameExpression { IsBuiltIn: true, PropertyName: BuiltInProperty.Message },
                    Format: null
                } message => encoder.Wrap(new CompiledMessageToken(formatProvider, message.Alignment)),
                FormattedExpression expression => encoder.MakeCompiledFormattedExpression(
                    ExpressionCompiler.Compile(expression.Expression, formatProvider, nameResolver), expression.Format, expression.Alignment, formatProvider),
                TemplateBlock block => new CompiledTemplateBlock(block.Elements.Select(e => Compile(e, formatProvider, nameResolver, encoder)).ToArray()),
                Conditional conditional => new CompiledConditional(
                    ExpressionCompiler.Compile(conditional.Condition, formatProvider, nameResolver),
                    Compile(conditional.Consequent, formatProvider, nameResolver, encoder),
                    conditional.Alternative == null ? null : Compile(conditional.Alternative, formatProvider, nameResolver, encoder)),
                Repetition repetition => new CompiledRepetition(
                    ExpressionCompiler.Compile(repetition.Enumerable, formatProvider, nameResolver),
                    repetition.BindingNames.Length > 0 ? repetition.BindingNames[0] : null,
                    repetition.BindingNames.Length > 1 ? repetition.BindingNames[1] : null,
                    Compile(repetition.Body, formatProvider, nameResolver, encoder),
                    repetition.Delimiter == null ? null : Compile(repetition.Delimiter, formatProvider, nameResolver, encoder),
                    repetition.Alternative == null ? null : Compile(repetition.Alternative, formatProvider, nameResolver, encoder)),
                _ => throw new NotSupportedException()
            };
        }
    }
}