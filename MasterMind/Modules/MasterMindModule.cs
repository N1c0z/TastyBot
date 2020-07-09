﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MasterMind.Contracts;
using System.Drawing;
using Discord;
using Discord.WebSocket;
using System.IO;
using System.Xml.Schema;
using System.Linq;

namespace MasterMind.Modules
{
    public class Line
    {
        public int LineNum { get; set; }
        public List<System.Drawing.Color> ColorGuessColor { get; set; }
        public List<int> ColorGuessNumber { get; set; }
        public int RedAmount { get; set; }

        public int WhiteAmount { get; set; }
    }
    
    public class NumberId
    {
        public System.Drawing.Color Color { get; set; }
        public int Id { get; set; }
    }
    public class MasterMindModule : IMasterMindModule
    {
        static public System.Drawing.Color[] aColor = {System.Drawing.Color.Gray, System.Drawing.Color.Black, System.Drawing.Color.Yellow, System.Drawing.Color.Orange, System.Drawing.Color.Purple, System.Drawing.Color.Green, System.Drawing.Color.Blue, System.Drawing.Color.Red };
        List<NumberId> numberIds;
        MemoryStream streamPicture;
        List<Line> lines;
        
        MemoryStream pictureStream;
        Bitmap bitPicture;
        Graphics g;
        int widthX, heightY;
        bool GameRunning;

        List<int> SecretPattern;
        public bool IsGameRunning()
        {
            return GameRunning;
        }
        public async Task<MemoryStream> StartBoardMaker(int height, int width)
        {
            //calculates the amount of pixels needed
            int numOfPizelHeight;
            int numOfPixelWidth;
            numOfPizelHeight = ((height * 2) + 1) * 50;
            numOfPixelWidth = width * 2;
            numOfPixelWidth = numOfPixelWidth * 50;
            numOfPixelWidth = numOfPixelWidth + ((width * 2 + 1) * 20);
            widthX = width;
            heightY = height;

            bitPicture = new Bitmap(numOfPixelWidth, numOfPizelHeight);
            //changes every pixel color to gray
            for (var x = 0; x < bitPicture.Width; x++)
            {
                for (var y = 0; y < bitPicture.Height; y++)
                {
                    bitPicture.SetPixel(x, y, System.Drawing.Color.Transparent);
                }
            }
            //idk
            g = Graphics.FromImage(bitPicture);
            Pen pPen = new Pen(System.Drawing.Color.Gray);
            float xcCircle = bitPicture.Width;
            float ycCircle = bitPicture.Height;
            float xsCircle = bitPicture.Width;
            float ysCircle = bitPicture.Height;
            float ydCircle = bitPicture.Height - 60;
            SolidBrush brush = new SolidBrush(System.Drawing.Color.Gray);
            SolidBrush brushh = new SolidBrush(System.Drawing.Color.Gray);
            //i actually dont know how his works but hey, it works :)
            for (int p = 0; p < height; p++)
            {
                ysCircle -= 100;
                ycCircle -= 100;
                
                for (int i = 0; i < width; i++)
                {
                    xcCircle -= 100;
                    
                    g.DrawEllipse(pPen, xcCircle, ycCircle, 50, 50);
                    g.FillEllipse(brush, xcCircle, ycCircle, 50, 50);
                    
                }
                xsCircle = xcCircle;
                for (int o = 0; o < width; o++)
                {
                    xsCircle -= 40;
                    g.DrawEllipse(pPen, xsCircle, ysCircle, 20, 20);
                    g.FillEllipse(brushh, xsCircle, ysCircle, 20, 20);
                    g.DrawEllipse(pPen, xsCircle, ydCircle, 20, 20);
                    g.FillEllipse(brushh, xsCircle, ydCircle, 20, 20);
                    
                }
                
                xcCircle = bitPicture.Width;
                xsCircle = bitPicture.Width - width * 100;
                ydCircle -= 100;
            }
            //totally not copied from stackoverflow, plz dont judge
            streamPicture = new MemoryStream();
            bitPicture.Save(streamPicture, System.Drawing.Imaging.ImageFormat.Png);
            streamPicture.Seek(0, SeekOrigin.Begin);
            return streamPicture;
        }
        
