﻿using System;

namespace Lycus.Satori.Instructions
{
    public sealed class BreakpointInstruction : Instruction
    {
        public override string Mnemonic
        {
            get { return "bkpt"; }
        }

        public BreakpointInstruction()
            : this(0)
        {
        }

        [CLSCompliant(false)]
        public BreakpointInstruction(uint value)
            : base(value, true)
        {
        }

        public override Operation Execute(Core core)
        {
            if (core == null)
                throw new ArgumentNullException("core");

            core.Registers.DebugStatus = Bits.Set(core.Registers.DebugStatus, 0);

            return Operation.Next;
        }
    }
}
