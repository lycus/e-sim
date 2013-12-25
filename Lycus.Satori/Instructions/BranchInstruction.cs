﻿using System;

namespace Lycus.Satori.Instructions
{
    public sealed class BranchInstruction : Instruction
    {
        public override string Mnemonic
        {
            get { return "b"; }
        }

        public BranchInstruction(bool is16Bit)
            : this(0, is16Bit)
        {
        }

        [CLSCompliant(false)]
        public BranchInstruction(uint value, bool is16Bit)
            : base(value, is16Bit)
        {
        }

        public ConditionCode Condition { get; set; }

        public int Immediate { get; set; }

        public override void Decode()
        {
            Condition = (ConditionCode)((Value & ~0xFFFFFF0F) >> 4);
            Immediate = (int)((Value & ~(Is16Bit ? 0xFFFF00FF : 0x000000FF)) >> 8);
        }

        public override Operation Execute(Core core)
        {
            if (core == null)
                throw new ArgumentNullException("core");

            if (!core.EvaluateCondition(Condition))
                return Operation.Next;

            if (Condition == ConditionCode.BranchAndLink)
                core.Registers[14] = (int)(core.Registers.ProgramCounter + (Is16Bit ? sizeof(ushort) : sizeof(uint)));

            // The immediate is actually the amount of 16-bit instructions
            // to jump back/ahead. So multiply it by 2 to get the real offset
            // to add to the PC. We do it with a shift just to match the
            // architecture manual.
            core.Registers.ProgramCounter += (uint)(Immediate << 1);

            return Operation.None;
        }

        public override string ToString()
        {
            return "b{0} {1}".Interpolate(Condition.ToAssemblyString(), Immediate << 1);
        }
    }
}