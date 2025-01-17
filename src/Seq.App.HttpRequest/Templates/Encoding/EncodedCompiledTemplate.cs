﻿using System.IO;
using Seq.App.HttpRequest.Expressions;
using Seq.App.HttpRequest.Templates.Compilation;

namespace Seq.App.HttpRequest.Templates.Encoding
{
    class EncodedCompiledTemplate : CompiledTemplate
    {
        readonly CompiledTemplate _inner;
        readonly TemplateOutputEncoder _encoder;

        public EncodedCompiledTemplate(CompiledTemplate inner, TemplateOutputEncoder encoder)
        {
            _inner = inner;
            _encoder = encoder;
        }

        public override void Evaluate(EvaluationContext ctx, TextWriter output)
        {
            var buffer = new StringWriter(output.FormatProvider);
            _inner.Evaluate(ctx, buffer);
            var encoded = _encoder.Encode(buffer.ToString());
            output.Write(encoded);
        }
    }
}