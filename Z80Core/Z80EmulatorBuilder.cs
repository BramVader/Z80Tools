using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Z80Core
{
    internal struct Timing
    {
        public int StatesNormal { get; set; }   // The highest value
        public int StatesLow { get; set; }      // A possible lower value
    }

    internal class Z80EmulatorBuilder
    {
        // Basic opcodes
        // When there are two different possible states, the least one is byte-shifted left
        private int[,] timingA = new int[16, 16]
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
        private int[,] timingB = new int[16, 4]
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
        private int[,] timingC = new int[16, 16]
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

        private Action<Z80Emulator>[] microCode, microCodeCB, microCodeDD, microCodeED, microCodeFD;
        private Action<Z80Emulator, byte>[] microCodeDDCB, microCodeFDCB;
        private Expression[] microExpr, microExprCB, microExprDD, microExprED, microExprFD, microExprDDCB, microExprFDCB;
        private Expression handleIntExpr, handleNmiExpr;
        private Action<Z80Emulator, byte> handleInt, handleNmi;
        private Timing[] timing, timingCB, timingDD, timingED, timingFD, timingDDCB, timingFDCB;
        private ParameterExpression par;
        private MemberExpression readByteProp, writeByteProp;
        private MemberExpression readInputProp, writeOutputProp;
        private Expression readImmediateByte, readImmediateWord, readImmediateOffset;
        private MemberExpression regs;
        private MemberExpression flagCY, flagN, flagPV, flagHC, flagZ, flagS, flagX1, flagX2;
        private MemberExpression iff1, iff2, im;
        private MemberExpression regA, regB, regC, regD, regE, regH, regL;
        private MemberExpression regBC, regDE, regHL, regSP, regPC, regAF;
        private MemberExpression regIXL, regIXH, regIX;
        private MemberExpression regIYL, regIYH, regIY;
        private MemberExpression regI, regR;
        private Expression[] readReg8, readReg16, readReg16Stack;
        private Expression takeStatesLow;
        private Func<Expression, Expression>[] writeReg8, writeReg16, writeReg16Stack;
        private Expression readIndirect8, readIndirect16;
        private Func<Expression, int, Expression> addByte;
        private Func<Expression, int, Expression> addWord;
        private Func<Expression, Expression> cInt, cByte, cWord;
        private Func<Expression, Expression> getParity;
        private bool[] parityTable;
        Func<Expression, Expression> writeIndirect8, writeIndirect16;
        Expression pop;
        Func<Expression, Expression> push;

        /// <summary>
        /// Visitor, constructs a new expression where registers H & L are replaced by IXH & IXL / IYH & IYL
        /// and (HL) is replaced by (IX + d) / (IY + d).
        /// </summary>
        private class IndexRegReplacer : ExpressionVisitor
        {
            private MemberExpression indexReg, indexRegL, indexRegH;
            private Z80EmulatorBuilder builder;
            private Expression indexed;

            public int Opcode { get; set; }

            public IndexRegReplacer(Z80EmulatorBuilder builder,
                                    MemberExpression indexReg, MemberExpression indexRegL, MemberExpression indexRegH)
            {
                this.builder = builder;
                this.indexReg = indexReg;
                this.indexRegL = indexRegL;
                this.indexRegH = indexRegH;

                this.indexed =
                    Expression.Convert(
                        Expression.Add(
                            Expression.Convert(
                                indexReg,
                                typeof(int)
                            ),
                            Expression.Convert(
                                Expression.Convert(
                                    Expression.Invoke(builder.readByteProp, Expression.Convert(Expression.PostIncrementAssign(builder.regPC), typeof(int))),
                                    typeof(sbyte)
                                ),
                                typeof(int)
                            )
                        ),
                        typeof(ushort)
                    );
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
                }
                if (node == builder.regHL)
                {
                    return base.VisitMember(indexReg);
                }
                return base.VisitMember(node);
            }

            protected override Expression VisitInvocation(InvocationExpression node)
            {
                MemberExpression mb1 = node.Expression as MemberExpression;
                if (mb1 != null)
                {
                    MemberExpression mb2 = node.Arguments[0] as MemberExpression;
                    if (mb2 != null && mb2 == builder.regHL)
                    {
                        if (mb1.Member.Name == "WriteMemory")
                        {
                            return VisitInvocation(node.Update(
                                mb1, new[] {
                                    indexed,
                                    node.Arguments[1]
                                })
                            );

                        }
                        else if (mb1.Member.Name == "ReadMemory")
                        {
                            return VisitInvocation(node.Update(
                                mb1, new[] {
                                    indexed
                                })
                            );
                        }
                    }
                }
                return base.VisitInvocation(node);
            }
        }

        private void ReplaceIndexedCB(MemberExpression indexReg, Expression[] microExprXDCB, ParameterExpression dispPar)
        {
            var indexed =
            Expression.Convert(
                Expression.Add(
                    Expression.Convert(
                        indexReg,
                        typeof(int)
                    ),
                    Expression.Convert(
                        Expression.Convert(
                            dispPar,
                            typeof(sbyte)
                        ),
                        typeof(int)
                    )
                ),
                typeof(ushort)
            );
            var temp1 = Expression.Variable(typeof(ushort));
            for (int n = 0; n < 256; n++)
            {
                var reg = n & 7;
                var expr = microExprCB[n];
                if (expr != null)
                {
                    var blockExpr = expr as BlockExpression;
                    var variables = blockExpr == null ? new List<ParameterExpression>() : blockExpr.Variables.ToList();
                    var list = blockExpr == null ? new List<Expression> { expr } : blockExpr.Expressions.ToList();

                    variables.Add(temp1);
                    list.InsertRange(0, new[] {
                            Expression.Assign(temp1, indexed),
                            writeReg8[reg](Expression.Invoke(readByteProp, Expression.Convert(temp1, typeof(int))))
                        });
                    list.Add(Expression.Invoke(writeByteProp, cInt(temp1), readReg8[reg]));
                    microExprXDCB[n] =
                        Expression.Block(
                            variables,
                            list
                        );
                }
            }
        }

        public Z80EmulatorBuilder()
        {
            Init();
            LoadInstructions();
            StackInstructions();
            ControlStructures();
            Arithmetic();
            RotateAndBit();
            InputOutput();
            LoopInstuctions();
            Interrupts();
            Misc();

            // Convert basic expressions to indexed expressions
            var converter = new IndexRegReplacer(this, regIX, regIXL, regIXH);
            for (int n = 0; n < 256; n++)
            {
                if (n == 0xEB)      // EX DE, HL        : remains unaffected
                {
                    microExprDD[n] = microExpr[n];
                }
                else
                {
                    if (n != 0x34 &&    // INC (IX + d)     : have specific implementation
                        n != 0x35)      // DEC (IX + d)       (would read offset twice otherwise)
                    {
                        converter.Opcode = n;
                        microExprDD[n] = converter.Visit(microExpr[n]);
                    }
                }
            }
            converter = new IndexRegReplacer(this, regIY, regIYL, regIYH);
            for (int n = 0; n < 256; n++)
            {
                if (n == 0xEB)      // EX DE, HL        : remains unaffected
                {
                    microExprFD[n] = microExpr[n];
                }
                else
                {
                    if (n != 0x34 &&    // INC (IY + d)     : have specific implementation
                        n != 0x35)      // DEC (IY + d)       (would read offset twice otherwise)
                    {
                        converter.Opcode = n;
                        microExprFD[n] = converter.Visit(microExpr[n]);
                    }
                }
            }

            var dispPar = Expression.Parameter(typeof(byte), "disp");
            var timingProp = GetMember<Z80Registers>(regs, p => p.Timing);
            ReplaceIndexedCB(regIX, microExprDDCB, dispPar);
            ReplaceIndexedCB(regIY, microExprFDCB, dispPar);

            // NOP's
            Action<Z80Emulator> nop1 = (e) => { Expression.Assign(timingProp, Expression.Constant(timing[0x00])); };
            Action<Z80Emulator, byte> nop2 = (e, f) => { Expression.Assign(timingProp, Expression.Constant(timingDDCB[0x00])); };
            Action<Z80Emulator> nop3 = (e) => { Expression.Assign(timingProp, Expression.Constant(timingDD[0x00])); };

            // Compile indexed prefixed expressions
            for (int n = 0; n < 256; n++)
            {
                microCodeDDCB[n] = microExprDDCB[n] != null ? Expression.Lambda<Action<Z80Emulator, byte>>(Expression.Block(Expression.Assign(timingProp, Expression.Constant(timingDDCB[n])), microExprDDCB[n]), par, dispPar).Compile() : nop2;
                microCodeFDCB[n] = microExprFDCB[n] != null ? Expression.Lambda<Action<Z80Emulator, byte>>(Expression.Block(Expression.Assign(timingProp, Expression.Constant(timingFDCB[n])), microExprFDCB[n]), par, dispPar).Compile() : nop2;
            }
            var disp = Expression.Variable(typeof(byte), "disp");
            microExprDD[0xCB] = Expression.Block(
                new[] { disp },
                Expression.Assign(disp, readImmediateByte),
                Expression.Invoke(Expression.ArrayAccess(Expression.Constant(microCodeDDCB), cInt(readImmediateByte)), par, disp)
            );
            microExprFD[0xCB] = Expression.Block(
                new[] { disp },
                Expression.Assign(disp, readImmediateByte),
                Expression.Invoke(Expression.ArrayAccess(Expression.Constant(microCodeFDCB), cInt(readImmediateByte)), par, disp)
            );

            // Compile prefixed expressions
            for (int n = 0; n < 256; n++)
            {
                microCodeCB[n] = microExprCB[n] != null ? Expression.Lambda<Action<Z80Emulator>>(Expression.Block(Expression.Assign(timingProp, Expression.Constant(timingCB[n])), microExprCB[n]), par).Compile() : nop3;
                if (n != 0xCB)
                    microCodeDD[n] = microExprDD[n] != null ? Expression.Lambda<Action<Z80Emulator>>(Expression.Block(Expression.Assign(timingProp, Expression.Constant(timingDD[n])), microExprDD[n]), par).Compile() : nop3;
                else
                    microCodeDD[n] = Expression.Lambda<Action<Z80Emulator>>(microExprDD[n], par).Compile();
                microCodeED[n] = microExprED[n] != null ? Expression.Lambda<Action<Z80Emulator>>(Expression.Block(Expression.Assign(timingProp, Expression.Constant(timingED[n])), microExprED[n]), par).Compile() : nop3;
                if (n != 0xCB)
                    microCodeFD[n] = microExprFD[n] != null ? Expression.Lambda<Action<Z80Emulator>>(Expression.Block(Expression.Assign(timingProp, Expression.Constant(timingFD[n])), microExprFD[n]), par).Compile() : nop3;
                else
                    microCodeFD[n] = Expression.Lambda<Action<Z80Emulator>>(microExprFD[n], par).Compile();
            }
            microExpr[0xCB] = Expression.Invoke(Expression.ArrayAccess(Expression.Constant(microCodeCB), cInt(readImmediateByte)), par);
            microExpr[0xDD] = Expression.Invoke(Expression.ArrayAccess(Expression.Constant(microCodeDD), cInt(readImmediateByte)), par);
            microExpr[0xED] = Expression.Invoke(Expression.ArrayAccess(Expression.Constant(microCodeED), cInt(readImmediateByte)), par);
            microExpr[0xFD] = Expression.Invoke(Expression.ArrayAccess(Expression.Constant(microCodeFD), cInt(readImmediateByte)), par);

            // Compile basic expressions
            for (int n = 0; n < 256; n++)
            {
                if (n != 0xCB && n != 0xDD && n != 0xED && n != 0xFD)
                {
                    microCode[n] = microExpr[n] != null ? Expression.Lambda<Action<Z80Emulator>>(Expression.Block(Expression.Assign(timingProp, Expression.Constant(timing[n])), microExpr[n]), par).Compile() : nop1;
                }
                else
                {
                    microCode[n] = Expression.Lambda<Action<Z80Emulator>>(microExpr[n], par).Compile();
                }
            }
        }

        private static MemberExpression GetMember<TElement>(Expression instance, Expression<Func<TElement, object>> prop)
        {
            Expression expr = prop.Body;
            UnaryExpression unary = expr as UnaryExpression;
            if (unary != null)
                expr = unary.Operand;
            MemberExpression member = expr as MemberExpression;
            return Expression.MakeMemberAccess(instance, member.Member);
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
            microCodeDDCB = new Action<Z80Emulator, byte>[256];
            microExprDDCB = new Expression[256];

            // FDCB-prefix
            microCodeFDCB = new Action<Z80Emulator, byte>[256];
            microExprFDCB = new Expression[256];

            cByte = (p) => Expression.Convert(p, typeof(byte));
            cWord = (p) => Expression.Convert(p, typeof(ushort));
            cInt = (p) => Expression.Convert(p, typeof(int));

            readByteProp = GetMember<Z80Emulator>(par, p => p.ReadMemory);
            writeByteProp = GetMember<Z80Emulator>(par, p => p.WriteMemory);
            readInputProp = GetMember<Z80Emulator>(par, p => p.ReadInput);
            writeOutputProp = GetMember<Z80Emulator>(par, p => p.WriteOutput);

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

            takeStatesLow = Expression.Assign(GetMember<Z80Registers>(regs, p => p.TakeStatesLow), Expression.Constant(true));

            readReg8 = new Expression[] {
                    regB, regC, regD, regE, regH, regL,
                    Expression.Invoke(readByteProp, cInt(regHL)),
                    regA
                };
            writeReg8 = new Func<Expression, Expression>[] {
                    (p) => Expression.Assign(regB, p),
                    (p) => Expression.Assign(regC, p),
                    (p) => Expression.Assign(regD, p),
                    (p) => Expression.Assign(regE, p),
                    (p) => Expression.Assign(regH, p),
                    (p) => Expression.Assign(regL, p),
                    (p) => Expression.Invoke(writeByteProp, cInt(regHL), p),
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

            // Reading 8-bit and 16-bit operands
            readImmediateByte = Expression.Invoke(readByteProp, cInt(Expression.PostIncrementAssign(regPC)));
            readImmediateOffset = Expression.Subtract(
                Expression.ExclusiveOr(
                    Expression.Convert(readImmediateByte, typeof(int)),
                    Expression.Constant(128)
                 ),
                Expression.Constant(128)
            );
            readImmediateWord =
                cWord(
                    Expression.Or(
                        cInt(
                            Expression.Invoke(readByteProp,
                                cInt(Expression.PostIncrementAssign(regPC))
                            )
                        ),
                        Expression.LeftShift(
                            cInt(
                                Expression.Invoke(readByteProp,
                                    cInt(Expression.PostIncrementAssign(regPC))
                                )
                            ),
                            Expression.Constant(8)
                        )
                    )
                );

            // Read 8-bit data, referenced by 16-bit address operand
            readIndirect8 = Expression.Invoke(readByteProp, cInt(readImmediateWord));
            // Read 16-bit data, referenced by 16-bit address operand
            var target = Expression.Variable(typeof(ushort));
            readIndirect16 = Expression.Block(
                new[] { target },
                Expression.Assign(target, readImmediateWord),
                cWord(
                    Expression.Or(
                        cInt(
                            Expression.Invoke(readByteProp,
                                cInt(Expression.PostIncrementAssign(target))
                            )
                        ),
                        Expression.LeftShift(
                            cInt(
                                Expression.Invoke(readByteProp,
                                    cInt(target)
                                )
                            ),
                            Expression.Constant(8)
                        )
                    )
                )
            );

            // Write 8-bit data, referenced by 16-bit address operand
            writeIndirect8 = (p) => Expression.Invoke(writeByteProp, cInt(readImmediateWord), p);
            // Write 16-bit data, referenced by 16-bit address operand
            writeIndirect16 = (p) => Expression.Block(
                new[] { target },
                Expression.Assign(target, readImmediateWord),
                Expression.Invoke(writeByteProp,
                    cInt(Expression.PostIncrementAssign(target)),
                    cByte(
                        Expression.And(cInt(p), Expression.Constant(0xFF))
                    )
                ),
                Expression.Invoke(writeByteProp,
                    cInt(target),
                    cByte(
                        Expression.RightShift(cInt(p), Expression.Constant(8))
                    )
                )
            );

            // Stack expressions
            // - PUSH
            push = (p) => Expression.Block(
                Expression.Assign(regSP, Expression.Subtract(regSP, Expression.Constant((ushort)1))),
                Expression.Invoke(writeByteProp,
                    cInt(regSP),
                    cByte(
                        Expression.RightShift(cInt(p), Expression.Constant(8))
                    )
                ),
                Expression.Assign(regSP, Expression.Subtract(regSP, Expression.Constant((ushort)1))),
                Expression.Invoke(writeByteProp,
                    cInt(regSP),
                    cByte(
                        Expression.And(cInt(p), Expression.Constant(0xFF))
                    )
                )
            );

            // - POP
            var temp = Expression.Variable(typeof(ushort));
            pop = Expression.Block(
                new[] { temp },
                Expression.Assign(temp,
                    cWord(
                        Expression.Invoke(readByteProp, cInt(regSP))
                    )
                ),
                Expression.Assign(regSP, Expression.Add(regSP, Expression.Constant((ushort)1))),
                Expression.Assign(
                    temp,
                    cWord(
                        Expression.Or(
                            cInt(temp),
                            Expression.LeftShift(
                                cInt(
                                    Expression.Invoke(readByteProp, cInt(regSP))
                                ),
                                Expression.Constant(8)
                            )
                        )
                    )
                ),
                Expression.Assign(regSP, Expression.Add(regSP, Expression.Constant((ushort)1))),
                temp
            );

            addByte = (p, v) =>
                cByte(Expression.Add(cInt(p), Expression.Constant(v)));
            addWord = (p, v) =>
                cWord(Expression.Add(cInt(p), Expression.Constant(v)));

            parityTable = new bool[256];
            for (int i = 0; i < 256; i++)
            {
                parityTable[i] =
                    ((i >> 7) + (i >> 6) + (i >> 5) + (i >> 4) + (i >> 3) + (i >> 2) + (i >> 1) + i) % 2 == 0;
            }

            getParity = (p) =>
                Expression.ArrayAccess(Expression.Field(null, typeof(Z80Emulator).GetField("parityTable", BindingFlags.NonPublic | BindingFlags.Static)), Expression.And(p, Expression.Constant(0xFF)));

            InitTiming();
        }

        private void LoadInstructions()
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
            microExpr[0x02] = Expression.Invoke(writeByteProp, cInt(regBC), regA);

            // LD (DE),A
            microExpr[0x12] = Expression.Invoke(writeByteProp, cInt(regDE), regA);

            // LD A,(BC)
            microExpr[0x0A] = Expression.Assign(regA, Expression.Invoke(readByteProp, cInt(regBC)));

            // LD A,(DE)
            microExpr[0x1A] = Expression.Assign(regA, Expression.Invoke(readByteProp, cInt(regDE)));

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
                Expression.Assign(temp1, cInt(regI)),
                Expression.Assign(flagS, Expression.Equal(Expression.And(temp1, Expression.Constant(0x80)), Expression.Constant(0x80))),
                Expression.Assign(flagZ, Expression.Equal(Expression.And(temp1, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                Expression.Assign(flagHC, Expression.Constant(false)),
                Expression.Assign(flagPV, iff2),
                Expression.Assign(flagN, Expression.Constant(false))
            );

            // LD A, R
            microExprED[0x5F] = Expression.Block(
                new[] { temp1 },
                Expression.Assign(regR, regA),
                Expression.Assign(temp1, cInt(regR)),
                Expression.Assign(flagS, Expression.Equal(Expression.And(temp1, Expression.Constant(0x80)), Expression.Constant(0x80))),
                Expression.Assign(flagZ, Expression.Equal(Expression.And(temp1, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                Expression.Assign(flagHC, Expression.Constant(false)),
                Expression.Assign(flagPV, iff2),
                Expression.Assign(flagN, Expression.Constant(false))
            );

        }

        private void StackInstructions()
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

        private void ControlStructures()
        {
            var conditions = new Expression[]
                {
                    Expression.Not(flagZ), flagZ,
                    Expression.Not(flagCY), flagCY,
                    Expression.Not(flagPV), flagPV,
                    Expression.Not(flagS), flagS
                };

            var jumpRel =
                cWord(
                    Expression.Add(
                        readImmediateOffset,
                        cInt(
                            regPC
                        )
                    )
                );

            var target = Expression.Variable(typeof(ushort));

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
                microExpr[opcode] =  Expression.Block(
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
                    Expression.Assign(regPC, Expression.Constant((ushort)(rst * 8)))
                );
            }

            // DJNZ
            var rel = Expression.Variable(typeof(int));
            microExpr[0x10] = Expression.Block(
                new[] { rel },
                Expression.Assign(rel, readImmediateOffset),
                Expression.Assign(regB, addByte(regB, -1)),
                Expression.IfThenElse(Expression.NotEqual(cInt(regB), Expression.Constant(0)),
                    Expression.Assign(
                        regPC,
                        cWord(
                            Expression.AddChecked(
                                rel,
                                cInt(regPC)
                            )
                        )
                    ),
                    takeStatesLow
                )
            );
        }

        private void Arithmetic()
        {
            var temp1 = Expression.Variable(typeof(int));
            var temp2 = Expression.Variable(typeof(int));
            var temp3 = Expression.Variable(typeof(int));
            var temp4 = Expression.Variable(typeof(ushort));

            // INC <reg8>
            for (int reg = 0; reg < 8; reg++)
            {
                var opcode = reg * 8 + 0x04;
                microExpr[opcode] = Expression.Block(
                    new[] { temp1 },
                    Expression.Assign(temp1, Expression.Increment(Expression.Convert(readReg8[reg], typeof(int)))),
                    writeReg8[reg](Expression.Convert(temp1, typeof(byte))),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp1, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagX1, Expression.Equal(Expression.And(temp1, Expression.Constant(0x08)), Expression.Constant(0x08))),
                    Expression.Assign(flagX2, Expression.Equal(Expression.And(temp1, Expression.Constant(0x20)), Expression.Constant(0x20))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp1, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagHC, Expression.Equal(Expression.And(temp1, Expression.Constant(0x0F)), Expression.Constant(0x00))),
                    Expression.Assign(flagPV, Expression.Equal(temp1, Expression.Constant(0x80))),
                    Expression.Assign(flagN, Expression.Constant(false))
                );
            }

            // INC (IX + d)
            // INC (IY + d)
            for (int reg = 0; reg < 2; reg++)
            {
                var block = Expression.Block(
                    new[] { temp1, temp4 },
                    Expression.Assign(temp4, cWord(Expression.Add(cInt(reg == 0 ? regIX : regIY), cInt(Expression.Convert(readImmediateByte, typeof(sbyte)))))),
                    Expression.Assign(temp1, Expression.Increment(cInt(Expression.Invoke(readByteProp, cInt(temp4))))),
                    Expression.Invoke(writeByteProp, cInt(temp4), cByte(temp1)),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp1, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagX1, Expression.Equal(Expression.And(temp1, Expression.Constant(0x08)), Expression.Constant(0x08))),
                    Expression.Assign(flagX2, Expression.Equal(Expression.And(temp1, Expression.Constant(0x20)), Expression.Constant(0x20))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp1, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagHC, Expression.Equal(Expression.And(temp1, Expression.Constant(0x0F)), Expression.Constant(0x00))),
                    Expression.Assign(flagPV, Expression.Equal(temp1, Expression.Constant(0x80))),
                    Expression.Assign(flagN, Expression.Constant(false))
                );
                if (reg == 0)
                    microExprDD[0x34] = block;
                else
                    microExprFD[0x34] = block;
            }

            // DEC <reg8>
            for (int reg = 0; reg < 8; reg++)
            {
                var opcode = reg * 8 + 0x05;
                microExpr[opcode] = Expression.Block(
                    new[] { temp1 },
                    Expression.Assign(temp1, Expression.And(Expression.Subtract(Expression.Convert(readReg8[reg], typeof(int)), Expression.Constant(1)), Expression.Constant(0xFF))),
                    writeReg8[reg](Expression.Convert(temp1, typeof(byte))),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp1, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagX1, Expression.Equal(Expression.And(temp1, Expression.Constant(0x08)), Expression.Constant(0x08))),
                    Expression.Assign(flagX2, Expression.Equal(Expression.And(temp1, Expression.Constant(0x20)), Expression.Constant(0x20))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp1, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagHC, Expression.Equal(Expression.And(temp1, Expression.Constant(0x0F)), Expression.Constant(0x0F))),
                    Expression.Assign(flagPV, Expression.Equal(temp1, Expression.Constant(0x7F))),
                    Expression.Assign(flagN, Expression.Constant(true))
                );
            }

            // DEC (IX + d)
            // DEC (IY + d)
            for (int reg = 0; reg < 2; reg++)
            {
                var block = Expression.Block(
                    new[] { temp1, temp4 },
                    Expression.Assign(temp4, cWord(Expression.Add(cInt(reg == 0 ? regIX : regIY), cInt(Expression.Convert(readImmediateByte, typeof(sbyte)))))),
                    Expression.Assign(temp1, Expression.Decrement(cInt(Expression.Invoke(readByteProp, cInt(temp4))))),
                    Expression.Invoke(writeByteProp, cInt(temp4), cByte(temp1)),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp1, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagX1, Expression.Equal(Expression.And(temp1, Expression.Constant(0x08)), Expression.Constant(0x08))),
                    Expression.Assign(flagX2, Expression.Equal(Expression.And(temp1, Expression.Constant(0x20)), Expression.Constant(0x20))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp1, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagHC, Expression.Equal(Expression.And(temp1, Expression.Constant(0x0F)), Expression.Constant(0x0F))),
                    Expression.Assign(flagPV, Expression.Equal(temp1, Expression.Constant(0x7F))),
                    Expression.Assign(flagN, Expression.Constant(true))
                );
                if (reg == 0)
                    microExprDD[0x35] = block;
                else
                    microExprFD[0x35] = block;
            }

            // INC <reg16>
            for (int reg = 0; reg < 4; reg++)
            {
                var opcode = reg * 16 + 0x03;
                microExpr[opcode] =
                    writeReg16[reg](Expression.Convert(Expression.Increment(Expression.Convert(readReg16[reg], typeof(int))), typeof(ushort)));
            }

            // DEC <reg16>
            for (int reg = 0; reg < 4; reg++)
            {
                var opcode = reg * 16 + 0x0B;
                microExpr[opcode] =
                    writeReg16[reg](Expression.Convert(Expression.Decrement(Expression.Convert(readReg16[reg], typeof(int))), typeof(ushort)));
            }

            // ADD(C) A, <reg8>
            // ADD(C) A, n
            for (int reg = 0; reg < 18; reg++)
            {
                var opcode = reg < 16 ? reg + 0x80 : (reg - 16) * 8 + 0xC6;
                microExpr[opcode] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    Expression.Assign(temp1, cInt(regA)),
                    Expression.Assign(temp2, cInt(reg < 16 ? readReg8[reg % 8] : readImmediateByte)),
                    Expression.Assign(temp3, reg < 8 || reg == 16 ? (Expression)Expression.Add(temp1, temp2) : (Expression)Expression.Condition(flagCY, Expression.Increment(Expression.Add(temp1, temp2)), Expression.Add(temp1, temp2))),
                    Expression.Assign(regA, cByte(temp3)),
                    Expression.Assign(flagCY, Expression.GreaterThan(temp3, Expression.Constant(0xFF))),
                    Expression.Assign(flagHC, Expression.NotEqual(
                        Expression.Add(
                            Expression.And(temp1, Expression.Constant(0x10)),
                            Expression.And(temp2, Expression.Constant(0x10))
                        ),
                        Expression.And(temp3, Expression.Constant(0x10))
                    )),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp3, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp3, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagPV, Expression.AndAlso(
                        Expression.Equal(
                            Expression.And(temp1, Expression.Constant(0x80)),
                            Expression.And(temp2, Expression.Constant(0x80))
                        ),
                        Expression.NotEqual(
                            Expression.And(temp1, Expression.Constant(0x80)),
                            Expression.And(temp3, Expression.Constant(0x80))
                        )
                    )), Expression.Assign(flagN, Expression.Constant(false))
                );
            }

            // SUB(C) A, <reg8>
            // SUB(C) A, n
            for (int reg = 0; reg < 18; reg++)
            {
                var opcode = reg < 16 ? reg + 0x90 : (reg - 16) * 8 + 0xD6;
                microExpr[opcode] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    Expression.Assign(temp1, cInt(regA)),
                    Expression.Assign(temp2, cInt(reg < 16 ? readReg8[reg % 8] : readImmediateByte)),
                    Expression.Assign(temp3, reg < 8 || reg == 16 ? (Expression)Expression.Subtract(temp1, temp2) : (Expression)Expression.Condition(flagCY, Expression.Decrement(Expression.Subtract(temp1, temp2)), Expression.Subtract(temp1, temp2))),
                    Expression.Assign(regA, cByte(temp3)),
                    Expression.Assign(flagCY, Expression.LessThan(temp3, Expression.Constant(0x00))),
                    Expression.Assign(flagHC, Expression.NotEqual(
                        Expression.Subtract(
                            Expression.And(temp1, Expression.Constant(0x10)),
                            Expression.And(temp2, Expression.Constant(0x10))
                        ),
                        Expression.And(temp3, Expression.Constant(0x10))
                    )),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp3, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp3, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagPV, Expression.AndAlso(
                        Expression.NotEqual(
                            Expression.And(temp1, Expression.Constant(0x80)),
                            Expression.And(temp2, Expression.Constant(0x80))
                        ),
                        Expression.NotEqual(
                            Expression.And(temp1, Expression.Constant(0x80)),
                            Expression.And(temp3, Expression.Constant(0x80))
                        )
                    )),
                    Expression.Assign(flagN, Expression.Constant(true))
                );
            }

            // ADD(C) HL, <reg16>
            for (int reg = 0; reg < 8; reg++)
            {
                var code = new List<Expression>
                    {
                        Expression.Assign(temp1, cInt(regHL)),
                        Expression.Assign(temp2, cInt(readReg16[reg % 4])),
                        Expression.Assign(temp3, reg < 4 ? (Expression)Expression.Add(temp1, temp2) : (Expression)Expression.Condition(flagCY, Expression.Increment(Expression.Add(temp1, temp2)), Expression.Add(temp1, temp2))),
                        Expression.Assign(regHL, cWord(temp3)),
                        Expression.Assign(flagCY, Expression.GreaterThan(temp3, Expression.Constant(0xFFFF))),
                        Expression.Assign(flagHC, Expression.NotEqual(
                            Expression.Add(
                                Expression.And(temp1, Expression.Constant(0x1000)),
                                Expression.And(temp2, Expression.Constant(0x1000))
                            ),
                            Expression.And(temp3, Expression.Constant(0x1000))
                        )),
                        Expression.Assign(flagS, Expression.Equal(Expression.And(temp3, Expression.Constant(0x8000)), Expression.Constant(0x8000))),
                        Expression.Assign(flagZ, Expression.Equal(Expression.And(temp3, Expression.Constant(0xFFFF)), Expression.Constant(0x0000))),
                        Expression.Assign(flagPV, Expression.AndAlso(
                            Expression.Equal(
                                Expression.And(temp1, Expression.Constant(0x8000)),
                                Expression.And(temp2, Expression.Constant(0x8000))
                            ),
                            Expression.NotEqual(
                                Expression.And(temp1, Expression.Constant(0x8000)),
                                Expression.And(temp3, Expression.Constant(0x8000))
                            )
                        )),
                        Expression.Assign(flagN, Expression.Constant(false))
                    };

                if (reg < 4)
                    microExpr[reg * 0x10 + 0x09] = Expression.Block(
                        new[] { temp1, temp2, temp3 },
                        code
                    );
                else
                    microExprED[(reg % 4) * 0x10 + 0x4A] = Expression.Block(
                        new[] { temp1, temp2, temp3 },
                        code
                    );
            }

            // SBC HL, <reg16>
            for (int reg = 0; reg < 4; reg++)
            {
                var code = new List<Expression>
                    {
                        Expression.Assign(temp1, cInt(regHL)),
                        Expression.Assign(temp2, cInt(readReg16[reg])),
                        Expression.Assign(temp3, (Expression)Expression.Condition(flagCY, Expression.Decrement(Expression.Subtract(temp1, temp2)), Expression.Subtract(temp1, temp2))),
                        Expression.Assign(regHL, cWord(temp3)),
                        Expression.Assign(flagCY, Expression.LessThan(temp3, Expression.Constant(0x0000))),
                        Expression.Assign(flagHC, Expression.NotEqual(
                            Expression.Subtract(
                                Expression.And(temp1, Expression.Constant(0x1000)),
                                Expression.And(temp2, Expression.Constant(0x1000))
                            ),
                            Expression.And(temp3, Expression.Constant(0x1000))
                        )),
                        Expression.Assign(flagS, Expression.Equal(Expression.And(temp3, Expression.Constant(0x8000)), Expression.Constant(0x8000))),
                        Expression.Assign(flagZ, Expression.Equal(Expression.And(temp3, Expression.Constant(0xFFFF)), Expression.Constant(0x0000))),
                        Expression.Assign(flagPV, Expression.AndAlso(
                            Expression.NotEqual(
                                Expression.And(temp1, Expression.Constant(0x8000)),
                                Expression.And(temp2, Expression.Constant(0x8000))
                            ),
                            Expression.NotEqual(
                                Expression.And(temp1, Expression.Constant(0x8000)),
                                Expression.And(temp3, Expression.Constant(0x8000))
                            )
                        )),
                        Expression.Assign(flagN, Expression.Constant(true))
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
                var opcode = reg < 8 ? reg + 0xA0 : 0xE6;
                microExpr[opcode] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    Expression.Assign(temp1, cInt(regA)),
                    Expression.Assign(temp2, cInt(reg < 8 ? readReg8[reg] : readImmediateByte)),
                    Expression.Assign(temp3, Expression.And(temp1, temp2)),
                    Expression.Assign(regA, cByte(temp3)),
                    Expression.Assign(flagCY, Expression.Constant(false)),
                    Expression.Assign(flagHC, Expression.Constant(true)),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp3, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagZ, Expression.Equal(temp3, Expression.Constant(0x00))),
                    // Special case: Overflow can never occur in AND-operation
                    // PV is set when bit7 of both operands are equal and bit7 of result is different
                    Expression.Assign(flagPV, Expression.Constant(false)),
                    Expression.Assign(flagN, Expression.Constant(false))
                );
            }

            // OR A, <reg8>
            // OR A, n
            for (int reg = 0; reg < 9; reg++)
            {
                var opcode = reg < 8 ? reg + 0xB0 : 0xF6;
                microExpr[opcode] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    Expression.Assign(temp1, cInt(regA)),
                    Expression.Assign(temp2, cInt(reg < 8 ? readReg8[reg] : readImmediateByte)),
                    Expression.Assign(temp3, Expression.Or(temp1, temp2)),
                    Expression.Assign(regA, cByte(temp3)),
                    Expression.Assign(flagCY, Expression.Constant(false)),
                    Expression.Assign(flagHC, Expression.Constant(false)),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp3, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagZ, Expression.Equal(temp3, Expression.Constant(0x00))),
                    // Special case: Overflow can never occur in OR-operation
                    // PV is set when bit7 of both operands are equal and bit7 of result is different
                    Expression.Assign(flagPV, Expression.Constant(false)),
                    Expression.Assign(flagN, Expression.Constant(false))
                );
            }

            // XOR A, <reg8>
            // XOR A, n
            for (int reg = 0; reg < 9; reg++)
            {
                var opcode = reg < 8 ? reg + 0xA8 : 0xEE;
                microExpr[opcode] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    Expression.Assign(temp1, cInt(regA)),
                    Expression.Assign(temp2, cInt(reg < 8 ? readReg8[reg] : readImmediateByte)),
                    Expression.Assign(temp3, Expression.ExclusiveOr(temp1, temp2)),
                    Expression.Assign(regA, cByte(temp3)),
                    Expression.Assign(flagCY, Expression.Constant(false)),
                    Expression.Assign(flagHC, Expression.Constant(false)),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp3, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagZ, Expression.Equal(temp3, Expression.Constant(0x00))),
                    Expression.Assign(flagPV, getParity(temp3)),
                    Expression.Assign(flagN, Expression.Constant(false))
                );
            }

            // CP A, <reg8>
            // CP A, n
            for (int reg = 0; reg < 9; reg++)
            {
                var opcode = reg < 8 ? reg + 0xB8 : 0xFE;
                microExpr[opcode] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    Expression.Assign(temp1, cInt(regA)),
                    Expression.Assign(temp2, cInt(reg < 8 ? readReg8[reg] : readImmediateByte)),
                    Expression.Assign(temp3, (Expression)Expression.Subtract(temp1, temp2)),
                    Expression.Assign(flagCY, Expression.LessThan(temp3, Expression.Constant(0x00))),
                    Expression.Assign(flagHC, Expression.NotEqual(
                        Expression.Subtract(
                            Expression.And(temp1, Expression.Constant(0x10)),
                            Expression.And(temp2, Expression.Constant(0x10))
                        ),
                        Expression.And(temp3, Expression.Constant(0x10))
                    )),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp3, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp3, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagPV, Expression.AndAlso(
                        Expression.NotEqual(
                            Expression.And(temp1, Expression.Constant(0x80)),
                            Expression.And(temp2, Expression.Constant(0x80))
                        ),
                        Expression.NotEqual(
                            Expression.And(temp1, Expression.Constant(0x80)),
                            Expression.And(temp3, Expression.Constant(0x80))
                        )
                    )),
                    Expression.Assign(flagN, Expression.Constant(true))
                );
            }

            // SCF
            microExpr[0x37] = Expression.Assign(flagCY, Expression.Constant(true));

            // CCF
            microExpr[0x3F] = Expression.Assign(flagCY, Expression.Constant(false));

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
                Expression.Assign(temp1, cInt(regA)),
                Expression.IfThenElse(Expression.OrElse(flagCY, Expression.GreaterThan(temp1, Expression.Constant(0x90))),
                    Expression.Block(
                        Expression.Assign(corrFactor, Expression.Constant(0x60)),
                        Expression.Assign(flagCY, Expression.Constant(true))
                    ),
                    Expression.Block(
                        Expression.Assign(corrFactor, Expression.Constant(0x00)),
                        Expression.Assign(flagCY, Expression.Constant(false))
                    )
                ),
                Expression.IfThen(Expression.OrElse(flagHC, Expression.GreaterThan(Expression.And(temp1, Expression.Constant(0x0F)), Expression.Constant(0x09))),
                    Expression.AddAssign(corrFactor, Expression.Constant(0x06))
                ),
                Expression.Assign(temp2, Expression.Condition(flagN,
                        Expression.Subtract(temp1, corrFactor),
                        Expression.Add(temp1, corrFactor)
                    )
                ),
                Expression.Assign(regA, cByte(temp2)),
                Expression.Assign(flagHC,
                    Expression.Equal(
                        Expression.And(
                            Expression.ExclusiveOr(temp1, temp2),
                            Expression.Constant(0x10)
                        ),
                        Expression.Constant(0x10)
                    )
                ),
                Expression.Assign(flagPV,
                    Expression.Equal(
                        Expression.And(
                            Expression.ExclusiveOr(temp1, temp2),
                            Expression.Constant(0x80)
                        ),
                        Expression.Constant(0x80)
                    )
                ),
                Expression.Assign(flagS, Expression.Equal(Expression.And(temp2, Expression.Constant(0x80)), Expression.Constant(0x80))),
                Expression.Assign(flagZ, Expression.Equal(Expression.And(temp2, Expression.Constant(0xFF)), Expression.Constant(0x00)))
            );

            // NEG
            for (int n = 0; n < 8; n++)
            {
                microExprED[0x44 + n * 8] =
                    Expression.Assign(regA, cByte(Expression.Negate(cInt(regA))));
            }
        }

        private void RotateAndBit()
        {
            var temp1 = Expression.Variable(typeof(int), "temp1");
            var temp2 = Expression.Variable(typeof(int), "temp2");
            var temp3 = Expression.Variable(typeof(int), "temp3");

            // RLCA
            microExpr[0x07] = Expression.Block(
                new[] { temp1 },
                Expression.Assign(temp1, cInt(regA)),
                Expression.Assign(regA, cByte(Expression.Or(
                    Expression.LeftShift(temp1, Expression.Constant(1)),
                    Expression.RightShift(temp1, Expression.Constant(7))
                ))),
                Expression.Assign(flagCY, Expression.Equal(Expression.And(temp1, Expression.Constant(0x80)), Expression.Constant(0x80)))
            );

            // RRCA
            microExpr[0x0F] = Expression.Block(
                new[] { temp1 },
                Expression.Assign(temp1, cInt(regA)),
                Expression.Assign(regA, cByte(Expression.Or(
                    Expression.RightShift(temp1, Expression.Constant(1)),
                    Expression.LeftShift(temp1, Expression.Constant(7))
                ))),
                Expression.Assign(flagCY, Expression.Equal(Expression.And(temp1, Expression.Constant(0x01)), Expression.Constant(0x01)))
            );

            // RLA
            microExpr[0x17] = Expression.Block(
                new[] { temp1, temp2 },
                Expression.Assign(temp1, cInt(regA)),
                Expression.Assign(temp2, Expression.Condition(
                    flagCY,
                    Expression.Or(
                        Expression.LeftShift(temp1, Expression.Constant(1)),
                        Expression.Constant(0x01)
                    ),
                    Expression.LeftShift(temp1, Expression.Constant(1))
                )),
                Expression.Assign(regA, cByte(temp2)),
                Expression.Assign(flagCY, Expression.Equal(Expression.And(temp1, Expression.Constant(0x80)), Expression.Constant(0x80)))
            );

            // RRA
            microExpr[0x1F] = Expression.Block(
                new[] { temp1, temp2 },
                Expression.Assign(temp1, cInt(regA)),
                Expression.Assign(temp2, Expression.Condition(
                    flagCY,
                    Expression.Or(
                        Expression.RightShift(temp1, Expression.Constant(1)),
                        Expression.Constant(0x80)
                    ),
                    Expression.RightShift(temp1, Expression.Constant(1))
                )),
                Expression.Assign(regA, cByte(temp2)),
                Expression.Assign(flagCY, Expression.Equal(Expression.And(temp1, Expression.Constant(0x01)), Expression.Constant(0x01)))
            );

            // RLC <reg8>
            for (int reg = 0; reg < 8; reg++)
            {
                microExprCB[reg] = Expression.Block(
                    new[] { temp1, temp2 },
                    Expression.Assign(temp1, cInt(readReg8[reg])),
                    Expression.Assign(temp2,
                        Expression.Or(
                            Expression.LeftShift(temp1, Expression.Constant(1)),
                            Expression.RightShift(temp1, Expression.Constant(7))
                        )
                    ),
                    writeReg8[reg](cByte(temp2)),
                    Expression.Assign(flagCY, Expression.Equal(Expression.And(temp1, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp2, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp2, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagHC, Expression.Constant(false)),
                    Expression.Assign(flagN, Expression.Constant(false)),
                    Expression.Assign(flagPV, getParity(temp2))
                );
            }

            // RRC <reg8>
            for (int reg = 0; reg < 8; reg++)
            {
                microExprCB[0x08 + reg] = Expression.Block(
                    new[] { temp1, temp2 },
                    Expression.Assign(temp1, cInt(readReg8[reg])),
                    Expression.Assign(temp2,
                        Expression.Or(
                            Expression.RightShift(temp1, Expression.Constant(1)),
                            Expression.LeftShift(temp1, Expression.Constant(7))
                        )
                    ),
                    writeReg8[reg](cByte(temp2)),
                    Expression.Assign(flagCY, Expression.Equal(Expression.And(temp1, Expression.Constant(0x01)), Expression.Constant(0x01))),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp2, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp2, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagHC, Expression.Constant(false)),
                    Expression.Assign(flagN, Expression.Constant(false)),
                    Expression.Assign(flagPV, getParity(temp2))
                );
            }

            // RL <reg8>
            for (int reg = 0; reg < 8; reg++)
            {
                microExprCB[0x10 + reg] = Expression.Block(
                    new[] { temp1, temp2 },
                    Expression.Assign(temp1, cInt(readReg8[reg])),
                    Expression.Assign(temp2,
                        Expression.Condition(
                            flagCY,
                            Expression.Or(
                                Expression.LeftShift(temp1, Expression.Constant(1)),
                                Expression.Constant(0x01)
                            ),
                            Expression.LeftShift(temp1, Expression.Constant(1))
                        )
                    ),
                    writeReg8[reg](cByte(temp2)),
                    Expression.Assign(flagCY, Expression.Equal(Expression.And(temp1, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp2, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp2, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagHC, Expression.Constant(false)),
                    Expression.Assign(flagN, Expression.Constant(false)),
                    Expression.Assign(flagPV, getParity(temp2))
                );
            }

            // RR <reg8>
            for (int reg = 0; reg < 8; reg++)
            {
                microExprCB[0x18 + reg] = Expression.Block(
                    new[] { temp1, temp2 },
                    Expression.Assign(temp1, cInt(readReg8[reg])),
                    Expression.Assign(temp2,
                        Expression.Condition(
                            flagCY,
                            Expression.Or(
                                Expression.RightShift(temp1, Expression.Constant(1)),
                                Expression.Constant(0x80)
                            ),
                            Expression.RightShift(temp1, Expression.Constant(1))
                        )
                    ),
                    writeReg8[reg](cByte(temp2)),
                    Expression.Assign(flagCY, Expression.Equal(Expression.And(temp1, Expression.Constant(0x01)), Expression.Constant(0x01))),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp2, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp2, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagHC, Expression.Constant(false)),
                    Expression.Assign(flagN, Expression.Constant(false)),
                    Expression.Assign(flagPV, getParity(Expression.And(temp2, Expression.Constant(0xFF))))
                );
            }

            // SLA <reg8>
            for (int reg = 0; reg < 8; reg++)
            {
                microExprCB[0x20 + reg] = Expression.Block(
                    new[] { temp1, temp2 },
                    Expression.Assign(temp1, cInt(readReg8[reg])),
                    Expression.Assign(temp2, Expression.LeftShift(temp1, Expression.Constant(1))),
                    writeReg8[reg](cByte(temp2)),
                    Expression.Assign(flagCY, Expression.Equal(Expression.And(temp1, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp2, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp2, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagHC, Expression.Constant(false)),
                    Expression.Assign(flagN, Expression.Constant(false)),
                    Expression.Assign(flagPV, getParity(temp2))
                );
            }

            // SRA <reg8>
            for (int reg = 0; reg < 8; reg++)
            {
                microExprCB[0x28 + reg] = Expression.Block(
                    new[] { temp1, temp2 },
                    Expression.Assign(temp1, cInt(readReg8[reg])),
                    Expression.Assign(temp2, Expression.Or(
                        Expression.RightShift(temp1, Expression.Constant(1)),
                        Expression.And(temp1, Expression.Constant(0x80))
                    )),
                    writeReg8[reg](cByte(temp2)),
                    Expression.Assign(flagCY, Expression.Equal(Expression.And(temp1, Expression.Constant(0x01)), Expression.Constant(0x01))),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp2, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp2, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagHC, Expression.Constant(false)),
                    Expression.Assign(flagN, Expression.Constant(false)),
                    Expression.Assign(flagPV, getParity(temp2))
                );
            }

            // SLL <reg8>
            for (int reg = 0; reg < 8; reg++)
            {
                microExprCB[0x30 + reg] = Expression.Block(
                    new[] { temp1, temp2 },
                    Expression.Assign(temp1, cInt(readReg8[reg])),
                    Expression.Assign(temp2, Expression.Or(
                        Expression.LeftShift(temp1, Expression.Constant(1)),
                        Expression.Constant(0x01)
                    )),
                    writeReg8[reg](cByte(temp2)),
                    Expression.Assign(flagCY, Expression.Equal(Expression.And(temp1, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp2, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp2, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagHC, Expression.Constant(false)),
                    Expression.Assign(flagN, Expression.Constant(false)),
                    Expression.Assign(flagPV, getParity(temp2))
                );
            }

            // SRL <reg8>
            for (int reg = 0; reg < 8; reg++)
            {
                microExprCB[0x38 + reg] = Expression.Block(
                    new[] { temp1, temp2 },
                    Expression.Assign(temp1, cInt(readReg8[reg])),
                    Expression.Assign(temp2, Expression.RightShift(temp1, Expression.Constant(1))),
                    writeReg8[reg](cByte(temp2)),
                    Expression.Assign(flagCY, Expression.Equal(Expression.And(temp1, Expression.Constant(0x01)), Expression.Constant(0x01))),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp2, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp2, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagHC, Expression.Constant(false)),
                    Expression.Assign(flagN, Expression.Constant(false)),
                    Expression.Assign(flagPV, getParity(temp2))
                );
            }

            // RLD
            microExprED[0x6F] = Expression.Block(
                new[] { temp1, temp2, temp3 },
                Expression.Assign(temp1, cInt(regA)),
                Expression.Assign(temp2, Expression.Or(
                        cInt(Expression.Invoke(readByteProp, cInt(regHL))),
                        Expression.LeftShift(temp1, Expression.Constant(8))
                    )
                ),
                Expression.Assign(temp2,
                    Expression.Or(
                        Expression.LeftShift(
                            temp2,
                            Expression.Constant(4)
                        ),
                        Expression.RightShift(
                            Expression.And(
                                temp2,
                                Expression.Constant(0x0F00)
                            ),
                            Expression.Constant(8)
                        )
                    )
                ),
                Expression.Invoke(writeByteProp, cInt(regHL), cByte(temp2)),
                Expression.Assign(temp3,
                    Expression.Or(
                        Expression.And(
                            temp1,
                            Expression.Constant(0xF0)
                        ),
                        Expression.And(
                            Expression.RightShift(
                                temp2,
                                Expression.Constant(8)
                            ),
                            Expression.Constant(0x0F)
                        )
                    )
                ),
                Expression.Assign(regA, cByte(temp3)),
                Expression.Assign(flagS, Expression.Equal(Expression.And(temp3, Expression.Constant(0x80)), Expression.Constant(0x80))),
                Expression.Assign(flagZ, Expression.Equal(Expression.And(temp3, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                Expression.Assign(flagHC, Expression.Constant(false)),
                Expression.Assign(flagN, Expression.Constant(false)),
                Expression.Assign(flagPV, getParity(temp3))
            );

            // RRD
            microExprED[0x67] = Expression.Block(
                new[] { temp1, temp2, temp3 },
                Expression.Assign(temp1, cInt(regA)),
                Expression.Assign(temp2, Expression.Or(
                        cInt(Expression.Invoke(readByteProp, cInt(regHL))),
                        Expression.LeftShift(temp1, Expression.Constant(8))
                    )
                ),
                Expression.Assign(temp2,
                    Expression.Or(
                        Expression.And(
                            Expression.RightShift(
                                temp2,
                                Expression.Constant(4)
                            ),
                            Expression.Constant(0x00FF)
                        ),
                        Expression.LeftShift(
                            Expression.And(
                                temp2,
                                Expression.Constant(0x000F)
                            ),
                            Expression.Constant(8)
                        )
                    )
                ),
                Expression.Invoke(writeByteProp, cInt(regHL), cByte(temp2)),
                Expression.Assign(temp3,
                    Expression.Or(
                        Expression.And(
                            temp1,
                            Expression.Constant(0xF0)
                        ),
                        Expression.And(
                            Expression.RightShift(
                                temp2,
                                Expression.Constant(8)
                            ),
                            Expression.Constant(0x0F)
                        )
                    )
                ),
                Expression.Assign(regA, cByte(temp3)),
                Expression.Assign(flagS, Expression.Equal(Expression.And(temp3, Expression.Constant(0x80)), Expression.Constant(0x80))),
                Expression.Assign(flagZ, Expression.Equal(Expression.And(temp3, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                Expression.Assign(flagHC, Expression.Constant(false)),
                Expression.Assign(flagN, Expression.Constant(false)),
                Expression.Assign(flagPV, getParity(temp3))
            );

            // BIT <bit>, <reg>
            for (int reg = 0; reg < 8; reg++)
                for (int bit = 0; bit < 8; bit++)
                {
                    microExprCB[bit * 8 + reg + 0x40] = Expression.Block(
                        new[] { temp1 },
                        Expression.Assign(temp1, cInt(readReg8[reg])),
                        Expression.Assign(flagZ, Expression.Equal(Expression.And(temp1, Expression.Constant(1 << bit)), Expression.Constant(0x00))),
                        Expression.Assign(flagHC, Expression.Constant(true)),
                        Expression.Assign(flagN, Expression.Constant(false))
                    );
                }

            // RES <bit>, <reg>
            for (int reg = 0; reg < 8; reg++)
                for (int bit = 0; bit < 8; bit++)
                {
                    microExprCB[bit * 8 + reg + 0x80] =
                        writeReg8[reg](cByte(Expression.And(cInt(readReg8[reg]), Expression.Constant(~(1 << bit)))));
                }


            // SET <bit>, <reg>
            for (int reg = 0; reg < 8; reg++)
                for (int bit = 0; bit < 8; bit++)
                {
                    microExprCB[bit * 8 + reg + 0xC0] =
                        writeReg8[reg](cByte(Expression.Or(cInt(readReg8[reg]), Expression.Constant(1 << bit))));
                }
        }

        private void LoopInstuctions()
        {
            // LDI(R)
            // LDD(R)
            for (int n = 0; n < 4; n++)
            {
                var list = new List<Expression>
                    {
                        Expression.Invoke(writeByteProp, cInt(regDE), Expression.Invoke(readByteProp, cInt(regHL))),
                        Expression.Assign(regDE, cWord((n & 1) == 0 ?
                            Expression.Increment(cInt(regDE)) :
                            Expression.Decrement(cInt(regDE))
                        )),
                        Expression.Assign(regHL, cWord((n & 1) == 0 ?
                            Expression.Increment(cInt(regHL)) :
                            Expression.Decrement(cInt(regHL))
                        )),
                        Expression.Assign(regBC, cWord(Expression.Decrement(cInt(regBC)))),
                        Expression.Assign(flagHC, Expression.Constant(false)),
                        Expression.Assign(flagN, Expression.Constant(false)),
                        Expression.Assign(flagPV, Expression.NotEqual(cInt(regBC), Expression.Constant(0)))
                    };

                // the (R) part...
                if (n >= 2)
                    list.Add(Expression.IfThenElse(flagPV, Expression.Assign(regPC, cWord(Expression.Subtract(cInt(regPC), Expression.Constant(2)))), takeStatesLow));

                microExprED[0xA0 + n * 8] = Expression.Block(
                    list
                );
            }

            // CPI(R)
            // CPD(R)
            var temp1 = Expression.Variable(typeof(int));
            var temp2 = Expression.Variable(typeof(int));
            var temp3 = Expression.Variable(typeof(int));
            for (int n = 0; n < 4; n++)
            {
                var list = new List<Expression>
                    {
                        Expression.Assign(temp1, cInt(regA)),
                        Expression.Assign(temp2, cInt(Expression.Invoke(readByteProp, cInt(regHL)))),
                        Expression.Assign(temp3, (Expression)Expression.Subtract(temp1, temp2)),
                        Expression.Assign(regHL, cWord((n & 1) == 0 ?
                            Expression.Increment(cInt(regHL)) :
                            Expression.Decrement(cInt(regHL))
                        )),
                        Expression.Assign(regBC, cWord(Expression.Decrement(cInt(regBC)))),
                        Expression.Assign(flagHC, Expression.NotEqual(
                            Expression.Subtract(
                                Expression.And(temp1, Expression.Constant(0x10)),
                                Expression.And(temp2, Expression.Constant(0x10))
                            ),
                            Expression.And(temp3, Expression.Constant(0x10))
                        )),
                        Expression.Assign(flagS, Expression.Equal(Expression.And(temp3, Expression.Constant(0x80)), Expression.Constant(0x80))),
                        Expression.Assign(flagZ, Expression.Equal(Expression.And(temp3, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                        Expression.Assign(flagPV, Expression.NotEqual(cInt(regBC), Expression.Constant(0))),
                        Expression.Assign(flagN, Expression.Constant(true))
                    };

                // the (R) part...
                if (n >= 2)
                    list.Add(Expression.IfThenElse(Expression.And(flagPV, Expression.NotEqual(temp3, Expression.Constant(0))),
                        Expression.Assign(regPC, cWord(Expression.Subtract(cInt(regPC), Expression.Constant(2)))),
                        takeStatesLow
                    ));

                microExprED[0xA1 + n * 8] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    list
                );
            }

            // INI(R)
            // IND(R)
            for (int n = 0; n < 4; n++)
            {
                var list = new List<Expression>
                    {
                        Expression.Assign(temp1, cInt(Expression.Invoke(readInputProp, cInt(regBC)))),
                        Expression.Invoke(writeByteProp, cInt(regHL), cByte(temp1)),
                        Expression.Assign(regHL, cWord((n & 1) == 0 ?
                            Expression.Increment(cInt(regHL)) :
                            Expression.Decrement(cInt(regHL))
                        )),
                        Expression.Assign(temp3, Expression.Decrement(cInt(regB))),
                        Expression.Assign(regB, cByte(temp3)),
                        Expression.Assign(temp2, Expression.Add(temp1, Expression.And((n & 1) == 0 ?
                            Expression.Increment(cInt(regC)) :
                            Expression.Decrement(cInt(regC)),
                            Expression.Constant(0xFF)))),
                        Expression.Assign(flagHC, Expression.GreaterThan(temp2, Expression.Constant(0xFF))),
                        Expression.Assign(flagCY, Expression.GreaterThan(temp2, Expression.Constant(0xFF))),
                        Expression.Assign(flagPV, getParity(Expression.ExclusiveOr(Expression.And(temp2, Expression.Constant(0x07)), cInt(regB)))),
                        Expression.Assign(flagS, Expression.Equal(Expression.And(temp3, Expression.Constant(0x80)), Expression.Constant(0x80))),
                        Expression.Assign(flagZ, Expression.Equal(Expression.And(temp3, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                        Expression.Assign(flagN, Expression.Equal(Expression.And(temp1, Expression.Constant(0x80)), Expression.Constant(0x80)))
                    };

                // the (R) part...
                if (n >= 2)
                    list.Add(Expression.IfThenElse(Expression.NotEqual(temp3, Expression.Constant(0)), Expression.Assign(regPC, cWord(Expression.Subtract(cInt(regPC), Expression.Constant(2)))), takeStatesLow));

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
                        Expression.Assign(temp1, cInt(Expression.Invoke(readByteProp, cInt(regHL)))),
                        Expression.Assign(temp3, Expression.Decrement(cInt(regB))),
                        Expression.Assign(regB, cByte(temp3)),
                        Expression.Invoke(writeOutputProp, cInt(regBC), cByte(temp1)),
                        Expression.Assign(regHL, cWord((n & 1) == 0 ?
                            Expression.Increment(cInt(regHL)) :
                            Expression.Decrement(cInt(regHL))
                        )),
                        Expression.Assign(temp2, Expression.Add(temp1, cInt(regL))),
                        Expression.Assign(flagHC, Expression.GreaterThan(temp2, Expression.Constant(0xFF))),
                        Expression.Assign(flagCY, Expression.GreaterThan(temp2, Expression.Constant(0xFF))),
                        Expression.Assign(flagPV, getParity(Expression.ExclusiveOr(Expression.And(temp2, Expression.Constant(0x07)), cInt(regB)))),
                        Expression.Assign(flagS, Expression.Equal(Expression.And(temp3, Expression.Constant(0x80)), Expression.Constant(0x80))),
                        Expression.Assign(flagZ, Expression.Equal(Expression.And(temp3, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                        Expression.Assign(flagN, Expression.Equal(Expression.And(temp1, Expression.Constant(0x80)), Expression.Constant(0x80)))
                    };

                // the (R) part...
                if (n >= 2)
                    list.Add(Expression.IfThenElse(Expression.NotEqual(temp3, Expression.Constant(0)), Expression.Assign(regPC, cWord(Expression.Subtract(cInt(regPC), Expression.Constant(2)))), takeStatesLow));

                microExprED[0xA3 + n * 8] = Expression.Block(
                    new[] { temp1, temp2, temp3 },
                    list
                );
            }
        }

        private void Misc()
        {
            var temp1 = Expression.Variable(typeof(int));
            var temp2 = Expression.Variable(typeof(int));
            var temp3 = Expression.Variable(typeof(ushort));
            var temp4 = Expression.Variable(typeof(byte));

            // CPL
            microExpr[0x2F] = Expression.Block(
                Expression.Assign(regA, cByte(Expression.ExclusiveOr(cInt(regA), Expression.Constant(0xFF)))),
                Expression.Assign(flagHC, Expression.Constant(true)),
                Expression.Assign(flagN, Expression.Constant(true))
            );

            // DI
            microExpr[0xF3] = Expression.Block(
                Expression.Assign(iff1, Expression.Constant(false)),
                Expression.Assign(iff2, Expression.Constant(false))
            );

            // EI
            var maskInterruptsNextProp = GetMember<Z80Registers>(regs, p => p.MaskInterruptsNext);
            microExpr[0xFB] = Expression.Block(
                Expression.Assign(iff1, Expression.Constant(true)),
                Expression.Assign(iff2, Expression.Constant(true)),
                Expression.Assign(maskInterruptsNextProp, Expression.Constant(true))  // Interrupts are masked after execution of EI
            );

            // IM0
            microExprED[0x46] = microExprED[0x4E] = microExprED[0x66] = microExprED[0x6E] =
                Expression.Assign(im, Expression.Constant(0));

            // IM1
            microExprED[0x56] = microExprED[0x76] =
                Expression.Assign(im, Expression.Constant(1));

            // IM2
            microExprED[0x5E] = microExprED[0x7E] =
                Expression.Assign(im, Expression.Constant(2));

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
                Expression.Assign(temp4, Expression.Invoke(readByteProp, cInt(temp3))),
                Expression.Invoke(writeByteProp, cInt(Expression.PostIncrementAssign(temp3)), regL),
                Expression.Assign(regL, temp4),
                Expression.Assign(temp4, Expression.Invoke(readByteProp, cInt(temp3))),
                Expression.Invoke(writeByteProp, cInt(temp3), regH),
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
                Expression.Assign(regPC, cWord(Expression.Subtract(cInt(regPC), Expression.Constant(1)))),
                Expression.Assign(GetMember<Z80Registers>(regs, p => p.Halted), Expression.Constant(true))
            );
        }

        private void InputOutput()
        {
            // IN A, (n)
            microExpr[0xDB] = Expression.Assign(regA,
                Expression.Invoke(readInputProp,
                    Expression.Or(
                        cInt(readImmediateByte),
                        Expression.LeftShift(
                            cInt(regA),
                            Expression.Constant(8)
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
                    Expression.Assign(temp1,
                        cInt(
                            Expression.Invoke(readInputProp, cInt(regBC))
                        )
                    ),
                    reg == 6 ? Expression.Assign(temp1, temp1) : Expression.Assign(readReg8[reg], cByte(temp1)),
                    Expression.Assign(flagS, Expression.Equal(Expression.And(temp1, Expression.Constant(0x80)), Expression.Constant(0x80))),
                    Expression.Assign(flagZ, Expression.Equal(Expression.And(temp1, Expression.Constant(0xFF)), Expression.Constant(0x00))),
                    Expression.Assign(flagHC, Expression.Constant(false)),
                    Expression.Assign(flagPV, getParity(temp1)),
                    Expression.Assign(flagN, Expression.Constant(false))
                );
            }

            // OUT (n), A
            microExpr[0xD3] = Expression.Invoke(writeOutputProp,
                Expression.Or(
                    cInt(readImmediateByte),
                    Expression.LeftShift(
                        cInt(regA),
                        Expression.Constant(8)
                    )
                ),
                regA
            );

            // OUT (C), <reg8>
            for (int reg = 0; reg < 8; reg++)
            {
                microExprED[0x41 + reg * 8] = Expression.Invoke(writeOutputProp,
                        cInt(regBC),
                        reg == 6 ? Expression.Constant((byte)0) : readReg8[reg]
                );
            }
        }

        private void Interrupts()
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
                    Expression.Assign(regR, cByte(Expression.Increment(cInt(regR)))),
                    Expression.IfThen(haltedProp, Expression.Block(
                        Expression.Assign(regPC, cWord(Expression.Increment(regPC))),   // Bail out of HALT instruction
                        Expression.Assign(haltedProp, Expression.Constant(false))
                    )),
                    Expression.Assign(iff1, Expression.Constant(false)),
                    Expression.Assign(iff2, Expression.Constant(false)),
                    Expression.IfThenElse(Expression.Equal(im, Expression.Constant(0)),
                        Expression.Block(       // Interrupt modus 0 - read next opcode from databus
                            Expression.Assign(nextOpcodeProp, Expression.Convert(dataOnBusPar, typeof(int?))),
                            Expression.Assign(statesNormalProp, Expression.Constant(13)),
                            Expression.Assign(statesLowProp, Expression.Constant(13))
                        ),
                        Expression.IfThenElse(Expression.Equal(im, Expression.Constant(1)),
                            Expression.Block(       // Interrupt modus 1 - next opcode is a RST 0x0038
                                Expression.Assign(nextOpcodeProp, Expression.Convert(Expression.Constant(0xFF), typeof(int?))),   // RST 0x0038
                                Expression.Assign(statesNormalProp, Expression.Constant(13)),
                                Expression.Assign(statesLowProp, Expression.Constant(13))
                            ),
                            Expression.Block(       // Interrupt modus 2
                                push(regPC),
                                Expression.Assign(nextOpcodeProp, Expression.Convert(Expression.Invoke(readByteProp, Expression.Or(Expression.LeftShift(cInt(regI), Expression.Constant(8)), cInt(dataOnBusPar))), typeof(int?))),
                                Expression.Assign(statesNormalProp, Expression.Constant(19)),
                                Expression.Assign(statesLowProp, Expression.Constant(19))
                            )
                        )
                    )
                );
            handleInt = Expression.Lambda<Action<Z80Emulator, byte>>(handleIntExpr, par, dataOnBusPar).Compile();

            handleNmiExpr =
                Expression.Block(
                    new[] { dataOnBusPar },
                    Expression.Assign(regR, cByte(Expression.Increment(cInt(regR)))),
                    Expression.IfThen(haltedProp, Expression.Block(
                        Expression.Assign(regPC, cWord(Expression.Increment(regPC))),   // Bail out of HALT instruction
                        Expression.Assign(haltedProp, Expression.Constant(false))
                    )),
                    Expression.Assign(iff2, iff1),
                    Expression.Assign(iff1, Expression.Constant(false)),
                    push(regPC),
                    Expression.Assign(regPC, Expression.Constant((UInt16)0x0066)),
                    Expression.Assign(statesNormalProp, Expression.Constant(11)),
                    Expression.Assign(statesLowProp, Expression.Constant(11))
                );
            handleNmi = Expression.Lambda<Action<Z80Emulator, byte>>(handleNmiExpr, par, dataOnBusPar).Compile();
        }

        #region Generated code

        public Action<Z80Emulator>[] MicroCode
        {
            get { return microCode; }
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
