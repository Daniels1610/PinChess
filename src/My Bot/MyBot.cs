using System;
using Raylib_cs;
using System.Linq;
using ChessChallenge.API;
using static PieceSquareTables;
using System.Runtime.InteropServices;
using System.Reflection.Metadata.Ecma335;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;


// PinChess Bot Implementation
public class MyBot : IChessBot
{
    
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    static int[] pieceValues = { 0, 10, 30, 30, 50, 90, 1000 };
    int depth = 3;

    public Move Think(Board board, Timer timer)
    {

        Move[] botMoves = board.GetLegalMoves();
        // Gets each move and displays PieceType and Move's start and end square
        Console.WriteLine("BOT MOVES");
        foreach (Move move in board.GetLegalMoves())
        {
            // Console.WriteLine("Move: {0} - {1}", move.MovePieceType, move);
            if (move.MovePieceType.Equals(PieceType.Pawn)) {
                Console.WriteLine("TARGET SQUARE {0}: FILE {1} | RANK {2}", move, move.TargetSquare.File, move.TargetSquare.Rank);
                Console.WriteLine("PAWN POSITION EVAL: {0}", PestoEval(board, move, board.IsWhiteToMove));
            }
        }
        Console.WriteLine("MOVE PLAYED: {0} - {1}", botMoves[0].MovePieceType, botMoves[0]);
        
        board.TrySkipTurn();
        Console.WriteLine("IsWhiteToMove: {0}", board.IsWhiteToMove);
        Console.WriteLine("HUMAN MOVES");
        foreach (Move move in board.GetLegalMoves())
        {
            // Console.WriteLine("Move: {0} - {1}", move.MovePieceType, move);
            if (move.MovePieceType.Equals(PieceType.Pawn)) {
                Console.WriteLine("TARGET SQUARE {0}: FILE {1} | RANK {2}", move, move.TargetSquare.File, move.TargetSquare.Rank);
                Console.WriteLine("PAWN POSITION EVAL: {0}", PestoEval(board, move, board.IsWhiteToMove));
            }
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

    private int PestoEval(Board board, Move move, bool player)
		{
			int eval = 0;
			if (move.MovePieceType.Equals(PieceType.Pawn)){
                if (player) {eval += PawnTable[PawnTable.GetUpperBound(0) + move.TargetSquare.Rank * -1, move.TargetSquare.File];}
                else {eval += PawnTable[move.TargetSquare.Rank, move.TargetSquare.File] * -1;}
			}
			else if (move.MovePieceType.Equals(PieceType.Knight)){
				eval += KnightTable[KnightTable.GetUpperBound(0) + move.TargetSquare.Rank * -1, move.TargetSquare.File];
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

}