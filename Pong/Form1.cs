namespace Pong
{
    public partial class Form1 : Form
    {
    
    const int ScoreMax = 5;
    const int VitesseDepart = 2;
    const int VitesseMax = 5;
    const int VitesseRequette = 4;

    
    readonly Rectangle zoneJeu = new Rectangle(0, 0, 200, 200);

    
    Rectangle rj1, rj2, balle;      
    int sj1, sj2;                   
    int vBalle;                     
    int sensH, sensV;               
    bool j1Haut, j1Bas, j2Haut, j2Bas;

    readonly Random rnd = new Random();
    readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

    public Form1()
    {
        InitializeComponent();
        Text = "Ping Pong";
        KeyPreview = true;
        KeyDown += Form1_KeyDown;
        KeyUp += Form1_KeyUp;

        DoubleBuffered = true;
        BackColor = Color.Black;
        ClientSize = new Size(800, 600);

        Initialiser();

        timer.Interval = 20;
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    
    int Sensalea()
    {
        return rnd.Next(2) == 0 ? -1 : 1;
    }

    
    void Initialiser() 
    {
        sj1 = 0;
        sj2 = 0;

        
        j1Haut = j1Bas = j2Haut = j2Bas = false;

        RemettreJeu(Sensalea());
    }

 
    void RemettreJeu(int sens)
    {
        rj1 = new Rectangle(10, 85, 5, 30);
        rj2 = new Rectangle(185, 85, 5, 30);
        balle = new Rectangle(98, 98, 4, 4);

        vBalle = VitesseDepart;
        sensH = sens;
        sensV = Sensalea();
    }

    
    void EffetRequette(Rectangle r)
    {
        int centreBalle = balle.Y + balle.Height / 2;
        int tiers = r.Height / 3;

        if (centreBalle < r.Y + tiers) sensV = -1;
        else if (centreBalle > r.Bottom - tiers) sensV = 1;

        if (vBalle < VitesseMax) vBalle++;
    }

    void Form1_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Z || e.KeyCode == Keys.W) j1Haut = true;
        if (e.KeyCode == Keys.S) j1Bas = true;
        if (e.KeyCode == Keys.Up || e.KeyCode == Keys.O) j2Haut = true;
        if (e.KeyCode == Keys.Down || e.KeyCode == Keys.L) j2Bas = true;

        if (e.KeyCode == Keys.Space && !timer.Enabled)
        {
            Initialiser();
            timer.Start();
        }
        e.Handled = true;
    }

    void Form1_KeyUp(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Z || e.KeyCode == Keys.W) j1Haut = false;
        if (e.KeyCode == Keys.S) j1Bas = false;
        if (e.KeyCode == Keys.Up || e.KeyCode == Keys.O) j2Haut = false;
        if (e.KeyCode == Keys.Down || e.KeyCode == Keys.L) j2Bas = false;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        
        if (j1Haut) rj1.Y = Math.Max(0, rj1.Y - VitesseRequette);
        if (j1Bas) rj1.Y = Math.Min(zoneJeu.Height - rj1.Height, rj1.Y + VitesseRequette);
        if (j2Haut) rj2.Y = Math.Max(0, rj2.Y - VitesseRequette);
        if (j2Bas) rj2.Y = Math.Min(zoneJeu.Height - rj2.Height, rj2.Y + VitesseRequette);

       
        balle.X += sensH * vBalle;
        balle.Y += sensV * vBalle;

      
        if (balle.Y <= 0)
        {
            balle.Y = 0;
            sensV = 1;
        }
        if (balle.Bottom >= zoneJeu.Height)
        {
            balle.Y = zoneJeu.Height - balle.Height;
            sensV = -1;
        }

      
        if (sensH == -1 && balle.IntersectsWith(rj1))
        {
            sensH = 1;
            balle.X = rj1.Right;
            EffetRequette(rj1);
        }

     
        if (sensH == 1 && balle.IntersectsWith(rj2))
        {
            sensH = -1;
            balle.X = rj2.X - balle.Width;
            EffetRequette(rj2);
        }

        
        if (balle.X <= 0)
        {
            sj2++;
            RemettreJeu(-1);
        }

        
        if (balle.Right >= zoneJeu.Width)
        {
            sj1++;
            RemettreJeu(1);
        }

       
        if (sj1 >= ScoreMax || sj2 >= ScoreMax)
        {
            timer.Stop();
            Invalidate();
            Update();
            MessageBox.Show((sj1 >= ScoreMax ? "Joueur 1 gagne !" : "Joueur 2 gagne !")
                + "Appuyez sur Espace pour rejouer.");
            return;
        }

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        float scale = Math.Min(
            (float)ClientSize.Width / zoneJeu.Width,
            (float)ClientSize.Height / zoneJeu.Height);

        float offsetX = (ClientSize.Width - zoneJeu.Width * scale) / 2f;
        float offsetY = (ClientSize.Height - zoneJeu.Height * scale) / 2f;

       
        e.Graphics.DrawRectangle(Pens.Gray,
            offsetX, offsetY, zoneJeu.Width * scale, zoneJeu.Height * scale);

     
        using (var pen = new Pen(Color.Gray) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
        {
            float milieu = offsetX + zoneJeu.Width * scale / 2f;
            e.Graphics.DrawLine(pen, milieu, offsetY, milieu, offsetY + zoneJeu.Height * scale);
        }

     
        e.Graphics.FillRectangle(Brushes.White,
            offsetX + rj1.X * scale, offsetY + rj1.Y * scale,
            rj1.Width * scale, rj1.Height * scale);

        e.Graphics.FillRectangle(Brushes.White,
            offsetX + rj2.X * scale, offsetY + rj2.Y * scale,
            rj2.Width * scale, rj2.Height * scale);

      
        e.Graphics.FillRectangle(Brushes.Yellow,
            offsetX + balle.X * scale, offsetY + balle.Y * scale,
            balle.Width * scale, balle.Height * scale);

 
        using (var font = new Font("Consolas", 28, FontStyle.Bold))
        using (var sf = new StringFormat { Alignment = StringAlignment.Center })
        {
            e.Graphics.DrawString($"{sj1}  -  {sj2}", font, Brushes.White,
                ClientSize.Width / 2f, offsetY + 10, sf);
        }
    }
}
}
