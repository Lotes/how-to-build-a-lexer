﻿namespace Lexer.Automaton
{
    public interface IAutomatonBuilder
    {
        int AddState();
        void AddTransition(int source, ICharSet character, int target);
        void SetStartState(int state);
        void AcceptState(int state);
        IAutomaton Build();
    }
}