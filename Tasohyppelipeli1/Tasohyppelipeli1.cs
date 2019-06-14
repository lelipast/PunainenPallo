using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class Tasohyppelipeli1 : PhysicsGame
{
    double Nopeus = 300;
    const double HyppyNopeus = 1000;
    const int RUUDUN_KOKO = 40;

    PlatformCharacter pelaaja1;
    PlatformCharacter pelaaja2;

    Image pelaajan2Kuva = LoadImage("Sininen pallo");
    Image pelaajanKuva = LoadImage("Punainen pallo");
    Image tahtiKuva = LoadImage("tahti");

    SoundEffect maaliAani = LoadSoundEffect("maali");
    Image taustaKuva = LoadImage("Redball");

    int kenttaNro = 1;

    public override void Begin()
    {
        ClearAll();
        Gravity = new Vector(0, -1000);

        LuoKentta();
        LisaaNappaimet();
        LuoAikaLaskuri();


        Camera.Follow(pelaaja1);
        Camera.ZoomFactor = 1.2;
        Camera.StayInLevel = false;
    }

    void LuoKentta()
    {
        if (kenttaNro > 5) kenttaNro = 1;


        TileMap kentta = TileMap.FromLevelAsset("Level " + kenttaNro);
        kentta.SetTileMethod('#', LisaaTaso);
        kentta.SetTileMethod('*', LisaaTahti);
        kentta.SetTileMethod('N', LisaaPelaaja);
        kentta.SetTileMethod('M', LisaaPelaaja2);
        kentta.SetTileMethod('S', LisaaNopeus);
        kentta.SetTileMethod('F', LisaaHitaus);
        kentta.SetTileMethod('L', LisaaLopetus);
        kentta.SetTileMethod('I', LisaaNakymaton);
        kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
        Level.CreateBorders();
        Level.Background.Image = taustaKuva;
        Level.Background.ScaleToLevelFull();
    }

    void LisaaLopetus(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject Lopetus = PhysicsObject.CreateStaticObject(leveys, korkeus);
        Lopetus.Position = paikka;
        Lopetus.Color = Color.HotPink;
        Add(Lopetus);
        AddCollisionHandler(Lopetus, "pelaaja", OsuiMaaliin);
    }



    private void OsuiMaaliin(PhysicsObject maali, PhysicsObject pelaaja)
    {
        SeuraavaKentta();
    }

    
    
     

    void LisaaNakymaton(Vector paikka, double leveys, double korkeus)
    {

        PhysicsObject nakymaton = PhysicsObject.CreateStaticObject(leveys, korkeus);
        nakymaton.Position = paikka;
        nakymaton.IsVisible = false;
        Add(nakymaton);
    }

    void SeuraavaKentta()
    {
        kenttaNro ++;
        Begin();
    }
    void LisaaTaso(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Color = Color.Green;
        Add(taso);
    }


    void LisaaNopeus(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject nopeus = PhysicsObject.CreateStaticObject(leveys, korkeus);
        nopeus.Position = paikka;
        nopeus.Color = Color.LimeGreen;
        Add(nopeus);
        nopeus.IgnoresCollisionResponse = true;
        nopeus.Tag = "nopeus";
        nopeus.IsVisible = false;
    }

    void LisaaHitaus(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject hitaus = PhysicsObject.CreateStaticObject(leveys, korkeus);
        hitaus.Position = paikka;
        hitaus.Color = Color.BloodRed;
        Add(hitaus);
        hitaus.IgnoresCollisionResponse = true;
        hitaus.Tag = "hitaus";
    }

    void LisaaTahti(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject tahti = PhysicsObject.CreateStaticObject(leveys, korkeus);
        tahti.IgnoresCollisionResponse = true;
        tahti.Position = paikka;
        tahti.Image = tahtiKuva;
        tahti.Tag = "tahti";
        Add(tahti);
    }

    void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
    {
        pelaaja1 = new PlatformCharacter(leveys, korkeus);
        pelaaja1.Position = paikka;
        pelaaja1.Mass = 4.0;
        pelaaja1.Image = pelaajanKuva;
        AddCollisionHandler(pelaaja1, "tahti", TormaaTahteen);
        AddCollisionHandler(pelaaja1, "nopeus", TormaaNopeuteen);
        AddCollisionHandler(pelaaja1, "hitaus", TormaaHitauteen);
        Add(pelaaja1);
        pelaaja1.Tag = "pelaaja";

    }
    void LisaaPelaaja2(Vector paikka, double leveys, double korkeus)
    {
        pelaaja2 = new PlatformCharacter(leveys, korkeus);
        pelaaja2.Position = paikka;
        pelaaja2.Mass = 4.0;
        pelaaja2.Image = pelaajan2Kuva;
        AddCollisionHandler(pelaaja2, "tahti", TormaaTahteen);
        AddCollisionHandler(pelaaja2, "nopeus", TormaaNopeuteen);
        AddCollisionHandler(pelaaja2, "hitaus", TormaaHitauteen);
        Add(pelaaja2);
        pelaaja2.Tag = "pelaaja";

    }

    void LisaaNappaimet()
    {
        Keyboard.Listen(Key.R, ButtonState.Pressed, Begin, "Aloita alusta");
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -1.0);
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, 1.0);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HyppyNopeus);

        Keyboard.Listen(Key.T, ButtonState.Pressed, Teleporttaa, "Pelaaja teleporttaa", pelaaja2, pelaaja1);
        Keyboard.Listen(Key.A, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja2, -1.0);
        Keyboard.Listen(Key.D, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja2, 1.0);
        Keyboard.Listen(Key.W, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja2, HyppyNopeus);

        ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Poistu pelistä");

        //ControllerOne.Listen(Button.DPadLeft, ButtonState.Down, Liikuta, "Pelaaja liikkuu vasemmalle", pelaaja1, -Nopeus);
        //ControllerOne.Listen(Button.DPadRight, ButtonState.Down, Liikuta, "Pelaaja liikkuu oikealle", pelaaja1, Nopeus);
        //ControllerOne.Listen(Button.A, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HyppyNopeus);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
    }

    void Liikuta(PlatformCharacter hahmo, double kerroin)
    {
        hahmo.Walk(Nopeus * kerroin);
    }

    void Teleporttaa(PhysicsObject teleportattava, PhysicsObject kohde)
    {
        teleportattava.Position = kohde.Position;
    }

    void Hyppaa(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Jump(nopeus);
    }

    void TormaaTahteen(PhysicsObject hahmo, PhysicsObject tahti)
    {
        maaliAani.Play();
        MessageDisplay.Add("Menetit tähden!");
        tahti.Destroy();
    }
    void TormaaNopeuteen(PhysicsObject hahmo, PhysicsObject nopeus)
    {
        Nopeus = 2000;
    }

    void TormaaHitauteen(PhysicsObject hahmo, PhysicsObject hitaus)
    {
        Nopeus = 300;

    }
    

    void LuoAikaLaskuri()
    {
        Timer aikaLaskuri = new Timer();
        aikaLaskuri.Start();

        Label aikaNaytto = new Label();
        aikaNaytto.TextColor = Color.White;
        aikaNaytto.DecimalPlaces = 3;
        aikaNaytto.BindTo(aikaLaskuri.SecondCounter);
        aikaNaytto.X = Screen.Right - 30;
        aikaNaytto.Y = Screen.Top - 20;
        Add(aikaNaytto);
    }


}
