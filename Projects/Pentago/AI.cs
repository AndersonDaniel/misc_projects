using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Pentago
{
    public class AI
    {
        public const int WIN_SCORE = 9999;

        public static int StateGrade(Player pPositiveCol, Board[,] matBoards)
        {


            return (0);
        }
    }

    public class Move
    {
        private Player m_pColor;
        private Board m_bPlaceBoard;
        private Point m_pPlaceLoc;
        private Board m_bRotBoard;

        public Player Player
        {
            get
            {
                return (this.m_pColor);
            }
        }

        public Move(Player pPlayer, Board bPlaceBoard, Point pLocation, Board bRotBoard)
        {
            this.m_pColor = pPlayer;
            this.m_bPlaceBoard = bPlaceBoard;
            this.m_pPlaceLoc = pLocation;
            this.m_bRotBoard = bRotBoard;
        }
    }

    public class MovesTreeNode
    {
        private Move m_mvMove;
        private List<Move> m_lstMoves;
        private int m_nPrice;
        public MovesTreeNode(Move mvMove)
        {
            this.m_mvMove = mvMove;
            this.m_lstMoves = new List<Move>();
        }
    }
}
