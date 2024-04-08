using System;
using ChessChallenge.API;
using static PieceSquareTables;

// PinChess Bot Implementation
public class MyBot : IChessBot
{
    
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    static readonly int[] pieceValues = { 0, 10, 30, 30, 50, 90, 1000 };
    readonly int depth = 3;

    public Move Think(Board board, Timer timer)
    {
        Move bestMove = Move.NullMove;
        int bestWhiteEval = int.MinValue;
        int bestBlackEval = int.MaxValue;
        int MMEval = 0;

        // Gets each move and displays PieceType and Move's start and end square
        Console.WriteLine("IsWhiteToMove: {0}", board.IsWhiteToMove);
        Console.WriteLine("BOT MOVES");
        foreach (Move move in board.GetLegalMoves())
        {
            board.MakeMove(move);
            MMEval = Minimax(board, depth-1, move, !board.IsWhiteToMove);
            board.UndoMove(move);
            // int MMEval = PestoEval(move, board.IsWhiteToMove);
            
            // Check if move is a checkmate
            if (MoveIsCheckmate(board, move))
                {
                    bestMove = move;
                    break;
                }

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
        return bestMove;
    }

    // Evaluation function should check for:
    //   - Material Balance for both Black and White players
    //   - Developed Pieces (Center Squares) PeSTO's Evaluation Function
    //   - King safety
    //   - Pawn Structure
    //   - Tactics (Gambits, Pins, Forks, Skewers)

    private int Minimax(Board board, int depth, Move move, bool isMaximizingPlayer) 
    {
        if (board.IsInCheckmate() || board.IsInStalemate() || depth == 0) 
        {
            return Evaluate(board) + PestoEval(move, isMaximizingPlayer);
        }

        if (isMaximizingPlayer)
        {
            int maxEval = int.MinValue;
            foreach (Move evalMove in board.GetLegalMoves())
            {
                board.MakeMove(evalMove);
                int val = Minimax(board, depth-1, evalMove, false);
                board.UndoMove(evalMove);
                maxEval = Math.Max(maxEval, val);
            }
            return maxEval;
        }

        else {
            int minEval = int.MaxValue;

            foreach (Move evalMove in board.GetLegalMoves())
            {
                board.MakeMove(evalMove);
                int val = Minimax(board, depth-1, evalMove, true);
                board.UndoMove(evalMove);
                minEval = Math.Min(minEval, val);
            }
            return minEval;
        }
    }

    int Evaluate(Board board)
    {
        int whiteMaterial = CountMaterial(board, true);
        int blackMaterial = CountMaterial(board, false)*-1;
        int materialEvaluation = whiteMaterial + blackMaterial;
        int capturedEvaluation = EvaluateCaptures(board, board.IsWhiteToMove);
        int evaluation = materialEvaluation + capturedEvaluation;
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

    int EvaluateCaptures(Board board, bool isWhite)
{
    int playerCaptureValue = 0;
    int opponentCaptureValue = 0;

    foreach (Move move in board.GetLegalMoves())
    {
        board.MakeMove(move);
        if (move.IsCapture)
        {
            Piece capturedPiece = board.GetPiece(move.TargetSquare);
            if (board.IsWhiteToMove == isWhite)
            {
                opponentCaptureValue += pieceValues[(int)capturedPiece.PieceType];
            }
            else
            {
                playerCaptureValue += pieceValues[(int)capturedPiece.PieceType];
            }
        }
        board.UndoMove(move);
    }
    return isWhite ? (playerCaptureValue - opponentCaptureValue) : (opponentCaptureValue - playerCaptureValue);
}

    
}