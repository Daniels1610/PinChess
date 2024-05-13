using System;
using ChessChallenge.API;


// PinChess Bot Implementation
public class MyBot : IChessBot
{
    readonly int depth = 4;
    int iterations = 0;

    public Move Think(Board board, Timer timer)
    {
        Console.WriteLine("PLY #: {0}", board.PlyCount);
        // Alfa-Beta Pruning Execution
        (int val, Move move) = AlfaBeta(board, depth, int.MinValue, int.MaxValue);

        // Minimax Execution
        // (int val, Move move) = Minimax(board, depth);
        Console.WriteLine("\nMOVE PLAYED BY BOT: {0} - {1} : EVALUATION: {2}", move.MovePieceType, move, val);
        Console.WriteLine("ITERATIONS TO FIND OPTIMAL MOVE: {0}", iterations);
        return move;
    }


    private (int, Move) Minimax(Board board, int depth) 
    {
        if (board.IsInCheckmate() || board.IsInStalemate() || depth == 0) 
        {
            return (board.Evaluate(), Move.NullMove);
        }
        
        int best_value = (board.PlyCount % 2 != 0) ? int.MaxValue: int.MinValue;
        Move best_move = Move.NullMove;

        foreach (Move move in board.GetLegalMoves())
        {
            iterations++;
            board.MakeMove(move);
            (int val, Move _) = Minimax(board, depth-1);
            board.UndoMove(move);

            val += board.PestoEvaluation(move, board.IsWhiteToMove);
            if (board.IsWhiteToMove)
            {
                if (val > best_value)
                {
                    best_value = val;
                    best_move = move;
                }
            }
            else
            {
                if (val < best_value)
                {
                    best_value = val;
                    best_move = move;
                }
            }
        }
        return (best_value, best_move);
    }


    private (int, Move) AlfaBeta(Board board, int depth, int alpha, int beta) 
    {
        int a = alpha; int b = beta;
        if (board.IsInCheckmate() || board.IsInStalemate() || depth == 0) 
        {
            return (board.Evaluate(), Move.NullMove);
        }
        
        int best_value = (board.PlyCount % 2 != 0) ? int.MaxValue: int.MinValue;
        Move best_move = Move.NullMove;

        foreach (Move move in board.GetLegalMoves())
        {
            iterations++;
            board.MakeMove(move);
            (int val, Move _) = Minimax(board, depth-1);
            board.UndoMove(move);

            val += board.PestoEvaluation(move, board.IsWhiteToMove);
            if (board.IsWhiteToMove)
            {
                if (val > best_value)
                {
                    best_value = val;
                    best_move = move;
                }
                a = Math.Max(a, val);
                if (a >= b){break;}
            }
            else
            {
                if (val < best_value)
                {
                    best_value = val;
                    best_move = move;
                }
                b = Math.Min(b, val);
                if (a >= b){break;}
            }
        }
        return (best_value, best_move);
    }
}