
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;


/// @author  Oskari Kainulainen
/// @version 1.0

public class Pallopeli : PhysicsGame
{
    /// <summary>
    /// kauhalle kuva
    /// </summary>
    private Image kauhaKuva = LoadImage("kauha.png");

    /// <summary>
    /// kauhan vasemmalle liikuttamista varten tarvittava vektori
    /// </summary>
    private Vector nopeusVasen = new Vector(-200, 0);

    /// <summary>
    /// kauhan oikealle liikuttamista varten tarvittava vektori
    /// </summary>
    private Vector nopeusOikea = new Vector(200, 0);


    /// <summary>
    /// pudotuspallon teko atribuutiksi
    /// </summary>
    private PhysicsObject pudotuspallo;

    /// <summary>
    /// kauhan teko atribuutiksi
    /// </summary>
    private PhysicsObject kauha;


    /// <summary>
    /// elämäpistelaskuri
    /// </summary>
    private IntMeter elamat;


    public override void Begin()
    {
        LuoKentta();
        AsetaOhjaimet();

        MultiSelectWindow alkuvalikko = new MultiSelectWindow("Pallopeli", "Aloita peli", "Lopeta");
        alkuvalikko.AddItemHandler(1, Exit);
        alkuvalikko.AddItemHandler(0, AloitaPeli);
        Add(alkuvalikko);
    }


    /// <summary>
    /// Luodaan kenttä, kauhat sekä elämäpistelaskuri
    /// </summary>
    public void LuoKentta()
    {
        Level.CreateLeftBorder();
        Level.CreateRightBorder();
        Level.CreateTopBorder();
        PhysicsObject alaReuna = Level.CreateBottomBorder();
        alaReuna.Tag = "ala";

        Level.Background.Color = Color.Black;

        //Kuva TitleMappiin
        ColorTileMap ruudut = ColorTileMap.FromLevelAsset("kentta");

        //Pikselien värit
        ruudut.SetTileMethod(Color.Black, LuoPallo);

        //luo kentän
        ruudut.Execute(34.2, 25);

        LuoElamat();

        kauha = LuoKauha(0.0, Level.Bottom + 10);

        Camera.ZoomToLevel();
    }


    /// <summary>
    /// luodaan elämäpistelaskuri
    /// </summary>
    void LuoElamat()
    {
        elamat = new IntMeter(3);

        Label elamanaytto = new Label();
        elamanaytto.X = Screen.Right - 10;
        elamanaytto.Y = Screen.Top - 10;
        elamanaytto.TextColor = Color.White;

        elamanaytto.BindTo(elamat);
        Add(elamanaytto);
    }


    /// <summary>
    /// Luodaan peliin pallo
    /// </summary>
    /// <param name="paikka">pallon paikka</param>
    /// <param name="leveys">pallon leveys</param>
    /// <param name="korkeus">pallon korkeus</param>
    private void LuoPallo(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject pallo = PhysicsObject.CreateStaticObject(40, 40);
        pallo.Shape = Shape.Circle;
        pallo.Color = Color.Gray;
        pallo.Position = paikka;
        pallo.Restitution = 1.0;
        pallo.KineticFriction = 0.0;
        Add(pallo);
    }


    /// <summary>
    /// luodaan peliin kauha
    /// </summary>
    /// <param name="x">kauhan sijainti x-akselilla</param>
    /// <param name="y">kauhan sijainti y-akselilla</param>
    private PhysicsObject LuoKauha(double x, double y)
    {
        kauha = PhysicsObject.CreateStaticObject(100, 15);
        kauha.Shape = Shape.Rectangle;
        kauha.Color = Color.Gray;
        kauha.X = x;
        kauha.Y = y;
        kauha.Restitution = 1.0;
        kauha.KineticFriction = 0.0;
        kauha.Image = kauhaKuva;
        Add(kauha);
        return kauha;
    }


