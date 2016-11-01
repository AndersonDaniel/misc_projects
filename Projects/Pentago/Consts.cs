using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Pentago
{
    public enum Player : int
    {
        Empty = 0,
        Red = 1,
        Gold = 2
    }
    static class Consts
    {
        private static readonly Random rand = new Random();
        public const int SIZE = 3;
        public const int BOARDS = 2;
        public const int BRD_WIDTH = 200;
        public const int BRD_HEIGHT = BRD_WIDTH;
        public const int SPACE = 10;
        public static readonly Image BOARD_IMAGE = Pentago.Properties.Resources.board;
        public static readonly Image BACKGROUND_IMAGE = Pentago.Properties.Resources.back1;
        public static readonly Image[] SPHERE_IMAGES = {Pentago.Properties.Resources.empty_sphere,
                                                 Pentago.Properties.Resources.red_sphere,
                                                 Pentago.Properties.Resources.gold_sphere};
        public static readonly Image DECOR = Pentago.Properties.Resources.decor;
        public static readonly int SPHERE_SIZE = (Consts.BOARD_IMAGE.Width - SPACE) / Consts.SIZE - SPACE;
        public static readonly double RELATION = (double)Consts.BRD_WIDTH / Consts.BOARD_IMAGE.Width;
        static Consts()
        {
            CorrectImage(BOARD_IMAGE);
            CorrectImage(BACKGROUND_IMAGE);
            CorrectImage(DECOR);
            for (int i = 0; i < Enum.GetValues(typeof(Player)).Length; i++)
            {
                CorrectImage(SPHERE_IMAGES[i]);
            }
        }

        private static void CorrectImage(Image img)
        {
            ((Bitmap)img).MakeTransparent(Color.White);
        }

        public static int GetRand(int min, int max)
        {
            return (Consts.rand.Next(min, max));
        }
    }
}
