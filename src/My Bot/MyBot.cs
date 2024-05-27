using System;
using ChessChallenge.API;
using static Metaweights;
using System.Collections.Generic;


// PinChess Bot Implementation
public class MyBot : IChessBot
{
    readonly int depth = 6;  

    int iterations = 0;

    public Move Think(Board board, Timer timer)
    {
        
        // Alfa-Beta Enhanced Pruning Execution
       (int val, Move move) = AlfaBetaEnhanced(board, depth, int.MinValue, int.MaxValue);

        // Alfa-Beta Pruning Classic Execution
        //(int val, Move move) = AlfaBeta(board, depth, int.MinValue, int.MaxValue);

        // Minimax Execution
        // (int val, Move move) = Minimax(board, depth);

        Console.WriteLine("\nPLY #: {0}", board.PlyCount);
        Console.WriteLine("EG WEIGHT: {0}", endgameWeight);
        Console.WriteLine("MOVE PLAYED BY BOT: {0} - {1} : EVALUATION: {2}", move.MovePieceType, move, val);
        Console.WriteLine("ITERATIONS TO FIND OPTIMAL MOVE: {0}", iterations);
        Console.WriteLine("Time Elapsed: {0} seconds\n",(double) timer.MillisecondsElapsedThisTurn / 1000);
        board.UpdateEndgameWeight();
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
        if (board.IsInCheckmate() || board.IsInStalemate() || depth == 0) 
        {
            return (board.Evaluate(), Move.NullMove);
        }
        
        int best_value = (board.PlyCount % 2 != 0) ? int.MaxValue: int.MinValue;
        Move best_move = Move.NullMove;

        foreach (Move move in board.GetLegalMoves())
        {
            iterations++;
            // Always play checkmate in one
            if (MoveIsCheckmate(board, move)){best_move = move; break;}
            board.MakeMove(move);
            (int val, Move _) = AlfaBeta(board, depth-1, alpha, beta);
            board.UndoMove(move);

            val += board.PestoEvaluation(move, board.IsWhiteToMove);
            if (board.IsWhiteToMove)
            {
                if (val > best_value)
                {
                    best_value = val;
                    best_move = move;
                }
                alpha = Math.Max(alpha, val);
                if (alpha >= beta){continue;}
            }
            else
            {
                if (val < best_value)
                {
                    best_value = val;
                    best_move = move;
                }
                beta = Math.Min(beta, val);
                if (alpha >= beta){continue;}
            }
        }
        return (best_value, best_move);
    }

    private (int, Move) AlfaBetaEnhanced(Board board, int depth, int alpha, int beta) 
    {
        if (board.IsInCheckmate() || board.IsInStalemate() || depth == 0) 
        {
            return (board.Evaluate(), Move.NullMove);
        }
        
        // Generate all legal moves
        Move[] moves = board.GetLegalMoves();

        // Order moves based on heuristic criteria
        // Console.WriteLine("ORDER MOVES FOR : {0}", board.IsWhiteToMove);
        List<Tuple<int, Move>> ordered_moves = board.IsWhiteToMove ? OrderMoves(board, moves) : OrderMoves(board, moves, reverse:true);

        int best_value = (board.PlyCount % 2 != 0) ? int.MaxValue: int.MinValue;
        Move best_move = Move.NullMove;

        foreach (Tuple<int, Move> t in ordered_moves)
        {
            int val = t.Item1; Move move = t.Item2;
            iterations++;

            // Always play checkmate in one
            if (MoveIsCheckmate(board, move)){best_move = move; break;}
            board.MakeMove(move);
            (val, Move _) = AlfaBetaEnhanced(board, depth-1, alpha, beta);
            board.UndoMove(move);

            val += board.PestoEvaluation(move, board.IsWhiteToMove);
            if (board.IsWhiteToMove)
            {
                if (val > best_value)
                {
                    best_value = val;
                    best_move = move;
                }
                alpha = Math.Max(alpha, val);
                if (alpha >= beta){break;}
            }
            else
            {
                if (val < best_value)
                {
                    best_value = val;
                    best_move = move;
                }
                beta = Math.Min(beta, val);
                if (alpha >= beta){break;}
            }
        }
        return (best_value, best_move);
    }

    private List<Tuple<int, Move>> OrderMoves(Board board, Move[] moves, bool reverse=false)
    {
        List<Tuple<int, Move>> ordered_moves = new(moves.Length);
        foreach (Move move in moves) {
            int val = 0;
            board.MakeMove(move);
            val += board.Evaluate();
            board.UndoMove(move);
            val += board.PestoEvaluation(move, board.IsWhiteToMove);
            ordered_moves.Add(new Tuple<int, Move>(val, move));
        }
        ordered_moves.Sort((x, y) => y.Item1.CompareTo(x.Item1));
        if (reverse) {ordered_moves.Reverse();}
        Console.WriteLine("\nPLAYER: {0}", board.IsWhiteToMove);
        foreach ((int val, Move move) in ordered_moves){
            Console.WriteLine("ORDER MOVES: {0} | {1}", val, move);    
        }
        return ordered_moves;
    }

    // Test if this move gives checkmate
    private bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }
}