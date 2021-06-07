﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicsTools.Alundra
{
    public partial class frmGame : Form
    {
        GameEngine engine;
        Timer tmr;
        public frmGame(DatasBin datasBin)
        {
            InitializeComponent();

            engine = new GameEngine(datasBin);
            var map = datasBin.gamemaps[165];
            engine.LoadMap(map);
            tmr = new Timer();
            tmr.Interval = 1000 / 30;
            tmr.Tick += Tmr_Tick;
        }

        private void Tmr_Tick(object sender, EventArgs e)
        {
            pctOut.Refresh();
        }

        private void pctOut_Paint(object sender, PaintEventArgs e)
        {
            var backbuffer = new Bitmap(320, 240);
            using (var g = Graphics.FromImage(backbuffer))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                engine.Render(g);
            }
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.Clear(Color.Black);
            e.Graphics.DrawImage(backbuffer, 0, 0, pctOut.Width, pctOut.Height);
                
        }
    }
}
