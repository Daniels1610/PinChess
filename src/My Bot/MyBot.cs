using System;
using ChessChallenge.API;
using static PieceSquareTables;

// PinChess Bot Implementation
public class MyBot : IChessBot
{
    
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    static readonly int[] pieceValues = { 0, 10, 30, 30, 50, 90, 1000 };
    readonly int depth = 1;

    public Move Think(Board board, Timer timer)
    {
        Move[] possibleMoves = board.GetLegalMoves();
        Move bestMove = Move.NullMove;
        int bestWhiteEval = -10000;
        int bestBlackEval = 10000;
        int miniMaxBestEval = 0;
        int MMEval;

        // Gets each move and displays PieceType and Move's start and end square
        Console.WriteLine("IsWhiteToMove: {0}", board.IsWhiteToMove);
        Console.WriteLine("BOT MOVES");
        foreach (Move move in board.GetLegalMoves())
        {
            MMEval = Minimax(board, depth, move, miniMaxBestEval);
            
            // Check if move is a checkmate
            if (MoveIsCheckmate(board, move))
                {
                    bestMove = move;
                    break;
                }
            // MMEval = PestoEval(move, board.IsWhiteToMove);
            if (board.IsWhiteToMove && MMEval > bestWhiteEval) {
                bestWhiteEval = MMEval;
                bestMove = move;
            }
            else if (!board.IsWhiteToMove && (MMEval < bestBlackEval)) {
                bestBlackEval = MMEval;
                bestMove = move;
            }
            
            Console.WriteLine("TARGET SQUARE {0}: FILE {1} | RANK {2}", move, move.TargetSquare.File, move.TargetSquare.Rank);
            Console.WriteLine("POSITION EVALUATION: {0}", MMEval);
        }
        Console.WriteLine("MOVE PLAYED BY BOT: {0} - {1}", bestMove.MovePieceType, bestMove);
        
        // board.MakeMove(bestMove);
        return bestMove;
    }

    // Evaluation function should check for:
    //   - Material Balance for both Black and White players
    //   - Developed Pieces (Center Squares) PeSTO's Evaluation Function
    //   - King safety
    //   - Pawn Structure
    //   - Tactics (Gambits, Pins, Forks, Skewers)

    private int Minimax(Board board, int depth, Move move, int miniMaxBestEval) 
    {
        if (board.IsInCheckmate() || depth == 0) 
        {
            return Evaluate(board, move);
        }

        if (board.IsWhiteToMove)
        {
            int best = -10000;
            foreach (Move evalMove in board.GetLegalMoves())
            {
                int val = Minimax(board, depth-1, evalMove, miniMaxBestEval);
                if (val > best)
                {
                    best = val;
                    miniMaxBestEval = best;
                }
            }
        }

        else {
            int best = 10000;

            foreach (Move evalMove in board.GetLegalMoves())
            {
                int val = Minimax(board, depth-1, evalMove, miniMaxBestEval);
                if (val < best) 
                {
                    best = val;
                    miniMaxBestEval = best;
                }
            }
        }
        return miniMaxBestEval;
    }

    int Evaluate(Board board, Move move)
    {
        int whiteMaterial = CountMaterial(board, true);
        int blackMaterial = CountMaterial(board, false) * -1;
        int evaluation = whiteMaterial + blackMaterial + PestoEval(move, board.IsWhiteToMove);
        return evaluation;
    }

    private static int PestoEval(Move move, bool player)
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

    bool MoveIsCheckmate(Board board, Move move)
        {
            board.MakeMove(move);
            bool isMate = board.IsInCheckmate();
            board.UndoMove(move);
            return isMate;
        }
    
}