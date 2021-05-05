using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Smb3
{
    public partial class frmSmb3 : Form
    {
        Rom rom;
        public frmSmb3()
        {
            InitializeComponent();
        }

        public void Init(Rom rom)
        {
            this.rom = rom;
        }

        private void frmSmb3_Load(object sender, EventArgs e)
        {
            pctTilesheet.Image = new Bitmap(pctTilesheet.Width, pctTilesheet.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var pal = new Color[4];
            pal[0] = Color.Aquamarine;
            pal[1] = Color.DarkGray;
            pal[2] = Color.White;
            pal[3] = Color.Black;
            palettes.Add(pal);

            RedrawTileSheet();
        }

        int tiledex = 0;
        int paldex = 0;
        List<Color[]> palettes = new List<Color[]>();
        void RedrawTileSheet()
        {
            using (var g = Graphics.FromImage(pctTilesheet.Image))
            {
                g.Clear(Color.Black);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                var img = rom.banks[tiledex].GenerateBitmap(palettes[paldex], 16, 16);
                g.DrawImage(img, 0, 0, img.Width * 2, img.Height * 2);
            }

            pctTilesheet.Refresh();
        }

        private void lstMapPalettes_SelectedIndexChanged(object sender, EventArgs e)
        {
            paldex = 0;
            if (lstMapPalettes.SelectedIndex >= 0)
                paldex = lstMapPalettes.SelectedIndex;
            RedrawTileSheet();
        }


        private void txtBank_KeyDown(object sender, KeyEventArgs e)
        {
            int.TryParse(txtBank.Text, out tiledex);
            RedrawTileSheet();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int.TryParse(txtBank.Text, out tiledex);
            tiledex++;
            txtBank.Text = tiledex.ToString();
            RedrawTileSheet();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            int.TryParse(txtBank.Text, out tiledex);
            tiledex--;
            if (tiledex < 0)
                tiledex = 0;
            txtBank.Text = tiledex.ToString();
            RedrawTileSheet();
        }
    }
}
