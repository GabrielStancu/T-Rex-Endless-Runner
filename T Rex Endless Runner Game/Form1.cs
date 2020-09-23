using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace T_Rex_Endless_Runner_Game
{
    public partial class Form1 : Form
    {
        bool jumping = false; //variabila bool care verifica daca jucatorul sare
        int jumpSpeed = 10; //viteza cu care jucatorul sare 
        int force = 12; //forta saltului
        int score = 0; //scorul jucatorului
        int obstacleSpeed = 10; //viteza cu care se apropie obstacolele
        Random rnd = new Random(); //o noua instanta a clasei random

        public Form1()
        {
            InitializeComponent();
            resetGame(); //apelam functia resetGame ca sa incepem un joc nou 
        }

        private void gameEvent(object sender, EventArgs e) //event-ul care are loc la fiecare tick al timer-ului din design, functioneaza ca un while, dar reintra in secventa de cod dupa un interval de timp (20 milisecunde in cazul de fata)
        {
            trex.Top += jumpSpeed; //pictureBox-ul cu dinozaurul sare cu viteza determinata de variabila, daca e 0 nu sare deloc, 10 = sare, -10 = cade 
            scoreText.Text = "Score " + score; //score-ul isi da refresh la fiecare tick al timerului

            if (jumping) //daca dinozaurul inca sare 
            {
                jumpSpeed = -12; //setam variabila pe -12, viteza cu care dinozaurul urca in ecran 
                force -= 1; //forta saltului scade treptat pentru a ajunge la "limita" de salt cand variabila ajunge pe false 
            }
            else //daca dinozaurul nu sare 
            {
                jumpSpeed = 12; //setam variabila pe 12, viteza cu care dinozaurul cade 
            }

            if (jumping && force < 0)
            {
                jumping = false; //daca forta a ajuns la 0 inseamna ca nu mai sare si setam variabila pe false 
            }

            foreach (Control x in this.Controls) //parcurgem cu un for toate controalele din fereastra 
            {
                if (x is PictureBox && x.Tag.ToString() == "obstacle") //daca e pictureBox si are tag-ul "obstacle" inseamna ca e obstacol, trebuie sa le deplasam spre stanga 
                {
                    x.Left -= obstacleSpeed; //deplasam obstacolele spre stanga 

                    if (x.Left + x.Width < -120) //daca partea din dreapta a obstacolului a iesit prin partea stanga a ecranului
                    {
                        x.Left = this.ClientSize.Width + rnd.Next(200, 800); //il amplasam foarte departe in dreapta, la o distanta aleatorie intre 200 si 800 pixeli + latimea ferestrei
                        score++; //scorul creste cu 1 
                    }

                    if (trex.Bounds.IntersectsWith(x.Bounds)) //daca trex se intersecteaza cu vreun obstacol
                    {
                        gameTimer.Stop(); //oprim cronometrul
                        trex.Image = Properties.Resources.dead; //schimbam imaginea cu cea a dinozaurului mort 
                        scoreText.Text += " Press R to restart the game..."; //in dreapta scorului adaugam mesajul pentru jucator 
                    }
                }
            }

            if (trex.Top >= 380 && !jumping) //daca trex a atins partea superioara a platformei pe care alearga in urma unui salt
            {
                force = 12; //setam variabila pe 12 pentru a permite urmatorul salt 
                trex.Top = floor.Top - trex.Height; //setam pozitiia pictureBox-ului cu trex pentru a nu cadea prin platforma si sa iasa din ecran 
                jumpSpeed = 0; //variabila e 0 pentru ca dinozaurul nu mai sare, tocmai a terminat un salt 
            }

            if (score >= 10)
            {
                obstacleSpeed = 15; //daca scorul este mai mare de 10, obstacolele se apropie mai repede, la fel si mai jos pentru 25, 50, 100
            }
            else if (score >= 25)
            {
                obstacleSpeed = 20;
            }
            else if (score >=50)
            {
                obstacleSpeed = 25;
            }
            else if(score >=100)
            {
                obstacleSpeed = 30;
            }
        }

        private void keyisup(object sender, KeyEventArgs e) //functie care are loc atunci cand jucatorul ridica degetul de pe o anumita tasta 
        {
            if (e.KeyCode == Keys.R)
            {
                resetGame(); //daca a ridicat degetul de pe tasta R, jocul se reseteaza prin apelarea functiei resetGame
            }

            if (jumping)
            {
                jumping = false; //daca a ridicat degetul de pe spatiu, dinozaurul nu mai sare si incepe sa cada. Setam variabila jumping pe false pentru a determina asta 
            }
        }

        private void keyisdown(object sender, KeyEventArgs e) //functie care are loc cand jucatorul apasa o anumita tasta 
        {
            if (e.KeyCode == Keys.Space && !jumping) //daca tasta spatiu este apasata si jucatorul nu este deja in mijlocul unui salt
            {
                jumping = true; //variabila devine true ca sa determinam ca jucatorul este in timpul unui salt 
            }
        }

        private void resetGame() //functie apelata la rularea programului pentru a incepe joc nou, respectiv la apasarea tastei R
        {
            force = 12; //setam forta pe 12 pentru a perminte primul salt
            trex.Top = floor.Top - trex.Height; //amplasam trex-ul pe platforma 
            jumpSpeed = 0; //trex este jos, nu sare la inceput 
            score = 0; //scorul se reseteaza 
            obstacleSpeed = 10; //resetam viteza cu care se apropie obstacolele 
            scoreText.Text = "Score: " + score; //schimbam textul pentru a afisa doar scorul, fara mesajul cu apasarea tastei R
            trex.Image = Properties.Resources.running; //schimbam imaginea dinozaurului in cea in care alearga

            foreach(Control x in this.Controls) //parcurgem cu un for toate controalele
            {
                if (x is PictureBox && x.Tag.ToString() == "obstacle") //daca e pictureBox si are tag-ul de obstacol
                {
                    int position = rnd.Next(600, 1000); //generam aleatoriu pozitia obstacolului
                    x.Left = 640 + x.Left + position + x.Width * 3; //amplasam obstacolul
                }
            }

            gameTimer.Start(); //pornim cronometrul 
        }
    }
}
//Tutorial original: https://www.mooict.com/c-tutorial-create-a-t-rex-endless-runner-game-in-visual-studio/
//Pentru a adauga event-uri la orice control (fereastra, pictureBox, timer etc), se selecteaza controlul in designer, in fereastra de proprietati a acestuia se da click pe fulgerul din partea de sus
//Se cauta in lista eventul dorit, se scrie numele functiei care va fi apelata, se apasa Enter si functia se adauga in cod 