        public async Task MakePattern()
        {
            Random random = new Random();
            SecretPattern = new List<int>();
            for (int i = 0; i < widthX; i++)
            {
                int Number = new int();
                Number = random.Next(1, 8); // gray isnt a color
                SecretPattern.Add(Number);
            }
        }

        public async Task LineEditor(int lineNum, List<System.Drawing.Color> colors, int amountRed, int amountWhite)
        {
            int pixelsHeight = bitPicture.Height;
            pixelsHeight -= lineNum * 100;
            int pixelWidth = bitPicture.Width;
            int pixelWidthh;
            foreach (System.Drawing.Color color in colors)
            {
                pixelWidth -= 100;
                Pen pen = new Pen(color);
                SolidBrush brush = new SolidBrush(color);
                g.DrawEllipse(pen, pixelsHeight, pixelWidth, 50, 50);
                g.FillRectangle(brush, pixelsHeight, pixelWidth, 50, 50);
                
            }
            pixelWidthh = pixelWidth;
            for (int i = 0; i < amountRed; i++)
            {
                pixelWidth -= 40;
                Pen pen = new Pen(System.Drawing.Color.Red);
                SolidBrush brush = new SolidBrush(System.Drawing.Color.Red);
                g.DrawEllipse(pen, pixelsHeight, pixelWidth, 50, 50);
                g.FillRectangle(brush, pixelsHeight, pixelWidth, 50, 50);
            }
            pixelWidth = pixelWidthh;
            pixelsHeight += 40;
            for (int i = 0; i < amountWhite; i++)
            {
                pixelWidth -= 40;
                Pen pen = new Pen(System.Drawing.Color.White);
                SolidBrush brush = new SolidBrush(System.Drawing.Color.White);
                g.DrawEllipse(pen, pixelsHeight, pixelWidth, 50, 50);
                g.FillRectangle(brush, pixelsHeight, pixelWidth, 50, 50);
            }

        }
        public async Task<List<bool>> RedNumberPosition(List<int> colorGuess)
        {
            List<bool> redPosition = new List<bool>();
            for (int i = 0; i < widthX; i++)
            {
                redPosition.Add(false);
            }
            int l = 0;
            foreach (int numPattern in SecretPattern)
            {
                if (numPattern == colorGuess[l])
                {
                    redPosition[l] = true;
                }
                l++;
            }
            return redPosition;
        }

        public async Task<int> RedNum(List<int> colorGuess)
        {
            int i = 0;
            int red = 0;
            foreach (int Num in SecretPattern)
            {
                if (Num == colorGuess[i])
                {
                    red++;
                }
                i++;
            }
            return red;
        }

        public async Task<int> WhiteNum(List<int> colorGuessGuess)
        {
            List<bool> RedNumber = await RedNumberPosition(colorGuessGuess);
            int x = 0;
            int y = 0;
            int white = 0;
            foreach (int num in SecretPattern)
            {
                foreach (int guess in colorGuessGuess)
                {
                    if (!RedNumber[x])
                    {
                        if (!RedNumber[y])
                        {
                            if (num == guess)
                            {
                                white++;
                            }
                        }
                    }
                    y++;
                }
                y = 0;
                x++;
            }
            return white;
        }

        //called when the game starts in case it wasnt obv enough
        public async Task StartGame()
        {
            GameRunning = true;
            await MakePattern();
            int num = 0;
            numberIds = new List<NumberId>();

            foreach (var item in aColor)
            {
                NumberId numberId = new NumberId();
                numberId.Color = aColor[num];
                numberId.Id = num;
                num++;
                numberIds.Add(numberId);
            }
            lines = new List<Line>();
            for (int i = 0; i < heightY; i++)
            {
                Line line = new Line();
                line.LineNum = i;
                lines.Add(line);
            }
        }

        public async Task RunGame()
        {
            
        }
    }
}