    /// <summary>
    /// Luodaan pudotuspallo
    /// </summary>
    /// <param name="x">pallon sijainti x-akselilla</param>
    public PhysicsObject LuoPudotuspallo(double x)
    {
        pudotuspallo = new PhysicsObject(15, 15);
        pudotuspallo.Shape = Shape.Circle;
        pudotuspallo.Color = Color.White;
        pudotuspallo.X = x;
        pudotuspallo.Y = Screen.Top - 10;
        pudotuspallo.Restitution = 1.0;
        pudotuspallo.KineticFriction = 0.0;
        Add(pudotuspallo);
        AddCollisionHandler(pudotuspallo, "ala", Huti);
        return pudotuspallo;
    }


    /// <summary>
    /// Jos pallo pääse kauhan ohi, niin vähennetään elämäpistelaskurista piste ja luodaan uusi pudotuspallo.
    /// Jos elämäpisteet loppuvat niin lopetetaan peli.
    /// </summary>
    /// <param name="pallo">pallo jota seurataan</param>
    /// <param name="kohde"></param>
    public void Huti(PhysicsObject pallo, PhysicsObject kohde)
    {
        pallo.Destroy();
        elamat.Value -= 1;
        if (elamat.Value == 0)
        {
            MessageDisplay.Add("Kuolit");
            Keyboard.Disable(Key.A);
            Keyboard.Disable(Key.D);
            kauha.Velocity = Vector.Zero;
            MultiSelectWindow alkuvalikko = new MultiSelectWindow("Pallopeli", "Aloita peli", "Lopeta");
            alkuvalikko.AddItemHandler(1, Exit);
            alkuvalikko.AddItemHandler(0, AloitaPeli);
            Add(alkuvalikko);
        }
        else
        {
            LuoPudotuspallo(20);
            pudotuspallo.Hit(new Vector(RandomGen.NextInt(-100, 100), -60));
        }

    }


    /// <summary>
    /// Aloitetaan peli ja nollataan elämäpistelaskuri
    /// </summary>
    void AloitaPeli()
    {
        Keyboard.Enable(Key.A);
        Keyboard.Enable(Key.D);
        elamat.Value = 3;
        LuoPudotuspallo(20);
        pudotuspallo.Hit(new Vector(RandomGen.NextInt(-100, 100), -60));
    }


    /// <summary>
    /// Asetetaan ohjelma kuuntelemaan näppäinpainalluksia
    /// </summary>
    private void AsetaOhjaimet()
    {
        Keyboard.Listen(Key.A, ButtonState.Down, LiikutaKauhaa, "Liikuta kauhaa vasemmalle", kauha, nopeusVasen);
        Keyboard.Listen(Key.A, ButtonState.Released, LiikutaKauhaa, null, kauha, Vector.Zero);
        Keyboard.Listen(Key.D, ButtonState.Down, LiikutaKauhaa, "liikuta kauhaa oikealle", kauha, nopeusOikea);
        Keyboard.Listen(Key.D, ButtonState.Released, LiikutaKauhaa, null, kauha, Vector.Zero);

        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.S, ButtonState.Pressed, palloJumissa, "Jos pallo jää jumiin");

    }


    /// <summary>
    /// Aliohjelmalla liikutetaan kauhaa
    /// </summary>
    /// <param name="kauha">liikutettava kohde</param>
    /// <param name="nopeus">nopeus jolla kauha liikkuu</param>
    private void LiikutaKauhaa(PhysicsObject kauha, Vector nopeus)
    {
        if ((nopeus.X < 0) && (kauha.Left < Level.Left))
        {
            kauha.Velocity = Vector.Zero;
            return;
        }
        if ((nopeus.X > 0) && (kauha.Right > Level.Right))
        {
            kauha.Velocity = Vector.Zero;
            return;
        }

        kauha.Velocity = nopeus;
    }


    /// <summary>
    /// Pallon jäädessä jumiin tönäistään sitä jolloin peli voi taas jatkua
    /// </summary>
    public void palloJumissa()
    {
        pudotuspallo.Hit(new Vector(RandomGen.NextInt(-100, 100), 0));
    }


}



