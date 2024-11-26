using Assembler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Z80Core
{
    public class Z80Assembler : MacroAssembler
    {
        private static ILookup<string, Instruction> instructions;

        [DebuggerDisplay("{Opcode} {Operands}")]
        private class Instruction
        {
            public string Opcode { get; set; }
            public string Operands { get; set; }
            public Regex OperandMatcher { get; set; }
            public IEnumerable<OperandType> OperandTypes { get; set; }
            public IEnumerable<Func<int[], byte>> Bytes { get; set; }
        }

        private enum OperandType
        {
            Relative,
            Absolute,
            ImWord,
            ImByte,
            Bit
        }

        /* (HL) -> null because it is mapped separately */
        static readonly string[] regs = ["B", "C", "D", "E", "H", "L", null /* (HL) */, "A"];

        static readonly Dictionary<string, OperandType> operandTypeMap = new()
        {
            ["$N+2"] = OperandType.Relative,  // -> XX
            ["$NN"] = OperandType.Absolute,   // -> XX XX
            ["NN"] = OperandType.ImWord,      // -> XX XX
            ["IX+N"] = OperandType.ImByte,    // -> 
            ["IY+N"] = OperandType.ImByte,    // -> 
            ["N"] = OperandType.ImByte,       // -> XX
            ["b"] = OperandType.Bit           // -> 8*b
        };

        static readonly Regex placeholdersRegex = new(
            "(?:" + String.Join("|", operandTypeMap.Select(it => $"(?:{Regex.Escape(it.Key)})")) + @")(?!\w)",
            RegexOptions.Compiled);

        private static Instruction DecodeInstruction(string opcode, string operands, string bytes)
        {
            var matches = placeholdersRegex.Matches(operands);
            var operandList = new List<OperandType>();

            string operandRegStr = $"^{Regex.Escape(operands)}$";
            foreach (Match match in matches)
            {
                var operandType = operandTypeMap[match.Value];
                operandList.Add(operandType);
                operandRegStr = match.Value == "IX+N"
                    ? $"IX((?:\\+|-)[^(),]+)"
                    : match.Value == "IY+N"
                    ? $"IY((?:\\+|-)[^(),]+)"
                    : operandRegStr.Replace(Regex.Escape(match.Value), "([^(),]+)");
            }
            var operandMatcher = new Regex(operandRegStr, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var byteExpr = new List<Func<int[], byte>>();

            int xcount = 0;
            var parExpr = Expression.Parameter(typeof(int[]), "p");
            foreach (var part in bytes.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries))
            {
                Expression expr1 = null;
                foreach (var subpart in part.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Expression expr2 = null;
                    if (subpart.Length <= 2 && Int32.TryParse(subpart, System.Globalization.NumberStyles.HexNumber, null, out int a))
                    {
                        expr2 = Expression.Constant(a);
                    }
                    else if (subpart == "XX")
                    {
                        expr2 = Expression.ArrayAccess(parExpr, Expression.Constant(operandList.FindIndex(it => it != OperandType.Bit)));
                        if (xcount++ == 0)
                            expr2 = Expression.And(expr2, Expression.Constant(0xFF));
                        else
                            expr2 = Expression.RightShift(expr2, Expression.Constant(8));
                    }
                    else if (subpart == "8*b")
                    {
                        expr2 = Expression.ArrayAccess(parExpr, Expression.Constant(operandList.FindIndex(it => it == OperandType.Bit)));
                        expr2 = Expression.Multiply(expr2, Expression.Constant(8));
                    }
                    expr1 = expr1 == null ? expr2 : Expression.Add(expr1, expr2);
                }
                var lambda = Expression.Lambda<Func<int[], byte>>(Expression.Convert(expr1, typeof(byte)), parExpr);
                byteExpr.Add(lambda.Compile());
            }

            return new Instruction
            {
                Opcode = opcode,
                Operands = operands,
                OperandTypes = operandList,
                OperandMatcher = operandMatcher,
                Bytes = byteExpr
            };
        }

        protected override async Task Initialize()
        {
            if (instructions != null) return;
            var assembly = typeof(Z80Assembler).Assembly;

            // Downloaded from https://www.ticalc.org/pub/text/z80/z80_reference.txt and modified
            // Copyright 2000.04.29 by Devin Gardner
            var streamName = assembly.GetManifestResourceNames().First(it => it.Contains("z80_reference.txt"));
            using var sr = new StreamReader(assembly.GetManifestResourceStream(streamName));

            var list = new List<Instruction>();
            var lineRegex = new Regex(@"\|([A-Z]+)\s+([^\s|]+)?\s*\|.*?\|.*?\|.*?\|(.+?)\s*\|", RegexOptions.Compiled);
            while (!sr.EndOfStream)
            {
                var rawline = await sr.ReadLineAsync();
                var lines =
                    rawline.Contains("~,")
                    ? new[] { rawline.Replace("~,", ""), rawline.Replace("~,", "A,") }
                    : new[] { rawline };
                foreach (string line in lines)
                {
                    var match = lineRegex.Match(line);
                    if (match.Success)
                    {
                        string opcode = match.Groups[1].Value;
                        string operands = match.Groups[2].Value;
                        string bytes = match.Groups[3].Value;

                        // Flatten the register instructions (containing "r")
                        if (operands.Contains('r'))
                        {
                            // null because (HL) is mentioned separately
                            for (int j = 0; j < 8; j++)
                            {
                                if (regs[j] != null)
                                {
                                    list.Add(DecodeInstruction(opcode, operands.Replace("r", regs[j]), bytes.Replace("rb", j.ToString())));
                                }
                            }
                        }
                        else
                        {
                            list.Add(DecodeInstruction(opcode, operands, bytes));
                        }
                    }
                }
            }
            instructions = list.ToLookup(it => it.Opcode);
        }

        private static int MapOperand(State state, string expr, OperandType operandType, Instruction instruction)
        {
            // TODO: Handle operandType such as bit (check 0..7) and relative addresses
            switch (operandType)
            {
                case OperandType.Relative:
                    var offset = ParseInt(state, expr) - state.Address - instruction.Bytes.Count();
                    if (state.Pass == 2 && (offset < -128 || offset > 127))
                        throw new InvalidOperationException("Relative offset out of range");
                    return offset & 0xFF;
                default:
                    return ParseInt(state, expr);
            }
        }

        protected override byte[] ParseOpcode(State state, OutputCollector outputCollector, string label, string opcode, string operands, string comment)
        {
            var instruction = instructions[opcode]
                .Select(it => new { Instruction = it, Match = it.OperandMatcher.Match(operands) })
                .FirstOrDefault(it => it.Match.Success);
            if (instruction == null) return null;

            var values = instruction.Match.Groups.OfType<Group>()
                .Skip(1)  // Group 0 is the complete match
                .Select(it => it.Value)
                .Zip(instruction.Instruction.OperandTypes, (a, b) => MapOperand(state, a, b, instruction.Instruction))
                .ToArray();

            return instruction.Instruction.Bytes.Select(it => it(values)).ToArray();
        }
    }
}
