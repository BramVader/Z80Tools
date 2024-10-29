using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Z80Core
{
    internal struct Timing
    {
        public int StatesNormal { get; set; }   // The highest value
        public int StatesLow { get; set; }      // A possible lower value

        public override string ToString()
        {
            return String.Format("{{ Normal: {0}, Low: {1} }}", StatesNormal, StatesLow);
        }
    }

    internal class Z80EmulatorBuilder
    {
        // Basic opcodes
        // When there are two different possible states, the least one is byte-shifted left
        private readonly int[,] timingA = new int[16, 16]
        {
            { 4, 13 | (8 << 8), 12 | (7 << 8), 12 | (7 << 8), 4, 4, 4, 7, 4, 4, 4, 4, 11 | (5 << 8), 11 | (5 << 8), 11 | (5 << 8), 11 | (5 << 8)},
            { 10, 10, 10, 10, 4, 4, 4, 7, 4, 4, 4, 4, 10, 10, 10, 10},
            { 7, 7, 16, 13, 4, 4, 4, 7, 4, 4, 4, 4, 10, 10, 10, 10},
            { 6, 6, 6, 6, 4, 4, 4, 7, 4, 4, 4, 4, 10, 11, 19, 4},
            { 4, 4, 4, 11, 4, 4, 4, 7, 4, 4, 4, 4, 17 | (10 << 8), 17 | (10 << 8), 17 | (10 << 8), 17 | (10 << 8)},
            { 4, 4, 4, 11, 4, 4, 4, 7, 4, 4, 4, 4, 11, 11, 11, 11},
            { 7, 7, 7, 10, 7, 7, 7, 4, 7, 7, 7, 7, 7, 7, 7, 7},
            { 4, 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 11, 11, 11, 11},
            { 4, 12, 12 | (7 << 8), 12 | (7 << 8), 4, 4, 4, 4, 4, 4, 4, 4, 11 | (5 << 8), 11 | (5 << 8), 11 | (5 << 8), 11 | (5 << 8)},
            { 11, 11, 11, 11, 4, 4, 4, 4, 4, 4, 4, 4, 10, 4, 4, 6},
            { 7, 7, 20, 13, 4, 4, 4, 4, 4, 4, 4, 4, 10, 10, 10, 10},
            { 6, 6, 6, 6, 4, 4, 4, 4, 4, 4, 4, 4, 0, 11, 4, 4},
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 17 | (10 << 8), 17 | (10 << 8), 17 | (10 << 8), 17 | (10 << 8)},
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 17, 0, 0, 0},
            { 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 11, 11, 11, 11}
        };
        // Opcodes ED40 to ED7F
        private readonly int[,] timingB = new int[16, 4]
        {
            { 12, 12, 12, 12},
            { 12, 12, 12, 12},
            { 15, 15, 15, 15},
            { 20, 20, 16, 20},
            { 8, 8, 8, 8},
            { 14, 14, 14, 14},
            { 8, 8, 8, 8},
            { 9, 9, 18, 8},
            { 12, 12, 12, 12},
            { 12, 12, 12, 12},
            { 15, 15, 15, 15},
            { 20, 20, 20, 20},
            { 8, 8, 8, 8},
            { 14, 14, 14, 14},
            { 8, 8, 8, 8},
            { 9, 9, 18, 8}
        };
        // Opcodes DD & FD prefix
        private readonly int[,] timingC = new int[16, 16]
        {
            { 8, 17 | (12 << 8), 16 | (11 << 8), 16 | (11 << 8), 8, 8, 8, 19, 8, 8, 8, 8, 15 | (9 << 8), 15 | (9 << 8), 15 | (9 << 8), 15 | (9 << 8)},
            { 14, 14, 14, 14, 8, 8, 8, 19, 8, 8, 8, 8, 14, 14, 14, 14},
            { 11, 11, 20, 17, 8, 8, 8, 19, 8, 8, 8, 8, 14, 14, 14, 14},
            { 10, 10, 10, 10, 8, 8, 8, 19, 8, 8, 8, 8, 14, 15, 23, 8},
            { 8, 8, 8, 23, 8, 8, 8, 19, 8, 8, 8, 8, 21 | (14 << 8), 21 | (14 << 8), 21 | (14 << 8), 21 | (14 << 8)},
            { 8, 8, 8, 23, 8, 8, 8, 19, 8, 8, 8, 8, 15, 15, 15, 15},
            { 11, 11, 11, 19, 19, 19, 19, 8, 19, 19, 19, 19, 11, 11, 11, 11},
            { 8, 8, 8, 8, 8, 8, 8, 19, 8, 8, 8, 8, 15, 15, 15, 15},
            { 8, 16, 16 | (11 << 8), 16 | (11 << 8), 8, 8, 8, 8, 8, 8, 8, 8, 15 | (9 << 8), 15 | (9 << 8), 15 | (9 << 8), 15 | (9 << 8)},
            { 15, 15, 15, 15, 8, 8, 8, 8, 8, 8, 8, 8, 14, 8, 8, 10},
            { 11, 11, 20, 17, 8, 8, 8, 8, 8, 8, 8, 8, 14, 14, 14, 14},
            { 10, 10, 10, 10, 8, 8, 8, 8, 8, 8, 8, 8, 0, 15, 8, 8},
            { 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 21 | (14 << 8), 21 | (14 << 8), 21 | (14 << 8), 21 | (14 << 8)},
            { 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 21, 0, 0, 0},
            { 11, 11, 11, 11, 19, 19, 19, 19, 19, 19, 19, 19, 11, 11, 11, 11},
            { 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 15, 15, 15, 15}
        };

        private Action<Z80Emulator>[] microCode, microCodeCB, microCodeED, microCodeDD, microCodeFD;
        private Action<Z80Emulator, int>[] microCodeDDCB, microCodeFDCB;
        private Expression[] microExpr, microExprCB, microExprDD, microExprED, microExprFD, microExprDDCB, microExprFDCB;
        private Expression handleIntExpr, handleNmiExpr;
        private Action<Z80Emulator, byte> handleInt, handleNmi;
        private Timing[] timing, timingCB, timingDD, timingED, timingFD, timingDDCB, timingFDCB;
        private ParameterExpression par;
        private Func<Expression, Expression> readByte, readInput;
        private Func<Expression, Expression, Expression> writeByte, writeOutput;
        private Expression readImmediateByte, readImmediateWord, readImmediateOffset;
        private MemberExpression regs;
        private MemberExpression flagCY, flagN, flagPV, flagHC, flagZ, flagS, flagX1, flagX2;
        private MemberExpression carry;
        private MemberExpression iff1, iff2, im, flags;
        private MemberExpression regA, regB, regC, regD, regE, regH, regL;
        private MemberExpression regBC, regDE, regHL, regSP, regPC, regAF;
        private MemberExpression regIXL, regIXH, regIX;
        private MemberExpression regIYL, regIYH, regIY;
        private MemberExpression regI, regR;
        private Expression[] readReg8, readReg16, readReg16Stack;
        private Expression takeStatesLow;
        private Func<Expression, Expression>[] writeReg8, writeReg16, writeReg16Stack;
        private Expression readIndirect8, readIndirect16;
        private Func<Expression, Expression> cInt;
        private Func<Expression, Expression> getParity;
        private Func<object, Expression> c;
        private Func<Expression, int, Expression> bit;
        private Func<Expression, Expression> isZero;
        private bool[] parityTable;
        private Func<Expression, Expression> writeDebug;
        Func<Expression, Expression> writeIndirect8, writeIndirect16;
        Expression pop;
        Func<Expression, Expression> push;

        /// <summary>
        /// Visitor, constructs a new expression where registers H & L are replaced by IXH & IXL / IYH & IYL
        /// and (HL) is replaced by (IX + d) / (IY + d).
        /// </summary>
        private class IndexRegReplacer : ExpressionVisitor
        {
            private readonly MemberExpression indexReg;
            private readonly MemberExpression indexRegL;
            private readonly MemberExpression indexRegH;
            private readonly Z80EmulatorBuilder builder;
            private readonly ParameterExpression dispPar;
            private readonly Expression index;

            public int Opcode { get; set; }

            public bool NeedsDisp { get; private set; }

            public IndexRegReplacer(
                Z80EmulatorBuilder builder,
                MemberExpression indexReg,
                MemberExpression indexRegL,
                MemberExpression indexRegH,
                ParameterExpression dispPar
            )
            {
                this.builder = builder;
                this.indexReg = indexReg;
                this.indexRegL = indexRegL;
                this.indexRegH = indexRegH;
                this.dispPar = dispPar;

                this.index = Expression.Add(
                    indexReg, // IX or IY
                    Expression.Subtract(    // Sign-extent the dispPar
                        Expression.ExclusiveOr(dispPar, Expression.Constant(0x80)),
                        Expression.Constant(0x80)
                    )
                );
            }

            public Expression Convert(Expression node)
            {
                NeedsDisp = false;
                return base.Visit(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                // These opcodes do not replace IXL & IXH / IYL & IYH:
                // 0x74: LD (IX + d), H,
                // 0x75: LD (IX + d), L,
                // 0x66: LD H, (IX + d),
                // 0x6E: LD L, (IX + d)
                if (Opcode != 0x74 && Opcode != 0x75 && Opcode != 0x66 && Opcode != 0x6E)
                {
                    if (node == builder.regH)
                    {
                        return base.VisitMember(indexRegH);
                    }
                    if (node == builder.regL)
                    {
                        return base.VisitMember(indexRegL);
                    }
                    if (node == builder.regHL)
                    {
                        return base.VisitMember(indexReg);
                    }
                }
                return base.VisitMember(node);
            }

            protected override Expression VisitInvocation(InvocationExpression node)
            {
                // is it a read from/write to (HL)?
                if (node.Expression is MemberExpression mb1 &&
                    node.Arguments[0] is MemberExpression mb2 &&
                    mb2 == builder.regHL
                )
                {
                    if (mb1.Member.Name == nameof(Z80Emulator.WriteMemory))
                    {
                        NeedsDisp = true;
                        return VisitInvocation(node.Update(
                            mb1, new[] {
                                index,
                                node.Arguments[1]
                            })
                        );

                    }
                    else if (mb1.Member.Name == nameof(Z80Emulator.ReadMemory))
                    {
                        NeedsDisp = true;
                        return VisitInvocation(node.Update(
                            mb1, new[] {
                                index
                            })
                        );
                    }
                }
                return base.VisitInvocation(node);
            }
        }

        // NOPs
        private Timing Nop1 => timing[0x00];

        private Timing Nop2 => timingDDCB[0x00];

        private Timing Nop3 => timingDD[0x00];

        private Expression ExtendWithTiming(Expression microExpr, MemberExpression timingProp, Timing timing, Timing nopTiming) =>
            microExpr != null
            ? Expression.Block(Expression.Assign(timingProp, Expression.Constant(timing)), microExpr)
            : Expression.Assign(timingProp, Expression.Constant(nopTiming));

        private Action<Z80Emulator> Compile(Expression expr) =>
            Expression.Lambda<Action<Z80Emulator>>(expr, par).Compile();

        private Action<Z80Emulator, int> Compile(Expression expr, ParameterExpression dispPar) =>
            Expression.Lambda<Action<Z80Emulator, int>>(expr, par, dispPar).Compile();

        // Converts a signed byte to a signed int (e.g. 7F -> 0000 007F, 80 -> FFFF FF80)
        private Expression SignExtend(Expression p) =>
            Expression.Subtract(Expression.ExclusiveOr(p, c(0x80)), c(0x80));

        // Converts instructions that access H, L, HL or (HL) to IXH/IYH, IXL/IYL, IX/IY, (IX+d)/(IY+d)
        private void ReplaceIndexedXD(IndexRegReplacer converter, Expression[] microExprXD, ParameterExpression dispPar)
        {
            for (int n = 0; n < 256; n++)
            {
                if (n == 0xEB)      // EX DE, HL        : remains unaffected
                {
                    microExprXD[n] = microExpr[n];
                }
                else
                {
                    if (n != 0x34 &&    // INC (I* + d)     : have specific implementation
                        n != 0x35)      // DEC (I* + d)       (would read offset twice otherwise)
                    {
                        converter.Opcode = n;
                        microExprXD[n] = converter.Convert(microExpr[n]);
                        if (converter.NeedsDisp)    // (IX + d) or (IY + d)? Read the d-byte first and assign it to 'disp'
                        {
                            var blockExpr = microExprXD[n] as BlockExpression;
                            var variables = blockExpr == null ? new List<ParameterExpression>() : blockExpr.Variables.ToList();
                            var list = blockExpr == null ? new List<Expression> { microExprXD[n] } : blockExpr.Expressions.ToList();
                            variables.Add(dispPar);
                            list.Insert(0, Expression.Assign(dispPar, readImmediateByte));
                            microExprXD[n] = Expression.Block
                            (
                                variables,
                                list
                            );
                        }
                    }
                }
            }
        }

        public void Build()
        {
            Init();

            // Contains the "d" offset value as in (IX+d) or (IY+d)
            var dispPar = Expression.Parameter(typeof(int), "disp");
            var timingProp = GetMember<Z80Registers>(regs, p => p.Timing);

            BuildLoadInstructions();
            BuildStackInstructions();
            BuildControlStructures();
            BuildArithmetic();
            BuildRotateAndBit();
            BuildInputOutput();
            BuildLoopInstuctions();
            BuildInterrupts();
            BuildMisc();
            BuildCBxx(microExprCB);
            BuildCBxx(microExprDDCB, regIX, dispPar);
            BuildCBxx(microExprFDCB, regIY, dispPar);

            // Convert basic expressions to indexed expressions
            var converter = new IndexRegReplacer(this, regIX, regIXL, regIXH, dispPar);
            ReplaceIndexedXD(converter, microExprDD, dispPar);

            converter = new IndexRegReplacer(this, regIY, regIYL, regIYH, dispPar);
            ReplaceIndexedXD(converter, microExprFD, dispPar);

            // Compile CB-prefixed expressions
            for (int opcode = 0; opcode < 256; opcode++)
            {
                // Add timing to the CB prefix instructions
                microExprCB[opcode] = ExtendWithTiming(microExprCB[opcode], timingProp, timingCB[opcode], Nop3);
                // and compile it
                microCodeCB[opcode] = Compile(microExprCB[opcode]);
            }

            // Compile DDCB-prefixed expressions
            for (int opcode = 0; opcode < 256; opcode++)
            {
                // Add timing to the DDCB prefix instructions
                microExprDDCB[opcode] = ExtendWithTiming(microExprDDCB[opcode], timingProp, timingDDCB[opcode], Nop3);
                // and compile it
                microCodeDDCB[opcode] = Compile(microExprDDCB[opcode], dispPar);
            }

            // Compile FDCB-prefixed expressions
            for (int opcode = 0; opcode < 256; opcode++)
            {
                // Add timing to the FDCB prefix instructions
                microExprFDCB[opcode] = ExtendWithTiming(microExprFDCB[opcode], timingProp, timingFDCB[opcode], Nop3);
                // and compile it
                microCodeFDCB[opcode] = Compile(microExprFDCB[opcode], dispPar);
            }

            // Compile DD-prefixed instructions
            for (int opcode = 0; opcode < 256; opcode++)
            {
                if (opcode == 0xCB)
                {
                    microExprDD[0xCB] = Expression.Block(
                        new[] { dispPar },
                        Expression.Assign(dispPar, readImmediateByte), // read "d" as in (IX+d) or (IY+d)
                        Expression.Invoke(Expression.ArrayAccess(GetMember<Z80Emulator>(par, "microCodeDDCB"), readImmediateByte), par, dispPar)
                    );
                }
                else
                {
                    microExprDD[opcode] = ExtendWithTiming(microExprDD[opcode], timingProp, timingDD[opcode], Nop3);
                }
                microCodeDD[opcode] = Compile(microExprDD[opcode]);
            }

            // Compile ED-prefixed instructions
            for (int opcode = 0; opcode < 256; opcode++)
            {
                microExprED[opcode] = ExtendWithTiming(microExprED[opcode], timingProp, timingED[opcode], Nop3);
                microCodeED[opcode] = Compile(microExprED[opcode]);
            }

            // Compile FD-prefixed instructions
            for (int opcode = 0; opcode < 256; opcode++)
            {
                if (opcode == 0xCB)
                {
                    microExprFD[0xCB] = Expression.Block(
                        new[] { dispPar },
                        Expression.Assign(dispPar, readImmediateByte),
                        Expression.Invoke(Expression.ArrayAccess(GetMember<Z80Emulator>(par, "microCodeFDCB"), readImmediateByte), par, dispPar)
                    );
                }
                else
                {
                    microExprFD[opcode] = ExtendWithTiming(microExprFD[opcode], timingProp, timingFD[opcode], Nop3);
                }
                microCodeFD[opcode] = Compile(microExprFD[opcode]);
            }

            microExpr[0xCB] = Expression.Invoke(Expression.ArrayAccess(GetMember<Z80Emulator>(par, "microCodeCB"), readImmediateByte), par);
            microExpr[0xDD] = Expression.Invoke(Expression.ArrayAccess(GetMember<Z80Emulator>(par, "microCodeDD"), readImmediateByte), par);
            microExpr[0xED] = Expression.Invoke(Expression.ArrayAccess(GetMember<Z80Emulator>(par, "microCodeED"), readImmediateByte), par);
            microExpr[0xFD] = Expression.Invoke(Expression.ArrayAccess(GetMember<Z80Emulator>(par, "microCodeFD"), readImmediateByte), par);

            // Compile basic expressions
            for (int n = 0; n < 256; n++)
            {
                // Add timing when it's not a prefixed instruction
                if (n != 0xCB && n != 0xDD && n != 0xED && n != 0xFD)
                {
                    microExpr[n] = ExtendWithTiming(microExpr[n], timingProp, timing[n], Nop1);
                }
                microCode[n] = Compile(microExpr[n]);
            }
        }

        private static MemberExpression GetMember<TElement>(Expression instance, Expression<Func<TElement, object>> prop)
        {
            Expression expr = prop.Body;
            if (expr is UnaryExpression unary)
                expr = unary.Operand;
            MemberExpression member = expr as MemberExpression;
            return Expression.MakeMemberAccess(instance, member.Member);
        }
        private static MemberExpression GetMember<TElement>(Expression instance, string name)
        {
            var member = typeof(TElement).GetMember(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return Expression.MakeMemberAccess(instance, member[0]);
        }



        private void InitTiming()
        {
            timing = new Timing[256];
            timingCB = new Timing[256];
            timingDD = new Timing[256];
            timingED = new Timing[256];
            timingFD = new Timing[256];
            timingDDCB = new Timing[256];
            timingFDCB = new Timing[256];
            for (int row = 0; row < 16; row++)
                for (int col = 0; col < 16; col++)
                {
                    timing[col * 16 + row].StatesNormal = timingA[row, col] & 0xFF;
                    timing[col * 16 + row].StatesLow = timingA[row, col] >> 8;
                    timingDD[col * 16 + row].StatesNormal = timingC[row, col] & 0xFF;
                    timingFD[col * 16 + row].StatesLow = timingC[row, col] >> 8;
                }
            for (int row = 0; row < 16; row++)
                for (int col = 0; col < 4; col++)
                {
                    timingED[col * 16 + row + 0x40].StatesNormal = timingB[row, col] & 0xFF;
                    timingED[col * 16 + row + 0x40].StatesLow = timingB[row, col] >> 8;
                }

            // LDIR/INIR/OTIR etc.
            for (int row = 0; row < 4; row++)
            {
                timingED[row + 0xA0].StatesNormal = 16;
                timingED[row + 0xA8].StatesNormal = 16;
                timingED[row + 0xB0].StatesNormal = 21;
                timingED[row + 0xB8].StatesNormal = 21;
                timingED[row + 0xB0].StatesLow = 16;
                timingED[row + 0xB8].StatesLow = 16;
            }

            for (int n = 0; n < 256; n++)
            {
                timingDDCB[n].StatesNormal = n < 0x40 || n >= 0x80 ? 23 : 20;
                timingFDCB[n].StatesNormal = n < 0x40 || n >= 0x80 ? 23 : 20;
            }
        }

        private void Init()
        {
            Type type = typeof(Z80Emulator);

            // Lambda parameter to instance of Emulator
            par = Expression.Parameter(type, "this");

            c = constant => Expression.Constant(constant, constant.GetType());
            bit = (par, bitNr) => Expression.Equal(Expression.And(par, c(1 << bitNr)), c(1 << bitNr));
            isZero = par => Expression.Equal(Expression.And(par, c(0xFF)), c(0));

            // Init 'microCode' array
            microCode = new Action<Z80Emulator>[256];
            microExpr = new Expression[256];

            // CB-prefix
            microCodeCB = new Action<Z80Emulator>[256];
            microExprCB = new Expression[256];

            // DD-prefix
            microCodeDD = new Action<Z80Emulator>[256];
            microExprDD = new Expression[256];

            // ED-prefix
            microCodeED = new Action<Z80Emulator>[256];
            microExprED = new Expression[256];

            // FD-prefix
            microCodeFD = new Action<Z80Emulator>[256];
            microExprFD = new Expression[256];

            // DDCB-prefix
            microCodeDDCB = new Action<Z80Emulator, int>[256];
            microExprDDCB = new Expression[256];

            // FDCB-prefix
            microCodeFDCB = new Action<Z80Emulator, int>[256];
            microExprFDCB = new Expression[256];

            cInt = (p) => Expression.Convert(p, typeof(int));

            var readByteProp = GetMember<Z80Emulator>(par, p => p.ReadMemory);
            var writeByteProp = GetMember<Z80Emulator>(par, p => p.WriteMemory);
            var readInputProp = GetMember<Z80Emulator>(par, p => p.ReadInput);
            var writeOutputProp = GetMember<Z80Emulator>(par, p => p.WriteOutput);

            // Register access
            regs = Expression.Field(par, "z80Registers");

            // Flags
            flagCY = GetMember<Z80Registers>(regs, p => p.CY);
            flagN = GetMember<Z80Registers>(regs, p => p.N);
            flagPV = GetMember<Z80Registers>(regs, p => p.PV);
            flagHC = GetMember<Z80Registers>(regs, p => p.HC);
            flagZ = GetMember<Z80Registers>(regs, p => p.Z);
            flagS = GetMember<Z80Registers>(regs, p => p.S);
            flagX1 = GetMember<Z80Registers>(regs, p => p.X1);
            flagX2 = GetMember<Z80Registers>(regs, p => p.X2);
            flags = GetMember<Z80Registers>(regs, p => p.F);

            carry = GetMember<Z80Registers>(regs, p => p.Carry);

            // 8-bit registers
            regB = GetMember<Z80Registers>(regs, p => p.B);
            regC = GetMember<Z80Registers>(regs, p => p.C);
            regD = GetMember<Z80Registers>(regs, p => p.D);
            regE = GetMember<Z80Registers>(regs, p => p.E);
            regH = GetMember<Z80Registers>(regs, p => p.H);
            regL = GetMember<Z80Registers>(regs, p => p.L);
            regA = GetMember<Z80Registers>(regs, p => p.A);
            regIXH = GetMember<Z80Registers>(regs, p => p.IXH);
            regIXL = GetMember<Z80Registers>(regs, p => p.IXL);
            regIYH = GetMember<Z80Registers>(regs, p => p.IYH);
            regIYL = GetMember<Z80Registers>(regs, p => p.IYL);

            // 16-bit registers
            regBC = GetMember<Z80Registers>(regs, p => p.BC);
            regDE = GetMember<Z80Registers>(regs, p => p.DE);
            regHL = GetMember<Z80Registers>(regs, p => p.HL);
            regAF = GetMember<Z80Registers>(regs, p => p.AF);
            regIX = GetMember<Z80Registers>(regs, p => p.IX);
            regIY = GetMember<Z80Registers>(regs, p => p.IY);

            // Interrupt & Refresh registers
            regI = GetMember<Z80Registers>(regs, p => p.I);
            regR = GetMember<Z80Registers>(regs, p => p.R);

            regSP = GetMember<Z80Registers>(regs, p => p.SP);
            regPC = GetMember<Z80Registers>(regs, p => p.PC);
            iff1 = GetMember<Z80Registers>(regs, p => p.Iff1);
            iff2 = GetMember<Z80Registers>(regs, p => p.Iff2);
            im = GetMember<Z80Registers>(regs, p => p.IM);

            takeStatesLow = Expression.Assign(GetMember<Z80Registers>(regs, p => p.TakeStatesLow), c(true));

            // Read a byte from I/O
            readInput = adr => Expression.Convert(Expression.Invoke(readInputProp, adr), typeof(int));

            // Read a byte from memory
            readByte = adr => Expression.Convert(Expression.Invoke(readByteProp, adr), typeof(int));

            // Reading 8-bit and 16-bit operands
            readImmediateByte = readByte(Expression.PostIncrementAssign(regPC));
            readImmediateWord =
                Expression.Or(
                    readImmediateByte,
                    Expression.LeftShift(
                        readImmediateByte,
                        c(8)
                    )
                );

            // Reads a signed byte and expands it to an int representation, e.g. 0000 00F9 becomes FFFF FFF9
            readImmediateOffset = SignExtend(readImmediateByte);

            // Read 8-bit data, referenced by 16-bit address operand
            readIndirect8 = readByte(readImmediateWord);

            // Read 16-bit data, referenced by 16-bit address operand
            var target = Expression.Variable(typeof(int));
            readIndirect16 = Expression.Block(
                new[] { target },
                Expression.Assign(target, readImmediateWord),
                Expression.Or(
                    readByte(Expression.PostIncrementAssign(target)),
                    Expression.LeftShift(
                        readByte(target),
                        c(8)
                    )
                )
            );

            // Write a byte to I/O
            writeOutput = (adr, p) => Expression.Invoke(writeOutputProp, adr, Expression.Convert(p, typeof(byte)));
            // Write a byte to memory
            writeByte = (adr, p) => Expression.Invoke(writeByteProp, adr, Expression.Convert(p, typeof(byte)));
            // Write 8-bit data, referenced by 16-bit address operand
            writeIndirect8 = (p) => writeByte(readImmediateWord, p);
            // Write 16-bit data, referenced by 16-bit address operand
            writeIndirect16 = (p) => Expression.Block(
                new[] { target },
                Expression.Assign(target, readImmediateWord),
                writeByte(Expression.PostIncrementAssign(target), p),
                writeByte(target, Expression.RightShift(p, c(8)))
            );

            readReg8 = new Expression[] {
                    regB, regC, regD, regE, regH, regL,
                    readByte(regHL),
                    regA
                };
            writeReg8 = new Func<Expression, Expression>[] {
                    (p) => Expression.Assign(regB, p),
                    (p) => Expression.Assign(regC, p),
                    (p) => Expression.Assign(regD, p),
                    (p) => Expression.Assign(regE, p),
                    (p) => Expression.Assign(regH, p),
                    (p) => Expression.Assign(regL, p),
                    (p) => writeByte(regHL, p),
                    (p) => Expression.Assign(regA, p)
                };
            readReg16 = new Expression[] { regBC, regDE, regHL, regSP };
            writeReg16 = new Func<Expression, Expression>[] {
                    (p) => Expression.Assign(regBC, p),
                    (p) => Expression.Assign(regDE, p),
                    (p) => Expression.Assign(regHL, p),
                    (p) => Expression.Assign(regSP, p)
                };
            readReg16Stack = new Expression[] { regBC, regDE, regHL, regAF };
            writeReg16Stack = new Func<Expression, Expression>[] {
                    (p) => Expression.Assign(regBC, p),
                    (p) => Expression.Assign(regDE, p),
                    (p) => Expression.Assign(regHL, p),
                    (p) => Expression.Assign(regAF, p)
                };

            // Stack expressions
            // - PUSH
            push = p => Expression.Block(
                writeByte(Expression.PreDecrementAssign(regSP), Expression.RightShift(p, c(8))),
                writeByte(Expression.PreDecrementAssign(regSP), Expression.And(p, c(0xFF)))
            );

            // - POP
            pop = Expression.Or(
                readByte(Expression.PostIncrementAssign(regSP)),
                Expression.LeftShift(
                    readByte(Expression.PostIncrementAssign(regSP)),
                    c(8)
                )
            );

            parityTable = new bool[256];
            for (int i = 0; i < 256; i++)
            {
                parityTable[i] =
                    ((i >> 7) + (i >> 6) + (i >> 5) + (i >> 4) + (i >> 3) + (i >> 2) + (i >> 1) + i) % 2 == 0;
            }

            getParity = (p) =>
                Expression.ArrayAccess(Expression.Field(par, typeof(Z80Emulator).GetField("parityTable", BindingFlags.NonPublic | BindingFlags.Instance)), Expression.And(p, c(0xFF)));

            Expression<Action> textwriterWriteObjectFinder = () => default(TextWriter).Write("", (object)null);

            writeDebug = (p) => Expression.Call(GetMember<Z80Emulator>(par, par => par.Debugger), (textwriterWriteObjectFinder.Body as MethodCallExpression).Method, Expression.Constant("[{0:X2}]"), Expression.Convert(p, typeof(object)));

            InitTiming();
        }

        private void BuildLoadInstructions()
        {
            // LD <reg8>, <reg8> instructions
            for (int rIn = 0; rIn < 8; rIn++)
                for (int rOut = 0; rOut < 8; rOut++)
                {
                    int opcode = rIn + rOut * 8 + 0x40;
                    microExpr[opcode] = writeReg8[rOut](readReg8[rIn]);
                }

            // LD <reg8>, n
            for (int rOut = 0; rOut < 8; rOut++)
            {
                int opcode = rOut * 8 + 0x06;
                microExpr[opcode] = writeReg8[rOut](readImmediateByte);
            }

            // LD <reg16>, nn
            for (int rOut = 0; rOut < 4; rOut++)
            {
                int opcode = rOut * 0x10 + 0x01;
                microExpr[opcode] = writeReg16[rOut](readImmediateWord);
            }

            // LD A,(nn)
            microExpr[0x3A] = Expression.Assign(regA, readIndirect8);

            // LD (nn),A
            microExpr[0x32] = writeIndirect8(regA);

            // LD (nn),HL
            microExpr[0x22] = writeIndirect16(regHL);

            // LD HL,(nn)
            microExpr[0x2A] = Expression.Assign(regHL, readIndirect16);

            // LD (BC),A
            microExpr[0x02] = writeByte(regBC, regA);

            // LD (DE),A
            microExpr[0x12] = writeByte(regDE, regA);

            // LD A,(BC)
            microExpr[0x0A] = Expression.Assign(regA, readByte(regBC));

            // LD A,(DE)
            microExpr[0x1A] = Expression.Assign(regA, readByte(regDE));

            // LD SP, HL
            microExpr[0xF9] = Expression.Assign(regSP, regHL);

            // LD (nn), <reg16>
            for (int opcode = 0; opcode < 4; opcode++)
                microExprED[0x43 + opcode * 0x10] = writeIndirect16(readReg16[opcode]);

            // LD <reg16>, (nn)
            for (int opcode = 0; opcode < 4; opcode++)
                microExprED[0x4B + opcode * 0x10] = writeReg16[opcode](readIndirect16);

            // LD I, A
            microExprED[0x47] = Expression.Assign(regI, regA);

            // LD R, A
            microExprED[0x4F] = Expression.Assign(regR, regA);

            // LD A, I
            var temp1 = Expression.Variable(typeof(int));
            microExprED[0x57] = Expression.Block(
                new[] { temp1 },
                Expression.Assign(regA, regI),
                Expression.Assign(temp1, regI),
                Expression.Assign(flagS, bit(temp1, 7)),
                Expression.Assign(flagX1, bit(temp1, 3)),
                Expression.Assign(flagX2, bit(temp1, 5)),
                Expression.Assign(flagZ, isZero(temp1)),
                Expression.Assign(flagHC, c(false)),
                Expression.Assign(flagPV, iff2),
                Expression.Assign(flagN, c(false))
            );

            // LD A, R
            microExprED[0x5F] = Expression.Block(
                new[] { temp1 },
                Expression.Assign(regR, regA),
                Expression.Assign(temp1, regR),
                Expression.Assign(flagS, bit(temp1, 7)),
                Expression.Assign(flagX1, bit(temp1, 3)),
                Expression.Assign(flagX2, bit(temp1, 5)),
                Expression.Assign(flagZ, isZero(temp1)),
                Expression.Assign(flagHC, c(false)),
                Expression.Assign(flagPV, iff2),
                Expression.Assign(flagN, c(false))
            );

        }

        private void BuildStackInstructions()
        {
            // PUSH
            for (int r = 0; r < 4; r++)
            {
                int opcode = r * 0x10 + 0xC5;
                microExpr[opcode] = push(readReg16Stack[r]);
            }

            // POP
            for (int r = 0; r < 4; r++)
            {
                int opcode = r * 0x10 + 0xC1;
                microExpr[opcode] = writeReg16Stack[r](pop);
            }
        }

        private void BuildControlStructures()
        {
            var conditions = new Expression[]
                {
                    Expression.Not(flagZ), flagZ,
                    Expression.Not(flagCY), flagCY,
                    Expression.Not(flagPV), flagPV,
                    Expression.Not(flagS), flagS
                };

            var jumpRel = Expression.Add(
                readImmediateOffset,
                regPC
            );

            var target = Expression.Variable(typeof(int));

            // Jump Absolute
            // - Unconditional
            microExpr[0xC3] = Expression.Assign(regPC, readImmediateWord);

            // - Conditional
            for (int cond = 0; cond < 8; cond++)
            {
                var opcode = cond * 8 + 0xC2;
                microExpr[opcode] = Expression.Block(
                    new[] { target },
                    Expression.Assign(target, readImmediateWord),
                    Expression.IfThen(conditions[cond], Expression.Assign(regPC, target))
                );
            }

            // Jump Relative
            // - Unconditional
            microExpr[0x18] = Expression.Assign(regPC, jumpRel);

            // - Conditional
            for (int cond = 0; cond < 4; cond++)
            {
                var opcode = cond * 8 + 0x20;
                microExpr[opcode] = Expression.Block(
                    new[] { target },
                    Expression.Assign(target, jumpRel),
                    Expression.IfThenElse(conditions[cond], Expression.Assign(regPC, target), takeStatesLow)
                );
            }

            // Call absolute
            // - Unconditional
            microExpr[0xCD] = Expression.Block(
                new[] { target },
                Expression.Assign(target, readImmediateWord),
                push(regPC),
                Expression.Assign(regPC, target)
            );

            // - Conditional
            for (int cond = 0; cond < 8; cond++)
            {
                var opcode = cond * 8 + 0xC4;
                microExpr[opcode] = Expression.Block(
                    new[] { target },
                    Expression.Assign(target, readImmediateWord),
                    Expression.IfThenElse(
                        conditions[cond],
                        Expression.Block(
                            push(regPC),
                            Expression.Assign(regPC, target)
                        ),
                        takeStatesLow
                    )
                );
            }

            // RET
            microExpr[0xC9] = Expression.Assign(regPC, pop);

            // RETN
            for (int n = 0; n < 8; n++)
            {
                microExprED[n * 8 + 0x45] = Expression.Block(
                    Expression.Assign(regPC, pop),
                    Expression.Assign(iff1, iff2)
                );
            }

            // RETI
            microExprED[0x4D] = Expression.Block(
                Expression.Assign(regPC, pop),
                Expression.Assign(iff1, iff2)
            );

            // RET<c>
            for (int cond = 0; cond < 8; cond++)
            {
                var opcode = cond * 8 + 0xC0;
                microExpr[opcode] = Expression.IfThenElse(conditions[cond], Expression.Assign(regPC, pop), takeStatesLow);
            }

            // JP HL
            microExpr[0xE9] = Expression.Assign(regPC, regHL);

            // Restarts
            for (int rst = 0; rst < 8; rst++)
            {
                var opcode = rst * 8 + 0xC7;
                microExpr[opcode] = Expression.Block(
                    push(regPC),
                    Expression.Assign(regPC, Expression.Constant((int)(rst * 8)))
                );
            }

            // DJNZ
            var rel = Expression.Variable(typeof(int));
            microExpr[0x10] = Expression.Block(
                new[] { rel },
                Expression.Assign(rel, readImmediateOffset),
                Expression.Assign(regB, Expression.Decrement(regB)),
                Expression.IfThenElse(Expression.NotEqual(regB, c(0)),
                    Expression.Assign(
                        regPC,
                        Expression.Add(regPC, rel)
                    ),
                    takeStatesLow
                )
            );
        }

        private void BuildArithmetic()
        {
            var temp1 = Expression.Variable(typeof(int), "temp1");
            var temp2 = Expression.Variable(typeof(int), "temp2");
            var temp3 = Expression.Variable(typeof(int), "temp3");
            var temp4 = Expression.Variable(typeof(int), "temp4");

            // INC/DEC <reg8>
            for (int incDec = 0; incDec < 2; incDec++)
                for (int reg = 0; reg < 8; reg++)
                {
                    var opcode = reg * 8 + 0x04 + incDec;
                    microExpr[opcode] = Expression.Block(
                        new[] { temp1, temp2 },
                        Expression.Assign(temp1, readReg8[reg]),
                        Expression.Assign(temp2, incDec == 0
                            ? Expression.Increment(temp1)
                            : Expression.Decrement(temp1)
                        ),
                        writeReg8[reg](temp2),
                        Expression.Assign(flagS, bit(temp2, 7)),
                        Expression.Assign(flagX1, bit(temp2, 3)),
                        Expression.Assign(flagX2, bit(temp2, 5)),
                        Expression.Assign(flagZ, isZero(temp2)),
                        Expression.Assign(flagHC, bit(
                            incDec == 0 
                            ? Expression.Increment(Expression.And(temp1, c(0x0F)))
                            : Expression.Decrement(Expression.And(temp1, c(0x0F))),
                            4
                        )),
                        Expression.Assign(flagPV, Expression.Equal(temp2, incDec == 0 ? c(0x80) : c(0x7F))),
                        Expression.Assign(flagN, Expression.Constant(incDec == 1))
                    );
                }

            // INC/DEC (IX/IY + d)
            for (int incDec = 0; incDec < 2; incDec++)
                for (int reg = 0; reg < 2; reg++)
                {
                    var block = Expression.Block(
                        new[] { temp1, temp2, temp4 },
                        Expression.Assign(temp4,
                            Expression.Add(reg == 0 ? regIX : regIY, readImmediateOffset)
                        ),
                        Expression.Assign(temp1, readByte(temp4)),
                        Expression.Assign(temp2, incDec == 0
                            ? Expression.Increment(temp1)
                            : Expression.Decrement(temp1)
                        ),
                        writeByte(temp4, temp2),
                        Expression.Assign(flagS, bit(temp2, 7)),
                        Expression.Assign(flagX1, bit(temp2, 3)),
                        Expression.Assign(flagX2, bit(temp2, 5)),
                        Expression.Assign(flagZ, isZero(temp2)),
                        Expression.Assign(flagHC, bit(
                            incDec == 0
                            ? Expression.Increment(Expression.And(temp1, c(0x0F)))
                            : Expression.Decrement(Expression.And(temp1, c(0x0F))),
                            4
                        )),
                        Expression.Assign(flagPV, Expression.Equal(temp2, incDec == 0 ? c(0x80) : c(0x7F))),
                        Expression.Assign(flagN, Expression.Constant(incDec == 1))
                    );
                    if (reg == 0)
                        microExprDD[0x34 + incDec] = block;
                    else
                        microExprFD[0x34 + incDec] = block;
                }

            // INC <reg16>
            for (int reg = 0; reg < 4; reg++)
            {
                var opcode = reg * 16 + 0x03;
                microExpr[opcode] =
                    writeReg16[reg](Expression.Increment(readReg16[reg]));
            }

            // DEC <reg16>
            for (int reg = 0; reg < 4; reg++)
            {
                var opcode = reg * 16 + 0x0B;
                microExpr[opcode] =
                    writeReg16[reg](Expression.Decrement(readReg16[reg]));
            }

            // ADD(C) A, <reg8>
            // ADD(C) A, n
            for (int reg = 0; reg < 18; reg++)
            {
                // 0..7  -> 0x80..0x87: ADD A,s    (s: B, C, D, E, H, L, (HL), A)
                // 0..15 -> 0x88..0x8F: ADC A,s    
                // 16    -> 0xC6:       ADD A,n
                // 17    -> 0xCE:       ADC A,n
                var opcode = reg < 16 ? reg + 0x80 : (reg - 16) * 8 + 0xC6;
                var withCarry = reg >= 8 && reg != 16;
                microExpr[opcode] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    Expression.Assign(temp1, regA),
                    Expression.Assign(temp2, reg < 16 ? readReg8[reg % 8] : readImmediateByte),
                    Expression.Assign(temp3, withCarry
                        ? Expression.Add(Expression.Add(temp1, temp2), carry)
                        : (Expression)Expression.Add(temp1, temp2)
                    ),
                    Expression.Assign(regA, temp3),
                    Expression.Assign(flagHC, bit(
                        withCarry
                        ? Expression.Add(Expression.Add(Expression.And(temp1, c(0x0F)), Expression.And(temp2, c(0x0F))), carry)
                        : Expression.Add(Expression.And(temp1, c(0x0F)), Expression.And(temp2, c(0x0F))),
                        4
                    )),
                    Expression.Assign(flagCY, Expression.GreaterThan(temp3, c(0xFF))),
                    Expression.Assign(flagS, bit(temp3, 7)),
                    Expression.Assign(flagX1, bit(temp3, 3)),
                    Expression.Assign(flagX2, bit(temp3, 5)),
                    Expression.Assign(flagZ, isZero(temp3)),
                    Expression.Assign(flagPV, Expression.AndAlso(
                        Expression.Equal(
                            Expression.And(temp1, c(0x80)),
                            Expression.And(temp2, c(0x80))
                        ),
                        Expression.NotEqual(
                            Expression.And(temp1, c(0x80)),
                            Expression.And(temp3, c(0x80))
                        )
                    )),
                    Expression.Assign(flagN, c(false))
                );
            }

            // SUB(C) A, <reg8>
            // SUB(C) A, n
            for (int reg = 0; reg < 18; reg++)
            {
                // 0..7  -> 0x90..0x97: SUB A,s    (s: B, C, D, E, H, L, (HL), A)
                // 0..15 -> 0x98..0x9F: SBC A,s    
                // 16    -> 0xD6:       SUB A,n
                // 17    -> 0xDE:       SBC A,n
                var opcode = reg < 16 ? reg + 0x90 : (reg - 16) * 8 + 0xD6;
                var withCarry = reg >= 8 && reg != 16;
                microExpr[opcode] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    Expression.Assign(temp1, regA),
                    Expression.Assign(temp2, reg < 16 ? readReg8[reg % 8] : readImmediateByte),
                    Expression.Assign(temp3, withCarry
                        ? Expression.Subtract(Expression.Subtract(temp1, temp2), carry)
                        : Expression.Subtract(temp1, temp2)),
                    Expression.Assign(regA, temp3),
                    Expression.Assign(flagHC, bit(
                        withCarry
                        ? Expression.Subtract(Expression.Subtract(Expression.And(temp1, c(0x0F)), Expression.And(temp2, c(0x0F))), carry)
                        : Expression.Subtract(Expression.And(temp1, c(0x0F)), Expression.And(temp2, c(0x0F))),
                        4
                    )),
                    Expression.Assign(flagCY, Expression.LessThan(temp3, c(0x00))),
                    Expression.Assign(flagS, bit(temp3, 7)),
                    Expression.Assign(flagX1, bit(temp3, 3)),
                    Expression.Assign(flagX2, bit(temp3, 5)),
                    Expression.Assign(flagZ, isZero(temp3)),
                    Expression.Assign(flagPV, Expression.AndAlso(
                        Expression.NotEqual(
                            Expression.And(temp1, c(0x80)),
                            Expression.And(temp2, c(0x80))
                        ),
                        Expression.NotEqual(
                            Expression.And(temp1, c(0x80)),
                            Expression.And(temp3, c(0x80))
                        )
                    )),
                    Expression.Assign(flagN, c(true))
                );
            }

            // ADD(C) HL, <reg16>
            for (int reg = 0; reg < 8; reg++)
            {
                var code = new List<Expression>
                    {
                        Expression.Assign(temp1, regHL),
                        Expression.Assign(temp2, readReg16[reg % 4]),
                        Expression.Assign(temp3, reg < 4 ? Expression.Add(temp1, temp2) : Expression.Add(Expression.Add(temp1, temp2), carry)),
                        Expression.Assign(regHL, temp3),
                        Expression.Assign(flagCY, Expression.GreaterThan(temp3, c(0xFFFF))),
                        Expression.Assign(flagN, c(false)),
                        Expression.Assign(flagX1, Expression.Equal(Expression.And(temp3, c(0x0800)), c(0x0800))),
                        Expression.Assign(flagHC, Expression.GreaterThan(
                            reg < 4 ?
                                Expression.Add(
                                    Expression.And(temp1, c(0xFFF)),
                                    Expression.And(temp2, c(0xFFF))
                                ) :
                                Expression.Add(
                                    Expression.Add(
                                        Expression.And(temp1, c(0xFFF)),
                                        Expression.And(temp2, c(0xFFF))
                                    ),
                                    carry
                                ),
                            c(0xFFF)
                        )),
                        Expression.Assign(flagX2, Expression.Equal(Expression.And(temp3, c(0x2000)), c(0x2000)))
                    };

                if (reg < 4)
                {
                    microExpr[reg * 0x10 + 0x09] = Expression.Block(
                        new[] { temp1, temp2, temp3 },
                        code
                    );
                }
                else
                {
                    code.Add(
                        Expression.Assign(flagPV, Expression.AndAlso(
                            Expression.Equal(
                                Expression.And(temp1, c(0x8000)),
                                Expression.And(temp2, c(0x8000))
                            ),
                            Expression.NotEqual(
                                Expression.And(temp1, c(0x8000)),
                                Expression.And(temp3, c(0x8000))
                            )
                        ))
                    );
                    code.Add(Expression.Assign(flagZ, Expression.Equal(Expression.And(temp3, c(0xFFFF)), c(0x0000))));
                    code.Add(Expression.Assign(flagS, Expression.Equal(Expression.And(temp3, c(0x8000)), c(0x8000))));
                    microExprED[(reg % 4) * 0x10 + 0x4A] = Expression.Block(
                        new[] { temp1, temp2, temp3 },
                        code
                    );
                }
            }

            // SBC HL, <reg16>
            for (int reg = 0; reg < 4; reg++)
            {
                var code = new List<Expression>
                    {
                        Expression.Assign(temp1, regHL),
                        Expression.Assign(temp2, readReg16[reg]),
                        Expression.Assign(temp3, Expression.Subtract(Expression.Subtract(temp1, temp2), carry)),
                        Expression.Assign(regHL, temp3),
                        Expression.Assign(flagCY, Expression.LessThan(temp3, c(0x0000))),
                        Expression.Assign(flagN, c(true)),
                        Expression.Assign(flagPV, Expression.AndAlso(
                            Expression.NotEqual(
                                Expression.And(temp1, c(0x8000)),
                                Expression.And(temp2, c(0x8000))
                            ),
                            Expression.NotEqual(
                                Expression.And(temp1, c(0x8000)),
                                Expression.And(temp3, c(0x8000))
                            )
                        )),
                        Expression.Assign(flagX1, Expression.Equal(Expression.And(temp3, c(0x0800)), c(0x0800))),
                        Expression.Assign(flagHC, Expression.LessThan(
                            Expression.Subtract(
                                Expression.Subtract(
                                    Expression.And(temp1, c(0xFFF)),
                                    Expression.And(temp2, c(0xFFF))
                                ),
                                carry
                            ),
                            c(0x0000)
                        )),
                        Expression.Assign(flagX2, Expression.Equal(Expression.And(temp3, c(0x2000)), c(0x2000))),
                        Expression.Assign(flagS, Expression.Equal(Expression.And(temp3, c(0x8000)), c(0x8000))),
                        Expression.Assign(flagZ, Expression.Equal(Expression.And(temp3, c(0xFFFF)), c(0x0000)))
                    };

                microExprED[reg * 0x10 + 0x42] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    code
                );
            }

            // AND A, <reg8>
            // AND A, n
            for (int reg = 0; reg < 9; reg++)
            {
                // 0..7  -> 0xA0..0xA7: AND A,s    (s: B, C, D, E, H, L, (HL), A)
                // 8     -> 0xE6:       AND A,n
                var opcode = reg < 8 ? reg + 0xA0 : 0xE6;
                microExpr[opcode] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    Expression.Assign(temp1, regA),
                    Expression.Assign(temp2, reg < 8 ? readReg8[reg] : readImmediateByte),
                    Expression.Assign(temp3, Expression.And(temp1, temp2)),
                    Expression.Assign(regA, temp3),
                    Expression.Assign(flagCY, c(false)),
                    Expression.Assign(flagHC, c(true)),
                    Expression.Assign(flagS, bit(temp3, 7)),
                    Expression.Assign(flagX1, bit(temp3, 3)),
                    Expression.Assign(flagX2, bit(temp3, 5)),
                    Expression.Assign(flagZ, isZero(temp3)),
                    Expression.Assign(flagPV, getParity(temp3)),
                    Expression.Assign(flagN, c(false))
                );
            }

            // OR A, <reg8>
            // OR A, n
            for (int reg = 0; reg < 9; reg++)
            {
                // 0..7  -> 0xB0..0xB7: OR A,s    (s: B, C, D, E, H, L, (HL), A)
                // 8     -> 0xF6:       OR A,n
                var opcode = reg < 8 ? reg + 0xB0 : 0xF6;
                microExpr[opcode] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    Expression.Assign(temp1, regA),
                    Expression.Assign(temp2, reg < 8 ? readReg8[reg] : readImmediateByte),
                    Expression.Assign(temp3, Expression.Or(temp1, temp2)),
                    Expression.Assign(regA, temp3),
                    Expression.Assign(flagCY, c(false)),
                    Expression.Assign(flagHC, c(false)),
                    Expression.Assign(flagS, bit(temp3, 7)),
                    Expression.Assign(flagX1, bit(temp3, 3)),
                    Expression.Assign(flagX2, bit(temp3, 5)),
                    Expression.Assign(flagZ, isZero(temp3)),
                    Expression.Assign(flagPV, getParity(temp3)),
                    Expression.Assign(flagN, c(false))
                );
            }

            // XOR A, <reg8>
            // XOR A, n
            for (int reg = 0; reg < 9; reg++)
            {
                // 0..7  -> 0xB0..0xB7: OR A,s    (s: B, C, D, E, H, L, (HL), A)
                // 8     -> 0xF6:       OR A,n
                var opcode = reg < 8 ? reg + 0xA8 : 0xEE;
                microExpr[opcode] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    Expression.Assign(temp1, regA),
                    Expression.Assign(temp2, reg < 8 ? readReg8[reg] : readImmediateByte),
                    Expression.Assign(temp3, Expression.ExclusiveOr(temp1, temp2)),
                    Expression.Assign(regA, temp3),
                    Expression.Assign(flagCY, c(false)),
                    Expression.Assign(flagHC, c(false)),
                    Expression.Assign(flagS, bit(temp3, 7)),
                    Expression.Assign(flagX1, bit(temp3, 3)),
                    Expression.Assign(flagX2, bit(temp3, 5)),
                    Expression.Assign(flagZ, isZero(temp3)),
                    Expression.Assign(flagPV, getParity(temp3)),
                    Expression.Assign(flagN, c(false))
                );
            }

            // CP A, <reg8>
            // CP A, n
            for (int reg = 0; reg < 9; reg++)
            {
                // 0..7  -> 0xB8..0xBF: CP A,s    (s: B, C, D, E, H, L, (HL), A)
                // 8     -> 0xFE:       CP A,n
                var opcode = reg < 8 ? reg + 0xB8 : 0xFE;
                microExpr[opcode] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    Expression.Assign(temp1, regA),
                    Expression.Assign(temp2, reg < 8 ? readReg8[reg] : readImmediateByte),
                    Expression.Assign(temp3, Expression.Subtract(temp1, temp2)),
                    Expression.Assign(flagHC, bit(
                        Expression.Subtract(Expression.And(temp1, c(0x0F)), Expression.And(temp2, c(0x0F))),
                        4
                    )),
                    Expression.Assign(flagCY, Expression.LessThan(temp3, c(0x00))),
                    Expression.Assign(flagS, bit(temp3, 7)),
                    // Note: X and Y are copied from the argument, not the result!
                    Expression.Assign(flagX1, bit(temp2, 3)),
                    Expression.Assign(flagX2, bit(temp2, 5)),
                    Expression.Assign(flagZ, isZero(temp3)),
                    Expression.Assign(flagPV, Expression.AndAlso(
                        Expression.NotEqual(
                            Expression.And(temp1, c(0x80)),
                            Expression.And(temp2, c(0x80))
                        ),
                        Expression.NotEqual(
                            Expression.And(temp1, c(0x80)),
                            Expression.And(temp3, c(0x80))
                        )
                    )),
                    Expression.Assign(flagN, c(true))
                );
            }

            // SCF
            microExpr[0x37] = Expression.Assign(flagCY, c(true));

            // CCF
            microExpr[0x3F] = Expression.Assign(flagCY, c(false));

            // DAA
            // - If the A register is greater than 0x99, OR the Carry flag is SET, then
            //      The upper four bits of the Correction Factor are set to 6,
            //      and the Carry flag will be SET.
            //   Else
            //      The upper four bits of the Correction Factor are set to 0,
            //      and the Carry flag will be CLEARED.
            // - If the lower four bits of the A register (A AND 0x0F) is greater than 9,
            //   OR the Half-Carry (H) flag is SET, then
            //      The lower four bits of the Correction Factor are set to 6.
            //   Else
            //      The lower four bits of the Correction Factor are set to 0.
            // - This results in a Correction Factor of 0x00, 0x06, 0x60 or 0x66.
            // - If the N flag is CLEAR, then
            //      ADD the Correction Factor to the A register.
            //   Else
            //      SUBTRACT the Correction Factor from the A register.
            // - The Flags are set as follows:
            //      Carry:      Set/clear as in the first step above.
            //      Half-Carry: Set if the correction operation caused a binary carry/borrow
            //                  from bit 3 to bit 4.
            //                  For this purpose, may be calculated as:
            //                  Bit 4 of: A(before) XOR A(after).
            //  S,Z,P,5,3:  Set as for simple logic operations on the resultant A value.
            //  N:          Leave.
            var corrFactor = Expression.Variable(typeof(int));
            microExpr[0x27] = Expression.Block(
                new[] { corrFactor, temp1, temp2 },
                Expression.Assign(temp1, regA),
                Expression.IfThenElse(Expression.OrElse(flagCY, Expression.GreaterThan(temp1, c(0x99))),
                    Expression.Block(
                        Expression.Assign(corrFactor, c(0x60)),
                        Expression.Assign(flagCY, c(true))
                    ),
                    Expression.Block(
                        Expression.Assign(corrFactor, c(0x00)),
                        Expression.Assign(flagCY, c(false))
                    )
                ),
                Expression.IfThen(Expression.OrElse(flagHC, Expression.GreaterThan(Expression.And(temp1, c(0x0F)), c(0x09))),
                    Expression.AddAssign(corrFactor, c(0x06))
                ),
                Expression.Assign(temp2, Expression.Condition(flagN,
                        Expression.Subtract(temp1, corrFactor),
                        Expression.Add(temp1, corrFactor)
                    )
                ),
                Expression.Assign(regA, temp2),
                Expression.Assign(flagHC,
                    Expression.Equal(
                        Expression.And(
                            Expression.ExclusiveOr(temp1, temp2),
                            c(0x10)
                        ),
                        c(0x10)
                    )
                ),
                Expression.Assign(flagPV, getParity(temp2)),
                Expression.Assign(flagS, bit(temp2, 7)),
                Expression.Assign(flagX1, bit(temp2, 3)),
                Expression.Assign(flagX2, bit(temp2, 5)),
                Expression.Assign(flagZ, isZero(temp2))
            );

            // NEG
            for (int n = 0; n < 8; n++)
            {
                microExprED[0x44 + n * 8] =
                    Expression.Assign(regA, Expression.Negate(regA));
            }
        }

        private void BuildRotateAndBit()
        {
            var temp1 = Expression.Variable(typeof(int), "temp1");
            var temp2 = Expression.Variable(typeof(int), "temp2");
            var temp3 = Expression.Variable(typeof(int), "temp3");

            // RLCA
            microExpr[0x07] = Expression.Block(
                new[] { temp1 },
                Expression.Assign(temp1, regA),
                Expression.Assign(regA, Expression.Or(
                    Expression.LeftShift(temp1, c(1)),
                    Expression.RightShift(temp1, c(7))
                )),
                Expression.Assign(flagCY, bit(temp1, 7))
            );

            // RRCA
            microExpr[0x0F] = Expression.Block(
                new[] { temp1 },
                Expression.Assign(temp1, regA),
                Expression.Assign(regA, Expression.Or(
                    Expression.RightShift(temp1, c(1)),
                    Expression.LeftShift(temp1, c(7))
                )),
                Expression.Assign(flagCY, bit(temp1, 0)),
                Expression.Assign(flagHC, c(false)),
                Expression.Assign(flagN, c(false))
            );

            // RLA
            microExpr[0x17] = Expression.Block(
                new[] { temp1, temp2 },
                Expression.Assign(temp1, regA),
                Expression.Assign(temp2, Expression.Condition(
                    flagCY,
                    Expression.Or(
                        Expression.LeftShift(temp1, c(1)),
                        c(0x01)
                    ),
                    Expression.LeftShift(temp1, c(1))
                )),
                Expression.Assign(regA, temp2),
                Expression.Assign(flagCY, bit(temp1, 7))
            );

            // RRA
            microExpr[0x1F] = Expression.Block(
                new[] { temp1, temp2 },
                Expression.Assign(temp1, regA),
                Expression.Assign(temp2, Expression.Condition(
                    flagCY,
                    Expression.Or(
                        Expression.RightShift(temp1, c(1)),
                        c(0x80)
                    ),
                    Expression.RightShift(temp1, c(1))
                )),
                Expression.Assign(regA, temp2),
                Expression.Assign(flagCY, bit(temp1, 0))
            );

            // RLD
            microExprED[0x6F] = Expression.Block(
                new[] { temp1, temp2, temp3 },
                Expression.Assign(temp1, regA),
                Expression.Assign(temp2, Expression.Or(
                        readByte(regHL),
                        Expression.LeftShift(temp1, c(8))
                    )
                ),
                Expression.Assign(temp2,
                    Expression.Or(
                        Expression.LeftShift(
                            temp2,
                            c(4)
                        ),
                        Expression.RightShift(
                            Expression.And(
                                temp2,
                                c(0x0F00)
                            ),
                            c(8)
                        )
                    )
                ),
                writeByte(regHL, temp2),
                Expression.Assign(temp3,
                    Expression.Or(
                        Expression.And(
                            temp1,
                            c(0xF0)
                        ),
                        Expression.And(
                            Expression.RightShift(
                                temp2,
                                c(8)
                            ),
                            c(0x0F)
                        )
                    )
                ),
                Expression.Assign(regA, temp3),
                Expression.Assign(flagS, bit(temp3, 7)),
                Expression.Assign(flagX1, bit(temp3, 3)),
                Expression.Assign(flagX2, bit(temp3, 5)),
                Expression.Assign(flagZ, isZero(temp3)),
                Expression.Assign(flagHC, c(false)),
                Expression.Assign(flagN, c(false)),
                Expression.Assign(flagPV, getParity(temp3))
            );

            // RRD
            microExprED[0x67] = Expression.Block(
                new[] { temp1, temp2, temp3 },
                Expression.Assign(temp1, regA),
                Expression.Assign(temp2, Expression.Or(
                        readByte(regHL),
                        Expression.LeftShift(temp1, c(8))
                    )
                ),
                Expression.Assign(temp2,
                    Expression.Or(
                        Expression.And(
                            Expression.RightShift(
                                temp2,
                                c(4)
                            ),
                            c(0x00FF)
                        ),
                        Expression.LeftShift(
                            Expression.And(
                                temp2,
                                c(0x000F)
                            ),
                            c(8)
                        )
                    )
                ),
                writeByte(regHL, temp2),
                Expression.Assign(temp3,
                    Expression.Or(
                        Expression.And(
                            temp1,
                            c(0xF0)
                        ),
                        Expression.And(
                            Expression.RightShift(
                                temp2,
                                c(8)
                            ),
                            c(0x0F)
                        )
                    )
                ),
                Expression.Assign(regA, temp3),
                Expression.Assign(flagS, bit(temp3, 7)),
                Expression.Assign(flagX1, bit(temp3, 3)),
                Expression.Assign(flagX2, bit(temp3, 5)),
                Expression.Assign(flagZ, isZero(temp3)),
                Expression.Assign(flagHC, c(false)),
                Expression.Assign(flagN, c(false)),
                Expression.Assign(flagPV, getParity(temp3))
            );
        }

        private void BuildLoopInstuctions()
        {
            var temp1 = Expression.Variable(typeof(int), "temp1");
            var temp2 = Expression.Variable(typeof(int), "temp2");
            var temp3 = Expression.Variable(typeof(int), "temp3");
            var temp4 = Expression.Variable(typeof(int), "temp4");

            // LDI(R)
            // LDD(R)
            for (int n = 0; n < 4; n++)
            {
                var list = new List<Expression>
                    {
                        writeByte(regDE, readByte(regHL)),
                        Expression.Assign(regDE, ((n & 1) == 0 ?
                            Expression.Increment(regDE) :
                            Expression.Decrement(regDE)
                        )),
                        Expression.Assign(regHL, (n & 1) == 0 ?
                            Expression.Increment(regHL) :
                            Expression.Decrement(regHL)
                        ),
                        Expression.Assign(regBC, Expression.Decrement(regBC)),
                        Expression.Assign(flagHC, c(false)),
                        Expression.Assign(flagN, c(false)),
                        Expression.Assign(flagPV, Expression.NotEqual(regBC, c(0)))
                    };

                // the (R) part...
                if (n >= 2)
                    list.Add(Expression.IfThenElse(flagPV, Expression.Assign(regPC, Expression.Subtract(regPC, c(2))), takeStatesLow));

                microExprED[0xA0 + n * 8] = Expression.Block(
                    list
                );
            }

            // CPI(R)
            // CPD(R) 
            for (int n = 0; n < 4; n++)
            {
                // 0     -> 0xED 0xA1:  CPI
                // 1     -> 0xED 0xA9:  CPD
                // 2     -> 0xED 0xB1:  CPIR
                // 3     -> 0xED 0xB9:  CPDR      
                var opcode = (n << 3) | 0xA1;
                var expr = new List<Expression>
                {
                    Expression.Assign(temp1, regA),
                    Expression.Assign(temp2, readByte(regHL)),
                    Expression.Assign(temp3, Expression.Subtract(temp1, temp2)),
                    Expression.Assign(regHL, (n & 1) == 0 ?
                        Expression.Increment(regHL) :
                        Expression.Decrement(regHL)
                    ),
                    Expression.Assign(regBC, Expression.Decrement(regBC)),
                    Expression.Assign(flagHC, bit(
                        Expression.Subtract(Expression.And(temp1, c(0x0F)), Expression.And(temp2, c(0x0F))),
                        4
                    )),
                    Expression.Assign(flagS, bit(temp3, 7)),
                    // Note: X1 is bit 3 of (A-(HL)-HC); X2 is bit 1 (!)
                    // of this value. (HL) is the value of (HL) before the
                    // instruction, whilst HC is the value of HC after the
                    // instruction!  (see https://worldofspectrum.org/faq/reference/z80reference.htm)
                    Expression.Assign(temp4, Expression.Condition(flagHC, Expression.Subtract(temp3, c(1)), temp3)),
                    Expression.Assign(flagX1, bit(temp4, 3)),
                    Expression.Assign(flagX2, bit(temp4, 1)),
                    Expression.Assign(flagZ, isZero(temp3)),
                    Expression.Assign(flagPV, Expression.NotEqual(regBC, c(0))),
                    Expression.Assign(flagN, c(true))
                };
                if (n >= 2)
                {
                    // CPIR/CPDR: Decrement PC by 2 when no true compare
                    expr.Add(Expression.IfThen(
                        Expression.AndAlso(
                            Expression.NotEqual(regBC, c(0)),
                            Expression.Not(flagZ)
                        ),
                        Expression.Assign(regPC, Expression.Subtract(regPC, c(2)))
                    ));
                }
                microExprED[opcode] = Expression.Block(new ParameterExpression[] { temp1, temp2, temp3, temp4 }, expr);
            }

            // INI(R)
            // IND(R)
            for (int n = 0; n < 4; n++)
            {
                var list = new List<Expression>
                    {
                        Expression.Assign(temp1, readInput(regBC)),
                        writeByte(regHL, temp1),
                        Expression.Assign(regHL, (n & 1) == 0 ?
                            Expression.Increment(regHL) :
                            Expression.Decrement(regHL)
                        ),
                        Expression.Assign(temp3, Expression.Decrement(regB)),
                        Expression.Assign(regB, temp3),
                        Expression.Assign(temp2, Expression.Add(temp1, Expression.And((n & 1) == 0 ?
                            Expression.Increment(regC) :
                            Expression.Decrement(regC),
                            c(0xFF)))),
                        Expression.Assign(flagHC, Expression.GreaterThan(temp2, c(0xFF))),
                        Expression.Assign(flagCY, Expression.GreaterThan(temp2, c(0xFF))),
                        Expression.Assign(flagPV, getParity(Expression.ExclusiveOr(Expression.And(temp2, c(0x07)), regB))),
                        Expression.Assign(flagS, bit(temp3, 7)),
                        Expression.Assign(flagX1, bit(temp3, 3)),
                        Expression.Assign(flagX2, bit(temp3, 5)),
                        Expression.Assign(flagZ, isZero(temp3)),
                        Expression.Assign(flagN, bit(temp1, 7))
                    };

                // the (R) part...
                if (n >= 2)
                    list.Add(Expression.IfThenElse(
                        Expression.NotEqual(temp3, c(0)),
                        Expression.Assign(regPC, Expression.Subtract(regPC, c(2))),
                        takeStatesLow
                ));

                microExprED[0xA2 + n * 8] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    list
                );
            }

            // OUTI/OTIR
            // OUTD/OTDR
            for (int n = 0; n < 4; n++)
            {
                var list = new List<Expression>
                    {
                        Expression.Assign(temp1, readByte(regHL)),
                        Expression.Assign(temp3, Expression.Decrement(regB)),
                        Expression.Assign(regB, temp3),
                        writeOutput(regBC, temp1),
                        Expression.Assign(regHL, (n & 1) == 0 ?
                            Expression.Increment(regHL) :
                            Expression.Decrement(regHL)
                        ),
                        Expression.Assign(temp2, Expression.Add(temp1, regL)),
                        Expression.Assign(flagHC, Expression.GreaterThan(temp2, c(0xFF))),
                        Expression.Assign(flagCY, Expression.GreaterThan(temp2, c(0xFF))),
                        Expression.Assign(flagPV, getParity(Expression.ExclusiveOr(Expression.And(temp2, c(0x07)), regB))),
                        Expression.Assign(flagS, bit(temp3, 7)),
                        Expression.Assign(flagX1, bit(temp3, 3)),
                        Expression.Assign(flagX2, bit(temp3, 5)),
                        Expression.Assign(flagZ, isZero(temp3)),
                        Expression.Assign(flagN, bit(temp1, 7))
                    };

                // the (R) part...
                if (n >= 2)
                    list.Add(Expression.IfThenElse(
                        Expression.NotEqual(temp3, c(0)),
                        Expression.Assign(regPC, Expression.Subtract(regPC, c(2))),
                        takeStatesLow
                    ));

                microExprED[0xA3 + n * 8] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    list
                );
            }
        }

        private void BuildMisc()
        {
            var temp1 = Expression.Variable(typeof(int));
            var temp2 = Expression.Variable(typeof(int));
            var temp3 = Expression.Variable(typeof(int));
            var temp4 = Expression.Variable(typeof(int));

            // CPL
            microExpr[0x2F] = Expression.Block(
                new[] { temp1 },
                Expression.Assign(temp1, Expression.ExclusiveOr(regA, c(0xFF))),
                Expression.Assign(regA, temp1),
                Expression.Assign(flagHC, c(true)),
                Expression.Assign(flagN, c(true)),
                Expression.Assign(flagX1, bit(temp1, 3)),
                Expression.Assign(flagX2, bit(temp1, 5))
            );

            // DI
            microExpr[0xF3] = Expression.Block(
                Expression.Assign(iff1, c(false)),
                Expression.Assign(iff2, c(false))
            );

            // EI
            var maskInterruptsNextProp = GetMember<Z80Registers>(regs, p => p.MaskInterruptsNext);
            microExpr[0xFB] = Expression.Block(
                Expression.Assign(iff1, c(true)),
                Expression.Assign(iff2, c(true)),
                Expression.Assign(maskInterruptsNextProp, c(true))  // Interrupts are masked after execution of EI
            );

            // IM0
            microExprED[0x46] = microExprED[0x4E] = microExprED[0x66] = microExprED[0x6E] =
                Expression.Assign(im, c(0));

            // IM1
            microExprED[0x56] = microExprED[0x76] =
                Expression.Assign(im, c(1));

            // IM2
            microExprED[0x5E] = microExprED[0x7E] =
                Expression.Assign(im, c(2));

            // EXX
            Expression<Action<Z80Registers>> call = rg => rg.Exx();
            microExpr[0xD9] = Expression.Call(regs, ((call as LambdaExpression).Body as MethodCallExpression).Method);

            // EX AF, AF'
            call = rg => rg.ExAf();
            microExpr[0x08] = Expression.Call(regs, ((call as LambdaExpression).Body as MethodCallExpression).Method);

            // EX (SP), HL
            microExpr[0xE3] = Expression.Block(
                new[] { temp3, temp4 },
                Expression.Assign(temp3, regSP),
                Expression.Assign(temp4, readByte(temp3)),
                writeByte(Expression.PostIncrementAssign(temp3), regL),
                Expression.Assign(regL, temp4),
                Expression.Assign(temp4, readByte(temp3)),
                writeByte(temp3, regH),
                Expression.Assign(regH, temp4)
            );

            // EX DE, HL
            microExpr[0xEB] = Expression.Block(
                new[] { temp3 },
                Expression.Assign(temp3, regDE),
                Expression.Assign(regDE, regHL),
                Expression.Assign(regHL, temp3)
            );

            // HALT
            microExpr[0x76] = Expression.Block(
                Expression.Assign(regPC, Expression.Decrement(regPC)),
                Expression.Assign(GetMember<Z80Registers>(regs, p => p.Halted), c(true))
            );
        }

        private void BuildInputOutput()
        {
            // IN A, (n)
            microExpr[0xDB] = Expression.Assign(regA,
                readInput(
                    Expression.Or(
                        readImmediateByte,
                        Expression.LeftShift(
                            regA,
                            c(8)
                        )
                    )
                )
            );

            // IN <reg8>, (C)
            var temp1 = Expression.Variable(typeof(int));
            for (int reg = 0; reg < 8; reg++)
            {
                microExprED[0x40 + reg * 8] = Expression.Block(
                    new[] { temp1 },
                    Expression.Assign(temp1, readInput(regBC)),
                    reg == 6 ? Expression.Assign(temp1, temp1) : Expression.Assign(readReg8[reg], temp1),
                    Expression.Assign(flagS, bit(temp1, 7)),
                    Expression.Assign(flagX1, bit(temp1, 3)),
                    Expression.Assign(flagX2, bit(temp1, 5)),
                    Expression.Assign(flagZ, isZero(temp1)),
                    Expression.Assign(flagHC, c(false)),
                    Expression.Assign(flagPV, getParity(temp1)),
                    Expression.Assign(flagN, c(false))
                );
            }

            // OUT (n), A
            microExpr[0xD3] = writeOutput(
                Expression.Or(
                    readImmediateByte,
                    Expression.LeftShift(
                        regA,
                        c(8)
                    )
                ),
                regA
            );

            // OUT (C), <reg8>
            for (int reg = 0; reg < 8; reg++)
            {
                microExprED[0x41 + reg * 8] = writeOutput(
                        regBC,
                        reg == 6 ? Expression.Constant((byte)0) : readReg8[reg]
                );
            }
        }


        private void BuildCBxx(Expression[] microExprCBxx, MemberExpression regXX = null, ParameterExpression dispPar = null)
        {
            var input = Expression.Variable(typeof(int));
            var output = Expression.Variable(typeof(int));

            for (int reg = 0; reg < 8; reg++)
            {
                // Read either from register or from (IX+d) / (IY + d)
                Expression read(Expression data) => Expression.Assign(data, regXX == null
                        ? readReg8[reg]
                        : readByte(Expression.Add(regXX, SignExtend(dispPar)))
                    );

                // Write either to register or to both register and (IX+d) / (IY + d)
                Expression write(Expression data) =>
                    regXX == null
                    ? writeReg8[reg](data)
                    : reg == 6     // when reg = (HL), write only to (IX + d) / (IY + d)
                    ? writeByte(Expression.Add(regXX, SignExtend(dispPar)), data)
                    :   // In other cases write both to the register and (IX + d) / (IY + d) (undocumented)
                        Expression.Block(
                            writeByte(Expression.Add(regXX, SignExtend(dispPar)), data),
                            writeReg8[reg](data)
                        );

                // RLC <reg8>
                microExprCBxx[reg] = Expression.Block(
                    new[] { input, output },
                    read(input),
                    Expression.Assign(output,
                        Expression.Or(
                            Expression.LeftShift(input, c(1)),
                            Expression.RightShift(input, c(7))
                        )
                    ),
                    write(output),
                    Expression.Assign(flagCY, bit(input, 7)),
                    Expression.Assign(flagS, bit(output, 7)),
                    Expression.Assign(flagX1, bit(output, 3)),
                    Expression.Assign(flagX2, bit(output, 5)),
                    Expression.Assign(flagZ, isZero(output)),
                    Expression.Assign(flagHC, c(false)),
                    Expression.Assign(flagN, c(false)),
                    Expression.Assign(flagPV, getParity(output))
                );

                // RRC <reg8>
                microExprCBxx[0x08 + reg] = Expression.Block(
                    new[] { input, output },
                    read(input),
                    Expression.Assign(output,
                        Expression.Or(
                            Expression.RightShift(input, c(1)),
                            Expression.LeftShift(input, c(7))
                        )
                    ),
                    writeReg8[reg](output),
                    Expression.Assign(flagCY, bit(input, 0)),
                    Expression.Assign(flagS, bit(output, 7)),
                    Expression.Assign(flagX1, bit(output, 3)),
                    Expression.Assign(flagX2, bit(output, 5)),
                    Expression.Assign(flagZ, isZero(output)),
                    Expression.Assign(flagHC, c(false)),
                    Expression.Assign(flagN, c(false)),
                    Expression.Assign(flagPV, getParity(output))
                );

                // RL <reg8>
                microExprCBxx[0x10 + reg] = Expression.Block(
                    new[] { input, output },
                    read(input),
                    Expression.Assign(output,
                        Expression.Condition(
                            flagCY,
                            Expression.Or(
                                Expression.LeftShift(input, c(1)),
                                c(0x01)
                            ),
                            Expression.LeftShift(input, c(1))
                        )
                    ),
                    writeReg8[reg](output),
                    Expression.Assign(flagCY, bit(input, 7)),
                    Expression.Assign(flagS, bit(output, 7)),
                    Expression.Assign(flagX1, bit(output, 3)),
                    Expression.Assign(flagX2, bit(output, 5)),
                    Expression.Assign(flagZ, isZero(output)),
                    Expression.Assign(flagHC, c(false)),
                    Expression.Assign(flagN, c(false)),
                    Expression.Assign(flagPV, getParity(output))
                );

                // RR <reg8>
                microExprCBxx[0x18 + reg] = Expression.Block(
                    new[] { input, output },
                    read(input),
                    Expression.Assign(output,
                        Expression.Condition(
                            flagCY,
                            Expression.Or(
                                Expression.RightShift(input, c(1)),
                                c(0x80)
                            ),
                            Expression.RightShift(input, c(1))
                        )
                    ),
                    writeReg8[reg](output),
                    Expression.Assign(flagCY, bit(input, 0)),
                    Expression.Assign(flagS, bit(output, 7)),
                    Expression.Assign(flagX1, bit(output, 3)),
                    Expression.Assign(flagX2, bit(output, 5)),
                    Expression.Assign(flagZ, isZero(output)),
                    Expression.Assign(flagHC, c(false)),
                    Expression.Assign(flagN, c(false)),
                    Expression.Assign(flagPV, getParity(Expression.And(output, c(0xFF))))
                );

                // SLA <reg8>
                microExprCBxx[0x20 + reg] = Expression.Block(
                    new[] { input, output },
                    read(input),
                    Expression.Assign(output, Expression.LeftShift(input, c(1))),
                    writeReg8[reg](output),
                    Expression.Assign(flagCY, bit(input, 7)),
                    Expression.Assign(flagS, bit(output, 7)),
                    Expression.Assign(flagX1, bit(output, 3)),
                    Expression.Assign(flagX2, bit(output, 5)),
                    Expression.Assign(flagZ, isZero(output)),
                    Expression.Assign(flagHC, c(false)),
                    Expression.Assign(flagN, c(false)),
                    Expression.Assign(flagPV, getParity(output))
                );

                // SRA <reg8>
                microExprCBxx[0x28 + reg] = Expression.Block(
                    new[] { input, output },
                    read(input),
                    Expression.Assign(output, Expression.Or(
                        Expression.RightShift(input, c(1)),
                        Expression.And(input, c(0x80))
                    )),
                    writeReg8[reg](output),
                    Expression.Assign(flagCY, bit(input, 0)),
                    Expression.Assign(flagS, bit(output, 7)),
                    Expression.Assign(flagX1, bit(output, 3)),
                    Expression.Assign(flagX2, bit(output, 5)),
                    Expression.Assign(flagZ, isZero(output)),
                    Expression.Assign(flagHC, c(false)),
                    Expression.Assign(flagN, c(false)),
                    Expression.Assign(flagPV, getParity(output))
                );

                // SLL <reg8>
                microExprCBxx[0x30 + reg] = Expression.Block(
                    new[] { input, output },
                    read(input),
                    Expression.Assign(output, Expression.Or(
                        Expression.LeftShift(input, c(1)),
                        c(0x01)
                    )),
                    writeReg8[reg](output),
                    Expression.Assign(flagCY, bit(input, 7)),
                    Expression.Assign(flagS, bit(output, 7)),
                    Expression.Assign(flagX1, bit(output, 3)),
                    Expression.Assign(flagX2, bit(output, 5)),
                    Expression.Assign(flagZ, isZero(output)),
                    Expression.Assign(flagHC, c(false)),
                    Expression.Assign(flagN, c(false)),
                    Expression.Assign(flagPV, getParity(output))
                );

                // SRL <reg8>
                microExprCBxx[0x38 + reg] = Expression.Block(
                    new[] { input, output },
                    read(input),
                    Expression.Assign(output, Expression.RightShift(input, c(1))),
                    writeReg8[reg](output),
                    Expression.Assign(flagCY, bit(input, 0)),
                    Expression.Assign(flagS, bit(output, 7)),
                    Expression.Assign(flagX1, bit(output, 3)),
                    Expression.Assign(flagX2, bit(output, 5)),
                    Expression.Assign(flagZ, isZero(output)),
                    Expression.Assign(flagHC, c(false)),
                    Expression.Assign(flagN, c(false)),
                    Expression.Assign(flagPV, getParity(output))
                );

                // BIT <bit>, <reg>
                for (int bitNr = 0; bitNr < 8; bitNr++)
                {
                    microExprCBxx[bitNr * 8 + reg + 0x40] = Expression.Block(
                        new[] { input },
                        read(input),
                        Expression.Assign(flagZ, Expression.Equal(Expression.And(input, Expression.Constant(1 << bitNr)), c(0x00))),
                        Expression.Assign(flagPV, flagZ),
                        Expression.Assign(flagHC, c(true)),
                        Expression.Assign(flagN, c(false)),
                        Expression.Assign(flagS, bitNr == 7 ? bit(input, 7) : c(false))
                    );
                }

                // RES <bit>, <reg>
                for (int bitNr = 0; bitNr < 8; bitNr++)
                {
                    microExprCBxx[bitNr * 8 + reg + 0x80] =
                        Expression.Block(
                        new[] { input, output },
                        read(input),
                        Expression.Assign(output, Expression.And(input, Expression.Constant(~(1 << bitNr)))),
                        write(output)
                    );
                }


                // SET <bit>, <reg>
                for (int bitNr = 0; bitNr < 8; bitNr++)
                {
                    microExprCBxx[bitNr * 8 + reg + 0xC0] =
                        Expression.Block(
                        new[] { input, output },
                        read(input),
                        Expression.Assign(output, Expression.Or(input, Expression.Constant(1 << bitNr))),
                        write(output)
                    );
                }
            }
        }

        private void BuildInterrupts()
        {
            var dataOnBusPar = Expression.Parameter(typeof(byte), "dataOnBus");
            var timingProp = GetMember<Z80Registers>(regs, p => p.Timing);
            var haltedProp = GetMember<Z80Registers>(regs, p => p.Halted);
            var nextOpcodeProp = GetMember<Z80Registers>(regs, p => p.NextOpcode);
            var statesNormalProp = Expression.Property(timingProp, "StatesNormal");
            var statesLowProp = Expression.Property(timingProp, "StatesLow");
            handleIntExpr =
                Expression.Block(
                    new[] { dataOnBusPar },
                    Expression.Assign(regR, Expression.Increment(regR)),
                    Expression.IfThen(haltedProp, Expression.Block(
                        Expression.Assign(regPC, Expression.Increment(regPC)),   // Bail out of HALT instruction
                        Expression.Assign(haltedProp, c(false))
                    )),
                    Expression.Assign(iff1, c(false)),
                    Expression.Assign(iff2, c(false)),
                    Expression.IfThenElse(Expression.Equal(im, c(0)),
                        Expression.Block(       // Interrupt modus 0 - read next opcode from databus
                            Expression.Assign(nextOpcodeProp, Expression.Convert(dataOnBusPar, typeof(int?))),
                            Expression.Assign(statesNormalProp, c(13)),
                            Expression.Assign(statesLowProp, c(13))
                        ),
                        Expression.IfThenElse(Expression.Equal(im, c(1)),
                            Expression.Block(       // Interrupt modus 1 - next opcode is a RST 0x0038
                                Expression.Assign(nextOpcodeProp, Expression.Convert(c(0xFF), typeof(int?))),   // RST 0x0038
                                Expression.Assign(statesNormalProp, c(13)),
                                Expression.Assign(statesLowProp, c(13))
                            ),
                            Expression.Block(       // Interrupt modus 2
                                push(regPC),
                                Expression.Assign(nextOpcodeProp, Expression.Convert(readByte(Expression.Or(Expression.LeftShift(regI, c(8)), cInt(dataOnBusPar))), typeof(int?))),
                                Expression.Assign(statesNormalProp, c(19)),
                                Expression.Assign(statesLowProp, c(19))
                            )
                        )
                    )
                );
            handleInt = Expression.Lambda<Action<Z80Emulator, byte>>(handleIntExpr, par, dataOnBusPar).Compile();

            handleNmiExpr =
                Expression.Block(
                    new[] { dataOnBusPar },
                    Expression.Assign(regR, Expression.Increment(regR)),
                    Expression.IfThen(haltedProp, Expression.Block(
                        Expression.Assign(regPC, Expression.Increment(regPC)),   // Bail out of HALT instruction
                        Expression.Assign(haltedProp, c(false))
                    )),
                    Expression.Assign(iff2, iff1),
                    Expression.Assign(iff1, c(false)),
                    push(regPC),
                    Expression.Assign(regPC, c(0x0066)),
                    Expression.Assign(statesNormalProp, c(11)),
                    Expression.Assign(statesLowProp, c(11))
                );
            handleNmi = Expression.Lambda<Action<Z80Emulator, byte>>(handleNmiExpr, par, dataOnBusPar).Compile();
        }

        #region Microcode

        public Action<Z80Emulator>[] MicroCode
        {
            get { return microCode; }
        }

        public Action<Z80Emulator>[] MicroCodeCB
        {
            get { return microCodeCB; }
        }

        public Action<Z80Emulator>[] MicroCodeDD
        {
            get { return microCodeDD; }
        }

        public Action<Z80Emulator>[] MicroCodeED
        {
            get { return microCodeED; }
        }

        public Action<Z80Emulator>[] MicroCodeFD
        {
            get { return microCodeFD; }
        }

        public Action<Z80Emulator, int>[] MicroCodeDDCB
        {
            get { return microCodeDDCB; }
        }

        public Action<Z80Emulator, int>[] MicroCodeFDCB
        {
            get { return microCodeFDCB; }
        }

        public Action<Z80Emulator, byte> HandleInt
        {
            get { return handleInt; }
        }

        public Action<Z80Emulator, byte> HandleNmi
        {
            get { return handleNmi; }
        }

        public bool[] ParityTable
        {
            get { return parityTable; }
        }

        #endregion

        #region Expression properties

        public Expression[] MicroExpr
        {
            get { return microExpr; }
        }

        public Expression[] MicroExprCB
        {
            get { return microExprCB; }
        }

        public Expression[] MicroExprDD
        {
            get { return microExprDD; }
        }

        public Expression[] MicroExprED
        {
            get { return microExprED; }
        }

        public Expression[] MicroExprFD
        {
            get { return microExprFD; }
        }

        public Expression[] MicroExprDDCB
        {
            get { return microExprDDCB; }
        }

        public Expression[] MicroExprFDCB
        {
            get { return microExprFDCB; }
        }

        public Expression HandleIntExpr
        {
            get { return handleIntExpr; }
        }

        public Expression HandleNmiExpr
        {
            get { return handleNmiExpr; }
        }

        #endregion
    }

}
