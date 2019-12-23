using System;
using System.Collections.Generic;
using CodeCracker;

namespace SpaceImageFormat
{
    public class SIFDecoder
    {
        // final will be 25x6
        public List<int[,]> Image;
        private int Xsize;
        private int Ysize;

        public SIFDecoder(int xSize, int ySize, string encodedImage)
        {
            this.Image = new List<int[,]>();
            this.Xsize = xSize;
            this.Ysize = ySize;

            this.PopulateImage(encodedImage);
        }

        public int[,] DecodeImage()
        {
            int[,] outputImage = new int[this.Xsize, this.Ysize];

            for(int col = 0; col < this.Xsize; col++)
            {
                for(int row = 0; row < this.Ysize; row++)
                {
                    for(int a = 0; a < this.Image.Count; a++)
                    {
                        if(this.Image[a][col, row] != 2)
                        {
                            outputImage[col, row] = this.Image[a][col, row];
                            break;
                        }
                    }
                }
            }

            return outputImage;
        }

        public int FindLayerForPart1()
        {
            if(this.Image == new List<int[,]>())
            {
                throw new NullReferenceException("ERROR: Image object has not been initialized!");
            }

            int minZeroes = int.MaxValue;
            int minZeroesIndex = 0;

            for(int i = 0; i < this.Image.Count; i++)
            {
                int zeroes = this.FindCountOf(this.Image[i], 0);
                if(zeroes < minZeroes)
                {
                    minZeroes = zeroes;
                    minZeroesIndex = i;
                }
            }

            return this.FindCountOf(this.Image[minZeroesIndex], 1) * this.FindCountOf(this.Image[minZeroesIndex], 2);
        }

        private void PopulateImage(string input)
        {
            if (input.Length % (this.Xsize * this.Ysize) != 0)
            {
                throw new ArgumentOutOfRangeException("Input Length", $"ERROR: Input Length {input.Length} does not match given dimensions {this.Xsize} x {this.Ysize}");
            }

            int inputPointer = 0;

            while (inputPointer < input.Length)
            {
                int[,] gridLayer = new int[this.Xsize, this.Ysize];

                for (int row = 0; row < this.Ysize; row++)
                {
                    for (int col = 0; col < this.Xsize; col++)
                    {
                        gridLayer[col, row] = Convert.ToInt32(input[inputPointer].ToString());
                        inputPointer++;
                    }
                }

                this.Image.Add(gridLayer);
            }
        }

        private int FindCountOf(int[,] gridLayer, int target)
        {
            int count = 0;

            for(int col = 0; col < this.Xsize; col++)
            {
                for(int row = 0; row < this.Ysize; row++)
                {
                    if(gridLayer[col,row] == target)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}
