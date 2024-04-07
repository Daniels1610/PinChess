using System;
using Raylib_cs;
using System.Linq;
using ChessChallenge.API;
using static PieceSquareTables;
using System.Runtime.InteropServices;
using System.Reflection.Metadata.Ecma335;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


// PinChess Bot Implementation
public class MyBot : IChessBot
{
    
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    static int[] pieceValues = { 0, 10, 30, 30, 50, 90, 1000 };

    public Move Think(Board board, Timer timer)
    {

        Move[] botMoves = board.GetLegalMoves();
        // Gets each move and displays PieceType and Move's start and end square
        Console.WriteLine("IsWhiteToMove: {0}", board.IsWhiteToMove);
        Console.WriteLine("BOT MOVES");
        foreach (Move move in board.GetLegalMoves())
        {
            PieceType pieceType = move.MovePieceType;
            Console.WriteLine("{0} {1}: FILE {2} | RANK {3}", pieceType,move, move.TargetSquare.File, move.TargetSquare.Rank);
            Console.WriteLine("POSITION EVALUATION: {0}", PestoEval(board, move, board.IsWhiteToMove, pieceType));
        }
        Console.WriteLine("MOVE PLAYED BY BOT: {0} - {1}", botMoves[0].MovePieceType, botMoves[0]);
        
        board.TrySkipTurn();
        Console.WriteLine("HUMAN MOVES");
        foreach (Move move in board.GetLegalMoves())
        {
            PieceType pieceType = move.MovePieceType;
            Console.WriteLine("{0} - {1}: FILE {2} | RANK {3}", pieceType,move, move.TargetSquare.File, move.TargetSquare.Rank);
            Console.WriteLine("POSITION EVALUATION: {0}", PestoEval(board, move, board.IsWhiteToMove, pieceType));
        }
        board.UndoSkipTurn();

        // int bestMove = Search(board, depth);
        // Console.WriteLine("BestEvalMove: {0}", bestMove);
        return botMoves[0];
    }

    // Evaluation function should check for:
    //   - Material Balance for both Black and White players
    //   - Developed Pieces (Center Squares) PeSTO's Evaluation Function
    //   - King safety
    //   - Pawn Structure
    //   - Tactics (Gambits, Pins, Forks, Skewers)

    int Search(Board board, int depth) 
    {
        if (depth == 0) {
            return Evaluate(board);
        }
        Move[] moves = board.GetLegalMoves();
        if (moves.Length == 0) {
            if (board.IsInCheck()) {
                return int.MinValue;
            }
            return 0;
        }

        int bestEvaluation = int.MinValue;
        foreach (Move move in moves) {
            board.MakeMove(move);
            int evaluation = - Search(board, depth-1);
            bestEvaluation = Math.Max(evaluation, bestEvaluation);
            board.UndoMove(move);
        }
        return bestEvaluation;
    }

    int Evaluate(Board board)
    {
        int whiteMaterial = CountMaterial(board, true);
        int blackMaterial = CountMaterial(board, false) * -1;

        int evaluation = whiteMaterial + blackMaterial;
        Console.WriteLine("Evaluation: {0}", evaluation);

        return evaluation;
    }

    private int PestoEval(Board board, Move move, bool player, PieceType pieceType)
		{
			int eval = 0;
            int[,] SquareTable = GetSquareTable(move);
            if (player)
            {
                eval += SquareTable[SquareTable.GetUpperBound(0) + move.TargetSquare.Rank * -1, move.TargetSquare.File];
            }
            else {
                eval += SquareTable[move.TargetSquare.Rank, SquareTable.GetUpperBound(0) + move.TargetSquare.File * -1] * -1;
            }
			return eval;
	}

    int CountMaterial(Board board, bool player)
    {
        int material = 0;
        material += board.GetPieceList(PieceType.Pawn, player).Count * pieceValues[1];
        material += board.GetPieceList(PieceType.Knight, player).Count * pieceValues[2];
        material += board.GetPieceList(PieceType.Bishop, player).Count * pieceValues[3];
        material += board.GetPieceList(PieceType.Rook, player).Count * pieceValues[4];
        material += board.GetPieceList(PieceType.Queen, player).Count * pieceValues[5];
        return material;
    }

    static int[,] GetSquareTable(Move move)
    {
        if (move.MovePieceType.Equals(PieceType.Pawn)){return PawnTable;}
        else if (move.MovePieceType.Equals(PieceType.Knight)){return KnightTable;}
        else if (move.MovePieceType.Equals(PieceType.Bishop)){return BishopTable;}
        else if (move.MovePieceType.Equals(PieceType.Rook)){return RookTable;}
        else if (move.MovePieceType.Equals(PieceType.Queen)){return QueenTable;}
        else{return KingTable;}
    }

}