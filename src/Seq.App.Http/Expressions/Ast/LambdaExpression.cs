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

using System.Linq;

namespace Seq.App.Http.Expressions.Ast
{
    class LambdaExpression : Expression
    {
        public LambdaExpression(ParameterExpression[] parameters, Expression body)
        {
            Parameters = parameters;
            Body = body;
        }

        public ParameterExpression[] Parameters { get; }

        public Expression Body { get; }

        public override string ToString()
        {
            return "|" + string.Join(", ", Parameters.Select(p => p.ToString())) + "| {" + Body + "}";
        }
    }
}