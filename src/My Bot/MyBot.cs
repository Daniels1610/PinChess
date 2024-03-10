using System;
using ChessChallenge.API;

// PinChess Bot Implementation
public class MyBot : IChessBot
{
    
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    static int[] pieceValues = { 0, 10, 30, 30, 50, 90, 1000 };
    int depth = 3;

    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        foreach (Move move in moves )
        {
            Console.WriteLine("Move: {0} - {1}", move.MovePieceType, move);
        }
        Console.WriteLine("Evaluation: {0}", Evaluate(board));
        
        return moves[0];
    }

    int Evaluate(Board board)
    {
        int whiteEval = CountMaterial(board);
        int blackEval = CountMaterial(board);

        int evaluation = whiteEval - blackEval;

        int perspective = board.IsWhiteToMove ? 1 : -1;
        return evaluation * perspective;
    }

    int CountMaterial(Board board)
    {
        int material = 0; bool player = board.IsWhiteToMove;
        material += board.GetPieceList(PieceType.Pawn, player).Count * pieceValues[1];
        material += board.GetPieceList(PieceType.Knight, player).Count * pieceValues[2];
        material += board.GetPieceList(PieceType.Bishop, player).Count * pieceValues[3];
        material += board.GetPieceList(PieceType.Rook, player).Count * pieceValues[4];
        material += board.GetPieceList(PieceType.Queen, player).Count * pieceValues[5];
        return material;
    }

}