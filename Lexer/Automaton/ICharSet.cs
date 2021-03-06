﻿using System;
using System.Collections.Generic;

namespace Lexer.Automaton
{
    public interface ICharSet : IEnumerable<CharRange>, IComparable<ICharSet>
    {
        int Length { get; }
        bool Includes(char c);
        bool Includes(char from, char to);
        bool Excludes(char c);
        bool Excludes(char from, char to);
    }
}